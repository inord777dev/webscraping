using MauiScrap.Models;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Headers;
using WpfScrap.Services;
using System.Text.Json.Nodes;

namespace MauiScrap.Services
{
    public class ProductService : IProductService
    {
        private DataContext dataContext;

        public ProductService(DataContext dataContext)
        {
            this.dataContext = dataContext;
        }

        public Task<int> AddProduct(Product product)
        {
            dataContext.Products.Add(product);
            return dataContext.SaveChangesAsync();
        }

        public Task<int> DeleteProduct(Product product)
        {
            dataContext.Products.Remove(product);
            return dataContext.SaveChangesAsync();
        }

        public Task<List<Product>> GetProductsAsync()
        {
            var products = dataContext.Products.Include(x => x.PriceChanges).OrderByDescending(x => x.IsFavorites).ToListAsync();
            return products;
        }

        public Task<int> UpdateProduct(Product product)
        {
            dataContext.Products.Update(product);
            return dataContext.SaveChangesAsync();
        }

        public async Task<int> Load()
        {
            int result = 0;

            //https://api.kufar.by/search-api/v2/search/rendered-paginated?cat=1010&cur=USD&gtsy=country-belarus~province-vitebskaja_oblast~locality-novopolock&lang=ru&rms=v.or:4,3&size=30&typ=sell
            var firstPage = "search-api/v2/search/rendered-paginated?cat=1010&cur=USD&gtsy=country-belarus~province-vitebskaja_oblast~locality-novopolock&lang=ru&rms=v.or:4,3&size=30&typ=sell";
            //Uri currentUri = new Uri(firstPage);
            //UriBuilder baseUriBuilder = new UriBuilder(currentUri.Scheme, currentUri.Host, -1);
            var pages = new Queue<string>();
            pages.Enqueue(firstPage);
            var pagesDiscovered = new List<string>();

            try
            {
                using (var client = new HttpClient())
                {
                    int i = 1;
                    int limit = 12;
                    DateTime now = DateTime.Now;
                    client.BaseAddress = new Uri("https://api.kufar.by");
                    client.DefaultRequestHeaders.Add("User-Agent", "Anything");
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    while (pages.Count != 0 && i < limit)
                    {
                        var currentPage = pages.Dequeue();

                        var response = client.GetAsync(currentPage).Result;
                        response.EnsureSuccessStatusCode();
                        var json = await response.Content.ReadAsStringAsync();
                        pagesDiscovered.Add(json);

                        JsonNode contect = JsonNode.Parse(json)!;
                        var ads = contect["ads"]!.AsArray();
                        foreach (var item in ads)
                        {
                            var url = item["ad_link"]!.GetValue<string>();
                            var parameters = item["account_parameters"]!.AsArray();
                            var parameter_address = parameters.Where(x => x["p"]!.GetValue<string>() == "address").FirstOrDefault();
                            var address = parameter_address["v"]!.GetValue<string>();
                            var name = item["subject"]!.GetValue<string>();
                            var price = item["price_byn"]!.GetValue<string>();
                            var price_usd = item["price_usd"]!.GetValue<string>();
                            var updated = item["list_time"]!.GetValue<string>();

                            if (url != null)
                            {
                                var product = dataContext.Products.Where(x => x.Url == url).FirstOrDefault();
                                if (product == null)
                                {
                                    product = new Product() { Url = url, Address = address, Name = name, Price = price, Updated = updated, Created = now };
                                    dataContext.Products.Add(product);
                                }
                                else
                                {
                                    var entry = dataContext.Entry(product);
                                    if (product.Updated != updated)
                                    {
                                        PriceChanges priceChanges = new PriceChanges() { Product = product, Price = product.Price, Updated = product.Updated };
                                        dataContext.PriceChanges.Add(priceChanges);

                                        product.Price = price;
                                        product.Updated = updated;

                                        entry.State = EntityState.Modified;
                                    }
                                }
                            }
                        }

                        var page_next = contect["pagination"]!["pages"]!.AsArray()!.Where(x => x["label"]!.GetValue<string>() == "next").FirstOrDefault();
                        if (page_next != null)
                        {
                            var token = page_next["token"]!.GetValue<string>();
                            if (!string.IsNullOrEmpty(token))
                            {
                                pages.Enqueue(firstPage + $"&cursor={token}");
                            }
                        }

                        i++;

                        result += dataContext.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                result = -1;
            }

            return result;
        }
    }
}

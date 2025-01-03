using Fizzler.Systems.HtmlAgilityPack;
using HtmlAgilityPack;
using MauiScrap.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfScrap.Services;

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

            var firstPage = "https://re.kufar.by/l/novopolock/kupit/kvartiru?cur=USD&rms=v.or%3A3%2C4";
            Uri currentUri = new Uri(firstPage);
            UriBuilder baseUriBuilder = new UriBuilder(currentUri.Scheme, currentUri.Host, -1);
            var pagesDiscovered = new List<string>();
            var pages = new Queue<string>();
            pages.Enqueue(firstPage);

            try
            {
                using (var client = new HttpClient())
                {
                    int i = 1;
                    int limit = 12;

                    while (pages.Count != 0 && i < limit)
                    {
                        var currentPage = pages.Dequeue();

                        var answer = await client.GetStringAsync(currentPage).ConfigureAwait(false);
                        pagesDiscovered.Add(currentPage);

                        var doc = new HtmlDocument();
                        doc.LoadHtml(answer);
                        var document = doc.DocumentNode;


                        var wrapper = document.QuerySelector("div.styles_cards__HMGBx");
                        var productHTMLElements = wrapper.QuerySelectorAll("a.styles_wrapper__Q06m9");
                        foreach (var productHTMLElement in productHTMLElements)
                        {
                            var href = productHTMLElement.GetAttributeValue("href", "");
                            var uri = new Uri(href);
                            var url = (new Uri(baseUriBuilder.Uri, uri.AbsolutePath)).AbsoluteUri;
                            var address = productHTMLElement.QuerySelector("span.styles_address__l6Qe_").InnerText;
                            var name = productHTMLElement.QuerySelector("div.styles_parameters__7zKlL").InnerText;
                            var price = productHTMLElement.QuerySelector("span.styles_price__byr__lLSfd").InnerText;
                            var updated = productHTMLElement.QuerySelector("div.styles_date__ssUVP").InnerText;

                            var product = dataContext.Products.Where(x => x.Url == url).FirstOrDefault();
                            if (product == null)
                            {
                                product = new Product() { Url = url, Address = address, Name = name, Price = price, Updated = updated };
                                dataContext.Products.Add(product);
                            }
                            else
                            {
                                product = dataContext.Products.Find(product.Id);
                                var entry = dataContext.Entry<Product>(product);
                                if (product.Url != url)
                                {
                                    product.Url = url;
                                    entry.State = EntityState.Modified;
                                }

                                if (product.Address != address)
                                {
                                    product.Address = address;
                                    entry.State = EntityState.Modified;
                                }

                                if (product.Name != name)
                                {
                                    product.Name = name;
                                    entry.State = EntityState.Modified;
                                }

                                if (product.Price != price || product.Updated != updated)
                                {
                                    PriceChanges priceChanges = new PriceChanges() { Product = product, Price = product.Price, Updated = product.Updated };
                                    dataContext.PriceChanges.Add(priceChanges);

                                    product.Price = price;
                                    product.Updated = updated;

                                    entry.State = EntityState.Modified;
                                }
                            }
                        }

                        var paginationWrapper = document.QuerySelector("div.styles_pagination__EEqgm");
                        var paginationElements = paginationWrapper.QuerySelectorAll("a.styles_link__8m3I9");
                        foreach (var paginationElement in paginationElements)
                        {
                            var nextPaginationLink = paginationElement.GetAttributeValue("href", "");
                            Uri nextUri = new Uri(baseUriBuilder.Uri, nextPaginationLink);
                            if (!pagesDiscovered.Contains(nextUri.AbsoluteUri) && !pages.Contains(nextUri.AbsoluteUri))
                            {
                                pages.Enqueue(nextUri.AbsoluteUri);
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

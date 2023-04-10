using RestSharp;
using SharedModels;
using System;

namespace OrderApi.Infrastructure
{
    public class ProductServiceGateway: IServiceGateway<ProductDto>
    {
        Uri productServiceBaseUrl;

        public ProductServiceGateway(Uri baseUrl)
        {
            productServiceBaseUrl = baseUrl;
        }

        public ProductDto Get(int id)
        {
            RestClient c = new RestClient(productServiceBaseUrl);
            var request = new RestRequest(id.ToString());
            var response = c.Execute<ProductDto>(request);
            var orderedProduct = response.Data;
            return orderedProduct;
        }
    }
}
using SharedModels;

namespace ProductApi.Models;

public class ProductConverter:IConverter<Product, ProductDto>
{
    public Product Convert(ProductDto model)
    {
        return new Product
        {
            Id = model.Id,
            Name = model.Name,
            Price = model.Price,
            ItemsInStock = model.ItemsInStock,
            ItemsReserved = model.ItemsReserved
        };
    }

    public ProductDto Convert(Product model)
    {
        return new ProductDto
        {
            Id = model.Id,
            Name = model.Name,
            Price = model.Price,
            ItemsInStock = model.ItemsInStock,
            ItemsReserved = model.ItemsReserved
        };
    }
}
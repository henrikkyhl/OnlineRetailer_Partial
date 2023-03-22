namespace CustomersApi.Data;

public interface IDbInitializer
{
    void Initialize(CustomerApiContext context);
}
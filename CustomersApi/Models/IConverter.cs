namespace CustomersApi.Models;

public interface IConverter<T,U>
{
    T ConvertDtoToModel(U model);
    U ConvertModelToDto(T model);
}
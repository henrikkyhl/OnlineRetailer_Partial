using System.Collections.Generic;

namespace OrderApi.Data
{
    public interface IOrderRepository<T>
    {
        IEnumerable<T> GetAll();
        IEnumerable<T> GetAllByCustomer(int customerId);
        T Get(int id);
        T Add(T entity);
        void Edit(int id, T entity);
        void Remove(int id);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Aspree.Provider.Interface
{
    public interface IProvider<T>
    {
        Task<T> GetByIdAsync(int Id);
        T GetById(int Id);
        Task<T> GetByGuidAsync(Guid guid);
        T GetByGuid(Guid guid);
        void Create(T Entity);
        void Update(T Entity);
        void Delete(T Entity);
        Task<bool> DeleteById(int Id);
        Task<bool> DeleteByGuid(Guid guid);
        IQueryable<T> GetAll();
        IQueryable<T> FindBy(Expression<Func<T, bool>> expression);
        void SaveChanges();
    }

    public interface IProviderCommon<T,E>
    {
        T Create(T model);
        T Update(T model);
        T ToModel(E entity);
        IEnumerable<T> GetAll();
        T GetById(int id);
        T GetByGuid(Guid guid);
        T DeleteById(int id, Guid DeletedBy);
        T DeleteByGuid(Guid guid, Guid DeletedBy);
        void SaveChanges();
    }
}

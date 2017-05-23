using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using Helios.Domain.Models;

namespace Helios.Domain.Contracts
{
    public interface IRepository<T> where T : EntityBase
    {
        int Add(T item);
        void Remove(int id);
        int Update(T item);
        T GetById(int id, params Expression<Func<T, object>>[] includes);
        IEnumerable<T> Find(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes);
    }
}

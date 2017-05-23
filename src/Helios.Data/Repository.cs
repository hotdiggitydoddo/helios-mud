using Helios.Domain.Contracts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Linq;
using Helios.Domain.Models;

namespace Helios.Data
{
    public class Repository<T> : IRepository<T> where T : EntityBase
    {
        private readonly HeliosDbContext _context;
        private DbSet<T> _entities;

        public Repository(HeliosDbContext context)
        {
            _context = context;
            _entities = context.Set<T>();
        }
        public int Add(T item)
        {
            var e = _entities.Add(item);
            _context.SaveChanges();
            return item.Id;
        }

        public IEnumerable<T> Find(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes)
        {
            var query = _entities.Where(predicate);
            foreach (var include in includes)
                query = query.Include(include);
            return query.AsEnumerable();
        }

        public T GetById(int id, params Expression<Func<T, object>>[] includes)
        {
            if (includes.Length == 0)
                return _entities.SingleOrDefault(x => x.Id == id);
            
            var query = _entities.AsQueryable();
            query = query.Where(x => x.Id == id);
            
            foreach (var include in includes)
                query = query.Include(include);
            
            return query.SingleOrDefault();
        }

        public void Remove(int id)
        {
            throw new NotImplementedException();
        }

        public int Update(T item)
        {
            _entities.Attach(item);
            var entry = _context.Entry(item);
            entry.State = EntityState.Modified;
            return _context.SaveChanges();
        }
    }
}

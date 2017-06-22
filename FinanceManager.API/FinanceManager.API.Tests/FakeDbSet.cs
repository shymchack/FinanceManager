using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

namespace FinanceManager.API.Tests
{
    public class FakeDbSet<T> : IDbSet<T> where T : class
    {
        private HashSet<T> _data = new HashSet<T>();

        public Type ElementType
        {
            get
            {
                return _data.AsQueryable().ElementType;
            }
        }

        public Expression Expression
        {
            get
            {
                return _data.AsQueryable().Expression;
            }
        }

        public ObservableCollection<T> Local
        {
            get
            {
                return new ObservableCollection<T>(_data);
            }
        }

        public IQueryProvider Provider
        {
            get
            {
                return _data.AsQueryable().Provider;
            }
        }

        public T Add(T entity)
        {
            _data.Add(entity);
            return entity;
        }

        public T Attach(T entity)
        {
            _data.Add(entity);
            return entity;
        }

        public T Create()
        {
            return Activator.CreateInstance<T>();
        }

        public T Find(params object[] keyValues)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _data.GetEnumerator();
        }

        public T Remove(T entity)
        {
            _data.Remove(entity);
            return entity;
        }

        TDerivedEntity IDbSet<T>.Create<TDerivedEntity>()
        {
            return Activator.CreateInstance<TDerivedEntity>();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _data.GetEnumerator();
        }
    }
}

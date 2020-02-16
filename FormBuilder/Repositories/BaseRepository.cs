#region Using

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Data.Objects;

#endregion

namespace FormBuilder.Repositories
{
    public abstract class BaseRespository<T, PrimaryKeyT>
         where T : class
    {
        public FormBuilderEntities DataContext;

        public abstract ObjectSet<T> EntitySet { get; }

        protected abstract T ConvertToNativeEntity(T entity);

        protected abstract PrimaryKeyT SelectPrimaryKey(T entity);

        public virtual PrimaryKeyT Save(T entity)
        {
            T native = ConvertToNativeEntity(entity);
            this.EntitySet.AddObject(native);
            this.DataContext.SaveChanges();
            return SelectPrimaryKey(native);
        }

        public virtual IQueryable<T> List()
        {
            return (from t in this.EntitySet
                    select t).Cast<T>();
        }

        protected virtual T GetByPrimaryKey(Func<T, bool> keySelection)
        {
            return (from r in this.EntitySet
                    select r).SingleOrDefault(keySelection) as T;
        }

        public virtual void Delete(PrimaryKeyT key)
        {
            this.EntitySet.DeleteObject(GetByPrimaryKey(key) as T);
            this.DataContext.SaveChanges();
        }

        public virtual void SaveChanges()
        {
            this.DataContext.SaveChanges();
        }

        public virtual void Close()
        {
            this.DataContext.Dispose();
        }

        public abstract T GetByPrimaryKey(PrimaryKeyT key);

        public BaseRespository(FormBuilderEntities dc)
        {            
            this.DataContext = dc;
        }

        public BaseRespository()
        {

        }        
    }
}

//
// Copyright (c) Artur Durasiewicz. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
//

using System;
using System.Collections.Generic;

namespace PetaPoco.Repository
{
    public abstract class Repository<T> : IRepository<T> where T : new()
    {
        public Repository(Database db)
        {
            _db = db;
        }

        public T GetSingle(Func<FluentFilter<T>, FluentFilter<T>> filter)
        {
            FluentFilter<T> fluentFilter = new FluentFilter<T>();
            filter(fluentFilter);
            return _db.SingleOrDefault<T>(GetSingleSelectQuery().Append(fluentFilter.Query));
        }

        public IEnumerable<T> GetList(Func<FluentFilter<T>, FluentFilter<T>> filter)
        {
            FluentFilter<T> fluentFilter = new FluentFilter<T>();
            filter(fluentFilter);

            return _db.Query<T>(GetSelectQuery().Append(fluentFilter.Query));
        }

        public void Save(T entity)
        {
            OnBeforeAction(entity);
            OnBeforeSave(entity);

            _db.Save(entity);

            OnAfterSave(entity);
            OnAfterAction(entity);
        }

        public void Save(IEnumerable<T> entityList)
        {
            foreach (var entity in entityList)
                Save(entity);
        }

        public void Delete(T entity)
        {
            OnBeforeAction(entity);
            OnBeforeDelete(entity);

            _db.Delete<T>(entity);

            OnAfterDelete(entity);
            OnAfterAction(entity);
        }

        public void Delete(IEnumerable<T> entityList)
        {
            foreach (var entity in entityList)
                Delete(entity);
        }

        public void Delete(Func<FluentFilter<T>, FluentFilter<T>> filter)
        {
            var entityList = GetList(filter);
            Delete(entityList);
        }

        protected abstract Sql GetSelectQuery();
        protected abstract Sql GetSingleSelectQuery();
        protected abstract Sql GetCountQuery();

        protected readonly Database _db;

        #region -- EVENTS --
        protected virtual void OnBeforeAction(T entity)
        {
        }

        protected virtual void OnAfterAction(T entity)
        {
        }

        protected virtual void OnBeforeSave(T entity)
        {
        }

        protected virtual void OnAfterSave(T entity)
        {
        }

        protected virtual void OnBeforeDelete(T entity)
        {
        }

        protected virtual void OnAfterDelete(T entity)
        {
        }
        #endregion -- EVENTS --

        #region -- DISPOSE --
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
                _db.Dispose();

            disposed = true;
        }

        ~Repository()
        {
            Dispose(false);
        }

        private bool disposed;

        #endregion -- DISPOSE --
    }
}
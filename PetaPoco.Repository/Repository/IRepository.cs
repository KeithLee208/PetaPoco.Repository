//
// Copyright (c) Artur Durasiewicz. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
//

using System;
using System.Collections.Generic;

namespace PetaPoco.Repository
{
    public interface IRepository<T> : IDisposable
    {
        T GetSingle(Func<FluentFilter<T>, FluentFilter<T>> filter);
        IEnumerable<T> GetList(Func<FluentFilter<T>, FluentFilter<T>> filter);

        void Save(T entity);
        void Save(IEnumerable<T> entityList);

        void Delete(T entity);
        void Delete(IEnumerable<T> entityList);
        void Delete(Func<FluentFilter<T>, FluentFilter<T>> filter);
    }
}

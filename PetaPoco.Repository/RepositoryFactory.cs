//
// Copyright (c) Artur Durasiewicz. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
//

using System;

namespace PetaPoco.Repository
{
    public static class RepositoryFactory
    {
        public static IRepository<TPoco> Create<TPoco>() where TPoco : new()
        {
            return (IRepository<TPoco>)(new SimpleRepository<TPoco>(GetDbContext()));
        }

        public static IRepository<TPoco> Create<TRepository, TPoco>()
        {
            return (IRepository<TPoco>)(Activator.CreateInstance(typeof(TRepository), GetDbContext()));
        }

        private static Database GetDbContext()
        {
            return new PetaPoco.Database("Default");
        }
    }
}

//
// Copyright (c) Artur Durasiewicz. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
//

namespace PetaPoco.Repository
{
    public class SimpleRepository<T> : Repository<T> where T : new()
    {
        public SimpleRepository(Database db) : base(db)
        {
            TableName = typeof(T).Name;
        }

        protected override Sql GetSelectQuery()
        {
            return new Sql("SELECT * FROM dbo." + TableName);
        }

        protected override Sql GetSingleSelectQuery()
        {
            return new Sql("SELECT TOP 1 * FROM dbo." + TableName);
        }

        protected override Sql GetCountQuery()
        {
            return new Sql("SELECT COUNT(*) FROM dbo." + TableName);
        }

        public string TableName { get; private set; }
    }
}

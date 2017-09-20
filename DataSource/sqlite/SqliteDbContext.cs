using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;

namespace dataSource.sqlite
{
    class SqliteDbContext : DbContext
    {
        

        public SqliteDbContext(SqliteDatabase sqliteDatabase, SQLiteConnection con)
            :base(sqliteDatabase, con){ }


        internal override IDbCommand getCommand()
        {
            return new SQLiteCommand((SQLiteConnection)this.connection);
        }

        protected override void addParam(IDbCommand cmd, string name, object value)
        {
            ((SQLiteCommand)cmd).Parameters.AddWithValue(name, value);
        }


        //===================
        private SelectStatement selectLastId;

        public override int lastId()
        {
            if (selectLastId == null)
            {
                selectLastId = this.select().fields(new FunctionExpression("last_insert_rowid"));
            }
            return Convert.ToInt32(selectLastId.executeScalar());
        }

    }
}

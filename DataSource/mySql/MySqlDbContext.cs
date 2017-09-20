using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace dataSource.mysql
{
    class MySqlDbContext: DbContext
    {
       
        public MySqlDbContext(MySqlDatabase mySqlDatabase, MySqlConnection con)
            : base(mySqlDatabase, con) { }


        internal override IDbCommand getCommand()
        {
            return new MySqlCommand() { Connection = (MySqlConnection)this.connection };
        }

        protected override void addParam(IDbCommand cmd, string name, object value)
        {
            ((MySqlCommand)cmd).Parameters.AddWithValue(name, value);
        }

        //===================
        private SelectStatement selectLastId;

        public override int lastId()
        {
            if (selectLastId == null)
            {
                selectLastId = this.select().fields(new FunctionExpression("LAST_INSERT_ID"));
            }
            return Convert.ToInt32(selectLastId.executeScalar());
        }

    }
}

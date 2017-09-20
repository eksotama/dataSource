using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;
using System.Data;
using dataSource.mySql;


namespace dataSource.mysql
{
    class MySqlDatabase : Database
    {

        public override DbContext newContext()
        {
            var con = new MySqlConnection(this.conectionString);
            return new MySqlDbContext(this, con);
        }

         //constructors
        public MySqlDatabase(string conString)
        {
            this.conectionString = conString;
        }

        public MySqlDatabase(string server, string db, string user, string pwd)
        {
            this.conectionString = string.Format("Host={0};UserName={1};Password={2};Database={3};", server, user, pwd, db);
        }

        //===== implementations ==============


        #region columns

        public override void addIdColumn(Table t)
        {
            t.fields.Add(
                new MySqlInteger(
                    string.Format("id_{0}", t.Name)
                    ).autoIncrement()
                );
        }


        



        public override IntColumn intColumn(string name)
        {
            return new MySqlInteger(name);
        }

        public override DoubleColumn doubleColumn(string name)
        {
            return new MySqlReal(name);
        }

        public override TextColumn textColumn(string name)
        {
            return new MySqlText(name);
        }

        public override StringColumn stringColumn(string name, int maxLength)
        {
            return new MySqlString(name, maxLength);
        }

        public override DateColumn dateColumn(string name)
        {
            return new MySqlDate(name);
        }

        public override DateTimeColumn dateTimeColumn(string name)
        {
            return new MySqlDateTime(name);
        }

       

        public override BinaryColumn binaryColumn(string name)
        {
            return new MySqlImage(name);
        }

        public override BoolColumn boolColumn(string name)
        {
            return new MySqlBool(name);
        }



        public override Table newTable(string name, params Column[] fields)
        {
            return new MySqlTable(name, this, fields);
        }

        #endregion

        

    }
}

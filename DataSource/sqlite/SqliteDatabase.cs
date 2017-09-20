using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SQLite;

namespace dataSource.sqlite
{
    public class SqliteDatabase : Database
    {
        

        public override DbContext newContext()
        {
            var con = new SQLiteConnection(this.conectionString);

            return new SqliteDbContext(this, con);
        }

        //constructors
        public SqliteDatabase(string path)
        {
            this.conectionString = string.Format("Data Source={0};Foreign Keys=True;", path);
            
        }

        public SqliteDatabase(string path, string pwd)
        {
            this.conectionString = string.Format("Data Source={0};Foreign Keys=True;Password={1}", path, pwd);   
        }

        //===== implementations ==============

        public override void addIdColumn(Table t)
        {
            t.fields.Add(
                new SqliteInteger(
                    string.Format("id_{0}", t.Name)
                    )
                );
        }

       

        public override IntColumn intColumn(string name)
        {
            return new SqliteInteger(name);
        }

        public override DoubleColumn doubleColumn(string name)
        {
            return new SqliteReal(name);
        }

        public override TextColumn textColumn(string name)
        {
            return new SqliteText(name);
        }

        public override StringColumn stringColumn(string name, int maxLength)
        {
            return new SqliteString(name, maxLength);
        }

        public override DateColumn dateColumn(string name)
        {
            return new SqliteDate(name);
        }

        public override DateTimeColumn dateTimeColumn(string name)
        {
            return new SqliteDateTime(name);
        }

       

        public override BinaryColumn binaryColumn(string name)
        {
            return new SqliteImage(name);
        }

        public override BoolColumn boolColumn(string name)
        {
            return new SqliteBool(name);
        }



        public override Table newTable(string name, params Column[] fields)
        {
            return new Table(name, this, fields);
        }

       

        
    }
}

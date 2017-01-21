# dataSource
A Code-First data access layer for .Net on top of ADO.Net, that automates database updates.

supports Sqlite and MySql, with good abstraction to extend support to include other database types.

it maps every table to an object not class, so no reflection is used , and hence you can obfuscate your code safely, 
but of course it's still more cumbersome than any code-first data access layer that maps a table to a class.

#Example

```cs

//creat a static class to include static fields of the db objects

    public static class dbm
    {
        public static Database db;
        public static StringColumn cust_name;
        public static Table tbl_customer;
        public static StringColumn cont_value;
        public static IntColumn cont_type;
        public static Table tbl_contact;
        public static IntColumn cont_fk_cust;
        public static StringColumn ctype_name;
        public static Table tbl_cont_type;
        public static View vw_contact;


        /// <summary>
        /// initializes the db objects, call from the app main entry point
        /// </summary>
        public static void init()
        {

            //one-line change changes the db type
            //db = new MySqlDatabase("localhost", "data", "root", "root");
            db = new SqliteDatabase("database.db", "pwd");

            //customers table -------------

            //define fields first, id of type int autoincrement is automatically created
            cust_name = db.stringColumn("cust_name", 150);

            //define table
            tbl_customer = db.newTable("tbl_customer", cust_name);

            //contacts table ----------------

            cont_value = db.stringColumn("cont_value", 150);
            cont_type = db.intColumn("cont_type");

            tbl_contact = db.newTable("tbl_contact", cont_type, cont_value);

            //add fk to tbl_customer
            cont_fk_cust = tbl_contact.addFKto(tbl_customer);

            //contact type table -------------

            ctype_name = db.stringColumn("ctype_name", 100);

            tbl_cont_type = db.newTable("tbl_cont_type", ctype_name);

            //contacts view --------------

            vw_contact= db.newView("vw_contacts", 
                 db.select().from( tbl_contact.innerJoin(tbl_cont_type, cont_type.equal(tbl_cont_type.id)) )
                );

            //create schema update passing every db object
            db.addSchemaUpdate(
                tbl_customer, tbl_contact, tbl_cont_type, vw_contact
                );

            //whenever you want to change db version, add a new schema update

            //this will silently maintain the db version at the publish machine
            db.updateSchemaIfNeeded();

        }

    }
    
```

manipulate data from outside of this class

```cs

          //inserting -----------------

            dbm.db.insertInto(dbm.tbl_customer).Values(
                dbm.cust_name.value("khaled")
                ).execute();

            int inserted_customer_id = dbm.db.lastId();

            dbm.db.insertInto(dbm.tbl_contact).Values(
                dbm.cont_fk_cust.value(inserted_customer_id),
                dbm.cont_type.value(1),
                dbm.cont_value.value("201234567890")
                ).execute();

            //call submitChanges() to commit transaction
            dbm.db.SubmitChanges();

            //selecting -----------------

            DataTable dt=
            dbm.db.select().from(dbm.vw_contact).fields(
                dbm.cont_type, 
                dbm.cont_fk_cust, 
                dbm.cont_value
                ).where(dbm.cont_fk_cust.equal(inserted_customer_id))
                .execute();

            //updating -----------------

            dbm.db.update(dbm.tbl_customer).Set(
                dbm.cust_name.value("khaled kamal")
                )
                .where(dbm.tbl_customer.id.equal(inserted_customer_id))
                .execute();

            dbm.db.SubmitChanges();

            //deleting ----------------

            dbm.db.deleteFrom(dbm.tbl_customer).where(dbm.tbl_customer.id.equal(inserted_customer_id));

            dbm.db.SubmitChanges();
            
```

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace dataSource
{
    public class SelectStatement : AbsSelect , Statement
    {
        //must be initialized
        public Database db;
        public bool distinct;
        public expr[] selectFields;
        public DbTable[] tableFields;
        public AbsTable targetTable;
        public expr cond;
        public expr[] groupBys;
        public expr havingExp;
        public SortExp[] sortList;
        public int pageIndex=0, pageSize=0;

        public SelectStatement(Database db ,bool distinct)
        {
            this.db = db;
            this.distinct = distinct;
        }

        public SelectStatement(Database db )
        {
            this.db = db;
            this.distinct = false;
        }

        /// <summary>
        /// if used multiple times the last one will override
        /// </summary>
        public SelectStatement fields(params expr[] exprs)
        {
            this.selectFields = exprs;
            return this;
        }

        /// <summary>
        /// if used multiple times the last one will override
        /// </summary>
        public SelectStatement fields(params DbTable[] tablesAndViews)
        {
            this.tableFields = tablesAndViews;
            return this;
        }

        /// <summary>
        /// use it once, if used multiple times, the last one will override
        /// </summary>
        public SelectStatement from(AbsTable table)
        {
            this.targetTable = table;
            return this;
        }

        /// <summary>
        /// use only once
        /// </summary>
        public SelectStatement where(expr cond)
        {
            this.cond = cond;
            return this;
        }

        public SelectStatement orderBy(params SortExp[] sortExps)
        {
            this.sortList = sortExps;
            return this;
        }

        public SelectStatement page(int index, int pageSize)
        {
            this.pageIndex = index;
            this.pageSize = pageSize;
            return this;
        }

        public SelectStatement groupBy(expr[] exps)
        {
            this.groupBys = exps;
            return this;
        }

        /// <summary>
        /// only use with gruopBy() otherwise it's useless
        /// </summary>
        public SelectStatement having(expr exp)
        {
            this.havingExp = exp;
            return this;
        }

        #region AbsTable

        /// <summary>
        /// the same as render(), but ignoring 'order by' and 'limit offset'
        /// </summary>
        public override string renderInSelect()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("SELECT {0}", distinct? "DISTINCT" : "");
            //select all
            if (tableFields == null && selectFields == null) sb.Append(" * ");
            //table fields
            if(tableFields != null)
            {
                foreach (DbTable t in tableFields)
                {
                    sb.AppendFormat(" {0}.* ,", t.Name);
                }
            }
            //fields
            if (selectFields != null)
            {
                foreach (expr exp in selectFields)
                {
                    sb.AppendFormat(" {0} ,", exp.Render(this));
                }
                sb.removeLastChar();
            }
            //from
            if (targetTable != null)
            {
                sb.AppendFormat(" FROM ({0}) ", targetTable.renderInSelect());
            }
            //where
            if (cond != null)
            {
                sb.AppendFormat(" WHERE ({0}) ", cond.Render(this));
            }
            //group by
            if (groupBys != null)
            {
                sb.Append(" GROUP BY ");
                foreach (expr ex in groupBys)
                {
                    sb.AppendFormat(" {0} ,", ex.Render(this));
                }
                sb.removeLastChar();
                if (havingExp != null)
                {
                    sb.AppendFormat(" HAVING ({0}) ", havingExp.Render(this));
                }
            }
            //
            return sb.ToString();
        }

        public string render()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(this.renderInSelect());
            //order by
            if (sortList != null)
            {
                sb.Append(" ORDER BY ");
                foreach (SortExp se in sortList)
                {
                    sb.AppendFormat("{0} ,", se.render());
                }
                sb.removeLastChar();
            }
            //page
            if (pageSize > 0)
            {
                sb.AppendFormat(" LIMIT {0} OFFSET {1}", pageSize, pageSize * pageIndex);
            }
            //
            return sb.ToString();
        }

        

        #endregion

        #region AbsSelect

        /// <summary>
        /// returns empty datatable for no data
        /// </summary>
        public override DataTable execute()
        {
            return this.db.executeSelect(this);
        }

        #endregion

        #region Statement interface

        public List<NameValuePair> parameters = new List<NameValuePair>();

        public void addParam(string name, object value)
        {
            parameters.Add(new NameValuePair(name, value));
        }

        #endregion


        /// <summary>
        /// returns the first row of the result set, or null if no data found
        /// </summary>
        public DataRow executeRow()
        {
            return db.executeSelectRow(this);
        }


        public Object executeScalar()
        {
            return db.executeScalar(this);
        }

       

    }

    public enum SortOrder { ASC , DESC }

    public class SortExp
    {
        expr exp;
        SortOrder so = SortOrder.ASC;

        public SortExp(expr exp, bool Desc)
        {
            this.exp = exp;
            so = Desc? SortOrder.DESC : SortOrder.ASC;
        }

        public SortExp(expr exp)
        {
            this.exp = exp;
        }

        public string render()
        {
            return string.Format(" ({0}) {1} ", exp.Render(null), so==SortOrder.ASC? "ASC" : "DESC" );
        }

    }
}

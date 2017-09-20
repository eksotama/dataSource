using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dataSource
{
    public abstract class expr
    {
        public dbType fieldType = dbType.String;

        public expr As(string alias)
        {
            return new AliasedExpr(this, alias);
        }

        public abstract string Render(Statement st);

        #region methods binary

        public expr equal(expr exp)
        {
            return new BinaryExpression(this, "=", exp) { fieldType=dbType.Bool };
        }

        public expr equal(object o)
        {
            return equal(new ValueExpr(o) { fieldType = dbType.Bool });
        }

        public expr notEqual(expr exp)
        {
            return new BinaryExpression(this, "<>", exp) { fieldType = dbType.Bool };
        }


        public expr notEqual(object o)
        {
            return notEqual(new ValueExpr(o) { fieldType = dbType.Bool });
        }

        public expr concat(expr exp)
        {
            return new BinaryExpression(this, "||", exp) { fieldType = dbType.String };
        }

		public expr concat(object o)
		{
            return concat(new ValueExpr(o) { fieldType = dbType.String });
		}

        public expr minus(expr exp)
        {
            return new BinaryExpression(this, "-", exp) { fieldType = this.fieldType };
        }

        public expr minus(Object ob)
        {
            return this.minus(new ValueExpr(ob) { fieldType = this.fieldType });
        }

        public expr plus(expr exp)
        {
            return new BinaryExpression(this, "+", exp) { fieldType = this.fieldType };
        }

        public expr plus(double value)
        {
            return plus(new ValueExpr(value) { fieldType = this.fieldType });
        }

        public expr mod(expr exp)
        {
            return new BinaryExpression(this, "%", exp) { fieldType = dbType.Int };
        }

        public expr mod(double value)
        {
            return mod(new ValueExpr(value) { fieldType = dbType.Int });
        }

        public expr multiply(expr exp)
        {
            return new BinaryExpression(this, "*", exp) { fieldType = dbType.Double };
        }

		public expr multiply(double o)
		{
            return multiply(new ValueExpr(o) { fieldType = dbType.Double });
		}

        public expr divide(expr exp)
        {
            return new BinaryExpression(this, "/", exp) { fieldType = dbType.Double };
        }

		public expr divide(double o)
		{
            return divide(new ValueExpr(o) { fieldType = dbType.Double });
		}

        public expr And(expr exp)
        {
            return new BinaryExpression(this, "AND", exp) { fieldType = dbType.Bool };
        }

        public expr OR(expr exp)
        {
            return new BinaryExpression(this, "OR", exp) { fieldType = dbType.Bool };
        }

        public expr greaterThan(expr exp)
        {
            return new BinaryExpression(this, ">", exp) { fieldType = dbType.Bool };
        }

        public expr greaterOrEqual(expr exp)
        {
            return new BinaryExpression(this, ">=", exp) { fieldType = dbType.Bool };
        }

        public expr lessThan(expr exp)
        {
            return new BinaryExpression(this, "<", exp) { fieldType = dbType.Bool };
        }

        public expr lessOrEqual(expr exp)
        {
            return new BinaryExpression(this, "<=", exp) { fieldType = dbType.Bool };
        }

        public expr IN(params object[] values)
        {
            return new InExpression(this, true, values) { fieldType = dbType.Bool };
        }

        public expr notIN(params object[] values)
        {
            return new InExpression(this, false, values) { fieldType = dbType.Bool };
        }

        public expr IN(AbsSelect select)
        {
            return new InExpression(this, true, select) { fieldType = dbType.Bool };
        }

        public expr notIN(AbsSelect select)
        {
            return new InExpression(this, false, select) { fieldType = dbType.Bool };
        }

        public expr IN(DbTable table)
        {
            return new InExpression(this, true, table) { fieldType = dbType.Bool };
        }

        public expr notIN(DbTable table)
        {
            return new InExpression(this, false, table) { fieldType = dbType.Bool };
        }

        public expr between(expr exp1, expr exp2)
        {
            return new BetweenExpression(this, true, exp1, exp2) { fieldType = dbType.Bool };
        }

        public expr notBetween(expr exp1, expr exp2)
        {
            return new BetweenExpression(this, false, exp1, exp2) { fieldType = dbType.Bool };
        }

        public expr between(object val1, object val2)
        {
            return new BetweenExpression(this, true, new ValueExpr(val1), new ValueExpr(val2)) { fieldType = dbType.Bool };
        }

        public expr notBetween(object val1, object val2)
        {
            return new BetweenExpression(this, false, new ValueExpr(val1), new ValueExpr(val2)) { fieldType = dbType.Bool };
        }

        public expr isNull()
        {
            return new NullCheckExp(this, true) { fieldType = dbType.Bool };
        }

        public expr notNull()
        {
            return new NullCheckExp(this, false) { fieldType = dbType.Bool };
        }

		/// <summary>
		/// matches against a pattern, use % to match any number of chars or digits(or none), 
		/// and _ to match one, you can use it with text or numeric columns
		/// </summary>
		public expr LIKE(string pattern)
		{
			return new BinaryExpression(this, "LIKE", new ValueExpr(pattern));
		}

		public expr notLIKE(string pattern)
		{
			return new BinaryExpression(this, "NOT LIKE", new ValueExpr(pattern));
		}

        #endregion


        #region static methods - unary

        public static expr exists(AbsSelect select)
        {
            return new ExistsExpression(select);
        }

        public static expr Minus(expr exp)
        {
            return new UnaryExpression("-", exp) { fieldType = dbType.Double };
        }

        public static expr not(expr exp)
        {
            return new UnaryExpression("NOT", exp) { fieldType = dbType.Bool };
        }

        public static expr abs(expr exp)
        {
            return new FunctionExpression("Abs", exp) { fieldType = exp.fieldType };
        }

        public static expr ceiling(expr exp)
        {
            return new FunctionExpression("ceiling", exp) { fieldType = dbType.Int };
        }

        public static expr floor(expr exp)
        {
            return new FunctionExpression("floor", exp) { fieldType = dbType.Int };
        }

        

        /// <summary>
        /// the length of a string value
        /// </summary>
        public static expr length(expr exp)
        {
            return new FunctionExpression("length", exp) { fieldType = dbType.Int };
        }

        /// <summary>
        /// a simple function if given more than one param, but an aggregation function if given one param
        /// </summary>
        public static expr Max(params expr[] exp)
        {
            return new FunctionExpression("max", exp) { fieldType = exp[0].fieldType };
        }

        /// <summary>
        /// a simple function if given more than one param, but an aggregation function if given one param
        /// </summary>
        public static expr Min(params expr[] exp)
        {
            return new FunctionExpression("min", exp) { fieldType = exp[0].fieldType };
        }

        public static expr Avg(params expr[] exp)
        {
            return new FunctionExpression("avg", exp) { fieldType = exp[0].fieldType };
        }

        public static expr count(params expr[] exp)
        {
            return new FunctionExpression("count", exp) { fieldType = dbType.Int };
        }

        public static expr median(params expr[] exp)
        {
            return new FunctionExpression("median", exp) { fieldType = exp[0].fieldType };
        }

        public static expr sum(params expr[] exp)
        {
            return new FunctionExpression("sum", exp) { fieldType = exp[0].fieldType };
        }


        #endregion

        //order by

        public SortExp Asc()
        {
            return new SortExp(this);
        }

        public SortExp Desc()
        {
            return new SortExp(this, true);
        }

    }

    #region subclasses

    public class BinaryExpression : expr
    {
        private expr exp1;
        private expr exp2;
        private string op;

        public BinaryExpression(expr exp1, string op, expr exp2)
        {
            this.exp1 = exp1;
            this.exp2 = exp2;
            this.op = op;
        }


        public override string Render(Statement st)
        {
            return string.Format("({0} {1} {2}) ", exp1.Render(st), op, exp2.Render(st) );
        }
    }

    public class UnaryExpression : expr
    {
        private expr exp;
        
        private string op;

        public UnaryExpression(string op, expr exp)
        {
            this.exp = exp;
            this.op = op;
        }


        public override string Render(Statement st)
        {
            return string.Format("({0} {1}) ",  op, exp.Render(st));
        }
    }


    public class FunctionExpression : expr
    {
        private expr[] exp_list;
        private string function;

        public FunctionExpression(string function, params expr[] exps)
        {
            this.exp_list = exps;
            this.function = function;
        }


        public override string Render(Statement st)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(function);
            sb.Append("( ");
            foreach (expr exp in exp_list)
            {
                sb.AppendFormat("{0}{1}", exp.Render(st), " ,");
            }
            sb.removeLastChar();
            sb.Append(") ");
            //sb.Append(alias == null ? "" : " AS " + alias);
            return sb.ToString();
        }
    }

    public class BetweenExpression : expr
    {
        private expr exp1;
        private expr exp2;
        private bool _between;
        private expr exp;

        public BetweenExpression(expr exp, bool betweenOrNot, expr exp1, expr exp2)
        {
            this.exp = exp;
            this.exp1 = exp1;
            this.exp2 = exp2;
            this._between = betweenOrNot;
        }


        public override string Render(Statement st)
        {
            return string.Format("(({0}) {1} ({2}) AND ({3})) ", exp.Render(st), _between? "BETWEEN": "NOT BETWEEN" , exp1.Render(st), exp2.Render(st));
        }
    }

    public class NullCheckExp : expr
    {
        private bool _isNull;
        private expr exp;

        public NullCheckExp(expr exp, bool isNull)
        {
            this.exp = exp;
            this._isNull = isNull;
        }

        public override string Render(Statement st)
        {
            return string.Format("({0}) {1}" ,exp.Render(st), _isNull? "ISNULL" : "NOTNULL");
        }
    }

    public class ExistsExpression : expr
    {
        AbsSelect select;

        public ExistsExpression(AbsSelect select)
        {
            this.select = select;
        }


        public override string Render(Statement st)
        {
            return string.Format("EXISTS ({0})", select.renderInSelect());
        }
    }

	
    public class ValueExpr : expr
    {
        object val = null;
        string name = null;

        public ValueExpr(object val)
        {
            this.val = val;
        }

		/// <summary>
		/// use this constructor for parametried query
		/// </summary>
        public ValueExpr(string name, object val)
        {
            this.name = name;
            this.val = val;
        }

        public ValueExpr(String val)
        {
            this.val = val;
        }

        public ValueExpr(double val)
        {
            this.val = val;
        }

        /// <summary>
        /// you may pass null
        /// </summary>
        public override string Render(Statement st)
        {
            if(name != null && st != null)
            {
                st.addParam(name, val);
                return string.Format("@{0}", name);
            }
            //
            if (val == null)
            {
                return "NULL";
            }
            else if (val is String)
            {
                return string.Format("'{0}'", val);
            }
			
            else //double
            {
                return val.ToString();
            }
        }
    }

    public class InExpression : expr
    {
        object[] values;
        AbsSelect select;
        DbTable table;
        //
        private bool inOrNotIn;
        private expr exp;

        private string renderValue(object val)
        {
            if (val == null)
            {
                return "NULL";
            }
            else if (val is String)
            {
                return string.Format("'{0}'", val);
            }
            else //double
            {
                return val.ToString();
            }
        }

        public InExpression(expr exp, bool InOrNotIn, DbTable table)
        {
            this.exp = exp;
            this.inOrNotIn = InOrNotIn;
            this.table = table;
        }

        public InExpression(expr exp, bool InOrNotIn, AbsSelect select)
        {
            this.exp = exp;
            this.inOrNotIn = InOrNotIn;
            this.select = select;
        }

        public InExpression(expr exp, bool InOrNotIn, params  object[] values)
        {
            this.exp = exp;
            this.inOrNotIn = InOrNotIn;
            this.values = values;
        }

        public override string Render(Statement st)
        {
            if (values != null)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(exp.Render(st));
                if(!inOrNotIn) sb.Append(" NOT ");
                sb.Append("BETWEEN ( ");
                foreach (object val in values)
                {
                    sb.AppendFormat("({0}){1} ", renderValue(val), " ,");
                }
                sb.removeLastChar();
                sb.Append(") ");
                return sb.ToString();
            }
            //======
            if (table != null)
            {
                return string.Format("(({0}) {1} ({2})", exp.Render(st), inOrNotIn ? "IN" : "NOT IN", table.renderInSelect());
            }
            //======
            if (select != null)
            {
                return string.Format("(({0}) {1} ({2})", exp.Render(st), inOrNotIn ? "IN" : "NOT IN", select.renderInSelect());
            }
            //not supposed to come here
            return null;
        }
       
    }

    /// <summary>
    /// alias with column is special because columns are static fields, so it won't 
    /// persist an alias, no problem with other exprs as it will be used once
    /// </summary>
    public class AliasedExpr : expr
    {
        string _alias;
        expr exp;

        public string alias
        {
            get
            {
                return _alias;
            }
        }

        public AliasedExpr(expr exp, string alias)
        {
            this.exp = exp;
            this._alias = alias;
            this.fieldType = exp.fieldType;
        }

        public override string Render(Statement st)
        {
            return string.Format("({0}) AS {1}", exp.Render(st), _alias);
        }
    }

    #endregion


    public struct FieldInfo
    {
        public string name;
        public dbType type;

        public FieldInfo(dbType type, string name)
        {
            this.name = name;
            this.type = type;
        }

        public FieldInfo(Column col)
        {
            this.name = col.Name;
            this.type = col.fieldType;
        }

    }

    public enum dbType { String, Int, Double, Text, Bool, Date, DateTime, Binary }

}

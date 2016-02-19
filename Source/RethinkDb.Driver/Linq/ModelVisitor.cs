using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using Remotion.Linq;
using Remotion.Linq.Clauses;
using Remotion.Linq.Clauses.Expressions;
using RethinkDb.Driver.Ast;
using RethinkDb.Driver.Model;

namespace RethinkDb.Driver.Linq
{
    /*
 IQueryModelVisitor

The first of those two visitors operates on a large scale: 
it provides a way to execute specific code for each clause
within a QueryModel, allowing you to translate one clause
at a time. You can collect the partial results of your translations,
and finally make one query for your target system from those parts.

The simplest way to make use of IQueryModelVisitor is to derive from 
QueryModelVisitorBase. That class implements the interface by automatically
iterating over sub-clauses and collections, dispatching to the correct visitor
methods for every element of the query. It’s also hardened against modifications
of the QueryModel being iterated, but more about this later. Simply override
its Visit… methods for the query components you want to handle, 
and generate your target query parts accordingly. Note that you need to
handle all the clauses, result operators, and so on defined by re-linq.
If you don’t at least throw an exception for those constructs you simply 
cannot translate, you’ll get invalid query translations.
*/

    public class ReqlTableAttribute : Attribute
    {
        public string Database { get; }
        public string Table { get; }

        public ReqlTableAttribute(string table) : this(table, null)
        {
        }

        /// <param name="database">If null, uses conneciton's default database.</param>
        public ReqlTableAttribute(string table, string database = null)
        {
            this.Database = database;
            this.Table = table;
        }
    }
    public class ReqlIndexAttribute : Attribute
    {
        public string IndexName { get; }

        public ReqlIndexAttribute(string indexName)
        {
            this.IndexName = indexName;
        }
    }
    public class ReqlPrimaryKey : Attribute
    {
        
    }

    public class Vars
    {
        public Dictionary<string, ReqlExpr> ByName = new Dictionary<string, ReqlExpr>();
        public Dictionary<int, ReqlExpr> ById = new Dictionary<int, ReqlExpr>();

        public void Add(string fromName, int varId, Var term)
        {
            this.ByName.Add(fromName, term);
            this.ById.Add(varId, term);
        }
    }
    public class ModelVisitor : QueryModelVisitorBase
    {
        private readonly Table table;
        private static RethinkDB R = RethinkDB.R;

        public ReqlAst Query => stack.Peek();

        public Stack<ReqlExpr> stack = new Stack<ReqlExpr>();

        public ModelVisitor(Table table)
        {
            this.table = table;
        }

        public Vars fromVars = new Vars();

        public override void VisitQueryModel(QueryModel queryModel)
        {
            base.VisitQueryModel(queryModel);
        }

        public override void VisitMainFromClause(MainFromClause fromClause, QueryModel queryModel)
        {
            if( fromClause.FromExpression is SubQueryExpression )
                throw new NotSupportedException($"{nameof(MainFromClause)} cannot be a SubQueryExpression");

            var linqName = fromClause.ItemName;
            var varId = Func.NextVarId();
            var var = new Var(varId);
            

            this.fromVars.Add(linqName, varId, var);

            base.VisitMainFromClause(fromClause, queryModel);

        }

        public override void VisitAdditionalFromClause(AdditionalFromClause fromClause, QueryModel queryModel, int index)
        {
            base.VisitAdditionalFromClause(fromClause, queryModel, index);
        }

        public override void VisitJoinClause(JoinClause joinClause, QueryModel queryModel, int index)
        {
            base.VisitJoinClause(joinClause, queryModel, index);
        }

        public override void VisitJoinClause(JoinClause joinClause, QueryModel queryModel, GroupJoinClause groupJoinClause)
        {
            base.VisitJoinClause(joinClause, queryModel, groupJoinClause);
        }

        public override void VisitGroupJoinClause(GroupJoinClause groupJoinClause, QueryModel queryModel, int index)
        {
            base.VisitGroupJoinClause(groupJoinClause, queryModel, index);
        }

        public override void VisitWhereClause(WhereClause whereClause, QueryModel queryModel, int index)
        {

            //Is it a FILTER, GET or GETALL, OR BETWEEN?
            var predicateExpr = GetReqlAst(whereClause.Predicate, queryModel, this.fromVars);

            var varIds = this.fromVars.ById.Keys.ToList();

            var func = Func.MakeFunc(varIds, predicateExpr);

            this.stack.Push(func);
        }

        private ReqlExpr GetReqlAst(Expression predicate, QueryModel queryModel, Vars vars)
        {
            var visitor = new ExpressionVisitor(this, queryModel, vars);
            visitor.Visit(predicate);
            return visitor.Current;
        }


        public override void VisitOrderByClause(OrderByClause orderByClause, QueryModel queryModel, int index)
        {
            base.VisitOrderByClause(orderByClause, queryModel, index);
        }

        public override void VisitOrdering(Ordering ordering, QueryModel queryModel, OrderByClause orderByClause, int index)
        {
            base.VisitOrdering(ordering, queryModel, orderByClause, index);
        }

        public override void VisitSelectClause(SelectClause selectClause, QueryModel queryModel)
        {
            //var map = GetReqlAst(selectClause.Selector, queryModel, this.fromVars);
            DefaultIfE


            base.VisitSelectClause(selectClause, queryModel);
        }

        public override void VisitResultOperator(ResultOperatorBase resultOperator, QueryModel queryModel, int index)
        {
            base.VisitResultOperator(resultOperator, queryModel, index);
        }

        protected override void VisitBodyClauses(ObservableCollection<IBodyClause> bodyClauses, QueryModel queryModel)
        {
            base.VisitBodyClauses(bodyClauses, queryModel);
        }

        protected override void VisitOrderings(ObservableCollection<Ordering> orderings, QueryModel queryModel, OrderByClause orderByClause)
        {
            base.VisitOrderings(orderings, queryModel, orderByClause);
        }

        protected override void VisitResultOperators(ObservableCollection<ResultOperatorBase> resultOperators, QueryModel queryModel)
        {
            base.VisitResultOperators(resultOperators, queryModel);
        }
    }
}
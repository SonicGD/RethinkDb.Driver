using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using Remotion.Linq;
using Remotion.Linq.Clauses;
using Remotion.Linq.Clauses.Expressions;
using RethinkDb.Driver.Ast;

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

    public class ModelVisitor : QueryModelVisitorBase
    {
        private readonly Table table;

        public ReqlAst Query { get; set; }

        public Stack<ReqlExpr> stack = new Stack<ReqlExpr>();

        public ModelVisitor(Table table)
        {
            this.table = table;
        }

        public Dictionary<string, ReqlExpr> FromExprs = new Dictionary<string, ReqlExpr>();

        public override void VisitQueryModel(QueryModel queryModel)
        {
            base.VisitQueryModel(queryModel);
        }

        public override void VisitMainFromClause(MainFromClause fromClause, QueryModel queryModel)
        {
            if( fromClause.FromExpression is SubQueryExpression )
                throw new NotSupportedException($"{nameof(MainFromClause)} cannot be a SubQueryExpression");

            var var = fromClause.ItemName;
            this.FromExprs.Add(var, new Var());

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

            var expr = GetReqlAst(whereClause.Predicate, queryModel);


            this.stack.Push(expr);
        }

        private ReqlExpr GetReqlAst(Expression predicate, QueryModel queryModel)
        {
            var visitor = new ExpressionVisitor(this, queryModel);
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
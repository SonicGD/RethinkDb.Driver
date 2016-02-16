using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using Remotion.Linq;
using Remotion.Linq.Clauses;
using Remotion.Linq.Clauses.Expressions;
using Remotion.Linq.Parsing;
using Remotion.Linq.Parsing.Structure;
using RethinkDb.Driver.Ast;
using RethinkDb.Driver.Net;

namespace RethinkDb.Driver.Linq
{
    public static class LinqUtil
    {
        public static IQueryParser CreateQueryParser()
        {
            
        }
    }

    public static class LinqExtensions
    {
        public static RethinkQueryable<T> AsQueryable<T>(this Table term)
        {
            var queryParser = LinqUtil.CreateQueryParser();
            var executor = new RethinkQueryExecutor(term);
            return new RethinkQueryable<T>(queryParser, executor);
        }

        public static RethinkQueryable<T> Table<T>(this Db db, string tableName)
        {
            return db.Table(tableName).AsQueryable<T>();
        }
    }

    

    public class RethinkQueryExecutor : IQueryExecutor
    {
        public RethinkQueryExecutor(Table table)
        {
        }

        public T ExecuteScalar<T>(QueryModel queryModel)
        {
            throw new System.NotImplementedException();
        }

        public T ExecuteSingle<T>(QueryModel queryModel, bool returnDefaultWhenEmpty)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<T> ExecuteCollection<T>(QueryModel queryModel)
        {
            return null;
        }
    }

    public class RethinkQueryable<T> : QueryableBase<T>
    {
        public RethinkQueryable(IQueryParser queryParser, IQueryExecutor executor) : base(queryParser, executor)
        {
        }

        public RethinkQueryable(IQueryProvider provider) : base(provider)
        {
        }

        public RethinkQueryable(IQueryProvider provider, Expression expression) : base(provider, expression)
        {
        }
    }


    public class ReqlModelVisitor : QueryModelVisitorBase
    {
        public Db Db;

        public ReqlAst Query { get; set; }

        public ReqlModelVisitor(Db db)
        {
            this.Db = db;
        }

        public override void VisitQueryModel(QueryModel queryModel)
        {
            base.VisitQueryModel(queryModel);
        }

        public override void VisitMainFromClause(MainFromClause fromClause, QueryModel queryModel)
        {
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
            base.VisitWhereClause(whereClause, queryModel, index);
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
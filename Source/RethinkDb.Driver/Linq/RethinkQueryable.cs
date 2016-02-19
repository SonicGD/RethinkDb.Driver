using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Remotion.Linq;
using Remotion.Linq.Parsing.Structure;
using RethinkDb.Driver.Ast;
using RethinkDb.Driver.Net;

namespace RethinkDb.Driver.Linq
{

    public static class LinqUtil
    {
        /*

        from o in Orders 
where o.Date == DateTime.Today 
select o

        STEP 1:
        PARTIAL EVALUATOR: is responsible for translating DateTime.Today into constant.

        STEP 2:
        TRANSFORMING PROCESSOR: is responsible for executing expression transformers.

            STEP 2 - 1:
            EXPRESSION TRANSFORMER: runs though the expression tree transforming
              nodes into different kinds of expressions. like Nullable<T> 
              gets transformed into NULL or T. The transformers are called 'inside out',
              i.e., child nodes are transformed before their parent and ancestor nodes. 
              When more than one transformer qualifies for the same expression,
              the transformers are called in a chain in the order of registration. 
              When a transformer changes the expression, the chain is aborted and 
              transformers are again chosen for the new expression (which may have a
              different type than the original one).


            

private static IQueryParser CreateQueryParser ()
{
  var customNodeTypeRegistry = new MethodInfoBasedNodeTypeRegistry();
  // Register custom node parsers here:
  // customNodeTypeRegistry.Register (MyExpressionNode.SupportedMethods, typeof (MyExpressionNode));
  // Alternatively, use the CreateFromTypes factory method.
  // Use MethodNameBasedNodeTypeRegistry to register parsers by query operator name instead of MethodInfo.

  var nodeTypeProvider = ExpressionTreeParser.CreateDefaultNodeTypeProvider ();
  nodeTypeProvider.InnerProviders.Add (customNodeTypeRegistry);

  var transformerRegistry = ExpressionTransformerRegistry.CreateDefault ();
  // Register custom expression transformers executed _after_ partial evaluation here (this should be the default):
  // transformerRegistry.Register (new MyExpressionTransformer());

  var processor = ExpressionTreeParser.CreateDefaultProcessor (transformerRegistry);

  // To register custom expression transformers executed _before_ partial evaluation, use this code:
  // var earlyTransformerRegistry = new ExpressionTransformerRegistry();
  // earlyTransformerRegistry.Register (new MyEarlyExpressionTransformer());
  // processor.InnerProcessors.Insert (0, new TransformingExpressionTreeProcessor (tranformationProvider));

  // Add custom processors here (use Insert (0, ...) to add at the beginning):
  // processor.InnerProcessors.Add (new MyExpressionTreeProcessor());

  var expressionTreeParser = new ExpressionTreeParser (nodeTypeProvider, processor);
  var queryParser = new QueryParser (expressionTreeParser);

  return queryParser;
}


            



        re-linq differentiates between clauses, which form the main part of
        a query, and result operators, which describe operations conducted
        on the result of the query. Query methods such as Select and Where are 
        parsed into clause objects, whereas methods such as Distinct, Take,
        or Count are parsed into result operator objects. Both clauses and 
        result operators are represented in the QueryModel that represents t
        he query analyzed and simplified by re-linq’s front-end.

            The distinction between result operators and clauses is important because it defines what goes into a sub-query, and what is simply appended to the current query. For example, consider the following query:

Query<Order>() 
  .Where (o => o.DeliveryDate <= DateTime.UtcNow) 
  .OrderByDescending (o => o.DeliveryDate) 
  .Take (5) 
  .OrderBy (o => o.OrderNumber)

In this case, the Where and OrderByDescending method calls are parsed into 
clauses of a single QueryModel. The Take method call is appended to that
QueryModel as a result operator. The OrderBy call following the Take call,
however, is parsed into a different, outer QueryModel, which embeds the former 
QueryModel as a sub-query.

 (It is necessary to form a sub-query in this case because the OrderBy
 operation must take place after the Take operation. This makes a huge semantic difference!)


            https://www.re-motion.org/blogs/mix/2009/09/02/how-to-write-a-linq-provider-the-simple-way-again/


*/
        public static IQueryParser CreateQueryParser()
        {
            return null;
        }
    }

    public static class LinqExtensions
    {
        public static RethinkQueryable<T> AsQueryable<T>(this Table term, IConnection conn)
        {
            var executor = new RethinkQueryExecutor(term, conn);
            return new RethinkQueryable<T>(
                new DefaultQueryProvider(
                    typeof(RethinkQueryable<>),
                    QueryParser.CreateDefault(),
                    executor)
                );
        }

        public static RethinkQueryable<T> Table<T>(this Db db, string tableName, IConnection conn)
        {
            return db.Table(tableName).AsQueryable<T>(conn);
        }
    }

    
    public class RethinkQueryExecutor : IQueryExecutor
    {
        private readonly Table table;
        private readonly IConnection conn;

        public RethinkQueryExecutor(Table table, IConnection conn)
        {
            this.table = table;
            this.conn = conn;
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
            var visitor = new ModelVisitor(table);

            visitor.VisitQueryModel(queryModel);

            var result = visitor.Query.RunCursor<T>(conn);

            return result;
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
}
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Remotion.Linq;
using Remotion.Linq.Clauses.Expressions;
using Remotion.Linq.Parsing;
using RethinkDb.Driver.Ast;

namespace RethinkDb.Driver.Linq
{

    /*
While you’re visiting the clauses and result operators, 
you’ll notice that some of them contain LINQ Expressions.
For example, WhereClause.Predicate contains an Expression,
SelectClause.Selector does, and even MainFromClause.FromExpression 
is an expression tree. Now, haven’t I said earlier that LINQ 
expressions are inherently complex and hard to understand?

They are, but the expressions you can find in re-linq’s 
clauses have already been simplified. In them,

references to outer variables (closures) and other evaluatable 
expressions have already been pre-evaluated into constants,
sub-queries have been parsed and replaced by QueryModels wrapped 
in SubQueryExpressions, and, most importantly,
transparent identifiers have been removed and references to query 
sources (from clauses, joins) have been replaced by 
QuerySourceReferenceExpressions, which link back to the respective 
query source. Therefore, the expressions you find in re-linq’s clauses 
are usually quite straight-forward to translate to the target query system.
Depending on the target query system, of course.

To implement the translation of expressions, you derive a
class from ExpressionTreeVisitor or, better, ThrowingExpressionTreeVisitor. Both of them are meant to iterate over an expression tree and to visit each of the nodes in the tree, but ThrowingExpressionTreeVisitor throws an exception for unsupported node types by default.

Simply override the Visit… methods for those node types you 
want to support, and generate a semantically equivalent query 
element for your target query system. Then, from your IQueryModelVisitor,
take the elements and integrate them into the current query part.
*/

    public static class ExprHelper
    {
        public static ReqlExpr TranslateUnary(ExpressionType type, ReqlExpr term)
        {
            switch( type )
            {
                case ExpressionType.Not:
                    return term.Not();
                default:
                    throw new NotSupportedException("Unary term not supported.");
            }
        }

        public static ReqlExpr TranslateBinary(ExpressionType type, ReqlExpr left, ReqlExpr right)
        {
            switch( type )
            {
                case ExpressionType.Equal:
                    return left.Eq(right);
                case ExpressionType.NotEqual:
                    return left.Eq(right).Not();
                case ExpressionType.LessThan:
                    return left.Lt(right);
                case ExpressionType.LessThanOrEqual:
                    return left.Le(right);
                case ExpressionType.GreaterThan:
                    return left.Gt(right);
                case ExpressionType.GreaterThanOrEqual:
                    return left.Ge(right);
                case ExpressionType.And:
                case ExpressionType.AndAlso:
                    return left.And(right);
                case ExpressionType.Or:
                case ExpressionType.OrElse:
                    return left.Or(right);
                case ExpressionType.Not:
                    throw new InvalidOperationException("ExpresionType:Not cannot be called on a binary translation.");
                case ExpressionType.Add:
                    return left.Add(right);
                case ExpressionType.Subtract:
                    return left.Sub(right);
                case ExpressionType.Multiply:
                    return left.Mul(right);
                case ExpressionType.Divide:
                    return left.Div(right);
                case ExpressionType.Modulo:
                    return left.Mod(right);
                default:
                    throw new NotSupportedException("Binary expression not supported.");
            }
        }
        
    }

    public class ExpressionVisitor : ThrowingExpressionVisitor
    {
        private readonly ModelVisitor modelVisitor;
        private readonly QueryModel queryModel;

        private Stack<ReqlExpr> stack = new Stack<ReqlExpr>();

        public ExpressionVisitor(ModelVisitor modelVisitor, QueryModel queryModel)
        {
            this.modelVisitor = modelVisitor;
            this.queryModel = queryModel;
        }

        public ReqlExpr Current => this.stack.Peek();

        protected override Exception CreateUnhandledItemException<T>(T unhandledItem, string visitMethod)
        {
            string itemText = unhandledItem.ToString();
            var message = $"The expression '{itemText}' (type: {typeof(T)}) is not supported by RethinkDB LINQ provider.";
            return new NotSupportedException(message);
        }

        public override Expression Visit(Expression expression)
        {
            return base.Visit(expression);
        }

        protected override Expression VisitUnknownStandardExpression(Expression expression, string visitMethod, Func<Expression, Expression> baseBehavior)
        {
            return base.VisitUnknownStandardExpression(expression, visitMethod, baseBehavior);
        }

        protected override Expression VisitBinary(BinaryExpression expression)
        {
            Visit(expression.Left);
            var left = stack.Pop();

            Visit(expression.Right);
            var right = stack.Pop();

            stack.Push(ExprHelper.TranslateBinary(expression.NodeType, left, right));

            return expression;
        }

        protected override Expression VisitBlock(BlockExpression expression)
        {
            return base.VisitBlock(expression);
        }

        protected override Expression VisitConditional(ConditionalExpression expression)
        {
            return base.VisitConditional(expression);
        }

        protected override Expression VisitConstant(ConstantExpression expression)
        {
            stack.Push(Util.ToReqlExpr(expression.Value));
            return base.VisitConstant(expression);
        }

        protected override Expression VisitDebugInfo(DebugInfoExpression expression)
        {
            return base.VisitDebugInfo(expression);
        }

        protected override Expression VisitDynamic(DynamicExpression node)
        {
            return base.VisitDynamic(node);
        }

        protected override Expression VisitDefault(DefaultExpression expression)
        {
            return base.VisitDefault(expression);
        }

        protected override TResult VisitUnhandledItem<TItem, TResult>(TItem unhandledItem, string visitMethod, Func<TItem, TResult> baseBehavior)
        {
            return base.VisitUnhandledItem(unhandledItem, visitMethod, baseBehavior);
        }

        protected override Expression VisitExtension(Expression expression)
        {

            https://www.re-motion.org/blogs/mix/2010/02/18/net-4-0-expression-trees-extension-expressions/

            return base.VisitExtension(expression);
        }

        protected override Expression VisitGoto(GotoExpression expression)
        {
            return base.VisitGoto(expression);
        }

        protected override Expression VisitInvocation(InvocationExpression expression)
        {
            return base.VisitInvocation(expression);
        }

        protected override LabelTarget VisitLabelTarget(LabelTarget expression)
        {
            return base.VisitLabelTarget(expression);
        }

        protected override Expression VisitLabel(LabelExpression expression)
        {
            return base.VisitLabel(expression);
        }

        protected override Expression VisitLambda<T>(Expression<T> expression)
        {
            return base.VisitLambda(expression);
        }

        protected override Expression VisitLoop(LoopExpression expression)
        {
            return base.VisitLoop(expression);
        }

        protected override Expression VisitMember(MemberExpression expression)
        {
            var propName = expression.Member.Name;

            



            return base.VisitMember(expression);
        }

        protected override Expression VisitNewArray(NewArrayExpression expression)
        {
            return base.VisitNewArray(expression);
        }

        protected override Expression VisitNew(NewExpression expression)
        {
            return base.VisitNew(expression);
        }

        protected override Expression VisitSubQuery(SubQueryExpression expression)
        {
            throw new NotSupportedException("subqueries not allowed .");
        }

        protected override Expression VisitQuerySourceReference(QuerySourceReferenceExpression expression)
        {
            return base.VisitQuerySourceReference(expression);
        }

        protected override Expression VisitParameter(ParameterExpression expression)
        {
            return base.VisitParameter(expression);
        }

        protected override Expression VisitRuntimeVariables(RuntimeVariablesExpression expression)
        {
            return base.VisitRuntimeVariables(expression);
        }

        protected override SwitchCase VisitSwitchCase(SwitchCase expression)
        {
            return base.VisitSwitchCase(expression);
        }

        protected override Expression VisitIndex(IndexExpression expression)
        {
            return base.VisitIndex(expression);
        }

        protected override Expression VisitMethodCall(MethodCallExpression expression)
        {
            return base.VisitMethodCall(expression);
        }

        protected override MemberMemberBinding VisitMemberMemberBinding(MemberMemberBinding binding)
        {
            return base.VisitMemberMemberBinding(binding);
        }

        protected override MemberListBinding VisitMemberListBinding(MemberListBinding listBinding)
        {
            return base.VisitMemberListBinding(listBinding);
        }

        protected override Expression VisitSwitch(SwitchExpression expression)
        {
            return base.VisitSwitch(expression);
        }

        protected override CatchBlock VisitCatchBlock(CatchBlock expression)
        {
            return base.VisitCatchBlock(expression);
        }

        protected override Expression VisitTry(TryExpression expression)
        {
            return base.VisitTry(expression);
        }

        protected override Expression VisitTypeBinary(TypeBinaryExpression expression)
        {
            return base.VisitTypeBinary(expression);
        }

        protected override Expression VisitUnary(UnaryExpression expression)
        {
            return base.VisitUnary(expression);
        }

        protected override Expression VisitMemberInit(MemberInitExpression expression)
        {
            return base.VisitMemberInit(expression);
        }

        protected override Expression VisitListInit(ListInitExpression expression)
        {
            return base.VisitListInit(expression);
        }

        protected override ElementInit VisitElementInit(ElementInit elementInit)
        {
            return base.VisitElementInit(elementInit);
        }

        protected override MemberBinding VisitMemberBinding(MemberBinding expression)
        {
            return base.VisitMemberBinding(expression);
        }

        protected override MemberAssignment VisitMemberAssignment(MemberAssignment memberAssigment)
        {
            return base.VisitMemberAssignment(memberAssigment);
        }
    }
}
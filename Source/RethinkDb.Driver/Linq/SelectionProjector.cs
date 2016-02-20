using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using Remotion.Linq.Clauses.Expressions;
using Remotion.Linq.Parsing;
using RethinkDb.Driver.Ast;

namespace RethinkDb.Driver.Linq
{

    //internal class ColumnProjection
    //{
    //    internal string Columns;
    //    internal Expression Selector;
    //}

    //internal class ColumnProjector : ExpressionVisitor
    //{
    //    StringBuilder sb;
    //    int iColumn;
    //    ParameterExpression row;
    //    static MethodInfo getValueMethodInfo;

    //    internal ColumnProjector()
    //    {
    //        if (getValueMethodInfo == null)
    //        {
    //            getValueMethodInfo = typeof(ProjectionRow).GetMethod("GetValue");
    //        }
    //    }

    //    internal ColumnProjection ProjectColumns(Expression expression, ParameterExpression row)
    //    {
    //        this.sb = new StringBuilder();
    //        this.row = row;
    //        Expression selector = this.Visit(expression);
    //        return new ColumnProjection { Columns = this.sb.ToString(), Selector = selector };
    //    }

    //    protected override Expression VisitMemberAccess(MemberExpression m)
    //    {
    //        if (m.Expression != null && m.Expression.NodeType == ExpressionType.Parameter)
    //        {
    //            if (this.sb.Length > 0)
    //            {
    //                this.sb.Append(", ");
    //            }
    //            this.sb.Append(m.Member.Name);
    //            return Expression.Convert(Expression.Call(this.row, getValueMethodInfo, Expression.Constant(iColumn++)), m.Type);
    //        }
    //        else {
    //            return base.VisitMemberAccess(m);
    //        }
    //    }
    //}

    /// <summary>
    /// basically, what we need to do in here is figure
    /// if we need to perform any result transformations 
    /// due to the selection clause. IE: map() or 
    /// no transformation.
    /// </summary>
    public class SelectionProjector : ThrowingExpressionVisitor
    {
        public SelectionProjector()
        {
        }

        private Stack<ReqlExpr> stack = new Stack<ReqlExpr>();

        public ReqlExpr Current => this.stack.Count > 0 ? this.stack.Peek() : null;

        protected override Exception CreateUnhandledItemException<T>(T unhandledItem, string visitMethod)
        {
            throw new NotImplementedException();
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
            return base.VisitBinary(expression);
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
            return base.VisitSubQuery(expression);
        }

        protected override Expression VisitQuerySourceReference(QuerySourceReferenceExpression expression)
        {
            return expression;
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
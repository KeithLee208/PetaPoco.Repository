//
// Copyright (c) Artur Durasiewicz. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
//

using System;
using System.Linq.Expressions;

namespace PetaPoco.Repository
{
    public class FluentFilter<T>
    {
        public FluentFilter()
        {
            query = new Sql();
            order = new Sql();
            isFirstOrdering = true;
        }

        public FluentFilter<T> Where(Expression<Func<T, bool>> expression)
        {
            query.Append("WHERE");
            query.Append(BuildCondition(expression));
            HasInitialCondition = true;
            return this;
        }

        public FluentFilter<T> And(Expression<Func<T, bool>> expression)
        {
            if (!HasInitialCondition)
                throw new FilterBuilderException("Filter should start with Where() condition");

            query.Append("AND");
            query.Append(BuildCondition(expression));
            return this;
        }

        public FluentFilter<T> Or(Expression<Func<T, bool>> expression)
        {
            if (!HasInitialCondition)
                throw new FilterBuilderException("Filter should start with Where() condition");

            query.Append("OR");
            query.Append(BuildCondition(expression));
            return this;
        }

        public FluentFilter<T> Order()
        {
            order.Append("ORDER BY");
            return this;
        }

        public FluentFilter<T> By(Expression<Func<T, object>> expression, bool ascending = true)
        {
            var lambda = expression as LambdaExpression;

            if (isFirstOrdering == false)
                order.Append(",");

            if (lambda.Body is UnaryExpression)
                order.Append(((lambda.Body as UnaryExpression).Operand as MemberExpression).Member.Name);
            else if (lambda.Body is MemberExpression)
                order.Append((lambda.Body as MemberExpression).Member.Name);

            order.Append(ascending ? "ASC" : "DESC");

            isFirstOrdering = false;

            return this;
        }

        private Sql BuildCondition(Expression<Func<T, bool>> expression)
        {
            Sql result = new Sql();

            var lambda = expression as LambdaExpression;
            result.Append(GetLeftSide(lambda));
            result.Append(GetOperand(lambda.Body.NodeType, IsNullExpr(lambda), IsLikeExpr(lambda)));
            result.Append(GetRightSide(lambda));

            return result;
        }

        private Sql GetLeftSide(LambdaExpression expr)
        {
            Sql result = new Sql();

            var exprBody = expr.Body as BinaryExpression;
            result.Append((exprBody.Left as MemberExpression).Member.Name);

            return result;
        }

        private Sql GetRightSide(LambdaExpression expr)
        {
            var exprBody = expr.Body as BinaryExpression;

            Sql result = new Sql();

            if (exprBody.Right is ConstantExpression)
                if ((exprBody.Right as ConstantExpression).Value == null)
                    result.Append("NULL");
                else
                    result.Append("@0", (exprBody.Right as ConstantExpression).Value);
            else if (exprBody.Right is MemberExpression)
                result.Append((exprBody.Right as MemberExpression).Member.Name);
            else
                throw new InvalidOperationException("Unsupported lambda expression");

            return result;
        }

        private bool IsLikeExpr(LambdaExpression expr)
        {
            var binaryExpression = expr.Body as BinaryExpression;
            var rightSide = binaryExpression.Right as ConstantExpression;

            if (rightSide != null &&
                rightSide.Value is string &&
                ((rightSide.Value as string).Contains("%") || (rightSide.Value as string).Contains("_")))
                return true;

            return false;
        }

        private bool IsNullExpr(LambdaExpression expr)
        {
            var exprBody = expr.Body as BinaryExpression;
            if (exprBody.Right is ConstantExpression)
                if ((exprBody.Right as ConstantExpression).Value == null)
                    return true;

            return false;
        }

        private Sql GetOperand(ExpressionType expType, bool isNullExpr = false, bool isLikeExpr = false)
        {
            Sql result = new Sql();

            switch (expType)
            {
                case ExpressionType.GreaterThan:
                    result.Append(">");
                    break;
                case ExpressionType.LessThan:
                    result.Append("<");
                    break;
                case ExpressionType.Equal:
                    if (isNullExpr)
                        result.Append("IS");
                    else if (isLikeExpr)
                        result.Append("LIKE");
                    else
                        result.Append("=");
                    break;
                case ExpressionType.GreaterThanOrEqual:
                    result.Append(">=");
                    break;
                case ExpressionType.LessThanOrEqual:
                    result.Append("<=");
                    break;
                case ExpressionType.NotEqual:
                    if (isNullExpr)
                        result.Append("IS NOT");
                    else
                        result.Append("<>");
                    break;
                default:
                    throw new InvalidOperationException("Unsupported operand: " + expType.ToString());
            }

            return result;
        }

        private Sql order;
        private Sql query;
        private bool isFirstOrdering;

        public Sql Query { get { return new Sql().Append(query).Append(order); } }
        public bool HasInitialCondition { get; private set; }
    }
}

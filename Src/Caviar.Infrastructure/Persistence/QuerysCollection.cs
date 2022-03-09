using Caviar.SharedKernel.Entities.View;
using System;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using System.Reflection;

namespace Caviar.Infrastructure
{
    public class QueryCollection : Collection<QueryModel>
    {
        public Expression<Func<T, bool>> AsExpression<T>() where T : class
        {
            var targetType = typeof(T);
            var typeInfo = targetType.GetTypeInfo();
            var parameter = Expression.Parameter(targetType, "m");
            Expression expression = null;
            foreach (var item in this)
            {
                var property = typeInfo.GetProperty(item.Key);
                Type realType = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
                var value = Convert.ChangeType(item.Value, property.PropertyType);
                Expression<Func<object>> valueLamba = () => value;
                switch (item.QuerTypes)
                {
                    case QueryModel.QuerType.Equal:
                        expression = Append(item.QuerySplicings, expression, Expression.Equal(Expression.Property(parameter, item.Key),
                                Expression.Convert(valueLamba.Body, property.PropertyType)));
                        break;
                    case QueryModel.QuerType.LessThan:
                        expression = Append(item.QuerySplicings, expression, Expression.LessThan(Expression.Property(parameter, item.Key),
                                Expression.Convert(valueLamba.Body, property.PropertyType)));
                        break;
                    case QueryModel.QuerType.LessThanOrEqual:
                        expression = Append(item.QuerySplicings, expression, Expression.LessThanOrEqual(Expression.Property(parameter, item.Key),
                                Expression.Convert(valueLamba.Body, property.PropertyType)));
                        break;
                    case QueryModel.QuerType.GreaterThan:
                        expression = Append(item.QuerySplicings, expression, Expression.GreaterThan(Expression.Property(parameter, item.Key),
                                Expression.Convert(valueLamba.Body, property.PropertyType)));
                        break;
                    case QueryModel.QuerType.GreaterThanOrEqual:
                        expression = Append(item.QuerySplicings, expression, Expression.GreaterThanOrEqual(Expression.Property(parameter, item.Key),
                                Expression.Convert(valueLamba.Body, property.PropertyType)));
                        break;
                    case QueryModel.QuerType.NotEqual:
                        expression = Append(item.QuerySplicings, expression, Expression.NotEqual(Expression.Property(parameter, item.Key),
                                Expression.Convert(valueLamba.Body, property.PropertyType)));
                        break;
                    case QueryModel.QuerType.Contains:
                        var nullCheck = Expression.Not(Expression.Call(typeof(string), "IsNullOrEmpty", null, Expression.Property(parameter, item.Key)));
                        var contains = Expression.Call(Expression.Property(parameter, item.Key), "Contains", null,
                            Expression.Convert(valueLamba.Body, property.PropertyType));
                        expression = Append(item.QuerySplicings, expression, Expression.AndAlso(nullCheck, contains));
                        break;
                    default:
                        break;
                }
            }
            if (expression == null)
            {
                return null;
            }
            return ((Expression<Func<T, bool>>)Expression.Lambda(expression, parameter));
        }

        Expression Append(QueryModel.QuerySplicing condition, Expression expression1, Expression expression2)
        {
            if (expression1 == null)
            {
                return expression2;
            }
            return condition == QueryModel.QuerySplicing.And ? Expression.AndAlso(expression1, expression2) : Expression.OrElse(expression1, expression2);
        }
    }
}
using System;
using System.Linq.Expressions;
using System.Reflection;
using Caliburn.Micro;

namespace CQRS.Sample1.Client
{
    public class ExtendedScreen : Screen
    {
        protected TProperty GetValue<TProperty>(Expression<Func<TProperty>> propertyExpression)
        {
            var member = propertyExpression.Body as MemberExpression;
            if (member != null)
            {
                object basePropertyValue;
                if (member.Expression is MemberExpression)
                {
                    basePropertyValue = Expression.Lambda<Func<object>>(member.Expression, new ParameterExpression[0]).Compile()();
                }
                else
                {
                    basePropertyValue = propertyExpression.Compile()();
                }

                if (basePropertyValue != null)
                {
                    var propertyInfo = member.Member as PropertyInfo;
                    if (propertyInfo != null) return (TProperty)propertyInfo.GetValue(basePropertyValue, new object[0]);

                    var fieldInfo = member.Member as FieldInfo;
                    if (fieldInfo != null) return (TProperty)fieldInfo.GetValue(basePropertyValue);
                }
                else
                {
                    return default(TProperty);
                }
            }

            throw new ArgumentException("Only property or field type member expressions are supported.", "propertyExpression");
        }
        protected void SetValue<TProperty>(Expression<Func<TProperty>> propertyExpression, TProperty value)
        {
            var member = propertyExpression.Body as MemberExpression;
            if (member != null)
            {
                object basePropertyValue;
                if (member.Expression is MemberExpression)
                {
                    basePropertyValue = Expression.Lambda<Func<object>>(member.Expression, new ParameterExpression[0]).Compile()();
                }
                else
                {
                    basePropertyValue = propertyExpression.Compile()();
                }

                if (basePropertyValue != null)
                {
                    var propertyInfo = member.Member as PropertyInfo;
                    if (propertyInfo != null) propertyInfo.SetValue(basePropertyValue, value, new object[0]);

                    var fieldInfo = member.Member as FieldInfo;
                    if (fieldInfo != null) fieldInfo.SetValue(basePropertyValue, value);

                    return;
                }
            }

            throw new ArgumentException("Only property or field type member expressions are supported.", "propertyExpression");
        }

    }
}

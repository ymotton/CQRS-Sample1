using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;

namespace CQRS.Sample1.Process
{
    public class NotifyPropertyChanged : INotifyPropertyChanged
    {
        private IDictionary<string, object> _propertyValues = new Dictionary<string, object>();

        protected TProperty GetValue<TProperty>(Expression<Func<TProperty>> propertyExpression)
        {
            var member = propertyExpression.Body as MemberExpression;
            if (member != null)
            {
                object propertyValue;
                if (_propertyValues.TryGetValue(member.Member.Name, out propertyValue))
                {
                    return (TProperty) propertyValue;
                }

                return default(TProperty);
            }

            throw new ArgumentException("Only property member expressions are supported.", "propertyExpression");
        }
        protected void SetValue<TProperty>(Expression<Func<TProperty>> propertyExpression, TProperty value)
        {
            var member = propertyExpression.Body as MemberExpression;
            if (member != null)
            {
                if (member.Expression is MemberExpression)
                {
                    throw new ArgumentException("Nested property expressions are not supported", "propertyExpression");
                }

                _propertyValues[member.Member.Name] = value;

                NotifyOfPropertyChange(propertyExpression);

                return;
            }

            throw new ArgumentException("Only property member expressions are supported.", "propertyExpression");
        }

        protected void NotifyOfPropertyChange<TProperty>(Expression<Func<TProperty>> propertyExpression)
        {
            var member = propertyExpression.Body as MemberExpression;
            if (member != null)
            {
                RaisePropertyChanged(member.Member.Name);
            }

        }
        protected void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };
    }
}

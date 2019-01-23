using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace TerraSketch.Presenters
{
    public abstract class ABasePresenter : INotifyPropertyChanged, IDisposable
    {

        public ABasePresenter()
        {
            PropertyChanged += (object sender, PropertyChangedEventArgs e) =>
            {
                Action action;
                if(_actions.TryGetValue(e.PropertyName, out action))
                    action();
            };

        }
        private IDictionary<string, Action> _actions = new Dictionary<string, Action>();

        /// <summary>
        /// Register action and immediately calls it.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="propertyExpression"></param>
        /// <param name="action"></param>
        public void RegisterAction<T>(Expression<Func<T>> propertyExpression, Action action)
        {
            var me = propertyExpression.Body as MemberExpression;
            if (me == null) throw new InvalidOperationException("Delegate must be MemberExpression");
            var name =me.Member.Name;

            _actions.Add(name, action);
            
        }

        protected void NotifyPropertyChanged<T>(Expression<Func<T>> propertyExpression)
        {
            var me = propertyExpression.Body as MemberExpression;
            if (me == null) throw new InvalidOperationException("Delegate must be MemberExpression");
            var name = me.Member.Name;
            if(PropertyChanged !=null)
            PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if(PropertyChanged !=null)
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if(disposing)
            {
                // TODO prevent memory leaks
                foreach (var eve in PropertyChanged.GetInvocationList())
                {
                    PropertyChanged -= (PropertyChangedEventHandler)eve;
                }
            }
        }
        /// <summary>
        /// Be careful what you register here!
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
    }
}

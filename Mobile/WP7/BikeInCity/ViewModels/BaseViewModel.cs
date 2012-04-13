using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.ComponentModel;
using System.Linq.Expressions;

namespace BikeInCity.ViewModels
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// This methods allows raising the PropertyChanged event based on the lambda expression containing the property.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="property"></param>
        protected void OnPropertyChanged<T>(Expression<Func<T>> property)
        {
            var expression = property.Body as MemberExpression;
            var member = expression.Member;

            if (this.PropertyChanged != null)
                Deployment.Current.Dispatcher.BeginInvoke(() => 
                {
                    if (this.PropertyChanged != null)
                    {
                        this.PropertyChanged(this, new PropertyChangedEventArgs(member.Name));
                    }
                });
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}

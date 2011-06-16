using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Windows;
using System.Windows.Controls;
using Caliburn.Micro;

namespace CQRS.Sample1.Client.Controls
{
    /// <summary>
    /// Interaction logic for ConfirmableTextBox.xaml
    /// </summary>
    public partial class ConfirmableTextBox : UserControl, INotifyPropertyChanged
    {
        #region Properties

        public bool TextNonEditable
        {
            get { return !TextEditable; }
        }
        public bool TextEditable
        {
            get { return _textEditable; }
            set
            {
                _textEditable = value;
                NotifyOfPropertyChange(() => TextEditable);
                NotifyOfPropertyChange(() => TextNonEditable);
            }
        }
        private bool _textEditable;

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register(
                "Text",
                typeof(string),
                typeof(ConfirmableTextBox),
                new FrameworkPropertyMetadata("", FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public Action<string> Saved
        {
            get { return (Action<string>)GetValue(SavedProperty); }
            set { SetValue(SavedProperty, value); }
        }
        public static readonly DependencyProperty SavedProperty =
            DependencyProperty.Register(
                "Saved",
                typeof(Action<string>),
                typeof(ConfirmableTextBox));

        #endregion

        #region Ctor

        public ConfirmableTextBox()
        {
            InitializeComponent();
            
            TextEditable = false;
        }

        #endregion

        public void EditClick(object sender, RoutedEventArgs e)
        {
            TextEditable = true;
        }
        public void SaveClick(object sender, RoutedEventArgs e)
        {
            TextEditable = false;

            if (Saved != null)
            {
                Saved(textBox.Text);
            }
        }
        public void CancelClick(object sender, RoutedEventArgs e)
        {
            TextEditable = false;
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged = delegate {};
        private void NotifyOfPropertyChange<TProperty>(Expression<Func<TProperty>> property)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(property.GetMemberInfo().Name));
        }

        #endregion
    }
}

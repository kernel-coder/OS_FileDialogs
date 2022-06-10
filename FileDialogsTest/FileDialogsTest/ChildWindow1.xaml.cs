using GalaSoft.MvvmLight.Command;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace FileDialogsTest
{
    public partial class ChildWindow1 : ChildWindow
    {
        public ChildWindow1()
        {
            InitializeComponent();
            DataContext = new ChildWindowVM();
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }

    public class ChildWindowVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void NotifyPropChanged(string propname)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propname));
        }

        private bool? _dlgResult;

        public bool? DlgResult
        {
            get { return _dlgResult; }
            set
            {
                if (_dlgResult != value)
                {
                    _dlgResult = value;
                    NotifyPropChanged(nameof(DlgResult));
                }
            }
        }

        public ICommand DenyCommand => new RelayCommand(() => { DlgResult = false; }, () => true);
        public ICommand OkCommand => new RelayCommand(() => { DlgResult = true; }, () => true);
    }
}


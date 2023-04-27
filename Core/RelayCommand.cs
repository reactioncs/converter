using System;
using System.Windows.Input;

namespace Converter.Core
{
    public class RelayCommand : ICommand
    {
        private Action<object> mExecute;
        private Func<object, bool> mCanExecute;

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public RelayCommand(Action<object> mExecute, Func<object, bool> mCanExecute = null)
        {
            this.mExecute = mExecute;
            this.mCanExecute = mCanExecute;
        }

        public bool CanExecute(object parameter) => mCanExecute == null || mCanExecute(parameter);

        public void Execute(object parameter) => mExecute(parameter);
    }
}

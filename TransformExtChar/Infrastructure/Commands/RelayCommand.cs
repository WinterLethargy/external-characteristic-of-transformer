using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace TransformExtChar.Infrastructure.Command
{
    public class RelayCommand : ICommand
    {
        private Action<object> execute;
        private Func<object, bool> canExecute;
        private string _text;
        public string Text { get => _text; }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public RelayCommand(Action<object> execute, Func<object, bool> canExecute = null, string text = null)
        {
            this.execute = execute;
            this.canExecute = canExecute;
            _text = text;
        }

        public bool CanExecute(object parameter)
        {
            return this.canExecute == null || this.canExecute(parameter);
        }

        public void Execute(object parameter)
        {
            this.execute(parameter);
        }
    }
}

using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CsvImport.Wpf.Mvvm
{
    public sealed class Command : ICommand
    {
        private readonly Func<object, Task> _action;
        private bool _enabled;

        public Command(Func<object, Task> action, bool enabled = true)
        {
            _action = action;
            _enabled = enabled;
        }

        public bool Enabled
        {
            get { return _enabled; }
            set
            {
                _enabled = value;
                CanExecuteChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public bool CanExecute(object parameter)
        {
            return _enabled;
        }

        public async void Execute(object parameter)
        {
            await _action(parameter);
        }

        public event EventHandler CanExecuteChanged;
    }
}
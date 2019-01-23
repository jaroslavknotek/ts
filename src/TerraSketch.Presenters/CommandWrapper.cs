using System;
using System.Threading.Tasks;
using TerraSketch.Presenters.Interfaces;

namespace TerraSketch.Presenters
{
    public class CommandWrapper : ICommandWrapper
    {
        private readonly Predicate<object> _ce;
        private readonly Action<object> _e;
        private readonly Func<object, Task> _awaitableE;
        public CommandWrapper(Predicate<object> canExec, Func<object, Task> exec)
        {
            _ce = canExec;
            _awaitableE = exec;
        }

        public CommandWrapper(Predicate<object> canExec, Action<object> exec)
        {
            _ce = canExec;
            _e = exec;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return _ce(parameter);
        }

        public async Task Execute(object parameter)
        {
            if (_awaitableE != null)
                await _awaitableE(parameter);
            else
                _e(parameter);
        }

        public void OnCanExecuteChanged()
        {
            if (CanExecuteChanged != null)
                CanExecuteChanged(this, new EventArgs());
        }
    }
}

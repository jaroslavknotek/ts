using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace TerraSketch.Presenters.Interfaces
{
    public interface ICommandWrapper
    {
        bool CanExecute(object parameter);

        Task Execute(object parameter);
        void OnCanExecuteChanged();

        event EventHandler CanExecuteChanged;

    }
}

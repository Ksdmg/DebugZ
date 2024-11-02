using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DayZModdingToolbox.Common
{
    /// <summary>
    /// Simple bindable proxy class that executes <see cref="Action"/> if invoked on button press.
    /// This class can take parameters for <see cref="CanExecuteDelegate"/> and <see cref="ExecuteDelegate"/>.
    /// </summary>
    public class CommandWithParameter : ICommand
    {
        /// <inheritdoc/>
        public event EventHandler? CanExecuteChanged;

        /// <summary>
        /// Gets or sets the delegate that is executed by <see cref="CanExecute(object)"/>.
        /// </summary>
        public Predicate<object?>? CanExecuteDelegate { get; set; }

        /// <summary>
        /// Gets or sets the delegate that is executed by <see cref="Execute"/>.
        /// </summary>
        public Action<object?>? ExecuteDelegate { get; set; }

        /// <inheritdoc/>
        public bool CanExecute(object? parameter)
        {
            if (this.CanExecuteDelegate == null)
            {
                return false;
            }
            return this.CanExecuteDelegate.Invoke(parameter);
        }

        /// <inheritdoc/>
        public void Execute(object? parameter)
        {
            if (this.ExecuteDelegate == null)
            {
                return;
            }
            this.ExecuteDelegate.Invoke(parameter);
        }
    }
}
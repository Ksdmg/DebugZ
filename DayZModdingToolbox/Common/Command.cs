namespace DayZModdingToolbox.Common
{
    using System;
    using System.Windows.Input;

    /// <summary>
    /// Simple bindable proxy class that executes <see cref="Action"/> if invoked on button press.
    /// </summary>
    public class Command : ICommand
    {
        private readonly Action action;

        /// <summary>
        /// Initializes a new instance of the <see cref="Command"/> class.
        /// </summary>
        /// <param name="action">
        /// Delegate to execute during <see cref="ICommand.Execute(object?)"/>. Parameters are ignored.
        /// </param>
        public Command(Action action)
        {
            this.action = action;
        }

#pragma warning disable 67 // The event CommandWithParameter.CanExecuteChanged is never used

        /// <inheritdoc/>
        public event EventHandler CanExecuteChanged;

#pragma warning restore 67

        /// <summary>
        /// Returns always true.
        /// </summary>
        /// <param name="parameter">Parameter is ignored.</param>
        /// <returns>Always <see langword="true"/>.</returns>
        public bool CanExecute(object parameter)
        {
            return true;
        }

        /// <summary>
        /// Defines the method to be called when the command is invoked.
        /// </summary>
        /// <param name="parameter">This parameter is ignored.</param>
        public void Execute(object parameter)
        {
            this.action();
        }
    }
}

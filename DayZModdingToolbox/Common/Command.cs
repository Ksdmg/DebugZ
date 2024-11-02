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

        /// <inheritdoc/>
        public event EventHandler? CanExecuteChanged;

        /// <inheritdoc/>
        public bool CanExecute(object? parameter)
        {
            return true;
        }

        /// <inheritdoc/>
        public void Execute(object? parameter)
        {
            this.action();
        }
    }
}
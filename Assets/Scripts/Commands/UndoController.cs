using System.Collections.Generic;

namespace UndoControl.Commands
{
    public interface ICommand
    {
        void Redo();
        void Undo();
    }
    
    public class UndoController
    {
        private readonly Stack<ICommand> _commands = new();
        public bool CanUndo => _commands.Count > 0;

        public void Add(ICommand command)
        {
            if (command == null)
                return;

            _commands.Push(command);
        }

        public void Undo()
        {
            if (_commands.Count > 0)
            {
                var command = _commands.Pop();
                command.Undo();
            }
        }

        public void Clear()
        {
            _commands.Clear();
        }
    }
}
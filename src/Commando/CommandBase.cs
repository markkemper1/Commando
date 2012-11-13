using System;

namespace Commando
{
	public abstract class CommandBase : ICommand, ICompositeCommand
	{
        public ICommandExecutor Executor { get; set; }

        public abstract void Execute();
    }
}
using System;
using System.Diagnostics;

namespace Commando.Test
{
	public class CommandTestBase : ICommandExecutor
	{
		private ICommandExecutor Executor;

		public FakableCommandExecutor FakableExecutor { get; set; }

		public CommandTestBase()
		{
			this.SetupExecutor();
		}

		public virtual void SetupExecutor(CommandExecutor commandExecutor = null)
		{
			this.Executor = commandExecutor ?? new CommandExecutor();
			this.FakableExecutor = new FakableCommandExecutor(this.Executor);
			((CommandExecutor)this.Executor).ExecutorOverride = this.FakableExecutor;
		}

		public void Execute(ICommand command)
		{
			this.Executor.Execute(command);
		}

		public T Execute<T>(ICommandResult<T> command)
		{
			return this.Executor.Execute<T>(command);
		}
	}
}

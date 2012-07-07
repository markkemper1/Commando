using System.Diagnostics;

namespace Commando
{
	public abstract class CommandResultBase<T> : ICommandResult<T>, ICompositeCommand
	{
		[DebuggerNonUserCode]
		public void Execute()
		{
			this.Result = this.ExecuteWithResult();
		}

		public abstract T ExecuteWithResult();

		public ICommandExecutor Executor { get; set; }

		public T Result { get; protected set; }
	}
}
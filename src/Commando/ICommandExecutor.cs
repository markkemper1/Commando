using System;

namespace Commando
{
	public interface ICommandExecutor
	{
		void Execute(ICommand command);
		T Execute<T>(ICommandResult<T> command);
	}
}
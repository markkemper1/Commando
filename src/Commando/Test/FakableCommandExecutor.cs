using System;
using System.Collections.Generic;
using System.Linq;

namespace Commando.Test
{
	public class FakableCommandExecutor : ICommandExecutor
	{
		private readonly ICommandExecutor realExecutor;
		private Dictionary<Type, Queue<object>> cannedResults = new Dictionary<Type, Queue<object>>();
		private Dictionary<Type, Queue<ICommand>> executedCommands = new Dictionary<Type, Queue<ICommand>>();
		
		public FakableCommandExecutor(ICommandExecutor realExecutor)
		{
			this.realExecutor = realExecutor;
		}

		public void SetupResult<T>(object result)
		{
			var key = typeof (T);
			
			if(!cannedResults.ContainsKey(key))
				cannedResults[key] = new Queue<object>();

			cannedResults[key].Enqueue(result);
		}

		public void Execute(ICommand command)
		{
			var key = command.GetType();

			if(!this.executedCommands.ContainsKey(key))
				this.executedCommands[key] = new Queue<ICommand>();

			this.executedCommands[key].Enqueue(command);

			if (cannedResults.ContainsKey(key) && cannedResults[key].Count > 0)
				return;
			
			realExecutor.Execute(command);
		}

		public T Execute<T>(ICommandResult<T> command)
		{
			var key = command.GetType();

			this.Execute((ICommand)command);

			if (cannedResults.ContainsKey(key) && cannedResults[key].Count > 0)
			{
				if (!(cannedResults[key].Peek() is T))
				{
					throw new InvalidOperationException(String.Format("An invalid return type has been set. The command: \"{0}\" requires a return type of \"{1}\"", key.Name, typeof(T).Name));
				}
				return (T)cannedResults[key].Dequeue();

			}
			else
				return command.Result;
		}

		public T GetExecutedCommand<T>()
		{
			var key = typeof (T);

			if(!executedCommands.ContainsKey(key))
				throw new InvalidOperationException("The command: " + key.Name + " was not executed");

			return (T)executedCommands[key].Dequeue();
		}
	}
}

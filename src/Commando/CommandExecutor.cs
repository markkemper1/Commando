using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Commando
{
	public class CommandExecutor : ICommandExecutor
	{
		private readonly List<Tuple<Action<ICommand>,Action<ICommand>>> beforeExecuteActions = new List<Tuple<Action<ICommand>, Action<ICommand>>>()
		{
			new Tuple<Action<ICommand>, Action<ICommand>>(DbCommandBase<int>.DefaultBeforeAction, null)
		};

		public static ICommandExecutor Default = new CommandExecutor();

		public ICommandExecutor ExecutorOverride { get; set; }

		public void Register(Tuple<Action<ICommand>,Action<ICommand>> beforeAndAfter)
		{
			beforeExecuteActions.Add(new Tuple<Action<ICommand>, Action<ICommand>>(beforeAndAfter.Item1, beforeAndAfter.Item2));
		}

		public void Register(Action<ICommand> before =null, Action<ICommand> after = null)
		{
			beforeExecuteActions.Add(new Tuple<Action<ICommand>, Action<ICommand>>(before,after));
		}

		/// <summary>
		///		Executes the command.
		/// </summary>
		/// <param name="command">The command to execute</param>
		[DebuggerNonUserCodeAttribute]
		public void Execute(ICommand command)
		{
			this.AttachServices(command);
			command.Execute();
			this.DettachServices(command);
		}

		/// <summary>
		///		Executes the command and returns the result. 
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="command"></param>
		/// <returns></returns>
		[DebuggerNonUserCode]
		public T Execute<T>(ICommandResult<T> command)
		{
			this.Execute((ICommand)command);
			return command.Result;
		}

		/// <summary>
		///		Allows commands its properties populated with service classes (if this floats your boat)
		///		e.g.  if (command is ICompositeCommand)
		///				((ICompositeCommand)command).Executor = ExecutorOverride ?? this;
		///		NB: be sure to call base.AttachedServies(command) so the composite commands can have the executor attached, unless you want to override this too.
		/// </summary>
		/// <param name="command">The service to decorate</param>
		protected virtual void AttachServices(ICommand command)
		{
			if (command is ICompositeCommand)
				((ICompositeCommand)command).Executor = ExecutorOverride ?? this;

			foreach (var tuple in beforeExecuteActions)
			{
				if(tuple.Item1 != null)
					tuple.Item1(command);
			}
		}

		protected virtual void DettachServices(ICommand command)
		{
			if (command is ICompositeCommand)
				((ICompositeCommand)command).Executor = null;

			foreach (var tuple in beforeExecuteActions)
			{
				if(tuple.Item2 != null)
					tuple.Item2(command);
			}
		}
	}
}
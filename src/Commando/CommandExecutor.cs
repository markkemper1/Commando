using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Commando
{
	public class CommandExecutor : ICommandExecutor
	{
		private readonly List<Action<ICommand>> beforeExecuteActions = new List<Action<ICommand>>
		{
			DbCommandBase<int>.DefaultBeforeAction
		};

		public IList<Action<ICommand>> BeforeExecuteActions
		{
			get { return this.beforeExecuteActions; }
		}

		public static ICommandExecutor Default = new CommandExecutor();

		public ICommandExecutor ExecutorOverride { get; set; }

		/// <summary>
		///		Executes the command.
		/// </summary>
		/// <param name="command">The command to execute</param>
		[DebuggerNonUserCodeAttribute]
		public void Execute(ICommand command)
		{
			this.AttachServices(command);
			command.Execute();
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

			foreach (var action in BeforeExecuteActions)
			{
				action(command);
			}
		}
	}
}
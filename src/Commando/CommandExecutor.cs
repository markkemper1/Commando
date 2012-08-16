using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Commando
{
	public class CommandExecutor : ICommandExecutor
	{
		private readonly List<Func<ICommand, Action<ICommand>>> beforeExecuteActions = new List<Func<ICommand, Action<ICommand>>>()
		{
			DbCommandBase<int>.DefaultBeforeAction
		};

		public static ICommandExecutor Default = new CommandExecutor();

		public ICommandExecutor ExecutorOverride { get; set; }

		public void Register(Func<ICommand, Action<ICommand>> beforeAndAfter)
		{
			beforeExecuteActions.Add(beforeAndAfter);
		}

		public void Register(Action<ICommand> beforeCommand)
		{
			Func<ICommand, Action<ICommand>> wrapped = c =>
				{
					beforeCommand(c);
					return null;
				};

			beforeExecuteActions.Add(wrapped);
		}

		/// <summary>
		///		Executes the command.
		/// </summary>
		/// <param name="command">The command to execute</param>
		[DebuggerNonUserCodeAttribute]
		public void Execute(ICommand command)
		{
			var afters = this.AttachServices(command);
			command.Execute();
			this.DettachServices(command, afters);
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
		protected virtual List<Action<ICommand>> AttachServices(ICommand command)
		{
			if (command is ICompositeCommand)
				((ICompositeCommand)command).Executor = ExecutorOverride ?? this;

			var afters = new List<Action<ICommand>>();

			foreach (var action in beforeExecuteActions)
			{
				afters.Add(action(command));
			}
			return afters;
		}

		protected virtual void DettachServices(ICommand command, List<Action<ICommand>> afters)
		{
			if (command is ICompositeCommand)
				((ICompositeCommand)command).Executor = null;

			foreach (var action in afters)
			{
				if(action != null)
					action(command);
			}
		}
	}
}
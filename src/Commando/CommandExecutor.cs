using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Commando
{
	public class CommandExecutor : ICommandExecutor
	{
        private readonly List<Func<ICommand, BeforeExecutionResult>> beforeExecuteActions = new List<Func<ICommand, BeforeExecutionResult>>()
		{
			DbCommandBase<int>.DefaultBeforeAction
		};

		public static ICommandExecutor Default = new CommandExecutor();

		public ICommandExecutor ExecutorOverride { get; set; }

		public void Register(Func<ICommand, Action<ICommand>> beforeAndAfter)
		{
		    Func<ICommand, BeforeExecutionResult> wrapper =
		        x =>
		            {
		                var result = new BeforeExecutionResult();
                        result.AfterAction = beforeAndAfter(x);
		                return result;
		            };

            beforeExecuteActions.Add(wrapper);
		}

		public void Register(Action<ICommand> beforeCommand)
		{
            Func<ICommand, BeforeExecutionResult> wrapper =
                x =>
                {
                    beforeCommand(x);
                    return null;
                };

            beforeExecuteActions.Add(wrapper);
		}

        public void Register(Func<ICommand, BeforeExecutionResult> filter)
        {
            beforeExecuteActions.Add(filter);
        }

		/// <summary>
		///		Executes the command.
		/// </summary>
		/// <param name="command">The command to execute</param>
		public void Execute(ICommand command)
		{
			var afters = this.AttachServices(command);
            
            if(!afters.Any(x=> x!= null && x.SkipExecution))
			    command.Execute();

			this.DettachServices(command, afters);
		}

		/// <summary>
		///		Executes the command and returns the result. 
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="command"></param>
		/// <returns></returns>
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
		protected virtual List<BeforeExecutionResult> AttachServices(ICommand command)
		{
			if (command is ICompositeCommand)
				((ICompositeCommand)command).Executor = ExecutorOverride ?? this;

            var afters = new List<BeforeExecutionResult>();

			foreach (var action in beforeExecuteActions)
			{
				afters.Add(action(command));
			}
			return afters;
		}

        protected virtual void DettachServices(ICommand command, List<BeforeExecutionResult> afters)
		{
			if (command is ICompositeCommand)
				((ICompositeCommand)command).Executor = null;

			foreach (var result in afters)
			{
                if (result != null)
                {
                    if(result.AfterAction != null)
                        result.AfterAction(command);
                }
			}
		}
	}
}
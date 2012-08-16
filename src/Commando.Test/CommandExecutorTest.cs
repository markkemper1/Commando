using System;
using System.Linq;
using NUnit.Framework;

namespace Commando.Test
{
	[TestFixture]
	public class CommandExecutorTest
	{

		[Test]
		public void executor_should_execute_command()
		{
			var command = new SimpleCommand();

			new CommandExecutor().Execute(command);

			Assert.AreEqual(1, command.Count);
		}

		[Test]
		public void executor_should_set_itself_on_commands_that_implement_ICompsiteCommand()
		{
			var command = new SimpleCommand();

			var executor = new CommandExecutor();
			executor.Execute(command);

			Assert.AreSame(executor, command.ExecutorUsed);
		}

		[Test]
		public void executor_should_run_any_before_actions_before_executing_command()
		{
			var command = new SimpleCommand();

			int beforeCount = 0;

			var executor = new CommandExecutor();
			executor.Register(x=> beforeCount++);
			executor.Execute(command);

			Assert.AreEqual(1, beforeCount);
		}

		[Test]
		public void executor_should_return_a_result_from_commands()
		{
			var command = new ResultCommand();

			Assert.AreEqual(0, command.Result);

			var executor = new CommandExecutor();
			var result = executor.Execute(command);

			Assert.AreEqual(1, result);
		}

		public class SimpleCommand : ICommand, ICompositeCommand
		{
			public int Count { get; private set; }

			public void Execute()
			{
				this.ExecutorUsed = this.Executor;
				Count++;
			}

			public ICommandExecutor ExecutorUsed { get; set; }
			public ICommandExecutor Executor { get; set; }
		}

		public class ResultCommand : CommandResultBase<int>
		{
			public int Count { get; private set; }

			public override int ExecuteWithResult()
			{
				return ++Count;	
			}
		}
	}
}

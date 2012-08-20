using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Commando.Test
{
	[TestFixture]
	public class CachingTest
	{

		[Test]
		public void no_caching_should_produce_different_results()
		{
			var command = new ResultCommand();
			var executor = new CommandExecutor(); 
			var result1 = executor.Execute(command);
			var result2 = executor.Execute(command);

			Assert.AreNotEqual(result1, result2);
		}

		[Test]
		public void caching_should_produce_the_same_result()
		{
			var command = new ResultCommand();
			var executor = new CommandExecutor();

            ResultCommand cachedResult = null;
			executor.Cache<ResultCommand>(x => cachedResult, (x) => cachedResult = x);

			var result1 = executor.Execute(command);
			var result2 = executor.Execute(command);

			Assert.AreEqual(result1, result2);
		}

		[Test]
		public void caching_should_not_be_affected_by_other_commands_()
		{
			var command = new ResultCommand();
			var executor = new CommandExecutor();

            ResultCommand cachedResult = null;
			executor.Cache<ResultCommand>(x => cachedResult, (x) => cachedResult = x);

			var result1 = executor.Execute(command);
			executor.Execute(new SimpleCommand());

			var result2 = executor.Execute(command);

            Assert.AreEqual(result1, result2);
		}

		[Test]
		public void caching_should_not_set_into_cache_if_fetched_from_cache()
		{
			var command = new ResultCommand();
			var executor = new CommandExecutor();

            ResultCommand cachedResult = new ResultCommand()
                                             {
                                                 Result = 1
                                             };
			int setCalls = 0;
			executor.Cache<ResultCommand>(x => cachedResult, (x) => setCalls+= 1);

			executor.Execute(command);
			executor.Execute(command);
			executor.Execute(command);
			executor.Execute(command);

			Assert.AreEqual(0, setCalls);
		}


        [Test]
        public void caching_of_null_should_be_allowed()
        {
            var command = new NullResultCommand();
            var executor = new CommandExecutor();

            NullResultCommand cachedResult = null;
            executor.Cache<NullResultCommand>(x => cachedResult, (x) => cachedResult = x);

            var result1 = executor.Execute(command);
            executor.Execute(new NullResultCommand());

            var result2 = executor.Execute(command);

            Assert.AreSame(result1, result2);
            Assert.IsNull(result1);
        }

		public class SimpleCommand : ICommand, ICompositeCommand
		{
			public int Count { get; private set; }

			public void Execute()
			{
				Count++;
			}

			public ICommandExecutor Executor { get; set; }
		}

        public class NullResultCommand : CommandResultBase<object>
        {
            public int Count { get; private set; }

            public override object ExecuteWithResult()
            {
                Count += 1;
                return null;
            }
        }

		public class ResultCommand : CommandResultBase<int?>
		{
			public int Count { get; private set; }

			public override int? ExecuteWithResult()
			{
				return ++Count;
			}
		}
	}
}

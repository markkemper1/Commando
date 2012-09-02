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

        [Test]
        public void should_be_easy_to_configure_cache_provider()
        {
            var executor = new CommandExecutor();
            var provider = new StupidCache();
            executor
                .UseCache(() => provider)
                .For<ResultCommand>(c => "ResultCommand")
                    .Do(
                        (c, k) => c.Get(k) as ResultCommand,
                        (c, k, r) => c.Store(k, r)
                    )
                .For<ResultCommand2>(c=> "ResultCommand2")
                    .Do(
                        (c, k) => c.Get(k) as ResultCommand2,
                        (c, k, r) => c.Store(k, r)
                    );
                            

            var command_1 = new ResultCommand();
            var result_1_1 = executor.Execute(command_1);
            var result_1_2 = executor.Execute(command_1);

            Assert.AreEqual(result_1_1, result_1_2);

            var command_2 = new ResultCommand();
            var result_2_1 = executor.Execute(command_2);
            var result_2_2 = executor.Execute(command_2);
            var result_1_3 = executor.Execute(command_1);

            Assert.AreEqual(result_2_1, result_2_2);
            Assert.AreNotSame(result_1_1, result_1_3);
        }

	    public class StupidCache
	    {
	        private object value;
	        private string key;

            public void Store(string key, object value)
            {
                this.key = key;
                this.value = value;
            }

            public object Get(string key)
            {
                return this.key == key ? this.value : null;
            }
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


        public class ResultCommand2 : CommandResultBase<int?>
        {
            public int Count { get; private set; }

            public override int? ExecuteWithResult()
            {
                return ++Count;
            }
        }
	}
}

using System;

namespace Commando
{
    public interface ICacheableCommand : ICommandResult
    {
        string CacheKey { get; }
    }

    public static class Caching
	{
        public static Func<ICommand, BeforeExecutionResult> Create<T>(Func<T, object> getter, Action<T> setter) where T : ICommandResult
		{
			var cacher = new CachingTypeGymnast<T>(getter, setter);
			return cacher.Before;
		}

		public static void Cache<T>(this CommandExecutor executor, Func<T, object> getter, Action<T> setter) where T : ICommandResult
		{
			executor.Register(Create(getter, setter));
		}

	}
	public class CachingTypeGymnast<T> where T : ICommandResult
	{
		protected Func<T, object> Get { get; set; }
		protected Action<T> Set { get; set; }

		public CachingTypeGymnast(Func<T, object> get, Action<T> set)
		{
			Get = get;
			Set = set;
		}

		public BeforeExecutionResult Before(ICommand command)
		{
			if (!(command is T))
				return null;

			var commandResult = (T)command;
			var cached = this.Get(commandResult);

			if (cached != null)
			{
				commandResult.ResultValue = cached;
			    return new BeforeExecutionResult()
			               {
			                   SkipExecution = true
			               };
			}

		    return new BeforeExecutionResult()
		               {
		                   AfterAction = command1 => Set(((T) command1))
		               };
		}
	}
}

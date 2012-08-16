using System;

namespace Commando
{
	public static class Caching
	{
		public static Tuple<Action<ICommand>, Action<ICommand>> Create<T>(Func<T, object> getter, Action<T> setter) where T : ICommandResult
		{
			var cacher = new CachingTypeGymnast<T>(getter, setter);
			return new Tuple<Action<ICommand>, Action<ICommand>>(cacher.Before, cacher.After);
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

		public void Before(ICommand command)
		{
			if (!(command is T))
				return;

			var commandResult = (T)command;
			var cached = this.Get(commandResult);

			if (cached != null)
				commandResult.ResultValue = cached;
		}

		public void After(ICommand command)
		{
			if (!(command is T))
				return;

			var commandResult = (T)command;
			Set(commandResult);
		}


	}
}

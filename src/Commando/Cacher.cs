﻿using System;

namespace Commando
{
    public interface ICacheableCommand
    {
        string CacheKey { get; }

        TimeSpan CacheTime { get; }
    }

    public static class Caching
	{
		public static Func<ICommand, Action<ICommand>> Create<T>(Func<T, object> getter, Action<T> setter) where T : ICommandResult
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

		public Action<ICommand> Before(ICommand command)
		{
			if (!(command is T))
				return null;

			var commandResult = (T)command;
			var cached = this.Get(commandResult);

			if (cached != null)
			{
				commandResult.ResultValue = cached;
				return null;
			}

			return command1 => Set( ((T)command1));
		}
	}
}

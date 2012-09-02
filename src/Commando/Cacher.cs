using System;

namespace Commando
{
    public static class Caching
    {
        public static Func<ICommand, BeforeExecutionResult> Create<T>(Func<T, T> getter, Action<T> setter) where T : ICommandResult
        {
            var cacher = new CachingTypeGymnast<T>(getter, setter);
            return cacher.Before;
        }

        public static void Cache<T>(this CommandExecutor executor, Func<T, T> getter, Action<T> setter) where T : ICommandResult
        {
            executor.Register(Create(getter, setter));
        }

        public static CacheHelper<T> UseCache<T>(this CommandExecutor executor, Func<T> getProvider) where T : class
        {
            return new CacheHelper<T>(getProvider, executor);
        }

        public class CacheHelper<T> where T : class
        {
            private readonly Func<T> getProvider;
            private readonly CommandExecutor executor;

            public CacheHelper(Func<T> getProvider, CommandExecutor executor)
            {
                if (getProvider == null) throw new ArgumentNullException("getProvider");
                if (executor == null) throw new ArgumentNullException("executor");
                this.getProvider = getProvider;
                this.executor = executor;
            }

            public CacheHelperCommand<T, CMD> For<CMD>(Func<CMD, string> getCommandKey) where CMD : ICommandResult
            {
                return new CacheHelperCommand<T, CMD>(this, getProvider, executor, getCommandKey);
            }
        }


        public class CacheHelperCommand<T, CMD>
            where T : class
            where CMD : ICommandResult
        {
            private readonly CacheHelper<T> cacheHelper;
            private readonly Func<T> getProvider;
            private readonly CommandExecutor executor;
            private readonly Func<CMD, string> getCommandKey;

            public CacheHelperCommand(CacheHelper<T> cacheHelper, Func<T> getProvider, CommandExecutor executor, Func<CMD, string> getCommandKey)
            {
                if (cacheHelper == null) throw new ArgumentNullException("cacheHelper");
                if (getProvider == null) throw new ArgumentNullException("getProvider");
                if (executor == null) throw new ArgumentNullException("executor");
                if (getCommandKey == null) throw new ArgumentNullException("getCommandKey");
                this.cacheHelper = cacheHelper;
                this.getProvider = getProvider;
                this.executor = executor;
                this.getCommandKey = getCommandKey;
            }


            public CacheHelper<T> Do(Func<T, string, CMD> get, Action<T, string, CMD> set)
            {
                executor.Cache<CMD>(
                                    c => get(getProvider(), getCommandKey(c))
                                    ,
                                    c => set(getProvider(), getCommandKey(c), c)
                               );

                return cacheHelper;
            }
        }

    }
    public class CachingTypeGymnast<T> where T : ICommandResult
    {
        protected Func<T, T> Get { get; set; }
        protected Action<T> Set { get; set; }

        public CachingTypeGymnast(Func<T, T> get, Action<T> set)
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
                commandResult.ResultValue = cached.ResultValue;
                return new BeforeExecutionResult()
                           {
                               SkipExecution = true
                           };
            }

            return new BeforeExecutionResult()
                       {
                           AfterAction = command1 => Set(((T)command1))
                       };
        }
    }
}

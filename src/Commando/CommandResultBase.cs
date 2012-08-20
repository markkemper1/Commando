using System;
using System.Diagnostics;

namespace Commando
{
	public abstract class CommandResultBase<T> : ICommandResult<T>, ICompositeCommand
	{
		private T result;

		[DebuggerNonUserCode]
		public void Execute()
		{
			this.Result = this.ExecuteWithResult();
		}

		public abstract T ExecuteWithResult();

		public ICommandExecutor Executor { get; set; }

		public T Result
		{
			get { return result; }
			set { this.result = value; }
		}

		public object ResultValue
		{
			get { return this.result; } 
			set
			{
                //if its not a value type then allow null to be set
                if (value == null  && ! (typeof(T).IsValueType))
                {
                    this.result = default(T);
                    return;
                }

			    if( value is T)
				{
					this.result = (T) value;
				}
				else
				{
					throw new InvalidOperationException("This commands result must be of type: " + typeof(T).Name);
				}
			}
		}
	}
}
using System;
using System.Data.Common;
using System.Diagnostics;

namespace Commando
{
	public interface ICompositeCommand
	{
		ICommandExecutor Executor { get; set; }
	}

	public interface IDbProviderCommmand
	{
		DbProviderFactory DbProvider { get; set; }
		string ConnectionString { get; set; }
	}

	public static class CompositeCommandExtensions
	{
		[DebuggerNonUserCode]
		public static void Execute(this ICompositeCommand composite, ICommand command)
		{
			composite.Executor.Execute(command);
		}

		[DebuggerNonUserCode]
		public static T Execute<T>(this ICompositeCommand composite, ICommandResult<T> command)
		{
			return composite.Executor.Execute(command);
		}
	}
}
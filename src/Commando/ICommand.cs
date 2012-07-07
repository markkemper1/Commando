using System;

namespace Commando
{
	public interface ICommand
	{
		void Execute();
	}

	public interface IHaveSubCommands
	{
		ICommandExecutor Executor { get; set; }
	}

	/* Not sure what the point of this was, or why it was here */
	//public static class IHaveSubCommandExtensions
	//{
	//    public static void Validate<T>(this T instance)
	//    {
	//        var validationContext = new ValidationContext(instance, null, null);
	//        Validator.ValidateObject(instance, validationContext);
	//    }

	//    public static void SubExecute(this IHaveSubCommands parent, ICommand child)
	//    {
	//        parent.Executor.Execute(child);
	//    }

	//    public static T SubExecute<T>(this IHaveSubCommands parent, ICommandResult<T> child)
	//    {
	//        return parent.Executor.Execute(child);
	//    }
	//}
}
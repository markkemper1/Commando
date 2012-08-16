namespace Commando
{
	public interface ICommandResult
	{
		object ResultValue { get; set; }
	}

	public interface ICommandResult<T> : ICommand, ICommandResult
	{
		T Result { get; }
	}
}
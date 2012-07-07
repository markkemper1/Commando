namespace Commando
{
	public interface ICommandResult<T> : ICommand
	{
		T Result { get; }
	}
}
using System.Data;

namespace Commando
{
	public static class DbCommandExtensions
	{

		public static T AndOpen<T>(this T connection)
			where T : IDbConnection
		{
			connection.Open();
			return connection;
		}

		public static IDbCommand SqlCommand(this IDbConnection connection, string sql)
		{
			var command = connection.CreateCommand();
			command.CommandText = sql;
			return command;
		}

		public static IDbDataParameter Param<T>(this T command, string name, object value)
			where T : IDbCommand
		{
			var param = command.CreateParameter();
			param.ParameterName = name;
			param.Value = value;
			command.Parameters.Add(param);
			return param;
		}
	}
}
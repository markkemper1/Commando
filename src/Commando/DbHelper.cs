using System;
using System.Configuration;
using System.Data;
using System.Data.Common;

namespace Commando
{
	public static class DbHelper
	{
		public static string ConnectionString = AppSettingResolver.Resolve(ConfigurationManager.ConnectionStrings[0].ConnectionString);
		public static string ProviderName = ConfigurationManager.ConnectionStrings[0].ProviderName;

		public static IDbConnection CreateConnection(string dbProviderName, string connectionString)
		{
			var dbProvider = DbProviderFactories.GetFactory(dbProviderName);

			var connection = dbProvider.CreateConnection();

			if (connection == null)
				throw new ApplicationException("Arhh f#@3!, the DbProvider can't create a connection.");

			connection.ConnectionString = connectionString;
			return connection;
		}

		public static IDbConnection CreateConnection()
		{
			if(ProviderName == null)
				throw new InvalidOperationException("The DbHelper.Provider is null, you must set this to a valid DbProvider name (which is registered in the configuration, e.g System.Data.SQLite or System.Data.SqlClient)");

			if (ConnectionString == null)
				throw new InvalidOperationException("The DbHelper.ConnectionString is null, you must set this to a valid connection string");

			return DbHelper.CreateConnection(ProviderName, ConnectionString);
		}

		public static int NonQuery(Func<IDbConnection, int> nonQuery)
		{
			var command = new DbNonQueryCommand(nonQuery);
			return CommandExecutor.Default.Execute(command);
		}

		public static RESULT Query<RESULT>(Func<IDbConnection, RESULT> query)
		{
			var command = new DbQueryCommand<RESULT>(query);
			return CommandExecutor.Default.Execute(command);
		}
	}
}
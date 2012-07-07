using System;
using System.Data;

namespace Commando
{
	public static class DbHelper
	{
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
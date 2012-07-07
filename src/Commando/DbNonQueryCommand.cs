using System;
using System.Data;
using System.Data.Common;

namespace Commando
{
	public class DbNonQueryCommand : DbCommandBase<int>
	{
		private readonly Func<IDbConnection, int> nonQuery;

		public DbNonQueryCommand(Func<IDbConnection, int> nonQuery)
		{
			if (nonQuery == null) throw new ArgumentNullException("nonQuery");
			this.nonQuery = nonQuery;
		}

		protected override int Run(IDbConnection db)
		{
			return nonQuery(db);
		}
	}
}
using System;
using System.Data;
using Commando;

namespace Commando
{
	public class DbQueryCommand<RESULT> : DbCommandBase<RESULT>
	{
		private readonly Func<IDbConnection, RESULT> query;

		public DbQueryCommand(Func<IDbConnection, RESULT> query)
		{
			if (query == null) throw new ArgumentNullException("query");
			this.query = query;
		}

		protected override RESULT Run(IDbConnection db)
		{
			return query(db);
		}
	}
}
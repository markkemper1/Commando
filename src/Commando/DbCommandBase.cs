using System;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Diagnostics;

namespace Commando
{
	public class DbCommandBaseDefaults
	{
		public static string ConnectionString = AppSettingResolver.Resolve(ConfigurationManager.ConnectionStrings[0].ConnectionString);
		public static string Provider = ConfigurationManager.ConnectionStrings[0].ProviderName;
	}

	public abstract class DbCommandBase<T> : CommandResultBase<T>, IDbProviderCommmand
	{
		public IDbTransaction Transaction { get; set; }

		public string ConnectionString { get; set; }

		public DbProviderFactory DbProvider { get; set; }

		public static void DefaultBeforeAction(ICommand command)
		{
			if(command is IDbProviderCommmand)
			{
				var dbCommand = command as IDbProviderCommmand;
				dbCommand.DbProvider = DbProviderFactories.GetFactory(DbCommandBaseDefaults.Provider);
				dbCommand.ConnectionString = DbCommandBaseDefaults.ConnectionString;
			}
		}

		public override T ExecuteWithResult()
		{
			if(this.DbProvider == null) throw new InvalidOperationException("Your must set the DbProvider property before calling execute");
			if(this.ConnectionString == null)throw new InvalidOperationException("Your must set the connection string property before calling execute");

			if (this.Transaction != null)
			{
				if (this.Transaction.Connection == null)
					throw new InvalidOperationException("The connection property of the transaction is NULL, are you trying to reuse an existing transaction?");
				return Run(this.Transaction.Connection);
			}

			using (var db = GetConnection().AndOpen())
			{
				return Run(db);
			}
		}

		protected virtual IDbConnection GetConnection()
		{
			var connection = DbProvider.CreateConnection();
			
			if(connection == null)
				throw new ApplicationException("Arhh f#@3!, the DbProvider can't create a connection.");

			connection.ConnectionString = this.ConnectionString;
			return connection;
		}

		protected abstract T Run(IDbConnection db);

		protected TransactionHelper EnsureTransaction(IDbConnection db, IsolationLevel level)
		{
			if(this.Transaction != null && (int)this.Transaction.IsolationLevel <  (int)level)
				throw new InvalidOperationException("This command: " 
					+ this.GetType().Name + " requires a transaction isolation level of: " + level);

			if (this.Transaction != null)
			{
				//we are part of a nested transaction.
				//this command does not own the transaction
				return new TransactionHelper(this.Transaction, false);
			}

			this.Transaction = db.BeginTransaction(level);
			return new TransactionHelper(this.Transaction, true);
		}
	}
}
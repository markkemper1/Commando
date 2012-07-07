using System.Data;

namespace Commando
{
	public class TransactionHelper : IDbTransaction 
	{
		private readonly IDbTransaction transaction;
		private readonly bool isOwned;

		public TransactionHelper(IDbTransaction transaction, bool isOwned)
		{
			this.transaction = transaction;
			this.isOwned = isOwned;
		}

		public void Dispose()
		{
			if(isOwned)
				this.transaction.Dispose();
		}

		public void Commit()
		{
			if(isOwned)
				this.transaction.Commit();
		}

		public void Rollback()
		{
			this.transaction.Rollback();
		}

		public IDbConnection Connection
		{
			get { return this.transaction.Connection; }
		}

		public IsolationLevel IsolationLevel
		{
			get { return this.transaction.IsolationLevel; }
		}
	}
}
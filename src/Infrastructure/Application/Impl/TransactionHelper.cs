using Core.Application;

namespace Infrastructure.Application.Impl
{
    public class TransactionHelper : ITransactionHelper
    {
        private readonly ISessionLocator _sessionLocator;
        private readonly IAppContextStore _appContextStore;

        private int _transactionReq;
        private const string KeyTransactionCount = "Session.TransactionCount";

        public TransactionHelper(ISessionLocator sessionLocator, IAppContextStore appContextStore)
        {
            _sessionLocator = sessionLocator;
            _appContextStore = appContextStore;
        }
        private int TransactionReq
        {
            get
            {
                var item = _appContextStore.Get(KeyTransactionCount);
                if (item == null)
                {
                    _appContextStore.AddItem(KeyTransactionCount, _transactionReq);
                    item = _transactionReq;
                }
                return (int)item;
            }
            set
            {
                _transactionReq = value;
                _appContextStore.UpdateItem(KeyTransactionCount, _transactionReq);
            }
        }

               public void BeginTransaction()
        {
            _sessionLocator.GetSession().BeginTransaction();
            TransactionReq++;
        }

        public void Commit()
        {
            TransactionReq--;
            var session = _sessionLocator.GetSession();
            var trans = session.Transaction;
            if (trans != null && trans.IsActive && TransactionReq == 0)
            {
                trans.Commit();
            }
        }

        public void Rollback(bool resetCounter = true)
        {
            if (resetCounter && TransactionReq > 0)
                TransactionReq = 0;

            var trans = _sessionLocator.GetSession().Transaction;
            if (trans != null && trans.IsActive)
                trans.Rollback();
        }

        public void Dispose()
        {
            var session = _sessionLocator.GetSession();

            if (session != null && session.IsOpen && session.IsConnected)
            {
                Rollback(false);
            }
        }
    }
}

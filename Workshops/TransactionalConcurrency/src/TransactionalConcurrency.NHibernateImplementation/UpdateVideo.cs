using NHibernate;
using TransactionalConcurrency.Data.Entities;

namespace TransactionalConcurrency.NHibernateImplementation
{
    public class UpdateVideo
    {
        public UpdateVideo(ISession session)
        {
            _session = session;
        }

        public ITransaction Execute(Video video)
        {
            var transaction = _session.BeginTransaction();
            _session.SaveOrUpdate(video);

            return transaction;
        }

        private readonly ISession _session;
    }
}
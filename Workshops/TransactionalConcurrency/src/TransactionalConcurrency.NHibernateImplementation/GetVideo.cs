using NHibernate;
using TransactionalConcurrency.Data.Entities;

namespace TransactionalConcurrency.NHibernateImplementation
{
    public class GetVideo
    {
        public GetVideo(ISession session)
        {
            _session = session;
        }

        public Video Execute(int videoId)
        {
            return _session.Get<Video>(videoId);
        }

        private readonly ISession _session;
    }
}
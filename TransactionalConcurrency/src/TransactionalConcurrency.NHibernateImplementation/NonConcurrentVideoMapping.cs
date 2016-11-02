using FluentNHibernate.Mapping;
using TransactionalConcurrency.Data.Entities;

namespace TransactionalConcurrency.NHibernateImplementation
{
    public class NonConcurrentVideoMapping : ClassMap<Video>
    {
        public NonConcurrentVideoMapping()
        {
            Table("Video");
            Id(s => s.VideoId);
            Map(s => s.SaleIncVat);
            Map(s => s.Title);
        }
    }
}
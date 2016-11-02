using FluentNHibernate.Mapping;
using TransactionalConcurrency.Data.Entities;

namespace TransactionalConcurrency.NHibernateImplementation
{
    public class ConcurrentVideoMapping : ClassMap<Video>
    {
        public ConcurrentVideoMapping()
        {
            Table("Video");
            Id(s => s.VideoId);
            Map(s => s.SaleIncVat);
            Map(s => s.Title);
            Version(s => s.RowVersion)
                .CustomSqlType("timestamp")
                .Generated
                .Always();
        }
    }
}
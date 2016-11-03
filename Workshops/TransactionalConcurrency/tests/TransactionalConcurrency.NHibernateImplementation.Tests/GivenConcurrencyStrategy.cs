using System;
using System.Configuration;
using System.Threading;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;
using NUnit.Framework;
using TransactionalConcurrency.Data.Migrations;
using TransactionalConcurrency.Tests.Common;

namespace TransactionalConcurrency.NHibernateImplementation.Tests
{
    public class GivenConcurrencyStrategy
    {
        [SetUp]
        public void SetUp()
        {
            Runner.SqlServer("VideoStore").Down();
            Runner.SqlServer("VideoStore").Up();

            _sessionFactory = Fluently.Configure()
                .Database(MsSqlConfiguration.MsSql2008.ConnectionString(ConfigurationManager.ConnectionStrings["VideoStore"].ConnectionString))
                .Mappings(m => { m.FluentMappings.AddFromAssemblyOf<ConcurrentVideoMapping>(); })
                .ExposeConfiguration(cfg => { })
                .BuildSessionFactory();
        }

        [Test]
        public void WhenIHaveCompetingTransactions_ThenStaleObjectStateExceptionIsThrownAndFirstUpdateIsHonoured()
        {
            TaskRunner.Execute(
            () =>
            {
                try
                {
                    var session = _sessionFactory.OpenSession();
                    session.FlushMode = FlushMode.Commit;

                    var getVideoByVideoId = new GetVideo(session);
                    var Video = getVideoByVideoId.Execute(2);
                    var updateSalesPrice = new UpdateVideo(session);
                    Video.SaleIncVat += 10;

                    var transaction = updateSalesPrice.Execute(Video);
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            },
            () =>
            {
                var session = _sessionFactory.OpenSession();
                session.FlushMode = FlushMode.Commit;

                var getVideoByVideoId = new GetVideo(session);
                var Video = getVideoByVideoId.Execute(2);
                var updateSalesPrice = new UpdateVideo(session);
                Thread.Sleep(TimeSpan.FromSeconds(1));
                var transaction = updateSalesPrice.Execute(Video);

                try
                {
                    Video.SaleIncVat -= 10;
                    transaction.Commit();
                }
                catch (StaleObjectStateException)
                {
                    transaction.Rollback();
                    _concurrencyExceptionThrown = true;
                }
            });

            var getVideo = new GetVideo(_sessionFactory.OpenSession());
            var so = getVideo.Execute(2);
            
            Assert.That(so.SaleIncVat, Is.EqualTo(110));
            Assert.That(_concurrencyExceptionThrown, Is.True);
        }

        private ISessionFactory _sessionFactory;
        private bool _concurrencyExceptionThrown;
    }
}
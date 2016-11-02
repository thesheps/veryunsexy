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
    public class GivenNoConcurrencyStrategy
    {
        [SetUp]
        public void SetUp()
        {
            Runner.SqlServer("VideoStore").Down();
            Runner.SqlServer("VideoStore").Up(1);

            _sessionFactory = Fluently.Configure()
                .Database(MsSqlConfiguration.MsSql2008.ConnectionString(ConfigurationManager.ConnectionStrings["VideoStore"].ConnectionString))
                .Mappings(m => { m.FluentMappings.Add<NonConcurrentVideoMapping>(); })
                .ExposeConfiguration(cfg => { })
                .BuildSessionFactory();
        }

        [Test]
        public void WhenIHaveCompetingTransactions_ThenUpdatesMayBeLost()
        {
            var session = _sessionFactory.OpenSession();
            session.FlushMode = FlushMode.Commit;

            var updateSalesPrice = new UpdateVideo(session);
            var getVideoByVideoId = new GetVideo(session);
            var video = getVideoByVideoId.Execute(2);

            TaskRunner.Execute(
            () =>
            {
                video.SaleIncVat += 10;
                var transaction = updateSalesPrice.Execute(video);
                transaction.Commit();
            },
            () =>
            {
                video.SaleIncVat -= 10;
                var transaction = updateSalesPrice.Execute(video);
                transaction.Commit();
            });

            video = getVideoByVideoId.Execute(2);
            Assert.That(video.SaleIncVat, Is.EqualTo(110));
        }

        [Test]
        public void WhenIHaveUncommittedData_ThenReadsMayBeDirty()
        {
            var session = _sessionFactory.OpenSession();
            session.FlushMode = FlushMode.Commit;

            var updateSalesPrice = new UpdateVideo(session);
            var getVideoByVideoId = new GetVideo(session);
            var video = getVideoByVideoId.Execute(2);

            TaskRunner.Execute(
            () =>
            {
                video.SaleIncVat = 10;
                var transaction = updateSalesPrice.Execute(video);
                Thread.Sleep(TimeSpan.FromSeconds(2));
                transaction.Rollback();
            },
            () =>
            {
                Thread.Sleep(TimeSpan.FromSeconds(1));
                video = getVideoByVideoId.Execute(2);
                video.SaleIncVat += 10;
                var transaction = updateSalesPrice.Execute(video);

                transaction.Commit();
            });

            video = getVideoByVideoId.Execute(2);
            Assert.That(video.SaleIncVat, Is.EqualTo(110));
        }

        private ISessionFactory _sessionFactory;
    }
}
using System;
using System.Data;
using System.Threading;
using NUnit.Framework;
using TransactionalConcurrency.Data.Migrations;
using TransactionalConcurrency.Tests.Common;

namespace TransactionalConcurrency.DapperImplementation.Tests
{
    public class GivenConcurrencyStrategy
    {
        [SetUp]
        public void SetUp()
        {
            Runner.SqlServer("VideoStore").Down();
            Runner.SqlServer("VideoStore").Up();
        }

        [Test]
        public void WhenIHaveCompetingTransactions_ThenConcurrencyExceptionIsThrownAndFirstUpdateIsHonoured()
        {
            var updateSalesPrice = new UpdateSalesPrice("VideoStore");
            var getVideoByVideoId = new GetVideo("VideoStore");
            var Video = getVideoByVideoId.Execute(2);

            TaskRunner.Execute(
            () =>
            {
                try
                {
                    var transaction = updateSalesPrice.Execute(Video.VideoId, Video.SaleIncVat + 10, Video.RowVersion);
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Test");
                }
            },
            () =>
            {
                Thread.Sleep(TimeSpan.FromSeconds(1));

                try
                {
                    var transaction = updateSalesPrice.Execute(Video.VideoId, Video.SaleIncVat - 10, Video.RowVersion);
                    transaction.Commit();
                }
                catch(DBConcurrencyException)
                {
                    _concurrencyExceptionThrown = true;
                }
            });

            Video = getVideoByVideoId.Execute(2);
            Assert.That(Video.SaleIncVat, Is.EqualTo(110));
            Assert.That(_concurrencyExceptionThrown, Is.True);
        }

        private bool _concurrencyExceptionThrown;
    }
}
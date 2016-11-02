using System;
using System.Threading;
using NUnit.Framework;
using TransactionalConcurrency.AdoImplementation.Pessimistic;
using TransactionalConcurrency.Data.Migrations;
using TransactionalConcurrency.Tests.Common;

namespace TransactionalConcurrency.AdoImplementation.Tests.Pessimistic
{
    public class GivenReadUncommitted
    {
        [SetUp]
        public void SetUp()
        {
            Runner.SqlServer("VideoStore").Down();
            Runner.SqlServer("VideoStore").Up(1);
        }

        [Test]
        public void WhenIHaveCompetingTransactions_ThenUpdatesMayBeLost()
        {
            var updateSalesPrice = new UpdateSalesPrice("VideoStore");
            var getVideoByVideoId = new GetVideoReadUncommitted("VideoStore");
            var Video = getVideoByVideoId.Execute(2);
            
            TaskRunner.Execute(
            () =>
            {
                Thread.Sleep(TimeSpan.FromSeconds(1));
                
                var transaction = updateSalesPrice.Execute(Video.VideoId, Video.SaleIncVat + 10);
                transaction.Commit();
            }, 
            () =>
            {
                var transaction = updateSalesPrice.Execute(Video.VideoId, Video.SaleIncVat - 10);
                transaction.Commit();
            });

            Video = getVideoByVideoId.Execute(2);
            Assert.That(Video.SaleIncVat, Is.EqualTo(110));
        }

        [Test]
        public void WhenIHaveUncommittedData_ThenReadsMayBeDirty()
        {
            var updateSalesPrice = new UpdateSalesPrice("VideoStore");
            var getVideoByVideoId = new GetVideoReadUncommitted("VideoStore");
            var Video = getVideoByVideoId.Execute(2);

            TaskRunner.Execute(
            () =>
            {
                var transaction = updateSalesPrice.Execute(Video.VideoId, 10);
                Thread.Sleep(TimeSpan.FromSeconds(2));
                transaction.Rollback();
            },
            () =>
            {
                Thread.Sleep(TimeSpan.FromSeconds(1));
                Video = getVideoByVideoId.Execute(2);
                var transaction = updateSalesPrice.Execute(Video.VideoId, Video.SaleIncVat + 10);

                transaction.Commit();
            });

            Video = getVideoByVideoId.Execute(2);
            Assert.That(Video.SaleIncVat, Is.EqualTo(110));
        }
    }
}
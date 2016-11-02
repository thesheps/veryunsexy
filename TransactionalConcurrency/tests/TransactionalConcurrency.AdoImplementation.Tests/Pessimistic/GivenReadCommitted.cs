using System;
using System.Threading;
using NUnit.Framework;
using TransactionalConcurrency.AdoImplementation.Pessimistic;
using TransactionalConcurrency.Data.Migrations;
using TransactionalConcurrency.Tests.Common;

namespace TransactionalConcurrency.AdoImplementation.Tests.Pessimistic
{
    public class GivenReadCommitted
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
            var getVideoByVideoId = new GetVideoReadCommitted("VideoStore");
            var Video = getVideoByVideoId.Execute(2);

            TaskRunner.Execute(() =>
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
            Assert.That(Video.SaleIncVat, Is.EqualTo(90));
        }

        [Test]
        public void WhenIHaveUncommittedData_ThenReadsAreNotDirty()
        {
            var updateSalesPrice = new UpdateSalesPrice("VideoStore");
            var getVideoByVideoId = new GetVideoReadCommitted("VideoStore");
            var video = getVideoByVideoId.Execute(2);

            TaskRunner.Execute(() =>
            {
                var transaction = updateSalesPrice.Execute(video.VideoId, 10);
                Thread.Sleep(TimeSpan.FromSeconds(2));
                transaction.Rollback();
            },
            () =>
            {
                Thread.Sleep(TimeSpan.FromSeconds(1));
                video = getVideoByVideoId.Execute(2);
                var transaction = updateSalesPrice.Execute(video.VideoId, video.SaleIncVat + 10);

                transaction.Commit();
            });

            video = getVideoByVideoId.Execute(2);
            Assert.That(video.SaleIncVat, Is.EqualTo(110));
        }
    }
}
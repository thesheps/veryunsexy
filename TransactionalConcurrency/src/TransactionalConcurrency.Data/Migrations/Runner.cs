using System.Configuration;
using System.Reflection;
using FluentMigrator;
using FluentMigrator.Runner;
using FluentMigrator.Runner.Announcers;
using FluentMigrator.Runner.Initialization;
using FluentMigrator.Runner.Processors;
using FluentMigrator.Runner.Processors.SqlServer;

namespace TransactionalConcurrency.Data.Migrations
{
    public class Runner
    {
        public static Runner SqlServer(string connectionStringName)
        {
            var connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
            return new Runner(GetRunner(connectionString, new SqlServer2008ProcessorFactory()));
        }

        public void Up()
        {
            _runner.MigrateUp();
        }

        public void Up(int version)
        {
            _runner.MigrateUp(version);
        }

        public void Down()
        {
            _runner.MigrateDown(0);
        }

        private Runner(MigrationRunner runner)
        {
            _runner = runner;
        }

        private static MigrationRunner GetRunner(string connectionString, IMigrationProcessorFactory factory)
        {
            var announcer = new NullAnnouncer();
            var assembly = Assembly.GetExecutingAssembly();
            var migrationContext = new RunnerContext(announcer) { Namespace = "TransactionalConcurrency.Data.Migrations" };
            var options = new MigrationOptions { PreviewOnly = false, Timeout = 60 };
            var processor = factory.Create(connectionString, announcer, options);

            return new MigrationRunner(assembly, migrationContext, processor);
        }

        private class MigrationOptions : IMigrationProcessorOptions
        {
            public bool PreviewOnly { get; set; }
            public int Timeout { get; set; }

            public string ProviderSwitches
            {
                get { return string.Empty; }
            }
        }

        private readonly MigrationRunner _runner;
    }
}
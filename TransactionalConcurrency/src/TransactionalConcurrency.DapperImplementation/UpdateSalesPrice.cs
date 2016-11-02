using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Dapper;

namespace TransactionalConcurrency.DapperImplementation
{
    public class UpdateSalesPrice
    {
        public UpdateSalesPrice(string connectionStringName)
        {
            _connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
        }

        public IDbTransaction Execute(int videoId, decimal saleIncVat, byte[] rowVersion)
        {
            var connection = new SqlConnection(_connectionString);
            connection.Open();

            var transaction = connection.BeginTransaction();

            try
            {
                var rowCount = connection.Query<int>(@"UPDATE Video 
                                                       SET SaleIncVat = @saleIncVat
                                                       WHERE VideoId = @videoId AND RowVersion = @rowVersion
                                                       SELECT @@ROWCOUNT", new { saleIncVat, videoId, rowVersion }, transaction).Single();

                if (rowCount == 0)
                    throw new DBConcurrencyException();

                return transaction;
            }
            catch (Exception)
            {
                transaction.Rollback();
                transaction.Dispose();
                connection.Dispose();
                throw;
            }
        }

        private readonly string _connectionString;
    }
}
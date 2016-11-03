using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace TransactionalConcurrency.AdoImplementation.Optimistic
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
                var command = connection.CreateCommand();
                command.Transaction = transaction;

                command.CommandText = @"UPDATE Video 
                                        SET SaleIncVat = @saleIncVat
                                        WHERE VideoId = @videoId AND RowVersion = @rowVersion
                                        SELECT @@ROWCOUNT";

                command.Parameters.Add(command.GetParameter("saleIncVat", saleIncVat));
                command.Parameters.Add(command.GetParameter("videoId", videoId));
                command.Parameters.Add(command.GetParameter("rowVersion", rowVersion));

                var rowCount = (int) command.ExecuteScalar();

                if (rowCount == 0)
                    throw new DBConcurrencyException();

                return transaction;
            }
            catch (Exception)
            {
                transaction.Dispose();
                connection.Dispose();
                throw;
            }
        }

        private readonly string _connectionString;
    }
}
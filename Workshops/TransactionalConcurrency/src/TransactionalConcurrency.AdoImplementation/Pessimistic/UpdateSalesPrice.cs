using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace TransactionalConcurrency.AdoImplementation.Pessimistic
{
    public class UpdateSalesPrice
    {
        public UpdateSalesPrice(string connectionStringName)
        {
            _connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
        }

        public IDbTransaction Execute(int videoId, decimal saleIncVat)
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
                                        WHERE VideoId = @videoId";

                command.Parameters.Add(command.GetParameter("saleIncVat", saleIncVat));
                command.Parameters.Add(command.GetParameter("videoId", videoId));
                command.ExecuteScalar();

                return transaction;
            }
            catch (Exception)
            {
                transaction.Rollback();
                return transaction;
            }
        }

        private readonly string _connectionString;
    }
}
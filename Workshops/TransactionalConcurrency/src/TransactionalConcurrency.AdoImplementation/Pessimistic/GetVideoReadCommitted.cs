using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using TransactionalConcurrency.Data.Entities;

namespace TransactionalConcurrency.AdoImplementation.Pessimistic
{
    public class GetVideoReadCommitted
    {
        public GetVideoReadCommitted(string connectionStringName)
        {
            _connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
        }

        public Video Execute(int videoId)
        {
            var connection = new SqlConnection(_connectionString);
            connection.Open();

            var transaction = connection.BeginTransaction();

            try
            {
                var command = connection.CreateCommand();
                command.Transaction = transaction;
                command.Parameters.Add(command.GetParameter("videoId", videoId));
                command.CommandText = @"SELECT VideoId, Title, SaleIncVat 
                                        FROM Video
                                        WHERE VideoId = @videoId";

                using (var reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                {
                    reader.Read();
                    return GetRow(reader);
                }
            }
            catch (Exception ex)
            {
                Console.Write(ex);
            }

            return null;
        }

        private static Video GetRow(IDataRecord reader)
        {
            return new Video
            {
                VideoId = int.Parse(reader[0].ToString()),
                Title = reader[1].ToString(),
                SaleIncVat = decimal.Parse(reader[2].ToString())
            };
        }

        private readonly string _connectionString;
    }
}
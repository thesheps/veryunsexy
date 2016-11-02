using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using TransactionalConcurrency.Data.Entities;

namespace TransactionalConcurrency.DapperImplementation
{
    public class GetVideo
    {
        public GetVideo(string connectionStringName)
        {
            _connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
        }

        public Video Execute(int videoId)
        {
            var connection = new SqlConnection(_connectionString);
            
            try
            {
                return connection.Query<Video>(@"SELECT VideoId, Title, SaleIncVat, RowVersion
                                                 FROM Video
                                                 WHERE VideoId = @videoId", new { videoId }).Single();
            }
            catch (Exception ex)
            {
                Console.Write(ex);
            }

            return null;
        }

        private readonly string _connectionString;
    }
}
using Microsoft.Data.SqlClient;
using System.Data;

namespace GestionSoftware.API.Data
{
    public class GestionProyectosSoftwareContext
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public GestionProyectosSoftwareContext(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("DefaultConnection");
        }

        public IDbConnection CreateConnection() => new SqlConnection(_connectionString);
    }
        
}

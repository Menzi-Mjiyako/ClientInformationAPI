using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientInformation.Data.DataAccess
{
    public static class DatabaseConnection
    {
        public static IDbConnection GetConnection(IConfiguration configuration)
        {
            string connectionString = configuration.GetConnectionString("DefaultConnection");
            return new SqlConnection(connectionString);
        }
    }
}

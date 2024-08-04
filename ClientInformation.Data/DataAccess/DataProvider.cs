using ClientInformation.Data.DataAccess.Interfaces;
using Dapper;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientInformation.Data.DataAccess
{
    public class DataProvider : IDataProvider
    {
        private readonly IConfiguration _configuration;

        public DataProvider(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<SqlMapper.GridReader> QueryMultipleAsync(string sql, object? param = null, IDbTransaction? transaction = null)
        {
            using (var db = DatabaseConnection.GetConnection(_configuration))
            {
                return await db.QueryMultipleAsync(sql, param, transaction);
            }
        }

        public async Task<IEnumerable<T>> QueryAsync<T>(string sql, object? param = null, IDbTransaction? transaction = null)
        {
            using (var db = DatabaseConnection.GetConnection(_configuration))
            {
                return await db.QueryAsync<T>(sql, param, transaction);
            }
        }

        public async Task<int> ExecuteAsync(string sql, object? param = null, IDbTransaction? transaction = null)
        {
            using (var db = DatabaseConnection.GetConnection(_configuration))
            {
                return await db.ExecuteAsync(sql, param, transaction);
            }
        }
    }

}

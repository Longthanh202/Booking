using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Container_App.Data.Connection
{
    public class StoredProcedureExecutor: IStoredProcedureExecutor
    {
        private readonly string _connectionString;
        public StoredProcedureExecutor(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("DefaultConnection");
        }
        public async Task<int> ExecuteAsync(string spName, params SqlParameter[] parameters)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand(spName, conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            if (parameters != null && parameters.Length > 0)
                cmd.Parameters.AddRange(parameters);

            await conn.OpenAsync();
            return await cmd.ExecuteNonQueryAsync();
        }

        public async Task<IEnumerable<T>> QueryAsync<T>(string spName, params SqlParameter[] parameters)

        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand(spName, conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            if (parameters != null && parameters.Length > 0)
                cmd.Parameters.AddRange(parameters);

            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();

            var result = new List<T>();
            var props = typeof(T).GetProperties();

            while (await reader.ReadAsync())
            {
                var item = Activator.CreateInstance<T>();
                foreach (var prop in props)
                {
                    if (!reader.HasColumn(prop.Name)) continue;

                    var val = reader[prop.Name];
                    prop.SetValue(item, val == DBNull.Value ? null : val);
                }
                result.Add(item);
            }

            return result;
        }

        public async Task<T> QuerySingleAsync<T>(string spName, params SqlParameter[] parameters)
        {
            return (await QueryAsync<T>(spName, parameters)).FirstOrDefault();
        }
    }
}

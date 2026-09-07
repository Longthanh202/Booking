using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Data.Connection
{
    public interface IStoredProcedureExecutor
    {
        Task<IEnumerable<T>> QueryAsync<T>(
        string spName,
        params SqlParameter[] parameters);

        Task<T> QuerySingleAsync<T>(
            string spName,
            params SqlParameter[] parameters);

        Task<int> ExecuteAsync(
            string spName,
            params SqlParameter[] parameters);
    }
}

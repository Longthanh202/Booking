using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Container_App.Data.Repository.Redis
{
    public interface IRedisService
    {
        Task Set (string key, string value);
        Task<string?> Get (string key);
        Task Delete (string key);
        Task SetObject<T> (string key, T value, TimeSpan expiry);
        Task<T?> GetObject<T>(string key);
    }
}

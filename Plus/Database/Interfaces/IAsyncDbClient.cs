using MySql.Data.MySqlClient;
using System;
using System.Threading.Tasks;

namespace Plus.Database.Interfaces
{
    public interface IAsyncDbClient : IDisposable
    {
        Task CreateTransaction(Action<MySqlTransaction> transaction);

        Query CreateQuery(string query, params object[] parameters);
    }
}

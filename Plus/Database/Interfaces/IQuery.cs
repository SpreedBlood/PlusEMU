using System;
using System.Data.Common;
using System.Threading.Tasks;

namespace Plus.Database.Interfaces
{
    public interface IQuery : IDisposable
    {
        Task<DbDataReader> ExecuteReaderAsync();

        Task<int> ExecuteNonQueryAsync();
    }
}

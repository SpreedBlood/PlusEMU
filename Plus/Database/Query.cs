using MySql.Data.MySqlClient;
using Plus.Database.Interfaces;
using System.Data.Common;
using System.Threading.Tasks;

namespace Plus.Database
{
    public class Query : IQuery
    {
        private readonly MySqlCommand _command;

        internal Query(MySqlCommand command)
        {
            _command = command;
        }

        public Task<DbDataReader> ExecuteReaderAsync() => _command.ExecuteReaderAsync();

        public Task<int> ExecuteNonQueryAsync() => _command.ExecuteNonQueryAsync();

        public void Dispose()
        {
            _command.Dispose();
        }
    }

    public static class QueryExtensions
    {
        public static T GetData<T>(this DbDataReader reader, string columnName) =>
            (T)reader[columnName];
    }
}

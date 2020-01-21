using MySql.Data.MySqlClient;
using Plus.Database.Interfaces;
using System;
using System.Threading.Tasks;

namespace Plus.Database
{
    internal class AsyncDatabaseClient : IAsyncDbClient
    {
        private readonly MySqlConnection _connection;

        internal AsyncDatabaseClient(MySqlConnection connection)
        {
            _connection = connection;
            _connection.Open();
        }

        public async Task CreateTransaction(Action<MySqlTransaction> transaction)
        {
            using (MySqlTransaction asyncTransaction = await _connection.BeginTransactionAsync())
            {
                transaction(asyncTransaction);
            }
        }

        public Query CreateQuery(string query, params object[] parameters)
        {
            MySqlCommand command = _connection.CreateCommand();
            command.CommandText = query;
            for (int i = 0; i < parameters.Length; i++)
            {
                command.Parameters.AddWithValue($"@{i}", parameters[i]);
            }

            return new Query(command);
        }

        public void Dispose()
        {
            _connection.Close();
            _connection.Dispose();
        }
    }
}
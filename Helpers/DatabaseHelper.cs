using System;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Baligyaay.Helpers
{
    public static class DatabaseHelper
    {
        public static async Task<bool> IsServerConnected(string connectionString)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    await connection.OpenAsync();
                    return true;
                }
                catch (SqlException)
                {
                    return false;
                }
            }
        }

        internal static async Task<bool> IsServerConnected(object value)
        {
            throw new NotImplementedException();
        }
    }
}

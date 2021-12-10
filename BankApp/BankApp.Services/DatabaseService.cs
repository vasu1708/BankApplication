using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;

namespace BankApp.Services
{
    public class DatabaseService
    {
        private static MySqlConnection Connection { get; set; }
        public static void OpenConnection(string serverName, string databaseName, string userId, string password)
        {
            string ConnectionString = $"SERVER={serverName};DATABASE={databaseName};UID={userId};PASSWORD={password}";
            Connection = new MySqlConnection(ConnectionString);
            Connection.Open();
        }
        public static MySqlCommand OpenConnectionAndCreateCommand(string query)
        {
            OpenConnection("localhost", "BankDB", "srinivas", "Mysql@1234");
            return new MySqlCommand(query, Connection);
        }
        public static Object ExecuteScalarAndCloseConnection(MySqlCommand cmd)
        {
            var value = cmd.ExecuteScalar();
            Connection.Close();
            return value;
        }
        public static void ExecuteNonQueryAndCloseConnection(MySqlCommand cmd)
        {
            cmd.ExecuteNonQuery();
            Connection.Close();
        }
        public static DataTable ExecuteDataAdapterAndCloseConnection(MySqlCommand cmd)
        {
            DataTable dt = new DataTable();
            var dataAdapter = new MySqlDataAdapter(cmd);
            dataAdapter.Fill(dt);
            Connection.Close();
            return dt;
        }
        
    }
}

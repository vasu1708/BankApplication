using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;

namespace BankApp.Services
{
    public class DatabaseService
    {
        private  MySqlConnection Connection { get; set; }
        public  MySqlCommand OpenConnectionAndCreateCommand(string query,List<MySqlParameter> parameterList)
        {
            string ConnectionString = $"SERVER={"localhost"};DATABASE={"BankDB"};UID={"srinivas"};PASSWORD={"Mysql@1234"}";
            Connection = new MySqlConnection(ConnectionString);
            Connection.Open();
            var cmd = new MySqlCommand(query, Connection);
            cmd.Parameters.AddRange(parameterList.ToArray());
            return cmd;
        }
        public  Object ExecuteScalar(string query,List<MySqlParameter> parameterList)
        {
            var value = OpenConnectionAndCreateCommand(query,parameterList).ExecuteScalar();
            Connection.Close();
            return value;
        }
        public  void ExecuteNonQuery(string query,List<MySqlParameter> parameterList)
        {
            OpenConnectionAndCreateCommand(query, parameterList).ExecuteNonQuery();
        }
        public  DataTable ExecuteDataAdapter(string query, List<MySqlParameter> parameterList)
        {
            DataTable dt = new DataTable();
            var cmd = OpenConnectionAndCreateCommand(query, parameterList);
            var dataAdapter = new MySqlDataAdapter(cmd);
            dataAdapter.Fill(dt);
            Connection.Close();
            return dt;
        }
        
    }
}

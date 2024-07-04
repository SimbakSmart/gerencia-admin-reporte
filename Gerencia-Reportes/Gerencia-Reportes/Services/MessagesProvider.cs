
using Gerencia_Reportes.Data;
using Gerencia_Reportes.Models.Sqlite;
using System.Collections.Generic;
using System.Data.SQLite;
using System;

namespace Gerencia_Reportes.Services
{
    public  class MessagesProvider
    {
        private SQLiteDb dbHelper;

        public MessagesProvider()
        {
            dbHelper = new SQLiteDb(@"C:\Mensajes Locales\messages.sqlite");
        }

        public void Insert(Messages message)
        {
            dbHelper.OpenConnection();
            SQLiteCommand cmd = dbHelper.GetConnection().CreateCommand();
            cmd.CommandText = "INSERT INTO Messages (Attribute, Value, Number, Comments) VALUES (@Attribute, @Value, @Number, @Comments)";
            cmd.Parameters.AddWithValue("@Attribute", message.Attribute);
            cmd.Parameters.AddWithValue("@Value", message.Value);
            cmd.Parameters.AddWithValue("@Number", message.Number);
            cmd.Parameters.AddWithValue("@Comments", message.Comments);
            cmd.ExecuteNonQuery();
            dbHelper.CloseConnection();
        }

        public List<Messages> GetAll()
        {
            List<Messages> messages = new List<Messages>();
            dbHelper.OpenConnection();
            SQLiteCommand cmd = dbHelper.GetConnection().CreateCommand();
            cmd.CommandText = "SELECT Id, Attribute, Value, Number, Comments FROM Messages";
            SQLiteDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                Messages message = new Messages();
                message.Id = Convert.ToInt32(reader["Id"]);
                message.Attribute = reader["Attribute"].ToString();
                message.Value = reader["Value"].ToString();
                message.Number = Convert.ToInt32(reader["Number"]);
                message.Comments = reader["Comments"].ToString();
                messages.Add(message);
            }
            reader.Close();
            dbHelper.CloseConnection();
            return messages;
        }

        public void Delete(Messages message)
        {
            dbHelper.OpenConnection();
            SQLiteCommand cmd = dbHelper.GetConnection().CreateCommand();
            cmd.CommandText = "DELETE FROM Messages WHERE Number = @Number  AND Attribute = @Attribute AND Value = @Value ";
            cmd.Parameters.AddWithValue("@Number", message.Number);
            cmd.Parameters.AddWithValue("@Attribute", message.Attribute);
            cmd.Parameters.AddWithValue("@Value", message.Value);
            cmd.ExecuteNonQuery();
            dbHelper.CloseConnection();
        }

        //public void Update(Messages message)
        //{
        //    dbHelper.OpenConnection();
        //    SQLiteCommand cmd = dbHelper.GetConnection().CreateCommand();
        //    cmd.CommandText = "UPDATE Messages SET Attribute = @Attribute, Value = @Value, Number = @Number, Comments = @Comments WHERE Id = @Id";
        //    cmd.Parameters.AddWithValue("@Attribute", message.Attribute);
        //    cmd.Parameters.AddWithValue("@Value", message.Value);
        //    cmd.Parameters.AddWithValue("@Number", message.Number);
        //    cmd.Parameters.AddWithValue("@Comments", message.Comments);
        //    cmd.Parameters.AddWithValue("@Id", message.Id);
        //    cmd.ExecuteNonQuery();
        //    dbHelper.CloseConnection();
        //}

        //public void Delete(int messageId)
        //{
        //    dbHelper.OpenConnection();
        //    SQLiteCommand cmd = dbHelper.GetConnection().CreateCommand();
        //    cmd.CommandText = "DELETE FROM Messages WHERE Id = @Id";
        //    cmd.Parameters.AddWithValue("@Id", messageId);
        //    cmd.ExecuteNonQuery();
        //    dbHelper.CloseConnection();
        //}

    }
}

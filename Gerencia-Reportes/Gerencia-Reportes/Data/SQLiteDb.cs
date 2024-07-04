

using System.Data.SQLite;
using System.IO;

namespace Gerencia_Reportes.Data
{
    public class SQLiteDb
    {

        private SQLiteConnection connection;
        private string dbPath;

        public SQLiteDb(string dbPath)
        {
            this.dbPath = dbPath;

            // Verificar y crear el directorio si no existe
            string directoryPath = Path.GetDirectoryName(dbPath);
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            // Inicializar la conexión
            string connectionString = $"Data Source={dbPath};Version=3;";
            connection = new SQLiteConnection(connectionString);

            // Crear la base de datos y la tabla si no existen
            if (!File.Exists(dbPath))
            {
                SQLiteConnection.CreateFile(dbPath);
                CreateTables();
            }
        }

        private void CreateTables()
        {
            string createTableQuery = @"
            CREATE TABLE IF NOT EXISTS Messages (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Attribute TEXT NOT NULL,
                Value TEXT,
                Number INTEGER NOT NULL,
                Comments TEXT
            );
        ";

            OpenConnection();
            SQLiteCommand cmd = new SQLiteCommand(createTableQuery, connection);
            cmd.ExecuteNonQuery();

            CloseConnection();
        }

        public void OpenConnection()
        {
            if (connection.State != System.Data.ConnectionState.Open)
                connection.Open();
        }

        public void CloseConnection()
        {
            if (connection.State != System.Data.ConnectionState.Closed)
                connection.Close();
        }

        public SQLiteConnection GetConnection()
        {
            return connection;
        }


    }
}


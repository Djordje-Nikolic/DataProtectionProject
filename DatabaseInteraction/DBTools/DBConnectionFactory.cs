using System;
using System.Data.SQLite;
using System.IO;

namespace ZIProject.DatabaseInteraction.DBTools
{
    internal static class DBConnectionFactory
    {
        public static readonly string DBDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "MyCloudStore\\Database\\");
        public static readonly string DBPath = Path.Combine(DBDirectory, "Data.db");

        public static SQLiteConnection CreateConnection()
        {
            SQLiteConnection connection;

            if (!File.Exists(DBPath))
            {
                Directory.CreateDirectory(DBDirectory);
                SQLiteConnection.CreateFile(DBPath);

                connection = new SQLiteConnection($"DataSource={DBPath};Version=3;");
                connection.Open();

                DBSeeder.Seed(connection);

            }
            else
            {
                connection = new SQLiteConnection($"DataSource={DBPath};Version=3;");
                connection.Open();
            }

            return connection;
        }
    }
}

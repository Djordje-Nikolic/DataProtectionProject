using System;
using System.Data.SQLite;
using Dapper;

namespace ZIProject.DatabaseInteraction.DBTools
{
    internal static class DBSeeder
    {
        #region SQL Creation queries
        private static readonly string CreateUser = @"CREATE TABLE IF NOT EXISTS User
                                                        (
                                                            ID INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                                                            Username NVARCHAR(50) NOT NULL UNIQUE,
                                                            Password NVARCHAR(50) NOT NULL,
                                                            LeftoverSpace INTEGER NOT NULL
                                                        );";
        private static readonly string CreateFile = @"CREATE TABLE IF NOT EXISTS [File]
                                                        (
                                                            ID INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                                                            Name NVARCHAR(50) NOT NULL,
                                                            UserID INTEGER NOT NULL,
                                                            HashValue NVARCHAR(50) NOT NULL,
                                                            FOREIGN KEY(UserID) REFERENCES [User](ID)
                                                                ON UPDATE CASCADE
                                                                ON DELETE RESTRICT
                                                        );";
        #endregion

        public static void Seed(SQLiteConnection conn)
        {

            if (conn == null)
            {
                throw new ArgumentNullException();
            }

            if (conn.State == System.Data.ConnectionState.Closed)
            {
                conn.Open();
            }

            conn.Execute("PRAGMA foreign_keys=off;");
            conn.Execute("BEGIN TRANSACTION;");
            conn.Execute(CreateUser);
            conn.Execute(CreateFile);
            conn.Execute("COMMIT;");
            conn.Execute("PRAGMA foreign_keys=on;");
        }
    }
}

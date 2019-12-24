using System;
using System.Collections.Generic;
using System.Linq;
using ZIProject.DatabaseInteraction.Models;
using System.Data.SQLite;
using Dapper;

namespace ZIProject.DatabaseInteraction.DBTools
{
    public class DBContext : IDisposable
    {
        public SQLiteConnection Connection { get; private set; }
        public DBContext()
        {
            var conn = DBConnectionFactory.CreateConnection();

            if (conn == null || conn.State == System.Data.ConnectionState.Closed)
            {
                throw new ArgumentNullException("The database connection is null or closed.");
            }

            Connection = conn;
        }

        public IEnumerable<FileInfo> GetAllFiles(UserInfo userInfo)
        {
            string sqlQuery = "SELECT * FROM File WHERE UserID = @ID;";

            IEnumerable<FileInfo> result;
            try
            {
                result = Connection.Query<FileInfo>(sqlQuery, new { userInfo.ID });
            }
            catch (Exception e)
            {
                throw new Exception("Error fecthing existing files for this user.", e);
            }

            return result;
        }
        public int TryAddUser(UserInfo userInfo)
        {
            if (CheckForUser(userInfo, Connection))
            {
                throw new InvalidOperationException("This user already exists.");
            }

            string sqlQuery = "INSERT INTO User(Username, Password, LeftoverSpace) VALUES (@Username, @Password, @LeftoverSpace); SELECT last_insert_rowid();";

            int newId = -1;
            try
            {
                newId = int.Parse(Connection.ExecuteScalar(sqlQuery, new { userInfo.Username, userInfo.Password, userInfo.LeftoverSpace }).ToString());
                userInfo.ID = newId;
            }
            catch (Exception e)
            {
                throw new Exception("Error inserting a new user.", e);
            }
            return newId;
        }
        public bool CheckUserCredentials(UserInfo userInfo)
        {
            string sqlQuery = "SELECT * User WHERE Username = @Username;";

            IEnumerable<UserInfo> result;
            try
            {
                result = Connection.Query<UserInfo>(sqlQuery, new { userInfo.Username });
            }
            catch (Exception e)
            {
                throw new Exception("Error fecthing existing user.", e);
            }

            int userCount = result.Count();
            if (userCount > 0)
            {
                if (userInfo.Password == result.First().Password)
                {
                    userInfo.ID = result.First().ID;
                    userInfo.LeftoverSpace = result.First().LeftoverSpace;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                throw new InvalidOperationException("This user doesn't exist.");
            }
        }
        public void RemoveUser(int userId)
        {
            string removeUser = "DELETE FROM [User] WHERE ID=@userId;";
            try
            {
                Connection.Execute(removeUser, new { userId });
            }
            catch (Exception e)
            {
                throw new Exception("Error updating removing user.", e);
            }
        }
        internal bool CheckForUser(int userId)
        {
            string sqlQuery = "SELECT EXISTS(SELECT 1 FROM User WHERE ID = @userId);";

            int result;
            try
            {
                result = int.Parse(Connection.ExecuteScalar(sqlQuery, new { userId }).ToString());
            }
            catch (Exception e)
            {
                throw new Exception("Error fecthing existing user.", e);
            }

            if (result != 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        private static bool CheckForUser(UserInfo userInfo, SQLiteConnection conn)
        {
            string sqlQuery = "SELECT EXISTS(SELECT 1 FROM User WHERE Username = @Username);";

            int result;
            try
            {
                result = int.Parse(conn.ExecuteScalar(sqlQuery, new { userInfo.Username }).ToString());
            }
            catch (Exception e)
            {
                throw new Exception("Error fecthing existing user.", e);
            }

            if (result != 0)
            {
                return true;
            }
            else
            {
                return false;
            }

        }
        public int TryAddFile(FileInfo fileInfo, UserInfo userInfo)
        {
            if (!CheckForUser(userInfo.ID))
            {
                throw new InvalidOperationException("This user doesn't exist.");
            }

            fileInfo.UserID = userInfo.ID;
            if (CheckForDuplicate(fileInfo))
            {
                throw new InvalidOperationException("This file already exists in the user's allocated space.");
            }

            string sqlQuery = "INSERT INTO File(Name, UserID, HashValue) VALUES (@Name, @UserID, @HashValue); SELECT last_insert_rowid();";

            int newId = -1;
            try
            {
                newId = int.Parse(Connection.ExecuteScalar(sqlQuery, new { fileInfo.Name, fileInfo.UserID, fileInfo.HashValue }).ToString());
                userInfo.ID = newId;
            }
            catch (Exception e)
            {
                throw new Exception("Error inserting a new user.", e);
            }
            return newId;
        }
        private bool CheckForDuplicate(FileInfo fileInfo)
        {
            string sqlQuery = "SELECT EXISTS(SELECT 1 FROM File WHERE UserID = @UserID AND Name = @Name);";

            int result;
            try
            {
                result = int.Parse(Connection.ExecuteScalar(sqlQuery, new { fileInfo.UserID, fileInfo.Name }).ToString());
            }
            catch (Exception e)
            {
                throw new Exception("Error fecthing existing file.", e);
            }

            if (result != 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public FileInfo CheckForFile(string filename, int userId)
        {
            string sqlQuery = "SELECT * FROM File WHERE UserID = @userId AND Name = @filename;";

            FileInfo result = null;
            try
            {
                result = Connection.Query<FileInfo>(sqlQuery, new { userId, filename}).FirstOrDefault();
            }
            catch (Exception e)
            {
                throw new Exception("Error fecthing existing file.", e);
            }

            return result;
        }
        public void RemoveFile(int fileId)
        {
            string removeFile = "DELETE FROM [File] WHERE ID=@fileId;";
            try
            {
                Connection.Execute(removeFile, new { fileId });
            }
            catch (Exception e)
            {
                throw new Exception("Error removing file.", e);
            }
        }

        public void Dispose()
        {
            if (Connection != null)
            {
                Connection.Close();
                Connection.Dispose();
                Connection = null;
            }
        }
    }
}

using System;

namespace ZIProject.DatabaseInteraction.Models
{
    public class UserInfo
    {
        public static readonly long StartingUserAllocatedSpace = 104857600;
        public int ID { get; internal set; }
        public string Username { get; internal set; }
        public string Password { get; internal set; }
        public long LeftoverSpace { get; internal set; }
        internal UserInfo() { ID = -1; }
        public UserInfo(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || username.Length > 50)
            {
                throw new ArgumentOutOfRangeException("Entered username is invalid.");
            }

            //posle vratiti ovu proveru
            if (string.IsNullOrWhiteSpace(password) /*|| password.Length != 40*/)
            {
                throw new ArgumentOutOfRangeException("Entered password is invalid.");
            }

            ID = -1;
            Username = username;
            Password = password;
            LeftoverSpace = StartingUserAllocatedSpace;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ZIProject.DatabaseInteraction.Models;
using ZIProject.DatabaseInteraction.DBTools;

namespace ZIProject.MyCloudStore
{
    public class SessionManager
    {
        private static readonly Lazy<SessionManager> instance = new Lazy<SessionManager>(() => new SessionManager());
        public static SessionManager Instance
        {
            get
            {
                return instance.Value;
            }
        }

        private Dictionary<string, SessionInfo> sessions;
        private SessionManager()
        {
            sessions = new Dictionary<string, SessionInfo>(50);
        }

        public SessionInfo CheckForSession(string identification)
        {
            sessions.TryGetValue(identification, out SessionInfo result);
            return result;
        }
        public SessionInfo AddSession(string identification, UserInfo userInfo)
        {
            if (string.IsNullOrWhiteSpace(identification) || userInfo == null)
                throw new ArgumentNullException();

            sessions.Remove(identification);

            SessionInfo newSession = new SessionInfo() { LoggedInUser = userInfo, TimeLogged = DateTime.Now };
            sessions.Add(identification, newSession);

            return newSession;
        }
    }

    public class SessionInfo
    {
        public UserInfo LoggedInUser { get; set; }
        public DateTime TimeLogged { get; set; }
    }
}
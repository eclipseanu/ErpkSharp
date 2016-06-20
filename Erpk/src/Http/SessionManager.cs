using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using NLog;

namespace Erpk.Http
{
    public class SessionManager
    {
        /// <summary>
        ///     NLog
        /// </summary>
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        ///     Holds sessions directory name.
        /// </summary>
        private readonly DirectoryInfo _sessionsDirectory;

        public SessionManager(string sessionsFolderPath)
        {
            _sessionsDirectory = new DirectoryInfo(sessionsFolderPath);
        }

        public Session OpenOrCreate(string email, string password)
        {
            var fi = GetSessionFileInfo(email);
            if (!fi.Exists)
            {
                return new Session(email, password);
            }

            var json = File.ReadAllText(fi.FullName);
            try
            {
                var session = JsonConvert.DeserializeObject<Session>(json);
                session.Password = password;
                return session;
            }
            catch (Exception e)
            {
                Logger.Warn(e, "Session file is corrupted, contents={0}", json);
                return new Session(email, password);
            }
        }

        public void Save(Session session)
        {
            if (string.IsNullOrWhiteSpace(session.Email))
            {
                return;
            }

            if (!_sessionsDirectory.Exists)
            {
                _sessionsDirectory.Create();
            }

            var fi = GetSessionFileInfo(session.Email);
            var json = JsonConvert.SerializeObject(session, Formatting.Indented);
            File.WriteAllText(fi.FullName, json);
        }

        private FileInfo GetSessionFileInfo(string email)
        {
            var hash = "sess_" + Hash(email).Substring(0, 8) + ".json";
            return new FileInfo(Path.Combine(_sessionsDirectory.FullName, hash));
        }

        private static string Hash(string input)
        {
            var hash = new SHA1Managed().ComputeHash(Encoding.UTF8.GetBytes(input));
            return string.Join("", hash.Select(b => b.ToString("x2")).ToArray());
        }
    }
}
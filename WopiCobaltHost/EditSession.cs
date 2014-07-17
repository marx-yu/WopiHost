using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cobalt;
using System.Security.Cryptography;

namespace WopiCobaltHost
{
    abstract public class EditSession
    {
        protected readonly string m_sessionId;
        protected readonly string m_login;
        protected readonly string m_name;
        protected readonly string m_email;
        protected readonly bool m_isAnonymous;
        protected readonly FileInfo m_fileinfo;
        protected DateTime m_lastUpdated;
        public EditSession(string sessionId, string filePath, string login, string name, string email, bool isAnonymous)
        {
            m_sessionId = sessionId;
            m_fileinfo = new FileInfo(filePath);
            m_name = name;
            m_login = login;
            m_email = email;
            m_isAnonymous = isAnonymous;
        }

        public string SessionId
        {
            get { return m_sessionId; }
            set { }
        }

        public string Login
        {
            get { return m_login; }
            set { }
        }

        public string Name
        {
            get { return m_name; }
            set { }
        }

        public string Email
        {
            get { return m_email; }
            set { }
        }

        public bool IsAnonymous
        {
            get { return m_isAnonymous; }
            set { }
        }

        public DateTime LastUpdated
        {
            get { return m_lastUpdated; }
            set { }
        }

        public WopiCheckFileInfo GetCheckFileInfo()
        {
            WopiCheckFileInfo cfi = new WopiCheckFileInfo();

            cfi.BaseFileName = m_fileinfo.Name;
            cfi.OwnerId = m_login;
            cfi.UserFriendlyName = m_name;

            lock (m_fileinfo)
            {
                if (m_fileinfo.Exists)
                {
                    cfi.Size = m_fileinfo.Length;
                    //using (FileStream stream = m_fileinfo.OpenRead())
                    //{
                    //    var checksum = SHA256.Create().ComputeHash(stream);
                    //    cfi.SHA256 = Convert.ToBase64String(checksum);
                    //}
                }
                else
                {
                    cfi.Size = 0;
                }
            }

            cfi.Version = m_fileinfo.LastWriteTimeUtc.ToString("s");
            cfi.SupportsCoauth = true;
            cfi.SupportsCobalt = true;
            cfi.SupportsFolders = true;
            cfi.SupportsLocks = true;
            cfi.SupportsScenarioLinks = false;
            cfi.SupportsSecureStore = false;
            cfi.SupportsUpdate = true;
            cfi.UserCanWrite = true;

            return cfi;
        }

        public long FileLength
        {
            get
            {
                return m_fileinfo.Length;
            }
        }
        abstract public void Save(byte[] new_content);
        abstract public void Dispose();
        abstract public byte[] GetFileContent();

        abstract public void Save();
        abstract public void ExecuteRequestBatch(RequestBatch requestBatch);
    }
}

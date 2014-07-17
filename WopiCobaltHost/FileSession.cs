using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Cobalt;

namespace WopiCobaltHost
{
    public class FileSession : EditSession
    {
        public FileSession(string sessionId, string filePath, string login = "Anonymous", string name = "Anonymous", string email = "", bool isAnonymous = true)
            :base(sessionId, filePath, login, name, email, isAnonymous)
        { }

        override public byte[] GetFileContent()
        {
            MemoryStream ms = new MemoryStream();
            lock (m_fileinfo)
            {
                using (FileStream fileStream = m_fileinfo.OpenRead())
                {
                    fileStream.CopyTo(ms);
                }
            }
            return ms.ToArray();
        }


        override public void Save()
        {
        }
        override public void Save(byte[] new_content)
        {
            lock (m_fileinfo)
            {
                using (FileStream fileStream = m_fileinfo.Open(FileMode.Truncate))
                {
                    fileStream.Write(new_content, 0, new_content.Length);
                }
            }
            m_lastUpdated = DateTime.Now;
        }
        override public void ExecuteRequestBatch(RequestBatch requestBatch)
        {            
        }

        override public void Dispose()
        {
        }
    }
}

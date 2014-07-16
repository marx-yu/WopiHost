// Copyright 2014 The Authors Marx-Yu. All rights reserved.
// Use of this source code is governed by a BSD-style license that can be
// found in the LICENSE file.


using Cobalt;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WopiCobaltHost
{
    public class CobaltSession
    {
        private readonly string m_sessionId;
        private readonly string m_login;
        private readonly string m_name;
        private readonly string m_email;
        private readonly bool m_isAnonymous;
        private readonly FileInfo m_fileinfo;
        private readonly CobaltFile m_cobaltFile;
        private readonly DisposalEscrow m_disposal;
        private DateTime m_lastUpdated;

        public CobaltSession(string sessionId, string filePath, string login = "Anonymous", string name = "Anonymous", string email = "", bool isAnonymous = true)
        {
            m_sessionId = sessionId;
            m_fileinfo = new FileInfo(filePath);
            m_name = name;
            m_login = login;
            m_email = email;
            m_isAnonymous = isAnonymous;
            m_disposal = new DisposalEscrow(m_sessionId);

            CobaltFilePartitionConfig content = new CobaltFilePartitionConfig();
            content.IsNewFile = true;
            content.HostBlobStore = new TemporaryHostBlobStore(new TemporaryHostBlobStore.Config(), m_disposal, m_sessionId + @".Content");
            content.cellSchemaIsGenericFda = true;
            content.CellStorageConfig = new CellStorageConfig();
            content.Schema = CobaltFilePartition.Schema.ShreddedCobalt;
            content.PartitionId = FilePartitionId.Content;

            CobaltFilePartitionConfig coauth = new CobaltFilePartitionConfig();
            coauth.IsNewFile = true;
            coauth.HostBlobStore = new TemporaryHostBlobStore(new TemporaryHostBlobStore.Config(), m_disposal, m_sessionId + @".CoauthMetadata");
            coauth.cellSchemaIsGenericFda = false;
            coauth.CellStorageConfig = new CellStorageConfig();
            coauth.Schema = CobaltFilePartition.Schema.ShreddedCobalt;
            coauth.PartitionId = FilePartitionId.CoauthMetadata;

            CobaltFilePartitionConfig wacupdate = new CobaltFilePartitionConfig();
            wacupdate.IsNewFile = true;
            wacupdate.HostBlobStore = new TemporaryHostBlobStore(new TemporaryHostBlobStore.Config(), m_disposal, m_sessionId + @".WordWacUpdate");
            wacupdate.cellSchemaIsGenericFda = false;
            wacupdate.CellStorageConfig = new CellStorageConfig();
            wacupdate.Schema = CobaltFilePartition.Schema.ShreddedCobalt;
            wacupdate.PartitionId = FilePartitionId.WordWacUpdate;

            Dictionary<FilePartitionId, CobaltFilePartitionConfig> partitionConfs = new Dictionary<FilePartitionId, CobaltFilePartitionConfig>();
            partitionConfs.Add(FilePartitionId.Content, content);
            partitionConfs.Add(FilePartitionId.WordWacUpdate, wacupdate);
            partitionConfs.Add(FilePartitionId.CoauthMetadata, coauth);

            m_cobaltFile = new CobaltFile(m_disposal, partitionConfs, new CobaltHostLockingStore(this), null);

            if (m_fileinfo.Exists)
            {
                String appdata_path = Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData);
                appdata_path = Path.Combine(appdata_path, @"WopiCobaltHost");
                if (!Directory.Exists(appdata_path))
                    Directory.CreateDirectory(appdata_path);
                String cache_file = Path.Combine(appdata_path, m_fileinfo.Name);
                if (File.Exists(cache_file))
                    File.Delete(cache_file);
                File.Copy(m_fileinfo.FullName, cache_file, true);
                var src_content = FileAtom.FromExisting(cache_file, m_disposal);
                Cobalt.Metrics o1;
                m_cobaltFile.GetCobaltFilePartition(FilePartitionId.Content).SetStream(RootId.Default.Value, src_content, out o1);
                m_cobaltFile.GetCobaltFilePartition(FilePartitionId.Content).GetStream(RootId.Default.Value).Flush();
            }
        }

        public string SessionId
        {
            get { return m_sessionId; }
            set {}
        }

        public string Login
        {
            get { return m_login; }
            set {}
        }

        public string Name
        {
            get { return m_name; }
            set {}
        }

        public string Email
        {
            get { return m_email; }
            set {}
        }

        public bool IsAnonymous
        {
            get { return m_isAnonymous; }
            set {}
        }

        public DateTime LastUpdated
        {
            get { return m_lastUpdated; }
            set {}
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

        public Bytes GetFileContent()
        {
            return new GenericFda(m_cobaltFile.CobaltEndpoint, null).GetContentStream();
        }

        public long FileLength
        {
            get
            {
                return m_fileinfo.Length;
            }
        }

        public void Save()
        {
            lock (m_fileinfo)
            {
                using (FileStream fileStream = m_fileinfo.Open(FileMode.Truncate))
                {
                    new GenericFda(m_cobaltFile.CobaltEndpoint, null).GetContentStream().CopyTo(fileStream);
                }
            }
        }

        public void Save(byte[] new_content)
        {
            lock (m_fileinfo)
            {
                using (FileStream fileStream = m_fileinfo.Open(FileMode.Truncate))
                {
                    fileStream.Write(new_content, 0, new_content.Length);
                }
            }
        }

        public void ExecuteRequestBatch(RequestBatch requestBatch)
        {
            m_cobaltFile.CobaltEndpoint.ExecuteRequestBatch(requestBatch);
            m_lastUpdated = DateTime.Now;
        }

        internal void Dispose()
        {
            m_disposal.Dispose();
        }
    }
}

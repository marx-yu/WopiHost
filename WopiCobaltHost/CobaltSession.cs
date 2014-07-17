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
    public class CobaltSession : EditSession
    {
        private readonly CobaltFile m_cobaltFile;
        private readonly DisposalEscrow m_disposal;

        public CobaltSession(string sessionId, string filePath, string login = "Anonymous", string name = "Anonymous", string email = "", bool isAnonymous = true)
            :base(sessionId, filePath, login, name, email, isAnonymous)
        {
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

        

        override public byte[] GetFileContent()
        {
            MemoryStream ms = new MemoryStream();
            new GenericFda(m_cobaltFile.CobaltEndpoint, null).GetContentStream().CopyTo(ms);
            return ms.ToArray();
        }
        override public void Save(byte[] new_content)
        { }
        override public void Save()
        {
            lock (m_fileinfo)
            {
                using (FileStream fileStream = m_fileinfo.Open(FileMode.Truncate))
                {
                    new GenericFda(m_cobaltFile.CobaltEndpoint, null).GetContentStream().CopyTo(fileStream);
                }
            }
        }

        override public void ExecuteRequestBatch(RequestBatch requestBatch) 
        {
            m_cobaltFile.CobaltEndpoint.ExecuteRequestBatch(requestBatch);
            m_lastUpdated = DateTime.Now;
        }

        override public void Dispose()
        {
            m_disposal.Dispose();
        }
    }
}

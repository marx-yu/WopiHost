// Copyright 2014 The Authors Marx-Yu. All rights reserved.
// Use of this source code is governed by a BSD-style license that can be
// found in the LICENSE file.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cobalt;
using System.Timers;

namespace WopiCobaltHost
{
    public class CobaltSessionManager
    {
        private static volatile CobaltSessionManager m_instance;
        private static object m_syncObj = new object();
        private Dictionary<String, CobaltSession> m_sessions;
        private Timer m_timer;
        private readonly int m_timeout = 60 * 60 * 1000;
        private readonly int m_closewait = 3 * 60 * 60;

        public static CobaltSessionManager Instance
        {
            get
            {
                if (CobaltSessionManager.m_instance == null)
                {
                    lock (CobaltSessionManager.m_syncObj)
                    {
                        if (CobaltSessionManager.m_instance == null)
                            CobaltSessionManager.m_instance = new CobaltSessionManager();
                    }
                }
                return CobaltSessionManager.m_instance;
            }
        }

        public CobaltSessionManager()
        {
            m_timer = new Timer(m_timeout);
            m_timer.AutoReset = true;
            m_timer.Elapsed += CleanUp;
            m_timer.Enabled = true;

            m_sessions = new Dictionary<String, CobaltSession>();
        }

        public CobaltSession GetSession(string sessionId)
        {
            CobaltSession cs;

            lock (CobaltSessionManager.m_syncObj)
            {
                if (!m_sessions.TryGetValue(sessionId, out cs))
                {
                    return null;
                }
            }

            return cs;
        }

        public void AddSession(CobaltSession session)
        {
            lock (CobaltSessionManager.m_syncObj)
            {
                m_sessions.Add(session.SessionId, session);
            }
        }

        public void DelSession(CobaltSession session)
        {
            lock (CobaltSessionManager.m_syncObj)
            {
                // clean up
                session.Dispose();
                m_sessions.Remove(session.SessionId);
            }
        }

        private void CleanUp(object sender, ElapsedEventArgs e)
        {
            lock (CobaltSessionManager.m_syncObj)
            {
                foreach (var session in m_sessions.Values)
                {
                    if (session.LastUpdated.AddSeconds(m_closewait) < DateTime.Now)
                    {
                        // save the changes to the file
                        session.Save();

                        // clean up
                        session.Dispose();
                        m_sessions.Remove(session.SessionId);
                    }
                }
            }
        }
    }
}

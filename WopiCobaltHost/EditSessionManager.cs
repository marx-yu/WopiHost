// Copyright 2014 The Authors Marx-Yu. All rights reserved.
// Use of this source code is governed by a BSD-style license that can be
// found in the LICENSE file.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;

namespace WopiCobaltHost
{
    public class EditSessionManager
    {
        private static volatile EditSessionManager m_instance;
        private static object m_syncObj = new object();
        private Dictionary<String, EditSession> m_sessions;
        private Timer m_timer;
        private readonly int m_timeout = 60 * 60 * 1000;
        private readonly int m_closewait = 3 * 60 * 60;

        public static EditSessionManager Instance
        {
            get
            {
                if (EditSessionManager.m_instance == null)
                {
                    lock (EditSessionManager.m_syncObj)
                    {
                        if (EditSessionManager.m_instance == null)
                            EditSessionManager.m_instance = new EditSessionManager();
                    }
                }
                return EditSessionManager.m_instance;
            }
        }

        public EditSessionManager()
        {
            m_timer = new Timer(m_timeout);
            m_timer.AutoReset = true;
            m_timer.Elapsed += CleanUp;
            m_timer.Enabled = true;

            m_sessions = new Dictionary<String, EditSession>();
        }

        public EditSession GetSession(string sessionId)
        {
            EditSession es;

            lock (EditSessionManager.m_syncObj)
            {
                if (!m_sessions.TryGetValue(sessionId, out es))
                {
                    return null;
                }
            }

            return es;
        }

        public void AddSession(EditSession session)
        {
            lock (EditSessionManager.m_syncObj)
            {
                m_sessions.Add(session.SessionId, session);
            }
        }

        public void DelSession(EditSession session)
        {
            lock (EditSessionManager.m_syncObj)
            {
                // clean up
                session.Dispose();
                m_sessions.Remove(session.SessionId);
            }
        }

        private void CleanUp(object sender, ElapsedEventArgs e)
        {
            lock (EditSessionManager.m_syncObj)
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

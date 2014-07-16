// Copyright 2014 The Authors Marx-Yu. All rights reserved.
// Use of this source code is governed by a BSD-style license that can be
// found in the LICENSE file.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Runtime.Serialization.Json;
using Cobalt;

namespace WopiCobaltHost
{
    public class CobaltServer
    {
        private HttpListener m_listener;
        private string m_docsPath;
        private int m_port;

        public CobaltServer(string docsPath, int port = 8080)
        {
            m_docsPath = docsPath;
            m_port = port;
        }

        public void Start()
        {
            m_listener = new HttpListener();
            m_listener.Prefixes.Add(String.Format("http://localhost:{0}/wopi/", m_port));
            m_listener.Start();
            m_listener.BeginGetContext(ProcessRequest, m_listener);

            Console.WriteLine(@"WopiServer Started");
        }

        public void Stop()
        {
            m_listener.Stop();
        }

        private void ErrorResponse(HttpListenerContext context, string errmsg)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(errmsg);
            context.Response.ContentLength64 = buffer.Length;
            context.Response.ContentType = @"application/json";
            context.Response.OutputStream.Write(buffer, 0, buffer.Length);
            context.Response.OutputStream.Close();
            context.Response.StatusCode = 500;
            m_listener.BeginGetContext(ProcessRequest, m_listener);
        }

        private void ProcessCobaltRequest(HttpListenerContext context)
        {
        }
        
        private void ProcessWopiRequest(HttpListenerContext context)
        {
        }

        private void ProcessRequest(IAsyncResult result)
        {
            try
            {
                HttpListener listener = (HttpListener)result.AsyncState;
                HttpListenerContext context = listener.EndGetContext(result);

                var stringarr = context.Request.Url.AbsolutePath.Split('/');
                var access_token = context.Request.QueryString["access_token"];

                if (stringarr.Length < 3 || access_token == null)
                {
                    Console.WriteLine(@"Invalid request");
                    ErrorResponse(context, @"Invalid request parameter");
                    return;
                }

                var filename = stringarr[3];
                //use filename as session id just test, recommend use file id and lock id as session id
                CobaltSession fileSession = CobaltSessionManager.Instance.GetSession(filename);
                if (fileSession == null)
                {
                    fileSession = new CobaltSession(filename, m_docsPath + "/" + filename, @"yonggui.yu", @"yuyg", @"yonggui.yu@emacle.com", false);
                    CobaltSessionManager.Instance.AddSession(fileSession);
                }

                if (stringarr.Length == 5 && context.Request.HttpMethod.Equals(@"GET"))
                {
                    // get file's content
                    var content = fileSession.GetFileContent();
                    context.Response.ContentType = @"application/octet-stream";
                    context.Response.ContentLength64 = content.Length;
                    content.CopyTo(context.Response.OutputStream);
                    context.Response.Close();
                }
                else if (context.Request.HttpMethod.Equals(@"POST") && context.Request.Headers["X-WOPI-Override"].Equals("COBALT"))
                {
                    var ms = new MemoryStream();
                    context.Request.InputStream.CopyTo(ms);
                    AtomFromByteArray atomRequest = new AtomFromByteArray(ms.ToArray());
                    RequestBatch requestBatch = new RequestBatch();

                    Object ctx;
                    ProtocolVersion protocolVersion;

                    requestBatch.DeserializeInputFromProtocol(atomRequest, out ctx, out protocolVersion);
                    fileSession.ExecuteRequestBatch(requestBatch);

                    foreach (Request request in requestBatch.Requests)
                    {
                        if (request.GetType() == typeof(PutChangesRequest) && request.PartitionId == FilePartitionId.Content)
                        {
                            fileSession.Save();
                        }
                    }
                    var response = requestBatch.SerializeOutputToProtocol(protocolVersion);

                    context.Response.Headers.Add("X-WOPI-CorellationID", context.Request.Headers["X-WOPI-CorrelationID"]);
                    context.Response.Headers.Add("request-id", context.Request.Headers["X-WOPI-CorrelationID"]);
                    context.Response.ContentType = @"application/octet-stream";
                    context.Response.ContentLength64 = response.Length;
                    response.CopyTo(context.Response.OutputStream);
                    context.Response.Close();
                }
                else if (stringarr.Length == 4 && context.Request.HttpMethod.Equals(@"GET"))
                {
                    // encode json
                    var memoryStream = new MemoryStream();
                    var json = new DataContractJsonSerializer(typeof(WopiCheckFileInfo));
                    json.WriteObject(memoryStream, fileSession.GetCheckFileInfo());
                    memoryStream.Flush();
                    memoryStream.Position = 0;
                    StreamReader streamReader = new StreamReader(memoryStream);
                    var jsonResponse = Encoding.UTF8.GetBytes(streamReader.ReadToEnd());

                    context.Response.ContentType = @"application/json";
                    context.Response.ContentLength64 = jsonResponse.Length;
                    context.Response.OutputStream.Write(jsonResponse, 0, jsonResponse.Length);
                    context.Response.Close();
                }
                else
                {
                    Console.WriteLine(@"Invalid request parameters");
                    ErrorResponse(context, @"Invalid request cobalt parameter");
                }

                m_listener.BeginGetContext(ProcessRequest, m_listener);
            }
            catch (Exception ex)
            {
                Console.WriteLine(@"process request exception:" + ex.Message);
                return;
            }
        }
    }
}

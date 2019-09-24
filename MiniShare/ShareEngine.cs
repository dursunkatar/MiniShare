using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;

namespace MiniShare
{
    public class ShareEngine
    {
        private readonly HttpListener listener;
        private static volatile object obj = new object();
        private static readonly List<SharedFileInfo> SharedFileList;

        static ShareEngine()
        {
            SharedFileList = new List<SharedFileInfo>();
        }

        public ShareEngine()
        {
            listener = new HttpListener();
            init();
        }

        private void init()
        {
            listener.Prefixes.Add("http://localhost:8085/");
            listener.Prefixes.Add("http://127.0.0.1:8085/");
            if (IpAddress.LocalIpAddress != null)
            {
                listener.Prefixes.Add($"http://{IpAddress.LocalIpAddress}:8085/");
            }
            listener.Start();
            getContext();
        }

        private void getContext()
        {
            new Thread(() =>
            {
                while (true)
                {

                    HttpListenerContext context;
                    try
                    {
                        context = listener.GetContext();
                    }
                    catch
                    {
                        return;
                    }

                    new Thread(() =>
                    {
                        var request = context.Request;

                        string queryString = request.RawUrl.Substring(1, request.RawUrl.Length - 1);
                        SharedFileInfo sharedFileInfo = null; ;
                        lock (obj)
                        {
                            sharedFileInfo = SharedFileList.FirstOrDefault(file => file.SharePath == queryString);
                        }
                        if (sharedFileInfo != null)
                        {
                            var response = context.Response;
                            response.SendChunked = false;
                            response.ContentType = System.Net.Mime.MediaTypeNames.Application.Octet;
                            response.AddHeader("Content-disposition", "attachment; filename=" + sharedFileInfo.Name);

                            byte[] buffer = sharedFileInfo.Bytes;
                            response.ContentLength64 = buffer.Length;
                            Stream output = response.OutputStream;
                            output.Write(buffer, 0, buffer.Length);
                            output.Flush();
                            output.Close();
                            output.Dispose();
                            response.StatusCode = (int)HttpStatusCode.OK;
                            response.StatusDescription = "OK";
                            response.OutputStream.Close();
                        }
                    })
                     .Start();
                }
            })
             .Start();
        }

        public void AddFile(SharedFileInfo fileInfo)
        {
            lock (obj)
            {
                SharedFileList.Add(fileInfo);
            }
        }

        public void RemoveFile(string path)
        {
            lock (obj)
            {
                SharedFileList.Remove(SharedFileList.First(file => file.Path == path));
            }
        }

        public void Close()
        {
            listener.Close();
        }
    }
}

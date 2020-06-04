using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Threading;

namespace UtilsLib.HttpUtils
{
    public class HttpServer
    {
        public const string search_version = "/search_version";

        public const string download_files = "/download_files";
        /// <summary>
        /// HttpListener
        /// </summary>
        HttpListener httpListener;
        /// <summary>
        /// 日志
        /// </summary>
        IMyLogFile myLogFile = new ServerLogFile();

        public HttpServer()
        {
            if (httpListener == null)
                httpListener = new HttpListener();
        }
        /// <summary>
        /// 添加监听服务
        /// </summary>
        /// <param name="Service"></param>
        public bool AddService(string serviceUrl)
        {
            bool isOk = false;
            try
            {
                httpListener.Prefixes.Add(serviceUrl);
            }
            catch (Exception ex)
            {
                isOk = false;
                myLogFile.WriteErrLogFile("添加HttpListener监听地址异常" + serviceUrl + ";" + ex.ToString());
            }
            return isOk;
        }

        /// <summary>
        /// 是否正在监听
        /// </summary>
        public bool IsListening
        {
            get
            {
                return (httpListener != null && httpListener.IsListening);
            }
        }
        /// <summary>
        /// 启动监听
        /// </summary>
        public void Start()
        {
            try
            {
                if (httpListener.Prefixes.Count <= 0)
                {
                    string host = Dns.GetHostName();
                    httpListener.Prefixes.Add("http://" + host + ":9981/");
                }

                //启动服务
                httpListener.Start();
                httpListener.BeginGetContext(new AsyncCallback(GetContextCallBack), httpListener);
            }
            catch (Exception)
            {
                throw;
            }
        }
        /// <summary>
        /// 停止监听
        /// </summary>
        public void Stop()
        {
            try
            {
                if (httpListener != null)
                {
                    httpListener.Close();
                    httpListener = null;
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        /// <summary>
        /// 异步处理接收请求
        /// </summary>
        /// <param name="ar"></param>
        private void GetContextCallBack(IAsyncResult ar)
        {
            try
            {
                if (httpListener == null || !httpListener.IsListening)
                    return;
                httpListener = ar.AsyncState as HttpListener;
                HttpListenerContext context = httpListener.EndGetContext(ar);
                //再次监听请求
                httpListener.BeginGetContext(new AsyncCallback(GetContextCallBack), httpListener);
                //处理请求
                ProcessRequest(context);
            }
            catch (Exception ex)
            {
                myLogFile.WriteErrLogFile("异步处理接收请求异常," + ex.ToString());
            }
        }

        private void ProcessRequest(HttpListenerContext context)
        {
            HttpListenerResponse response = context.Response;
            HttpListenerRequest request = context.Request;
            try
            {
                if (request.HttpMethod.ToLower().Equals("get"))
                {
                    string rawUrl = System.Web.HttpUtility.UrlDecode(request.RawUrl);

                    #region 获取版本信息
                    if (rawUrl.StartsWith(search_version))
                    {
                        string groupid = context.Request.QueryString["groupid"];

                        XmlSerializerWrapper<RFEntity.XmlUpdateFiles> xmlSerializer_UpdateFiles = new XmlSerializerWrapper<RFEntity.XmlUpdateFiles>();
                        List<RFEntity.UpdateFile> lstUpdateFile = xmlSerializer_UpdateFiles.Entity.lstupdatefiles;
                        if (lstUpdateFile != null && lstUpdateFile.Count > 0)
                        {
                            RFEntity.UpdateFile file = lstUpdateFile.Find(item => { return item.groupid.ToString() == groupid; });
                            if (file != null)
                            {
                                string files = JsonHelper.GetJson(file);
                                Response(context, files);
                            }
                        }
                    }
                    #endregion

                    #region 下载文件
                    if (rawUrl.StartsWith(download_files))
                    {
                        string groupid = context.Request.QueryString["groupid"];
                        string versioninfo = context.Request.QueryString["versioninfo"];

                        XmlSerializerWrapper<RFEntity.XmlUpdateFiles> xmlSerializer_UpdateFiles = new XmlSerializerWrapper<RFEntity.XmlUpdateFiles>();
                        List<RFEntity.UpdateFile> lstUpdateFile = xmlSerializer_UpdateFiles.Entity.lstupdatefiles;
                        if (lstUpdateFile != null && lstUpdateFile.Count > 0)
                        {
                            RFEntity.UpdateFile file = lstUpdateFile.Find(item => { return item.groupid.ToString() == groupid && item.versioninfo == versioninfo; });
                            if (file != null && !string.IsNullOrEmpty(file.filepath) && File.Exists(file.filepath))
                            {
                                response.StatusCode = 200;

                                FileStream fileStream = new System.IO.FileStream(file.filepath, System.IO.FileMode.Open, System.IO.FileAccess.Read, FileShare.ReadWrite);
                                int l = 0;
                                byte[] fileBytes = new byte[1024];
                                while ((l = fileStream.Read(fileBytes, 0, fileBytes.Length)) > 0)
                                {
                                    response.OutputStream.Write(fileBytes, 0, l);
                                }
                                fileStream.Close();
                                fileStream.Dispose();
                                response.OutputStream.Close();
                            }
                        }
                    }
                    #endregion
                }
            }
            catch (Exception ex)
            {
                myLogFile.WriteErrLogFile("客户端获取文件信息异常" + request.RemoteEndPoint.Address + ";" + ex);
            }

            try
            {
                if (response != null)
                    response.Close();
            }
            catch
            {
            }
        }

        /// <summary>
        /// 处理输入参数
        /// </summary>
        /// <param name="request">请求信息</param>
        /// <returns></returns>
        private string Request(HttpListenerRequest request)
        {
            string retunStr = string.Empty;
            //POST请求处理
            string urlName = request.RawUrl.ToString();
            try
            {

                if (request.HttpMethod.ToLower().Equals("get"))
                {

                    retunStr = "不支持GET请求";
                    myLogFile.WriteLogFile("停车服务接收get数据：" + request.Url.ToString());
                }
                else if (request.HttpMethod.ToLower().Equals("post"))
                {

                    Stream SourceStream = request.InputStream;
                    byte[] currentChunk = ReadLineAsBytes(SourceStream);

                    if (currentChunk != null)
                    {
                        //获取数据中有空白符需要去掉，输出的就是post请求的参数字符串 如：username=linezero
                        string postJson = Encoding.UTF8.GetString(currentChunk).Replace("�", "");
                        myLogFile.WriteLogFile(string.Format("停车服务路径:{0},接收post原始数据:{1}", urlName, postJson));
                    }
                }
            }
            catch (Exception ex)
            {
                myLogFile.WriteErrLogFile("处理停车服务请求失败," + urlName + ex.ToString());
            }
            finally
            {
                if (!string.IsNullOrEmpty(retunStr))
                    myLogFile.WriteLogFile(string.Format("停车服务路径:{0},返回数据:{1}", urlName, retunStr));
            }
            return retunStr;
        }
        /// <summary>
        /// 输出方法
        /// </summary>
        /// <param name="response">response对象</param>
        /// <param name="responseString">输出值</param>
        /// <param name="contenttype">输出类型默认为json</param>
        private void Response(HttpListenerContext context, string responsestring)
        {
            try
            {
                context.Response.StatusCode = 200;
                context.Response.ContentType = context.Request.ContentType;
                context.Response.ContentEncoding = context.Request.ContentEncoding;
                byte[] buffer = context.Request.ContentEncoding.GetBytes(responsestring);
                //对客户端输出相应信息.
                context.Response.ContentLength64 = buffer.Length;
                System.IO.Stream output = context.Response.OutputStream;
                output.Write(buffer, 0, buffer.Length);
                //关闭输出流，释放相应资源
                output.Close();
            }
            catch (Exception ex)
            {
                myLogFile.WriteErrLogFile("返回请求失败," + ex.ToString());
            }
        }
        /// <summary>
        /// 读取数据
        /// </summary>
        /// <param name="SourceStream"></param>
        /// <returns></returns>
        private byte[] ReadLineAsBytes(Stream SourceStream)
        {
            byte[] dataBytes = null;
            try
            {
                var resultStream = new MemoryStream();
                while (true)
                {
                    int data = SourceStream.ReadByte();
                    resultStream.WriteByte((byte)data);
                    if (data <= 0)
                        break;
                }
                resultStream.Position = 0;
                dataBytes = new byte[resultStream.Length];
                resultStream.Read(dataBytes, 0, dataBytes.Length);
            }
            catch (Exception ex)
            {
                myLogFile.WriteErrLogFile("读取请求失败," + ex.ToString());
            }
            return dataBytes;
        }
    }
}

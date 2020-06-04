using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.IO;

namespace UtilsLib.HttpUtils
{
    public class HttpClient
    {
        /// <summary>
        /// 上传基础数据
        /// </summary>
        /// <param name="url"></param>
        /// <param name="dataStr"></param>
        /// <returns></returns>
        public static string Post(string url, string dataStr)
        {
            System.GC.Collect();//垃圾回收，回收没有正常关闭的http连接
            string retStr = "";//数据返回
            HttpWebRequest request = null;
            HttpWebResponse response = null;

            byte[] data = Encoding.GetEncoding("utf-8").GetBytes(dataStr);

            if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(ValidateServerCertificate);
                request = WebRequest.Create(url) as HttpWebRequest;
                request.ProtocolVersion = HttpVersion.Version10;
            }
            else
            {
                request = WebRequest.Create(url) as HttpWebRequest;
            }
            //HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

            request.Method = "POST";
            request.ContentType = "application/json;charset=utf-8";
            request.ContentLength = data.Length;
            request.Proxy = null;
            request.Timeout = 10000;
            try
            {
                Stream sm = request.GetRequestStream();
                sm.Write(data, 0, data.Length);
                sm.Close();

                response = (HttpWebResponse)request.GetResponse();

                using (StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding("utf-8")))
                {
                    retStr = sr.ReadToEnd();
                }
            }
            finally
            {
                //关闭连接和流
                if (response != null)
                {
                    response.Close();
                }
                if (request != null)
                {
                    request.Abort();
                }
            }
            return retStr;
        }
        /// <summary>
        /// 处理http GET请求，返回数据
        /// </summary>
        /// <param name="url">请求的url地址</param>
        /// <returns>http GET成功后返回的数据，失败抛WebException异常</returns>
        public static string Get(string url)
        {
            System.GC.Collect();
            string result = "";

            HttpWebRequest request = null;
            HttpWebResponse response = null;

            //请求url以获取数据
            try
            {
                //设置最大连接数
                ServicePointManager.DefaultConnectionLimit = 200;
                //设置https验证方式
                if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
                {
                    ServicePointManager.ServerCertificateValidationCallback =
                            new RemoteCertificateValidationCallback(ValidateServerCertificate);
                }

                /***************************************************************
                * 下面设置HttpWebRequest的相关属性
                * ************************************************************/
                request = (HttpWebRequest)WebRequest.Create(url);

                request.Method = "GET";
                ////设置代理
                //WebProxy proxy = new WebProxy();
                //proxy.Address = new Uri(WxPayConfig.PROXY_URL);
                //request.Proxy = proxy;

                //获取服务器返回
                response = (HttpWebResponse)request.GetResponse();

                //获取HTTP返回数据
                StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                result = sr.ReadToEnd().Trim();
                sr.Close();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            finally
            {
                //关闭连接和流
                if (response != null)
                {
                    response.Close();
                }
                if (request != null)
                {
                    request.Abort();
                }
            }
            return result;
        }
        /// <summary>
        /// 处理http GET请求，返回数据
        /// </summary>
        /// <param name="url">下载地址</param>
        /// <param name="filename">下载文件重命名</param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static bool DownLoadFile(string url, string filename, out string msg)
        {
            System.GC.Collect();

            bool isOk = false;
            msg = "";

            HttpWebRequest request = null;
            HttpWebResponse response = null;

            //请求url以获取数据
            try
            {
                //设置最大连接数
                ServicePointManager.DefaultConnectionLimit = 200;
                //设置https验证方式
                if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
                {
                    ServicePointManager.ServerCertificateValidationCallback =
                            new RemoteCertificateValidationCallback(ValidateServerCertificate);
                }

                /***************************************************************
                * 下面设置HttpWebRequest的相关属性
                * ************************************************************/
                request = (HttpWebRequest)WebRequest.Create(url);

                request.Method = "GET";

                ////设置代理
                //WebProxy proxy = new WebProxy();
                //proxy.Address = new Uri(WxPayConfig.PROXY_URL);
                //request.Proxy = proxy;

                //获取服务器返回
                response = (HttpWebResponse)request.GetResponse();
                //创建相关目录
                FileHelper.CreateDirtory(Path.GetDirectoryName(filename));

                using (FileStream fs = new FileStream(filename, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite))
                {
                    BufferedStream sm = new BufferedStream(response.GetResponseStream());

                    byte[] buffer = new byte[1024];
                    int r = -1;
                    while ((r = sm.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        fs.Write(buffer, 0, r);
                    }
                    sm.Close();
                }

                isOk = File.Exists(filename);
            }
            catch (Exception e)
            {
                msg = e.ToString();
            }
            finally
            {
                //关闭连接和流
                if (response != null)
                {
                    response.Close();
                }
                if (request != null)
                {
                    request.Abort();
                }
            }

            return isOk;
        }

        /// <summary>
        /// 验证证书
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="certificate"></param>
        /// <param name="chain"></param>
        /// <param name="sslPolicyErrors"></param>
        /// <returns></returns>
        public static bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }
    }
}

using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows.Media.Imaging;

namespace Suretom.Client.Common
{
    /// <summary>
    /// 与服务器端接接口的http通信类
    /// </summary>
    //public class DefaultHttpChannel : IHttpChannel
    //{
    //    /// <summary>
    //    /// Get请求
    //    /// </summary>
    //    /// <returns></returns>
    //    public HttpResult Get(string url, Encoding encode = null)
    //    {
    //        try
    //        {
    //            WebRequest request = WebRequest.Create(url);
    //            if (url.ToLower().StartsWith("https"))
    //            {
    //                //用于解决：基础连接已经关闭: 未能为 SSL/TLS 安全通道建立信任关系。
    //                ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
    //            }

    //            request.Method = "Get";
    //            request.Timeout = 120000;
    //            //request.ContentType = "application/json";
    //            using (StreamWriter pushStream = new StreamWriter(request.GetRequestStream()))
    //            {
    //            }

    //            using (WebResponse response = request.GetResponse())
    //            {
    //                using (Stream stream = response.GetResponseStream())
    //                {
    //                    using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
    //                    {
    //                        string value = reader.ReadToEnd();
    //                        if (value.IndexOf("-ERR") == 0 || value.IndexOf("-err") == 0)
    //                        {
    //                            value = value.Replace("-ERR ", "").Replace("-err ", "");

    //                            return new HttpResult(value);
    //                        }
    //                        else
    //                        {
    //                            var result = JObject.Parse(value);
    //                            var success = result["status"]?.ToString() == "200" ? true : false;
    //                            if (success)
    //                            {
    //                                return new HttpResult(true, result["data"]);
    //                            }
    //                            else
    //                                return new HttpResult(result["message"]?.ToString());
    //                        }
    //                    }
    //                }
    //            }
    //        }
    //        catch (Exception ex)
    //        {
    //            return new HttpResult(ex.Message);
    //        }
    //    }

    //    /// <summary>
    //    /// Post请求
    //    /// </summary>
    //    /// <param name="url">请求的Url</param>
    //    /// <param name="requestParam">请求参数，必须为Json字符串</param>
    //    /// <returns></returns>
    //    public HttpResult Post(string url, string requestParam)
    //    {
    //        try
    //        {
    //            WebRequest request = WebRequest.Create(url);
    //            if (url.ToLower().StartsWith("https"))
    //            {
    //                //用于解决：基础连接已经关闭: 未能为 SSL/TLS 安全通道建立信任关系。
    //                ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
    //            }

    //            request.Method = "Post";
    //            request.Timeout = 120000;
    //            request.ContentType = "application/x-www-form-urlencoded";
    //            using (StreamWriter pushStream = new StreamWriter(request.GetRequestStream()))
    //            {
    //                pushStream.Write(requestParam);
    //            }

    //            using (WebResponse response = request.GetResponse())
    //            {
    //                using (Stream stream = response.GetResponseStream())
    //                {
    //                    using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
    //                    {
    //                        string value = reader.ReadToEnd();
    //                        if (value.IndexOf("-ERR") == 0 || value.IndexOf("-err") == 0)
    //                        {
    //                            value = value.Replace("-ERR ", "").Replace("-err ", "");

    //                            return new HttpResult(value);
    //                        }
    //                        else
    //                        {
    //                            var result = JObject.Parse(value);
    //                            var success = result["status"]?.ToString() == "200" ? true : false;
    //                            if (success)
    //                            {
    //                                return new HttpResult(true, result["data"]);
    //                            }
    //                            else
    //                                return new HttpResult(result["message"]?.ToString());
    //                        }
    //                    }
    //                }
    //            }
    //        }
    //        catch (Exception ex)
    //        {
    //            return new HttpResult(ex.Message);
    //        }
    //    }

    //    /// <summary>
    //    /// 上传文件
    //    /// </summary>
    //    /// <param name="url">请求的Url</param>
    //    /// <param name="updateFileParams">上传文件参数</param>
    //    /// <returns></returns>
    //    public HttpResult UpdateFile(string url, IList<UpdateFileParam> updateFileParams)
    //    {
    //        try
    //        {
    //            var fileParam = updateFileParams.FirstOrDefault(x => x.IsFile);
    //            if (fileParam != null)
    //            {
    //                if (fileParam.FileContent == null)
    //                {
    //                    throw new Exception("未设置文件流");
    //                }
    //                if (fileParam.FileContent.Length == 0)
    //                {
    //                    throw new Exception("文件无数据");
    //                }
    //            }
    //            else
    //            {
    //                throw new Exception("无文件参数");
    //            }

    //            WebRequest request = WebRequest.Create(url);
    //            request.Method = "Post";
    //            request.Timeout = 60000;
    //            request.ContentType = "application/octet-stream";

    //            #region param

    //            foreach (var item in updateFileParams)
    //            {
    //                request.Headers.Add(item.Key, System.Net.WebUtility.UrlEncode(item.Value));
    //            }

    //            using (var fs = fileParam.FileContent)
    //            {
    //                using (Stream pushStream = request.GetRequestStream())
    //                {
    //                    var buffer = new byte[1024];
    //                    int bytesRead = 0;
    //                    while ((bytesRead = fs.Read(buffer, 0, buffer.Length)) != 0)
    //                    {
    //                        pushStream.Write(buffer, 0, bytesRead);
    //                    }
    //                }
    //            }

    //            #endregion param

    //            //请求
    //            using (WebResponse response = request.GetResponse())
    //            {
    //                using (Stream stream = response.GetResponseStream())
    //                {
    //                    using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
    //                    {
    //                        string value = reader.ReadToEnd();
    //                        if (value.IndexOf("-ERR") == 0 || value.IndexOf("-err") == 0)
    //                        {
    //                            value = value.Replace("-ERR ", "").Replace("-err ", "");

    //                            return new HttpResult(value);
    //                        }
    //                        else
    //                        {
    //                            var result = JObject.Parse(value);
    //                            var success = result["status"]?.ToString() == "200" ? true : false;
    //                            if (success)
    //                            {
    //                                return new HttpResult(true, result["data"]);
    //                            }
    //                            else
    //                                return new HttpResult(result["message"]?.ToString());
    //                        }
    //                    }
    //                }
    //            }
    //        }
    //        catch (Exception ex)
    //        {
    //            var fileParam = updateFileParams.FirstOrDefault(x => x.IsFile);
    //            if (fileParam != null && fileParam.FileContent != null)
    //            {
    //                fileParam.FileContent.Dispose();
    //            }

    //            return new HttpResult(ex.Message);
    //        }
    //    }
    //}
}
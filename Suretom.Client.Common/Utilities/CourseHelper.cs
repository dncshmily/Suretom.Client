using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace Suretom.Client.Common
{
    public static class CourseHelper
    {
        /// <summary>
        /// Html中的src换成oss真实地址
        /// </summary>
        /// <param name="html">html内容</param>
        /// <returns></returns>
        public static string ReplaceOssUrl(string html)
        {
            var regex = new Regex(@"(?is)<a(?:(?!data-schoolcode=).)*data-schoolcode=(['""]?)(?<schoolcode>[^""\s>]*)\1[^>]*>(?<text>(?:(?!</?a\b).)*)</a>", RegexOptions.IgnoreCase);
            var matches = regex.Matches(html);

            var schoolcode = "";
            foreach (Match item in matches)
            {
                schoolcode = item.Groups["schoolcode"].ToString();
                if (!string.IsNullOrEmpty(schoolcode)) break;

                var allimg = item.ToString(); //完整a标签
                regex = new Regex("data-schoolcode=\"([^\"]+)\"", RegexOptions.IgnoreCase);
                schoolcode = regex.Match(allimg).Groups[1].ToString();
                if (!string.IsNullOrEmpty(schoolcode)) break;
            }

            return schoolcode;
        }

        /// <summary>
        /// Post提交数据
        /// </summary>
        /// <param name="postUrl">URL</param>
        /// <param name="paramData">参数</param>
        /// <returns></returns>
        public static string FromPost(string postUrl, string paramData, Encoding dataEncode)
        {
            string ret = string.Empty;
            try
            {
                byte[] byteArray = Encoding.Default.GetBytes(paramData);
                HttpWebRequest webReq = (HttpWebRequest)WebRequest.Create(new Uri(postUrl));

                ServicePointManager.Expect100Continue = false;
                webReq.Method = "POST";
                webReq.ContentType = "application/x-www-form-urlencoded";
                webReq.ContentLength = byteArray.Length;
                using (Stream newStream = webReq.GetRequestStream())
                {
                    newStream.Write(byteArray, 0, byteArray.Length);//写入参数
                    newStream.Close();
                    using (HttpWebResponse response = (HttpWebResponse)webReq.GetResponse())
                    {
                        var cookie = response.Headers["Set-Cookie"]?.ToString();
                        using (StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                        {
                            ret = cookie; //sr.ReadToEnd();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return ret;
        }

        /// <summary>
        /// Post提交数据
        /// </summary>
        /// <param name="postUrl">URL</param>
        /// <param name="paramData">参数</param>
        /// <returns></returns>
        public static string FromPost(string postUrl, Dictionary<string, string> header, string paramData)
        {
            string ret = string.Empty;
            try
            {
                byte[] byteArray = Encoding.Default.GetBytes(paramData);
                HttpWebRequest webReq = (HttpWebRequest)WebRequest.Create(new Uri(postUrl));

                if (header != null)
                {
                    foreach (var head in header)
                    {
                        webReq.Headers.Add(head.Key, head.Value);
                    }
                }

                ServicePointManager.Expect100Continue = false;
                webReq.Method = "POST";
                webReq.ContentType = "application/x-www-form-urlencoded";
                webReq.ContentLength = byteArray.Length;
                using (Stream newStream = webReq.GetRequestStream())
                {
                    newStream.Write(byteArray, 0, byteArray.Length);//写入参数
                    newStream.Close();
                    using (HttpWebResponse response = (HttpWebResponse)webReq.GetResponse())
                    {
                        using (StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                        {
                            ret = sr.ReadToEnd();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return ret;
        }

        /// <summary>
        /// Get请求
        /// </summary>
        /// <returns></returns>
        public static string HttpGet(string url, Dictionary<string, string> header, Encoding encode = null)
        {
            var result = string.Empty;

            try
            {
                var webClient = new WebClient { Encoding = Encoding.UTF8 };
                if (encode != null) webClient.Encoding = encode;

                ServicePointManager.Expect100Continue = false;
                if (url.StartsWith("https")) ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                if (header != null)
                {
                    foreach (var head in header)
                    {
                        webClient.Headers.Add(head.Key, head.Value);
                    }
                }

                webClient.Headers.Add("Content-Type", "application/json");
                var sendData = Encoding.GetEncoding("UTF-8").GetBytes("");
                webClient.Headers.Add("ContentLength", sendData.Length.ToString(CultureInfo.InvariantCulture));
                var readData = webClient.DownloadData(url);
                result = Encoding.GetEncoding("UTF-8").GetString(readData);
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }
            return result;
        }

        /// <summary>
        ///  AES 加密
        /// </summary>
        /// <param name="str">明文（待加密）</param>
        /// <param name="key">密文</param>
        /// <returns></returns>
        public static string AesEncrypt(string str, string key)
        {
            if (string.IsNullOrEmpty(str)) return null;
            Byte[] toEncryptArray = Encoding.UTF8.GetBytes(str);

            RijndaelManaged rm = new RijndaelManaged
            {
                Key = Encoding.UTF8.GetBytes(key),
                Mode = CipherMode.ECB,
                Padding = PaddingMode.PKCS7
            };

            ICryptoTransform cTransform = rm.CreateEncryptor();
            Byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }

        /// <summary>
        ///  AES 解密
        /// </summary>
        /// <param name="str">明文（待解密）</param>
        /// <param name="key">密文</param>
        /// <returns></returns>
        public static string AesDecrypt(string str, string key)
        {
            if (string.IsNullOrEmpty(str)) return null;
            Byte[] toEncryptArray = Convert.FromBase64String(str);

            RijndaelManaged rm = new RijndaelManaged
            {
                Key = Encoding.UTF8.GetBytes(key),
                Mode = CipherMode.ECB,
                Padding = PaddingMode.PKCS7
            };

            ICryptoTransform cTransform = rm.CreateDecryptor();
            Byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

            return Encoding.UTF8.GetString(resultArray);
        }
    }
}
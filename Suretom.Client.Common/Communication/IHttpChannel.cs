using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.Text;

namespace Suretom.Client.Common
{
    /// <summary>
    /// 与服务器端接接口的http通信接口
    /// </summary>
    public interface IHttpChannel
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="url"></param>
        /// <param name="encode"></param>
        /// <returns></returns>
        HttpResult Get(string url, Encoding encode = null);

        /// <summary>
        /// Post请求
        /// </summary>
        /// <param name="url">请求的Url</param>
        /// <param name="requestParam">请求参数，必须为Json字符串</param>
        /// <returns></returns>
        HttpResult Post(string url, string requestParam);

        /// <summary>
        /// Post请求
        /// </summary>
        /// <param name="url">请求的Url</param>
        /// <param name="valueCollection">请求参数</param>
        /// <returns></returns>
        HttpResult PostForm(string url, NameValueCollection valueCollection, CookieContainer cookie);

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="url">请求的Url</param>
        /// <param name="updateFileParams">上传文件参数</param>
        /// <returns></returns>
        HttpResult UpdateFile(string url, IList<UpdateFileParam> updateFileParams);
    }
}
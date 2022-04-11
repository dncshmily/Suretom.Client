using Suretom.Client.Entity;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace Suretom.Client.Common
{
    /// <summary>
    /// 服务基类
    /// </summary>
    public class ServiceBase
    {
        private static NLog.Logger ez_log = NLog.LogManager.GetCurrentClassLogger();

        /// <summary>
        /// 通信处理类
        /// </summary>
        protected IHttpChannel HttpChannel { get; set; }

        /// <summary>
        /// 服务接口信息
        /// </summary>
        public ServiceInfo ServiceInfo { get; protected set; }

        /// <summary>
        /// Url地址
        /// </summary>
        protected Dictionary<string, string> Urls { get; private set; } = new Dictionary<string, string>();

        public ServiceBase()
        {
            ServiceInfo = new ServiceInfo { ApiUrl = GlobalContext.ClientApiUrl, Token = GlobalContext.Token };

            HttpChannel = GlobalContext.Resolve<IHttpChannel>();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="httpJsonParam">参数Json</param>
        /// <param name="url">调用的Url</param>
        /// <param name="serviceType">调用的服务类型</param>
        /// <returns></returns>
        protected HttpResult Get(string httpJsonParam, string url, Type serviceType)
        {
            if (GlobalContext.ShowServiceTraceLog)
                TraceRequest(serviceType, httpJsonParam);

            var result = HttpChannel.Get($"{GlobalContext.ClientApiUrl}{url}");

            return result;
        }

        /// <summary>
        /// 通用的服务调用
        /// </summary>
        /// <param name="httpJsonParam">参数Json</param>
        /// <param name="url">调用的Url</param>
        /// <param name="serviceType">调用的服务类型</param>
        /// <returns></returns>
        protected HttpResult Post(string httpJsonParam, string url, Type serviceType)
        {
            if (GlobalContext.ShowServiceTraceLog)
                TraceRequest(serviceType, httpJsonParam);

            var jiekou = $"{GlobalContext.ClientApiUrl}{url}";

            var result = HttpChannel.Post(jiekou, httpJsonParam);

            if (GlobalContext.ShowServiceTraceLog)
            {
                TraceResponse(serviceType, result);
            }

            return result;
        }

        /// <summary>
        /// 通用的服务调用
        /// </summary>
        /// <param name="paramList">参数List</param>
        /// <param name="url">调用的Url</param>
        /// <param name="serviceType">调用的服务类型</param>
        /// <returns></returns>
        protected HttpResult PostForm(string url, NameValueCollection paramValue)
        {
            var result = HttpChannel.PostForm($"{GlobalContext.ClientApiUrl}{url}", paramValue, null);

            return result;
        }

        #region 日志

        /// <summary>
        /// 跟踪请求信息
        /// </summary>
        /// <param name="type"></param>
        /// <param name="paramJson"></param>
        protected void TraceRequest(Type type, string paramJson)
        {
            ez_log.Trace("【{0}】--> {1}", type.FullName, paramJson);
        }

        /// <summary>
        /// 跟踪结果信息
        /// </summary>
        /// <param name="type"></param>
        /// <param name="result"></param>
        protected void TraceResponse(Type type, HttpResult result)
        {
            ez_log.Trace("【{0}】<-- Success:{1}  Message:{2}  Data:{3}", type.FullName, result.Success, result.Message, result.Data?.ToString());
        }

        #endregion 日志
    }
}
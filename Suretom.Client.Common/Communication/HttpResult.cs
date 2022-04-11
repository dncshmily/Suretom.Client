using Newtonsoft.Json.Linq;

namespace Suretom.Client.Common
{
    /// <summary>
    /// HttpChannel请求结果
    /// </summary>
    public class HttpResult
    {
        public HttpResult(string message) => this.Message = message;

        public HttpResult(bool success, JToken data)
        {
            this.Success = success;
            this.Data = data;
        }

        /// <summary>
        /// 请求是否成功，true成功，false失败
        /// </summary>
        public bool Success { set; get; } = false;

        /// <summary>
        /// 信息，成功时一般为空，失败时，为错误信息
        /// </summary>
        public string Message { set; get; } = string.Empty;

        /// <summary>
        /// 请求返回的数据
        /// </summary>
        public JToken Data { set; get; }
    }
}
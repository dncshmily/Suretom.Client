namespace Suretom.Client.Entity
{
    /// <summary>
    /// 服务接口配置
    /// </summary>
    public class ServiceInfo
    {
        /// <summary>
        /// 服务接口基地址
        /// </summary>
        public string ApiUrl { get; set; }

        /// <summary>
        /// 验证信息
        /// </summary>
        public string Token { get; set; }
    }
}
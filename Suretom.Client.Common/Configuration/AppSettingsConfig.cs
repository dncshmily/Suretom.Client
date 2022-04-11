namespace Suretom.Client.Common
{
    /// <summary>
    ///  AppSettings配置
    /// </summary>
    public class AppSettingsConfig
    {
        /// <summary>
        /// 主页的本
        /// </summary>
        public string MainPageUrl { get; private set; }

        /// <summary>
        /// 客户端使用Api
        /// </summary>
        public string ClientApiUrl { get; private set; }

        public AppSettingsConfig()
        {
            //MainPageUrl = ConfigurationManager.AppSettings["MainPageUrl"];
            //ClientApiUrl = ConfigurationManager.AppSettings["ClientApiUrl"];
        }
    }
}
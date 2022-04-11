using Suretom.Client.Common;
using System.Collections.Specialized;

namespace Suretom.Client.IService
{
    /// <summary>
    /// 登录服务，客户端调用服务接口需要先设置登录信息
    /// </summary>
    public interface ILoginService
    {
        /// <summary>
        /// 设置登录信息
        /// </summary>
        /// <param name="token"></param>
        void SetLoginInfo(string token);

        /// <summary>
        /// 设置登录信息
        /// </summary>
        /// <param name="token"></param>
        /// <param name="dataPlatformUserInfo"></param>
        void SetLoginInfo(string token, DataPlatformUserInfo dataPlatformUserInfo);

        /// <summary>
        /// 设置用户角色信息
        /// </summary>
        /// <param name="userCurrentRole"></param>
        void SetUserRole(string userCurrentRole);

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        string GetVerifyCode();

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="userCode"></param>
        /// <param name="userPass"></param>
        HttpResult Login(NameValueCollection paramValue);
    }
}
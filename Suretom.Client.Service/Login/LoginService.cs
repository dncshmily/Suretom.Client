using Newtonsoft.Json;
using Suretom.Client.Common;
using Suretom.Client.IService;
using System;
using System.Collections.Specialized;

namespace Suretom.Client.Service
{
    /// <summary>
    /// 登录服务，客户端调用服务接口需要先登录
    /// </summary>
    public class LoginService : ServiceBase, ILoginService
    {
        public LoginService()
        {
            Urls.Add("Code", "Home/VerifyCode");
            Urls.Add("Login", "Home/Login");
        }

        /// <summary>
        /// 设置登录信息
        /// </summary>
        /// <param name="token"></param>
        public void SetLoginInfo(string token)
        {
            GlobalContext.Token = token;
        }

        /// <summary>
        /// 设置登录信息
        /// </summary>
        /// <param name="token"></param>
        public void SetLoginInfo(string token, DataPlatformUserInfo dataPlatformUserInfo)
        {
            GlobalContext.Token = token;

            GlobalContext.DataPlatformUserInfo.UserCode = dataPlatformUserInfo.UserCode;
            GlobalContext.DataPlatformUserInfo.UserName = dataPlatformUserInfo.UserName;
            //GlobalContext.DataPlatformUserInfo.UserCurrentRole = dataPlatformUserInfo.UserCurrentRole;
            GlobalContext.DataPlatformUserInfo.UserCurrentRole = "兼职切题员";
        }

        /// <summary>
        /// 设置用户角色信息
        /// </summary>
        /// <param name="userCurrentRole"></param>
        public void SetUserRole(string userCurrentRole)
        {
            if (!string.IsNullOrEmpty(userCurrentRole))
            {
                GlobalContext.DataPlatformUserInfo.UserCurrentRole = userCurrentRole;
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="userCode"></param>
        /// <param name="userPass"></param>
        /// <returns></returns>
        public string GetVerifyCode()
        {
            var param = new { GlobalContext.Token, };

            var result = Get(JsonConvert.SerializeObject(param), Urls["Code"], this.GetType());

            if (!result.Success)
            {
                return "";
            }

            return result.Data.ToString();
        }

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="userCode"></param>
        /// <param name="userPass"></param>
        /// <returns></returns>
        public HttpResult Login(NameValueCollection paramValue)
        {
            if (string.IsNullOrEmpty(paramValue["userCode"]))
                throw new ArgumentException("userCode");
            if (string.IsNullOrEmpty(paramValue["userPwd"]))
                throw new ArgumentException("userPwd");
            if (string.IsNullOrEmpty(paramValue["verifycode"]))
                throw new ArgumentException("verifycode");

            var result = PostForm(Urls["Login"], paramValue);

            return result;
        }
    }
}
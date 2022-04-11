namespace Suretom.Client.Common
{
    /// <summary>
    /// 数据平台，用户登陆信息
    /// </summary>
    public class DataPlatformUserInfo
    {
        /// <summary>
        /// 手机号
        /// </summary>
        public string UserCode { get; set; } = string.Empty;

        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; } = string.Empty;

        /// <summary>
        /// 当前用户角色
        /// </summary>
        public string UserCurrentRole { get; set; } = string.Empty;

        /// <summary>
        /// 角色列表
        /// </summary>
        //public List<string> UserRoleList { get; set; } = new List<string>();
    }

    /// <summary>
    /// 用户信息
    /// </summary>
    public class UserInfo
    {
        /// <summary>
        /// 登录名
        /// </summary>
        public string LoginName { get; set; } = string.Empty;

        /// <summary>
        /// 密码
        /// </summary>
        public string PassWord { get; set; } = string.Empty;

        /// <summary>
        /// RU.User中对应的Guid
        /// </summary>
        public string Guid { get; set; } = string.Empty;

        /// <summary>
        /// RU.User中对应的用户名
        /// </summary>
        public string UserName { get; set; } = string.Empty;

        /// <summary>
        /// 登录用户类型
        /// </summary>
        public string UserType { get; set; } = string.Empty;

        /// <summary>
        ///验证码
        /// </summary>
        public string Verifycode { get; set; } = string.Empty;

        /// <summary>
        ///
        /// </summary>
        public UserInfo userInfo { get; set; } = new UserInfo();
    }
}
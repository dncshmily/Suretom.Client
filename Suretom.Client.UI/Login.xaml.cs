using Suretom.Client.Common;
using Suretom.Client.Entity;
using Suretom.Client.IService;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace Suretom.Client.UI
{
    /// <summary>
    /// Login.xaml 的交互逻辑
    /// </summary>
    public partial class Login : Window
    {
        private NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private NameValueCollection config = ConfigurationManager.AppSettings;
        private ILoginService loginService;
        private IUserService userService;

        private ValidCodeHelper.ValidCode validCode;

        public Login()
        {
            InitializeComponent();
            LoginLoaded();
        }

        /// <summary>
        ///
        /// </summary>
        public void LoginLoaded()
        {
            //1
            loginService = GlobalContext.Resolve<ILoginService>();

            //2
            userService = GlobalContext.Resolve<IUserService>();

            //参数一：产生几个字符的验证码图片
            //参数二：验证码的形式(数字、字母、数字字母混合都有)
            validCode = new ValidCodeHelper.ValidCode(5, CodeTypeEnum.Alphas);

            MageCodeSource();
        }

        public void MageCodeSource()
        {
            var code = loginService.GetVerifyCode();
            if (!string.IsNullOrEmpty(code))
            {
                var uri = validCode.CreateCheckCodeImage(code);
                this.imageCode.Source = BitmapFrame.Create(uri);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (config != null)
                {
                    txtName.Text = GetValue("login");
                    if (bool.TryParse(GetValue("checked"), out bool check) && check)
                    {
                        txtPassword.Password = decode(GetValue("password"));
                        chkName.IsChecked = check;
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Image_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 1)
            {
                MageCodeSource();
                e.Handled = true;
            }
        }

        /// <summary>
        /// 关闭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnClose_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var userCode = txtName.Text.Trim();
                var userPwd = txtPassword.Password.Trim();
                var verifycode = txtCode.Text.Trim();

                if (string.IsNullOrEmpty(userCode) || string.IsNullOrEmpty(txtPassword.Password.Trim()))
                {
                    MessageBox.Show("用户名密码输入完整");
                    return;
                }

                if (string.IsNullOrEmpty(txtCode.Text.Trim()))
                {
                    MessageBox.Show("请输入验证码");
                    return;
                }

                btnLogin.Content = "登录中...";

                var paramValue = new NameValueCollection() {
                        { "userCode",userCode},
                       { "userPwd",userPwd},
                       { "verifycode",verifycode}
                };

                var result = loginService.Login(paramValue);

                if (result.Success)
                {
                    //记录用户名密码
                    if (chkName.IsChecked.Value)
                    {
                        SetValue("checked", "true");
                        SetValue("login", userCode);
                        SetValue("password", encode(txtPassword.Password.Trim()));
                    }

                    loginService.SetLoginInfo(result.Data.ToString());

                    var userInfo = userService.GetUserInfo();

                    if (userInfo==null)
                    {
                        MessageBox.Show("获取用户信息失败");
                        return;
                    }

                    GlobalContext.UserInfo = new UserInfo
                    {
                        UserName = userCode,
                        PassWord = userPwd,
                        Verifycode = verifycode
                    };

                    var mainForm = new MainWindow();
                    mainForm.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                    mainForm.WindowState = WindowState.Maximized;
                    mainForm.Show();
                    this.Close();
                }
                else
                {
                    btnLogin.Content = "安 全 登 录";
                    MessageBox.Show(this, "登录失败");
                }
            }
            catch (Exception ex)
            {
                btnLogin.Content = "安 全 登 录";
                log.Error(ex);
            }
        }

        private bool CheckKey(string key) => config.AllKeys.Contains(key);

        private string GetValue(string key) => config.AllKeys.Contains(key) ? config[key]?.ToString() : "";

        private void SetValue(string key, string value = "")
        {
            if (config.AllKeys.Contains(key))
            {
                Configuration temp = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                temp.AppSettings.Settings[key].Value = value;
                temp.Save();
            }
        }

        private string encode(string str)
        {
            string htext = "";

            for (int i = 0; i < str.Length; i++)
            {
                htext = htext + (char)(str[i] + 10 - 1 * 2);
            }
            return htext;
        }

        private string decode(string str)
        {
            string dtext = "";

            for (int i = 0; i < str.Length; i++)
            {
                dtext = dtext + (char)(str[i] - 10 + 1 * 2);
            }
            return dtext;
        }
    }
}
using FirstFloor.ModernUI.Windows.Controls;
using Suretom.Client.Common;
using Suretom.Client.IService;
using System;
using System.Collections.Specialized;
using System.Windows;

namespace Suretom.Client.UI
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : ModernWindow
    {
        private static NLog.Logger ez_log = NLog.LogManager.GetCurrentClassLogger();

        private readonly ILoginService _loginService;

        private System.Timers.Timer _tokenTimer = new System.Timers.Timer()
        {
            Enabled = true,
            Interval = 60 * 60 * 1000, //60分钟
        };

        public MainWindow()
        {
            InitializeComponent();

            var scope = GlobalContext.LifetimeScope();
            _loginService = GlobalContext.Resolve<ILoginService>(scope);

            Title = $"{Title}.{System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString()}";

            _tokenTimer.Elapsed += _tokenTimer_Elapsed;
        }

        private void _tokenTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                var paramValue = new NameValueCollection() {
                        { "userCode",GlobalContext.UserInfo.UserName},
                        { "userPwd",GlobalContext.UserInfo.PassWord},
                        { "verifycode",GlobalContext.UserInfo.Verifycode}
                    };

                var result = _loginService.Login(paramValue);
                if (result.Success)
                {
                    _loginService.SetLoginInfo(result.Data["token"].ToString());

                    ez_log.Info("自动刷新Token成功");
                }
            }
            catch (Exception ex)
            {
                ez_log.Error(ex);
            }
        }

        private void ModernWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                if (ModernDialog.ShowMessage("是否关闭？", "提示", MessageBoxButton.OKCancel, this)
                    == MessageBoxResult.Cancel)
                {
                    e.Cancel = true;
                }
                else
                {
                    //执行清理操作
                    //todo.....
                }
            }
            catch (Exception ex)
            {
                ez_log.Error(ex);
            }
        }
    }
}
using Suretom.Client.Common;
using Suretom.Client.IService;
using System;
using System.Windows;

namespace Suretom.Client.UI
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        private static NLog.Logger ez_log = NLog.LogManager.GetCurrentClassLogger();
        private static System.Timers.Timer ez_refreshTokenTimer = new System.Timers.Timer();

        public App()
        {
            try
            {
                this.Startup += App_Startup;
                this.Exit += App_Exit;
            }
            catch (Exception ex)
            {
                ez_log.Error(ex);
            }
        }

        private void App_Startup(object sender, StartupEventArgs e)
        {
            Inital();
        }

        private void App_Exit(object sender, ExitEventArgs e)
        {
            try
            {
                GlobalContext.ContainerManager.Container.Dispose();
            }
            catch (Exception)
            {
            }
        }

        private void Inital()
        {
            GlobalContext.Initialize();
            LocalDirectoryConfig();
        }

        /// <summary>
        /// 本地目录配置
        /// </summary>
        private static void LocalDirectoryConfig()
        {
            //清理目录
            GlobalContext.LocalDirectoryConfig.ClearDirectory();
        }

        #region 定时器操作

        /// <summary>
        /// 定时器配置
        /// </summary>
        private void TimerConfig()
        {
            //Token的刷新时间有可能需要配置
            //var ctbConfig = GlobalContext.Resolve<Suretom.Client.Common.Configuration.SuretomConfig>();
            //var appSettingsConfig = GlobalContext.Resolve<Suretom.Client.Common.Configuration.AppSettingsConfig>();

            //5分钟变更一次Token
            ez_refreshTokenTimer.Interval = TimeSpan.FromMinutes(5d).TotalMilliseconds;
            ez_refreshTokenTimer.Elapsed += ez_refreshTokenTimer_Elapsed;
        }

        private void ez_refreshTokenTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                using (var scope = GlobalContext.LifetimeScope())
                {
                    var loginService = GlobalContext.Resolve<ILoginService>(scope);

                    //loginService.RefreshToken();
                    ez_log.Info("刷新Token成功");
                }
            }
            catch (Exception inEx)
            {
                ez_log.Error($"刷新Token失败，{inEx.Message}");
            }
        }

        #endregion 定时器操作
    }
}
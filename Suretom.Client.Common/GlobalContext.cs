using Autofac;
using Suretom.Client.Entity;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Suretom.Client.Common
{
    /// <summary>
    /// 全局上下文
    /// </summary>
    public class GlobalContext
    {
        #region Fields

        private static ContainerManager _containerManager;
        private static object ez_lockObj = new object();
        private static string ez_token = string.Empty;

        #endregion Fields

        /// <summary>
        /// 初始化
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void Initialize()
        {
            RegisterDependencies();
        }

        /// <summary>
        /// 注册依赖项
        /// </summary>
        private static void RegisterDependencies()
        {
            var builder = new ContainerBuilder();

            var config = ConfigurationManager.GetSection("SuretomConfig") as SuretomConfig;
            var appSettingsConfig = new AppSettingsConfig();

            //依赖项
            var typeFinder = new AppDomainTypeFinder();
            builder.RegisterInstance(config).As<SuretomConfig>().SingleInstance();
            builder.RegisterInstance(appSettingsConfig).As<AppSettingsConfig>().SingleInstance();
            builder.RegisterInstance(typeFinder).As<ITypeFinder>().SingleInstance();

            var localDirectoryConfig = new LocalDirectoryConfig();
            builder.RegisterInstance(localDirectoryConfig).As<LocalDirectoryConfig>().SingleInstance();

            //程序集中实现IDependencyRegistrar接口的注册类，批量注册
            var drTypes = typeFinder.FindClassesOfType<IDependencyRegistrar>();
            var drInstances = new List<IDependencyRegistrar>();
            foreach (var drType in drTypes)
                drInstances.Add((IDependencyRegistrar)Activator.CreateInstance(drType));
            //排序
            drInstances = drInstances.AsQueryable().OrderBy(t => t.Order).ToList();
            foreach (var dependencyRegistrar in drInstances)
                dependencyRegistrar.Register(builder, typeFinder, config);

            var container = builder.Build();
            _containerManager = new ContainerManager(container);
        }

        /// <summary>
        /// IOC容器管理类
        /// </summary>
        public static ContainerManager ContainerManager
        {
            get
            {
                if (_containerManager == null)
                {
                    throw new CtbException("未初始化AppContext");
                }

                return _containerManager;
            }
        }

        #region 类型解析简化调用

        /// <summary>
        /// WinForm窗体适用的作用区域
        /// </summary>
        /// <returns></returns>
        public static ILifetimeScope WinFormScope()
        {
            return ContainerManager.Scope(ScopeType.WinFormScope);
        }

        /// <summary>
        /// 获取一般的生命周期作用域
        /// </summary>
        /// <returns></returns>
        public static ILifetimeScope LifetimeScope()
        {
            return ContainerManager.Scope(ScopeType.LifetimeScope);
        }

        /// <summary>
        /// 获取生命周期作用域
        /// </summary>
        /// <param name="scopeType"></param>
        /// <param name="tagName"></param>
        /// <returns></returns>
        public static ILifetimeScope Scope(ScopeType scopeType = ScopeType.Contianer, string tagName = "")
        {
            return ContainerManager.Scope(scopeType, tagName);
        }

        /// <summary>
        /// 解析类型，不传scope时在根容器上解析
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T Resolve<T>(ILifetimeScope scope = null) where T : class
        {
            return ContainerManager.Resolve<T>("", scope);
        }

        /// <summary>
        /// 解析类型，不传scope时在根容器上解析
        /// </summary>
        /// <param name="type"></param>
        /// <param name="scope"></param>
        /// <returns></returns>
        public static object Resolve(Type type, ILifetimeScope scope = null)
        {
            return ContainerManager.Resolve(type, scope);
        }

        /// <summary>
        /// 解析类型，不传scope时在根容器上解析
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="scope"></param>
        /// <returns></returns>
        public static T[] ResolveAll<T>(ILifetimeScope scope = null)
        {
            return ContainerManager.ResolveAll<T>("", scope);
        }

        #endregion 类型解析简化调用

        /// <summary>
        /// 安全令牌
        /// </summary>
        public static string Token
        {
            get
            {
                return ez_token;
            }
            set
            {
                lock (ez_lockObj)
                {
                    ez_token = value;
                }
            }
        }

        /// <summary>
        /// 登录后的用户信息
        /// </summary>
        public static UserInfo UserInfo { get; set; } = new UserInfo();

        /// <summary>
        /// Stone登录后的用户信息
        /// </summary>
        public static DataPlatformUserInfo DataPlatformUserInfo = new DataPlatformUserInfo();

        /// <summary>
        /// 主页的本
        /// </summary>
        public static string MainPageUrl
        {
            get
            {
                var mainPageUrl = "";
                var ctbConfig = Resolve<SuretomConfig>();
                var ctbConfig2 = Resolve<AppSettingsConfig>();

                switch (ctbConfig.AppType)
                {
                    case AppType.DataPlatform:
                        switch (ctbConfig.EnvironmentName.ToLower())
                        {
                            //开发环境
                            case "dev":
                                mainPageUrl = "http://localhost/";
                                break;
                            //测试环境stg
                            case "stg":
                                mainPageUrl = "";
                                break;
                            //测试环境stg2
                            case "stg2":
                                mainPageUrl = "";
                                break;
                            //灰度环境
                            case "gray":
                                mainPageUrl = "";
                                break;
                            //生产环境
                            case "production":
                                mainPageUrl = "";
                                break;

                            default:
                                break;
                        }
                        break;

                    default:
                        break;
                }

                if (string.IsNullOrEmpty(mainPageUrl))
                    throw new CtbException("未设置主页的Url");

                return mainPageUrl;
            }
        }

        /// <summary>
        /// 客户端使用Api
        /// </summary>
        public static string ClientApiUrl
        {
            get
            {
                var clientApiUrl = "";

                var suretomConfig = Resolve<SuretomConfig>();
                switch (suretomConfig.AppType)
                {
                    //错题本站点
                    case AppType.DataPlatform:
                        switch (suretomConfig.EnvironmentName.ToLower())
                        {
                            //开发环境
                            case "dev":
                                clientApiUrl = "http://localhost:5000/";
                                break;
                            //测试环境
                            case "stg":
                                clientApiUrl = "";
                                break;
                            //灰度环境
                            case "gray":
                                clientApiUrl = "";
                                break;
                            //生产环境
                            case "production":
                                clientApiUrl = "http://47.103.155.194:8088/";
                                break;

                            default:
                                break;
                        }
                        break;

                    default:
                        break;
                }

                if (string.IsNullOrEmpty(clientApiUrl))
                    throw new CtbException("未设置接口地址");

                return clientApiUrl;
            }
        }

        /// <summary>
        /// 本地目录管理
        /// </summary>
        public static LocalDirectoryConfig LocalDirectoryConfig
        {
            get
            {
                return Resolve<LocalDirectoryConfig>();
            }
        }

        /// <summary>
        /// 是否显示与服务接口交互的Trace日志，因为包含Token，所以默认不显示
        /// </summary>
        public static bool ShowServiceTraceLog { get; set; } = true;
    }
}
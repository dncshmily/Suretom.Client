using Autofac;
using Suretom.Client.Common;
using Suretom.Client.IService;
using Suretom.Client.Service;

namespace Suretom.Client.UI.Ioc
{
    /// <summary>
    /// Ioc注册类型
    /// </summary>
    public class DependencyRegistrar : IDependencyRegistrar
    {
        public int Order => 1;

        public void Register(ContainerBuilder builder, ITypeFinder typeFinder, SuretomConfig config)
        {
            DataPlatformServcieRegister(builder, typeFinder, config);
        }

        /// <summary>
        /// 数据平台服务
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="typeFinder"></param>
        /// <param name="config"></param>
        private void DataPlatformServcieRegister(ContainerBuilder builder, ITypeFinder typeFinder, SuretomConfig config)
        {
            var httpChannel = new FormHttpChannel();
            builder.RegisterInstance(httpChannel).As<IHttpChannel>().SingleInstance();

            builder.RegisterType<LoginService>().As<ILoginService>().InstancePerLifetimeScope();
            builder.RegisterType<UserService>().As<IUserService>().InstancePerLifetimeScope();
            builder.RegisterType<StudentService>().As<IStudentService>().InstancePerLifetimeScope();
        }
    }
}
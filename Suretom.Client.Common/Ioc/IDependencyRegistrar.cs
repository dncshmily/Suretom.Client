using Autofac;

namespace Suretom.Client.Common
{
    /// <summary>
    /// 依赖注册接口，每个dll至少要实现一个注册类
    /// </summary>
    public interface IDependencyRegistrar
    {
        /// <summary>
        /// 注册服务和接口
        /// </summary>
        /// <param name="builder">IoC容器生成器</param>
        /// <param name="typeFinder">类型查找器</param>
        /// <param name="config">配置项</param>
        void Register(ContainerBuilder builder, ITypeFinder typeFinder, SuretomConfig config);

        /// <summary>
        ///注册时的顺序
        /// </summary>
        int Order { get; }
    }
}
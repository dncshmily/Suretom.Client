using Autofac;
using Suretom.Client.Entity;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Suretom.Client.Common
{
    /// <summary>
    /// IOC容器管理类
    /// </summary>
    public class ContainerManager
    {
        private readonly IContainer _container;

        /// <summary>
        /// IOC容器管理类
        /// </summary>
        /// <param name="container"></param>
        public ContainerManager(IContainer container)
        {
            this._container = container;
        }

        /// <summary>
        /// 根容器
        /// </summary>
        public virtual IContainer Container
        {
            get
            {
                return _container;
            }
        }

        /// <summary>
        /// 解析
        /// </summary>
        /// <typeparam name="T">服务类型</typeparam>
        /// <param name="key">服务关键字</param>
        /// <param name="scope">生命周期作业域</param>
        /// <returns></returns>
        public virtual T Resolve<T>(string key = "", ILifetimeScope scope = null) where T : class
        {
            if (scope == null)
            {
                scope = Scope();
            }
            if (string.IsNullOrEmpty(key))
            {
                return scope.Resolve<T>();
            }
            return scope.ResolveKeyed<T>(key);
        }

        /// <summary>
        /// 解析
        /// </summary>
        /// <param name="type"></param>
        /// <param name="scope"></param>
        /// <returns></returns>
        public virtual object Resolve(Type type, ILifetimeScope scope = null)
        {
            if (scope == null)
            {
                scope = Scope();
            }
            return scope.Resolve(type);
        }

        /// <summary>
        /// 解析所有
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="scope"></param>
        /// <returns></returns>
        public virtual T[] ResolveAll<T>(string key = "", ILifetimeScope scope = null)
        {
            if (scope == null)
            {
                //no scope specified
                scope = Scope();
            }
            if (string.IsNullOrEmpty(key))
            {
                return scope.Resolve<IEnumerable<T>>().ToArray();
            }
            return scope.ResolveKeyed<IEnumerable<T>>(key).ToArray();
        }

        public virtual T ResolveUnregistered<T>(ILifetimeScope scope = null) where T : class
        {
            return ResolveUnregistered(typeof(T), scope) as T;
        }

        public virtual object ResolveUnregistered(Type type, ILifetimeScope scope = null)
        {
            if (scope == null)
            {
                scope = Scope();
            }
            var constructors = type.GetConstructors();
            foreach (var constructor in constructors)
            {
                try
                {
                    var parameters = constructor.GetParameters();
                    var parameterInstances = new List<object>();
                    foreach (var parameter in parameters)
                    {
                        var service = Resolve(parameter.ParameterType, scope);
                        if (service == null) throw new CtbException("Unknown dependency");
                        parameterInstances.Add(service);
                    }
                    return Activator.CreateInstance(type, parameterInstances.ToArray());
                }
                catch (Exception)
                {
                }
            }
            throw new CtbException("No constructor  was found that had all the dependencies satisfied.");
        }

        public virtual bool TryResolve(Type serviceType, ILifetimeScope scope, out object instance)
        {
            if (scope == null)
            {
                //no scope specified
                scope = Scope();
            }
            return scope.TryResolve(serviceType, out instance);
        }

        public virtual bool IsRegistered(Type serviceType, ILifetimeScope scope = null)
        {
            if (scope == null)
            {
                //no scope specified
                scope = Scope();
            }
            return scope.IsRegistered(serviceType);
        }

        public virtual object ResolveOptional(Type serviceType, ILifetimeScope scope = null)
        {
            if (scope == null)
            {
                //no scope specified
                scope = Scope();
            }
            return scope.ResolveOptional(serviceType);
        }

        /// <summary>
        /// 获取生命周期作用域
        /// </summary>
        /// <param name="scopeType"></param>
        /// <param name="tagName"></param>
        /// <returns></returns>
        public virtual ILifetimeScope Scope(ScopeType scopeType = ScopeType.Contianer, string tagName = "")
        {
            ILifetimeScope result = _container;
            switch (scopeType)
            {
                case ScopeType.Contianer:
                    result = _container;
                    break;

                case ScopeType.LifetimeScope:
                    result = _container.BeginLifetimeScope();
                    break;

                case ScopeType.TagScope:
                    if (string.IsNullOrEmpty(tagName))
                        result = _container.BeginLifetimeScope("Ctb");
                    else
                        result = _container.BeginLifetimeScope(tagName);
                    break;

                case ScopeType.WinFormScope:
                    result = _container.BeginLifetimeScope("WinForm");
                    break;

                default:
                    result = _container;
                    break;
            }
            return result;
        }
    }
}
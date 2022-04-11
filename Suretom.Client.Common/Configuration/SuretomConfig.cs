using System;
using System.Configuration;
using System.Xml;

namespace Suretom.Client.Common
{
    /// <summary>
    /// 客户端配置类
    /// </summary>
    public class SuretomConfig : IConfigurationSectionHandler
    {
        public object Create(object parent, object configContext, XmlNode section)
        {
            var config = new SuretomConfig();

            var ctbVersionNode = section.SelectSingleNode("Version");
            config.AppType = (AppType)Enum.Parse(typeof(AppType), GetString(ctbVersionNode, "AppType"));
            config.EnvironmentName = GetString(ctbVersionNode, "EnvironmentName");

            return config;
        }

        private string GetString(XmlNode node, string attrName)
        {
            return SetByXElement<string>(node, attrName, Convert.ToString);
        }

        private bool GetBool(XmlNode node, string attrName)
        {
            return SetByXElement<bool>(node, attrName, Convert.ToBoolean);
        }

        private T SetByXElement<T>(XmlNode node, string attrName, Func<string, T> converter)
        {
            if (node == null || node.Attributes == null) return default(T);
            var attr = node.Attributes[attrName];
            if (attr == null) return default(T);
            var attrVal = attr.Value;
            return converter(attrVal);
        }

        #region 配置属性

        /// <summary>
        /// 应用类型
        /// </summary>
        public AppType AppType { get; private set; }

        /// <summary>
        /// 环境名称：dev,test,gray,production
        /// </summary>
        public string EnvironmentName { get; private set; }

        #endregion 配置属性
    }
}
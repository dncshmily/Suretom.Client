using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace Suretom.Client.Common
{
    /// <summary>
    /// 枚举处理类
    /// </summary>
    public static class EnumHelper
    {
        /// <summary>
        /// 将枚举类型转换为list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static List<EnumModel> EnumToList<T>()
        {
            List<EnumModel> list = new List<EnumModel>();

            foreach (var e in Enum.GetValues(typeof(T)))
            {
                EnumModel m = new EnumModel();
                object[] objArr = e.GetType().GetField(e.ToString()).GetCustomAttributes(typeof(DescriptionAttribute), true);
                if (objArr != null && objArr.Length > 0)
                {
                    DescriptionAttribute da = objArr[0] as DescriptionAttribute;
                    m.Desction = da.Description;
                }
                m.EnumValue = Convert.ToInt32(e);
                m.EnumName = e.ToString();
                list.Add(m);
            }
            return list;
        }

        /// <summary>
        /// 获取枚举类型描述内容
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public static string GetEnumDesc(Object e)
        {
            try
            {
                FieldInfo EnumInfo = e.GetType().GetField(e.ToString());
                DescriptionAttribute[] EnumAttributes = (DescriptionAttribute[])EnumInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);
                if (EnumAttributes.Length > 0)
                {
                    return EnumAttributes[0].Description;
                }
            }
            catch (Exception)
            {
                return "其他";
            }
            return e.ToString();
        }

        /// <summary>
        /// 根据枚举描述获取内容
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="description"></param>
        /// <returns></returns>
        public static T GetEnumValue<T>(string description)
        {
            FieldInfo currentField = null;
            Type _type = typeof(T);
            foreach (FieldInfo field in _type.GetFields())
            {
                System.ComponentModel.DescriptionAttribute[] _curDesc = (System.ComponentModel.DescriptionAttribute[])field.GetCustomAttributes(typeof(System.ComponentModel.DescriptionAttribute), false);
                if (_curDesc != null && _curDesc.Length > 0)
                {
                    if (_curDesc[0].Description == description)
                        currentField = field;
                }
                else
                {
                    if (field.Name == description)
                        currentField = field;
                }
            }
            if (currentField != null)
            {
                return (T)currentField.GetValue(null);
            }
            else
            {
                return (T)_type.GetFields()[1].GetValue(null);
            }
        }

        public class EnumModel
        {
            /// <summary>
            /// 枚举的描述
            /// </summary>
            public string Desction { set; get; }

            /// <summary>
            /// 枚举名称
            /// </summary>
            public string EnumName { set; get; }

            /// <summary>
            /// 枚举对象的值
            /// </summary>
            public int EnumValue { set; get; }
        }
    }
}
﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Suretom.Client.Common
{
    public static partial class Extention
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        static Extention()
        {
            JsonSerializerSettings setting = new JsonSerializerSettings();
            JsonConvert.DefaultSettings = new Func<JsonSerializerSettings>(() =>
            {
                //日期类型默认格式化处理
                setting.DateFormatHandling = DateFormatHandling.MicrosoftDateFormat;
                setting.DateFormatString = "yyyy-MM-dd HH:mm:ss";
                return setting;
            });
        }

        /// <summary>
        /// 将一个object对象序列化，返回一个byte[]
        /// </summary>
        /// <param name="obj">能序列化的对象</param>
        /// <returns></returns>
        public static byte[] ToBytes(this object obj)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                IFormatter formatter = new BinaryFormatter();
                formatter.Serialize(ms, obj);
                return ms.GetBuffer();
            }
        }

        /// <summary>
        /// 判断是否为Null或者空
        /// </summary>
        /// <param name="obj">对象</param>
        /// <returns></returns>
        public static bool IsNullOrEmpty(this object obj)
        {
            if (obj == null)
                return true;
            else
            {
                string objStr = obj.ToString();
                return string.IsNullOrEmpty(objStr);
            }
        }

        /// <summary>
        /// 将对象序列化成Json字符串
        /// </summary>
        /// <param name="obj">需要序列化的对象</param>
        /// <returns></returns>
        public static string ToJson(this object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

        /// <summary>
        /// 实体类转json数据，速度快
        /// </summary>
        /// <param name="t">实体类</param>
        /// <returns></returns>
        public static string EntityToJson(this object t)
        {
            if (t == null)
                return null;
            string jsonStr = "";
            jsonStr += "{";
            PropertyInfo[] infos = t.GetType().GetProperties();
            for (int i = 0; i < infos.Length; i++)
            {
                jsonStr = jsonStr + "\"" + infos[i].Name + "\":\"" + infos[i].GetValue(t).ToString() + "\"";
                if (i != infos.Length - 1)
                    jsonStr += ",";
            }
            jsonStr += "}";
            return jsonStr;
        }

        /// <summary>
        /// 将对象序列化为XML字符串
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="obj">对象</param>
        /// <returns></returns>
        public static string ToXmlStr<T>(this T obj)
        {
            var jsonStr = obj.ToJson();
            var xmlDoc = JsonConvert.DeserializeXmlNode(jsonStr);
            string xmlDocStr = xmlDoc.InnerXml;

            return xmlDocStr;
        }

        /// <summary>
        /// 将对象序列化为XML字符串
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="obj">对象</param>
        /// <param name="rootNodeName">根节点名(建议设为xml)</param>
        /// <returns></returns>
        public static string ToXmlStr<T>(this T obj, string rootNodeName)
        {
            var jsonStr = obj.ToJson();
            var xmlDoc = JsonConvert.DeserializeXmlNode(jsonStr, rootNodeName);
            string xmlDocStr = xmlDoc.InnerXml;

            return xmlDocStr;
        }

        /// <summary>
        /// 获取某属性值
        /// </summary>
        /// <param name="obj">对象</param>
        /// <param name="propertyName">属性名</param>
        /// <returns></returns>
        public static object GetPropertyValue(this object obj, string propertyName)
        {
            return obj.GetType().GetProperty(propertyName, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static).GetValue(obj);
        }

        /// <summary>
        /// 设置某属性的值
        /// </summary>
        /// <param name="obj">对象</param>
        /// <param name="propertyName">属性名</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static void SetPropertyValue(this object obj, string propertyName, object value)
        {
            obj.GetType().GetProperty(propertyName, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static).SetValue(obj, value);
        }

        /// <summary>
        /// 改变类型
        /// </summary>
        /// <param name="obj">原对象</param>
        /// <param name="targetType">目标类型</param>
        /// <returns></returns>
        public static object ChangeType(this object obj, Type targetType)
        {
            object resObj = null;
            if (targetType.IsGenericType && targetType.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
            {
                NullableConverter newNullableConverter = new NullableConverter(targetType);
                resObj = newNullableConverter.ConvertFrom(obj);
            }
            else
            {
                resObj = Convert.ChangeType(obj, targetType);
            }

            return resObj;
        }

        /// <summary>
        /// 重新赋值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="func"></param>
        public static void AgainValue<T>(this IEnumerable<T> list, Action<T, int> func)
        {
            var index = 0;
            foreach (var item in list)
            {
                func(item, index);
                index++;
            }
        }
    }
}
using System;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Text.RegularExpressions;

namespace Suretom.Client.Common
{
    /// <summary>
    /// json序列化基类
    /// </summary>
    public static class JsonHelper
    {
        /// <summary>
        /// 复杂对象序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string GetJsonByObject<T>(this T obj)
        {
            if (null == obj)
            {
                return null;
            }

            //实例化DataContractJsonSerializer对象，需要待序列化的对象类型
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T));

            //实例化一个内存流，用于存放序列化后的数据
            using (MemoryStream stream = new MemoryStream())
            {
                //使用WriteObject序列化对象
                serializer.WriteObject(stream, obj);

                //写入内存流中
                byte[] dataBytes = new byte[stream.Length];
                stream.Position = 0;
                stream.Read(dataBytes, 0, (int)stream.Length);

                //通过UTF8格式转换为字符串
                return Encoding.UTF8.GetString(dataBytes);
            }
        }

        /// <summary>
        /// 复杂对象序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="jsonString"></param>
        /// <returns></returns>
        public static T GetObjectByJson<T>(this string jsonString)
        {
            try
            {
                //实例化DataContractJsonSerializer对象，需要待序列化的对象类型
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T));

                //把Json传入内存流中保存
                using (MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(jsonString)))
                {
                    // 使用ReadObject方法反序列化成对象
                    return (T)serializer.ReadObject(stream);
                }
            }
            catch (Exception)
            {
                return default(T);
            }
        }

        /// <summary>
        /// JSON序列化
        /// </summary>
        public static string JsonSerializer<T>(T t)
        {
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(T));
            MemoryStream ms = new MemoryStream();
            ser.WriteObject(ms, t);
            string jsonString = Encoding.UTF8.GetString(ms.ToArray());
            ms.Close();
            //替换Json的Date字符串
            string p = @"\\/Date\((\d+)\+\d+\)\\/";
            MatchEvaluator matchEvaluator = new MatchEvaluator(ConvertJsonDateToDateString);
            Regex reg = new Regex(p);
            jsonString = reg.Replace(jsonString, matchEvaluator);
            return jsonString;
        }

        /// <summary>
        /// JSON序列化
        /// </summary>
        public static string JsonSerializerNotProcessDate<T>(T t)
        {
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(T));
            MemoryStream ms = new MemoryStream();
            ser.WriteObject(ms, t);
            string jsonString = Encoding.UTF8.GetString(ms.ToArray());
            ms.Close();
            return jsonString;
        }

        /// <summary>
        /// JSON反序列化
        /// </summary>
        public static T JsonDeserialize<T>(string jsonString)
        {
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(T));
            MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(jsonString));
            T obj = (T)ser.ReadObject(ms);
            return obj;
        }

        /// <summary>
        /// 将Json序列化的时间由/Date(1294499956278+0800)转为字符串
        /// </summary>
        private static string ConvertJsonDateToDateString(Match m)
        {
            string result = string.Empty;
            DateTime dt = new DateTime(1970, 1, 1);
            dt = dt.AddMilliseconds(long.Parse(m.Groups[1].Value));
            dt = dt.ToLocalTime();
            result = dt.ToString("yyyy-MM-dd HH:mm:ss");
            return result;
        }

        // Summary:
        //     反序列化一个JSON对象到一个T类型的对象
        //
        // Parameters:
        //   value:
        //     要反序列化的JSON对象
        //
        // Type parameters:
        //   T:
        //     泛型T
        //
        // Returns:
        //     从JSON对象反序列化的T类型对象
        public static T DeserializeObject<T>(string value)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(value);
        }

        //
        // Summary:
        //     反序列化一个JSON对象到指定对象
        //
        // Parameters:
        //   value:
        //     要反序列化的JSON对象
        //
        // Returns:
        //     从JSON对象反序列化的对象
        public static object DeserializeObject(string value)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject(value);
        }

        //
        // Summary:
        //     反序列化一个JSON对象到指定类型的CLR对象
        //
        // Parameters:
        //   value:
        //     要反序列化的JSON对象
        //
        //   type:
        //     CLR类型
        //
        // Returns:
        //     从JSON对象反序列化的CLR类型对象
        public static object DeserializeObject(string value, Type type)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject(value, type);
        }

        //
        // Summary:
        //     将指定的对象序列化到JSON对象
        //
        // Parameters:
        //   value:
        //     要序列化的对象
        //
        // Returns:
        //     一个表示object的JSON字符串
        public static string SerializeObject(object value)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(value);
        }

        public static string JsonSerializerByNewtonsoft<T>(T t)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(t);
        }

        public static T JsonDeserializeByNewtonsoft<T>(string jsonString)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(jsonString);
        }
    }
}
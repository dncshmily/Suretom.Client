using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;

namespace Suretom.Client.Common
{
    /// <summary>
    /// 基础帮助类
    /// </summary>
    public static class UtilityHelper
    {
        #region 系统级别公共类

        /// <summary>
        /// 转成中文数字
        /// </summary>
        /// <param name="inputNum"></param>
        /// <returns></returns>
        public static string ConvertToChinese(int inputNum)
        {
            string[] strArr = { "零", "一", "二", "三", "四", "五", "六", "七", "八", "九", "十",
                "十二", "十三", "十四", "十五", "十六", "十七", "十八", "十九", "二十" };

            return strArr[inputNum];
        }

        /// <summary>
        /// 对象转换为字典
        /// </summary>
        /// <param name="obj">待转化的对象</param>
        /// <param name="isIgnoreNull">是否忽略NULL 这里我不需要转化NULL的值，正常使用可以不传参数 默认全转换</param>
        /// <returns></returns>
        public static Dictionary<string, object> ObjectToMap(object obj, bool isIgnoreNull = false)
        {
            Dictionary<string, object> map = new Dictionary<string, object>();

            Type t = obj.GetType(); // 获取对象对应的类， 对应的类型

            PropertyInfo[] pi = t.GetProperties(BindingFlags.Public | BindingFlags.Instance); // 获取当前type公共属性

            foreach (PropertyInfo p in pi)
            {
                MethodInfo m = p.GetGetMethod();

                if (m != null && m.IsPublic)
                {
                    // 进行判NULL处理
                    if (m.Invoke(obj, new object[] { }) != null || !isIgnoreNull)
                    {
                        map.Add(p.Name, m.Invoke(obj, new object[] { })); // 向字典添加元素
                    }
                }
            }

            return map;
        }

        /// <summary>
        /// 字典类型转化为对象
        /// </summary>
        /// <param name="dic"></param>
        /// <returns></returns>
        public static T DicToObject<T>(Dictionary<string, object> dic) where T : new()
        {
            var md = new T();
            CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
            TextInfo textInfo = cultureInfo.TextInfo;
            foreach (var d in dic)
            {
                var filed = textInfo.ToTitleCase(d.Key);
                try
                {
                    var value = d.Value;
                    md.GetType().GetProperty(filed).SetValue(md, value);
                }
                catch
                {
                }
            }

            return md;
        }

        /// <summary>
        /// 两个时间差
        /// </summary>
        /// <param name="beginDateTime"></param>
        /// <param name="endDateTime"></param>
        /// <returns></returns>
        public static double DateDiff(DateTime beginDateTime, DateTime endDateTime)
        {
            TimeSpan ts1 = new TimeSpan(beginDateTime.Ticks);
            TimeSpan ts2 = new TimeSpan(endDateTime.Ticks);
            TimeSpan ts = ts1.Subtract(ts2).Duration();
            return ts.TotalSeconds;
        }

        /// <summary>
        /// 转换int16函数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static Int16 ConvertToInt16<T>(T source)
        {
            Int16 result;
            if (source == null)
            {
                return default(Int16);
            }
            else if (Int16.TryParse(source.ToString(), out result))
            {
                return result;
            }
            else
            {
                return default(Int16);
            }
        }

        /// <summary>
        /// 转换Int32函数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static Int32 ConvertToInt<T>(T source)
        {
            Int32 result;
            if (source == null)
            {
                return default(Int32);
            }
            else if (Int32.TryParse(source.ToString(), out result))
            {
                return result;
            }
            else
            {
                return default(Int32);
            }
        }

        /// <summary>
        /// 转换int64函数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static Int64 ConvertToInt64<T>(T source)
        {
            Int64 result;
            if (source == null)
            {
                return default(Int64);
            }
            else if (Int64.TryParse(source.ToString(), out result))
            {
                return result;
            }
            else
            {
                return default(Int64);
            }
        }

        /// <summary>
        /// 时间戳转为时间
        /// </summary>
        /// <param name="timeStamp"></param>
        /// <returns></returns>
        public static DateTime StampToDateTime(string timeStamp)
        {
            DateTime dateTimeStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            long lTime;
            if (timeStamp.Length == 10)
            {
                lTime = long.Parse(timeStamp + "0000000");
            }
            else
            {
                lTime = long.Parse(timeStamp + "0000");
            }

            TimeSpan toNow = new TimeSpan(lTime);

            return dateTimeStart.Add(toNow);
        }

        /// <summary>
        /// 时间转为时间戳
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static long DateTimeToStamp(System.DateTime time)
        {
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
            return (long)(time - startTime).TotalSeconds;
        }

        private static long num = 0;//流水号
        private static object lockObj = new object();//锁

        /// <summary>
        /// 生成自增长码
        /// </summary>
        /// <returns></returns>
        private static long GenerateUniqueNumber()
        {
            lock (lockObj)//加锁
            {
                num = num + 1;
                num = (num == 100000 ? 1 : num); //如果大于10W则从零开始，由于一台服务器一秒内不太可能有10W并发，所以yymmddhhmmss+num是唯一号。yymmddhhmmss+num+SystemNo针对多台服务器也是唯一号。
            }
            return num;
        }

        /// <summary>
        /// 获取唯一码
        /// </summary>
        /// <param name="SystemNo">系统号</param>
        /// <returns>唯一码</returns>
        public static string GetUniqueNumber(int SystemNo, string time)
        {
            if (SystemNo > 99 || SystemNo < 1)
            {
                throw new Exception("系统号有误");
            }
            lock (lockObj)// 要使静态变量多并发下同步，需要两次加锁。
            {
                return time + GenerateUniqueNumber().ToString().PadLeft(5, '0') + SystemNo.ToString().PadLeft(2, '0');//21位
            }
        }

        /// <summary>
        /// MD5加密
        /// </summary>
        /// <param name="md5_str"></param>
        /// <returns></returns>
        public static string MD5_Encrypt(string md5_str)
        {
            System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5CryptoServiceProvider.Create();
            byte[] passwordAndSaltBytes = System.Text.Encoding.UTF8.GetBytes(md5_str);
            byte[] hashBytes = md5.ComputeHash(passwordAndSaltBytes);

            System.Text.StringBuilder hashString = new StringBuilder();
            foreach (var item in hashBytes)
            {
                hashString.Append(item.ToString("x").PadLeft(2, '0'));
            }
            return hashString.ToString().ToUpper();
        }

        #endregion 系统级别公共类

        #region 文件后缀类型

        public static string GetContentType(string fileExtension)
        {
            switch (fileExtension.ToLower())
            {
                case ".zip":
                case ".doc":
                case ".docx":
                    return "application/octet-stream";

                case ".jpg":
                    return "image/jpeg";

                case ".png":
                    return "image/png";

                default:
                    return "text/plain";
            }
        }

        #endregion 文件后缀类型

        /// <summary>
        /// 得到私有字段的值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance"></param>
        /// <param name="fieldname"></param>
        /// <returns></returns>
        public static T GetPrivateField<T>(object instance, string fieldname)
        {
            BindingFlags flag = BindingFlags.Instance | BindingFlags.NonPublic;
            Type type = instance.GetType();
            FieldInfo field = type.GetField(fieldname, flag);
            return (T)field.GetValue(instance);
        }

        /// <summary>
        /// 得到私有属性的值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance"></param>
        /// <param name="propertyname"></param>
        /// <returns></returns>
        public static T GetPrivateProperty<T>(object instance, string propertyname)
        {
            BindingFlags flag = BindingFlags.Instance | BindingFlags.NonPublic;
            Type type = instance.GetType();
            PropertyInfo field = type.GetProperty(propertyname, flag);
            return (T)field.GetValue(instance, null);
        }

        #region 生成缩略图

        //// <summary>
        /// 生成缩略图
        /// </summary>
        /// <param name="originalImagePath">源图路径（物理路径）</param>
        /// <param name="thumbnailPath">缩略图路径（物理路径）</param>
        /// <param name="width">缩略图宽度</param>
        /// <param name="height">缩略图高度</param>
        /// <param name="mode">生成缩略图的方式</param>
        public static void MakeThumbnail(string originalImagePath, string thumbnailPath, int width, int height, string mode)
        {
            var pathRoot = Path.GetDirectoryName(thumbnailPath);

            if (!Directory.Exists(pathRoot))
                Directory.CreateDirectory(pathRoot);

            Image originalImage = Image.FromFile(originalImagePath);

            int towidth = width;
            int toheight = height;

            int x = 0;
            int y = 0;
            int ow = originalImage.Width;
            int oh = originalImage.Height;

            switch (mode)
            {
                case "HW"://指定高宽缩放（可能变形）
                    break;

                case "W"://指定宽，高按比例
                    toheight = originalImage.Height * width / originalImage.Width;
                    break;

                case "H"://指定高，宽按比例
                    towidth = originalImage.Width * height / originalImage.Height;
                    break;

                case "Cut"://指定高宽裁减（不变形）
                    if ((double)originalImage.Width / (double)originalImage.Height > (double)towidth / (double)toheight)
                    {
                        oh = originalImage.Height;
                        ow = originalImage.Height * towidth / toheight;
                        y = 0;
                        x = (originalImage.Width - ow) / 2;
                    }
                    else
                    {
                        ow = originalImage.Width;
                        oh = originalImage.Width * height / towidth;
                        x = 0;
                        y = (originalImage.Height - oh) / 2;
                    }
                    break;

                default:
                    break;
            }

            //新建一个bmp图片
            Image bitmap = new System.Drawing.Bitmap(towidth, toheight);

            //新建一个画板
            Graphics g = System.Drawing.Graphics.FromImage(bitmap);

            //设置高质量插值法
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;

            //设置高质量,低速度呈现平滑程度
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

            //清空画布并以透明背景色填充
            g.Clear(Color.Transparent);

            //在指定位置并且按指定大小绘制原图片的指定部分
            g.DrawImage(originalImage, new Rectangle(0, 0, towidth, toheight),
                new Rectangle(x, y, ow, oh),
                GraphicsUnit.Pixel);

            try
            {
                //以jpg格式保存缩略图
                bitmap.Save(thumbnailPath);
            }
            catch (System.Exception e)
            {
                throw e;
            }
            finally
            {
                originalImage.Dispose();
                bitmap.Dispose();
                g.Dispose();
            }
        }

        #endregion 生成缩略图

        #region 文件下载

        /// <summary>
        /// 文件下载
        /// </summary>
        /// <param name="fileUrl"></param>
        /// <param name="saveName"></param>
        /// <returns></returns>
        public static void DownloadFile(string fileUrl, string saveName)
        {
            var pathRoot = Path.GetDirectoryName(saveName);

            try
            {
                if (!Directory.Exists(pathRoot))
                {
                    Directory.CreateDirectory(pathRoot);
                }

                using (var client = new System.Net.WebClient())
                {
                    client.Proxy = null;
                    client.DownloadFile(fileUrl, saveName);
                    client.Dispose();
                }
            }
            catch
            {
            }
        }

        #endregion 文件下载
    }
}
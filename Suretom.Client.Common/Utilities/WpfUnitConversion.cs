using System;
using System.Drawing;

namespace Suretom.Client.Common
{
    /// <summary>
    /// 厘米和像素转换类
    /// </summary>
    public class WpfUnitConversion
    {
        private static float _widthDpi;
        private static float _heightDpi;

        static WpfUnitConversion()
        {
            var tuple = GetDpi();
            _widthDpi = tuple.Item1;
            _heightDpi = tuple.Item2;
        }

        public static float WidthDpi
        {
            get { return _widthDpi; }
        }

        public static float HeightDpi
        {
            get { return _heightDpi; }
        }

        /// <summary>
        /// 像素值转为厘米
        /// </summary>
        /// <param name="pixel"></param>
        /// <param name="isWidthDpi"></param>
        /// <returns></returns>
        public static double PixToCm(double pixel, bool isWidthDpi = true)
        {
            if (isWidthDpi)
                return (2.54 / _widthDpi) * pixel;
            else
                return (2.54 / _heightDpi) * pixel;
        }

        /// <summary>
        /// 厘米转为像素值
        /// </summary>
        /// <param name="cm"></param>
        /// <param name="isWidthDpi"></param>
        /// <returns></returns>
        public static double CmToPix(double cm, bool isWidthDpi = true)
        {
            if (isWidthDpi)
                return (_widthDpi / 2.54) * cm;
            else
                return (_heightDpi / 2.54) * cm;
        }

        /// <summary>
        /// 像素值转为毫米
        /// </summary>
        /// <param name="pixel"></param>
        /// <param name="isWidthDpi"></param>
        /// <returns></returns>
        public static double PixToMm(double pixel, bool isWidthDpi = true)
        {
            return PixToCm(pixel, isWidthDpi) * 10;
        }

        /// <summary>
        /// 毫米转为像素值
        /// </summary>
        /// <param name="mm"></param>
        /// <param name="isWidthDpi"></param>
        /// <returns></returns>
        public static double MmToPix(double mm, bool isWidthDpi = true)
        {
            return CmToPix(mm / 10, isWidthDpi);
        }

        /// <summary>
        /// Pt转为像素值
        /// </summary>
        /// <param name="mm"></param>
        /// <param name="isWidthDpi"></param>
        /// <returns></returns>
        public static double PtToPix(double pt, bool isWidthDpi = true)
        {
            if (isWidthDpi)
                return (_widthDpi / 72) * pt;
            else
                return (_heightDpi / 72) * pt;
        }

        /// <summary>
        /// 获取系统的dpi
        /// </summary>
        /// <returns></returns>
        private static Tuple<float, float> GetDpi()
        {
            using (Graphics graphics = Graphics.FromHwnd(IntPtr.Zero))
            {
                float dpiX = graphics.DpiX;
                float dpiY = graphics.DpiY;

                return new Tuple<float, float>(dpiX, dpiY);
            }
        }
    }
}
using Suretom.Client.Entity;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace Suretom.Client.UI.Others
{
    /// <summary>
    /// 转换器
    /// </summary>
    public class BatchImportDealResultConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool)value)
            {
                return "成功";
            }
            else
                return "失败";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return false;
            if (value.ToString() == "成功")
                return true;
            else
                return false;
        }
    }

    public class Bool2VisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool)value)
            {
                return Visibility.Visible;
            }
            else
                return Visibility.Hidden;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return false;
            return (Visibility)value == Visibility.Visible;
        }
    }

    /// <summary>
    ///
    /// </summary>
    public class BatchImportProcessInfoTypeConverter : IValueConverter
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var infoType = (BatchImportProcessInfoType)value;
            switch (infoType)
            {
                case BatchImportProcessInfoType.信息:
                    return Brushes.Black;

                case BatchImportProcessInfoType.警告:
                    return Brushes.Green;

                case BatchImportProcessInfoType.错误:
                    return Brushes.Red;

                default:
                    return Brushes.Black;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
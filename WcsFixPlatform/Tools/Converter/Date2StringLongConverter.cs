﻿using System;
using System.Globalization;
using System.Windows.Data;

namespace wcs.Tools.Converter
{
    public class Date2StringLongConverter : IValueConverter
    {
        private readonly string DefaultFormat = "MM-dd HH:mm:ss";
        //createtime?.ToString(, CultureInfo.InvariantCulture)
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string date = string.Empty;
            if (value is DateTime dateValue)
            {
                if (parameter is string dateformat)
                {
                    date = dateValue.ToString(dateformat, culture);
                }
                else
                {
                    date = dateValue.ToString(DefaultFormat, culture);
                }
            }
            return date;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

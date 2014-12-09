using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ColorPicker {
	public sealed class LinearConverter : Windows.UI.Xaml.Data.IValueConverter {
		public object Convert(object value, Type targetType, object parameter, string language) {
			if (parameter != null)
				return Double.Parse((string)parameter) * (double)value;
			else
				return value;
		}

		public object ConvertBack(object value, Type targetType, object parameter, string language) {
			if (parameter == null)
				return value;
			else {
				if (Double.Parse((string)parameter) == 0)
					return value;
				else
					return (double)value / Double.Parse((string)parameter);
			}
		}
	}
}

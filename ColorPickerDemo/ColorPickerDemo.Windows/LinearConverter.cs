using System;

namespace ColorPickerDemo {
	public sealed class LinearConverter : Windows.UI.Xaml.Data.IValueConverter {
		public object Convert(object value, Type targetType, object parameter, string language) {
			if (parameter == null)
				parameter = (double)0.8;
			return Math.Abs((double)value * (double)parameter);
		}

		public object ConvertBack(object value, Type targetType, object parameter, string language) {
			if (parameter == null)
				parameter = (double)0.8;
			return Math.Abs((double)value / (double)parameter);
		}
	}
}

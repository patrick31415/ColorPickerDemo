using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// “用户控件”项模板在 http://go.microsoft.com/fwlink/?LinkId=234236 上提供

namespace ColorPicker {
	public sealed partial class ColorPicker1 : UserControl {
		private Windows.UI.Color colorFinal, colorPure;
		private double xPanel, yPanel;		//左下角(0, 0)，右上角(1, 1)
		private double pPurepanel;

		public Windows.UI.Color color {
			get {
				return colorFinal;
			}
			set {
				colorFinal = value;
				separateColor();
			}
		}

		//把finalColor分解成pureColor和两个坐标值
		private void separateColor() {
			Byte maxByte, minByte, midByte, B;
			Byte[] b = new Byte[3];
			b[0] = colorFinal.R;
			b[1] = colorFinal.G;
			b[2] = colorFinal.B;
			System.Array.Sort(b);
			minByte = b[0];
			midByte = b[1];
			maxByte = b[2];

			if (maxByte == minByte) {
				colorPure = Windows.UI.Colors.Lime;
				xPanel = 0;
				yPanel = 1 - (double)minByte / 255;
			}
			else {
				B =(Byte)(255.0 * (midByte - minByte) / (maxByte - minByte));
				if (midByte == minByte) {
					xPanel = 0;
					yPanel = 1 - (double)minByte / 255;
				}
				else {
					xPanel = 255.0 * (midByte - minByte) / (B * minByte + 255.0 * (midByte - minByte));
					yPanel = 1.0 - minByte / (255.0 * (1.0 - xPanel));
				}

				colorFinal.A = 255;
				if (colorFinal.R == midByte)
					colorPure.R = B;
				if (colorFinal.G == midByte)
					colorPure.G = B;
				if (colorFinal.B == midByte)
					colorPure.B = B;
				if (colorFinal.R == maxByte)
					colorPure.R = 255;
				if (colorFinal.G == maxByte)
					colorPure.G = 255;
				if (colorFinal.B == maxByte)
					colorPure.B = 255;
				if (colorFinal.R == minByte)
					colorPure.R = 0;
				if (colorFinal.G == minByte)
					colorPure.G = 0;
				if (colorFinal.B == minByte)
					colorPure.B = 0;
			}

			pPurepanel = GetPureColorPosition();
		}

		//把pureColor和两个坐标值合并成finalColor
		private void combineColor() {
			double gray = 255 * yPanel;
			double R = yPanel * colorPure.R;
			double G = yPanel * colorPure.G;
			double B = yPanel * colorPure.B;
			colorFinal.A = 255;
			colorFinal.R = (Byte)((R - gray) * xPanel + gray);
			colorFinal.G = (Byte)((G - gray) * xPanel + gray);
			colorFinal.B = (Byte)((B - gray) * xPanel + gray);
		}

		public ColorPicker1() {
			this.InitializeComponent();
			color = Windows.UI.Colors.Lime;
		}

		private void userControl_Loaded(object sender, RoutedEventArgs e) {
			DrawDoubleLinearGradient();
			elpsSelectColor.Visibility = Windows.UI.Xaml.Visibility.Visible;
			DrawElpsSelecteColor();
			Canvas.SetTop(rectSelectPureColor, pPurepanel * Height - 3);
			Canvas.SetLeft(rectSelectPureColor, 0.85 * Width);
		}

		//绘制颜色选择区的选择点
		private void DrawElpsSelecteColor() {
			Canvas.SetLeft(elpsSelectColor, xPanel * 0.8 * Width - 5);
			Canvas.SetTop(elpsSelectColor, (1 - yPanel) * Height - 5);
			Canvas.SetLeft(cvsColorPointer, xPanel * 0.8 * Width);
			Canvas.SetTop(cvsColorPointer, (1 - yPanel) * Height);

			elpsShow.Fill = new SolidColorBrush(color);
		}
		
		//绘制颜色选择区rectColor
		private void DrawDoubleLinearGradient() {
			cvsDoubleLinear.Children.Clear();
			int nRect = (int)Height;
			Windows.UI.Xaml.Shapes.Rectangle[] rectLinearGradient = new Windows.UI.Xaml.Shapes.Rectangle[nRect];
			LinearGradientBrush[] linearBrush = new LinearGradientBrush[nRect];
			for (int i = 0; i < nRect; ++i) {
				Windows.UI.Color clr1 = new Windows.UI.Color(), clr2 = new Windows.UI.Color();
				clr1.A = clr2.A = 255;
				clr2.R = clr2.G = clr2.B = (Byte)((nRect - 1 - i) * 255 / (nRect - 1));
				clr1.R = (Byte)((nRect - i - 1) * colorPure.R / (nRect - 1));
				clr1.G = (Byte)((nRect - i - 1) * colorPure.G / (nRect - 1));
				clr1.B = (Byte)((nRect - i - 1) * colorPure.B / (nRect - 1));
				GradientStop gradientStop1 = new GradientStop(), gradientStop2 = new GradientStop();
				gradientStop1.Offset = 0;
				gradientStop1.Offset = 1;
				gradientStop1.Color = clr1;
				gradientStop2.Color = clr2;
				linearBrush[i] = new LinearGradientBrush();
				linearBrush[i].StartPoint = new Point(0, 0);
				linearBrush[i].EndPoint = new Point(1, 0);
				linearBrush[i].GradientStops.Add(gradientStop2);
				linearBrush[i].GradientStops.Add(gradientStop1);

				rectLinearGradient[i] = new Windows.UI.Xaml.Shapes.Rectangle();
				rectLinearGradient[i].Width = 0.8 * Width;
				rectLinearGradient[i].Height = 1;
				rectLinearGradient[i].StrokeThickness = 0;
				rectLinearGradient[i].Fill = linearBrush[i];
				cvsDoubleLinear.Children.Add(rectLinearGradient[i]);
				Canvas.SetTop(rectLinearGradient[i], i);
			}
		}

		private void cvsBody_PointerPressed(object sender, PointerRoutedEventArgs e) {
			var pointer = e.GetCurrentPoint(cvsBody).Position;
			if (pointer.X > 0.85 * Width - 1 && pointer.X < Width + 1 && pointer.Y > -1 && pointer.Y < Height) {
				//纯色选择器
				cvsColorPointer.Visibility = Windows.UI.Xaml.Visibility.Visible;
				Canvas.SetTop(rectSelectPureColor, pointer.Y - 3);
				//Canvas.SetLeft(rectSelectPureColor, 0.85 * Width);
				pPurepanel = (double)pointer.Y / Height;
				colorPure = GetPureColor(pPurepanel);
				combineColor();
				elpsShow.Fill = new SolidColorBrush(colorFinal);
				DrawDoubleLinearGradient();
			}
			else if (pointer.X > -1 && pointer.X < Width * 0.8 + 1 && pointer.Y > -1 && pointer.Y < Height + 1) {
				//颜色选择器
				xPanel = pointer.X / (0.8 * Width);
				yPanel = 1 - pointer.Y / Height;
				DrawElpsSelecteColor();
				combineColor();
				elpsShow.Fill = new SolidColorBrush(colorFinal);
				cvsColorPointer.Visibility = Windows.UI.Xaml.Visibility.Visible;
			}
			else {
				cvsColorPointer.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
			}
		}

		//获取当前颜色对应的colorPure在rectPureColor中的位置
		private double GetPureColorPosition() {
			Byte r = colorPure.R, g = colorPure.G, b = colorPure.B;

			if (r == 255) {
				if (b == 0)
					return (double)g / 1530;					/*Part 1*/
				else
					return (double)(1530 - b) / 1530;		/*Part 6*/
			}
			else if (r == 0) {
				if (g == 255)
					return (double)(510 + b) / 1530;		/*Part 3*/
				else
					return (double)(1020 - g) / 1530;		/*Part 4*/
			}
			else {
				if (g == 255)
					return (double)(510 - r) / 1530;			/*Part 2*/
				else
					return (double)(1020 + r) / 1530;		/*Part 5*/
			}
		}

		//获取当前在rectPureColor中的位置对应的colorPure
		private Windows.UI.Color GetPureColor(double k) {
			Windows.UI.Color c = new Windows.UI.Color();
			c.A = 255;
			if (k < 1.0 / 6) {
				k -= 1.0 / 6;
				k = -k;
				k *= 6;
				c.R = 255;
				c.B = 0;
				k = 1 - k;
				c.G = (Byte)(k * 255);
			}
			else if (k < 1.0 / 3) {
				k -= 1.0 / 3;
				k = -k;
				k *= 6;
				c.G = 255;
				c.B = 0;
				c.R = (Byte)(k * 255);
			}
			else if (k < 1.0 / 2) {
				k -= 0.5;
				k = -k;
				k *= 6;
				c.R = 0;
				c.G = 255;
				k = 1 - k;
				c.B = (Byte)(k * 255);
			}
			else if (k < 2.0 / 3) {
				k -= 2.0 / 3;
				k = -k;
				k *= 6;
				c.R = 0;
				c.B = 255;
				c.G = (Byte)(k * 255);
			}
			else if (k < 5.0 / 6) {
				k -= 5.0 / 6;
				k = -k;
				k *= 6;
				c.G = 0;
				c.B = 255;
				k = 1 - k;
				c.R = (Byte)(k * 255);
			}
			else {
				k--;
				k = -k;
				k *= 6;
				c.R = 255;
				c.G = 0;
				c.B = (Byte)(k * 255);
			}
			return c;
		}

		private void cvsBody_PointerMoved(object sender, PointerRoutedEventArgs e) {
			var pointer = e.GetCurrentPoint(cvsBody).Position;
			if (e.Pointer.IsInContact && pointer.X > 0.85 * Width - 1 && pointer.X < Width + 1 && pointer.Y > -1 && pointer.Y < Height) {
				//纯色选择器
				Canvas.SetTop(rectSelectPureColor, pointer.Y - 3);
				//Canvas.SetLeft(rectSelectPureColor, 0.85 * Width);
				pPurepanel = (double)pointer.Y / Height;
				colorPure = GetPureColor(pPurepanel);
				combineColor();
				elpsShow.Fill = new SolidColorBrush(colorFinal);
				DrawDoubleLinearGradient();
			}
			else if (e.Pointer.IsInContact && pointer.X > -1 && pointer.X < Width * 0.8 + 1 && pointer.Y > -1 && pointer.Y < Height + 1) {
				//颜色选择器
				xPanel = pointer.X / (0.8 * Width);
				yPanel = 1 - pointer.Y / Height;
				DrawElpsSelecteColor();
				combineColor();
				elpsShow.Fill = new SolidColorBrush(colorFinal);
				cvsColorPointer.Visibility = Windows.UI.Xaml.Visibility.Visible;
			}
			else {
				cvsColorPointer.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
			}
		}

		private void cvsBody_PointerReleased(object sender, PointerRoutedEventArgs e) {
			cvsColorPointer.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
		}
	}
}

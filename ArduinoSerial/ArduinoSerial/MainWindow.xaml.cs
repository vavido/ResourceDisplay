using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Management;
using System.IO.Ports;
using System.Diagnostics;
using System.Threading;
using ArduinoSerial.Connection;

namespace ArduinoSerial {

    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {

        public MainWindow() {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e) {
            var sc = SerialConnection.GetInstance();
            sc.Connect();

            StatusText.Text = sc.GetConnectionInfo();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e) { }

        private void OK_Button_Click(object sender, RoutedEventArgs e) {

            var value = float.Parse(TextToSend.Text);
            StatusText.Text = $"\"{value,5:##0.###}\"";

        private void OK_Button_Click(object sender, RoutedEventArgs e) {
            var sc = SerialConnection.GetInstance();

            if (sc.IsConnected()) {
                sc.PrintFloat(float.Parse(TextToSend.Text), 0);
            }

            //var sc = SerialConnection.GetInstance();

            //if (sc.IsConnected()) {
            //  sc.PrintString(TextToSend.Text, new byte[4], 0x0);
            //}
        }

        private void Arduino_Serial_Interface_Closed(object sender, EventArgs e) {
            SerialConnection.GetInstance().CloseConnection();
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) {
            double value = SliderFormatTest.Value;

            if (value < 999) {
                TextBlockFormat.Text = $"{value,5:#.#}";
            } else {
                TextBlockFormat.Text = $"{value,4:#}";
            }
        }
    }

}

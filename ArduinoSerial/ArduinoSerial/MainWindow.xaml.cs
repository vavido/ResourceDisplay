using System;
using System.Windows;
using System.Windows.Controls;
using ArduinoSerial.Connection;
using static ArduinoSerial.Connection.SerialConnection.ConnectionState;

namespace ArduinoSerial {

    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow {

        private SerialConnection sc;

        public MainWindow() {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e) {
            sc = new SerialConnection();
            sc.Connect();

            StatusText.Text = sc.GetConnectionInfo();

            if (!(sc.State == Connected)) return;

            var sm = new SystemMonitor(4, 4, sc);
            sm.StartMonitoring();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e) { }

        private void OK_Button_Click(object sender, RoutedEventArgs e) { }

        private void Arduino_Serial_Interface_Closed(object sender, EventArgs e) {
            if (sc.State == Connected) {
                sc.CloseConnection();
            }
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) {
            var value = SliderFormatTest.Value;

            TextBlockFormat.Text = value < 999 ? $"{value,5:#.#}" : $"{value,4:#}";
        }

    }

}
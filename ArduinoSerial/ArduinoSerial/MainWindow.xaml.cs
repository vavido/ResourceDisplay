﻿using System;
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

namespace ArduinoSerial {
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {

        

        private SerialPort sPort;

        public MainWindow() {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e) {

        }

        private void ValueSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) {
            int value = (int)ValueSlider.Value;
            send(value);
        }
    }
}

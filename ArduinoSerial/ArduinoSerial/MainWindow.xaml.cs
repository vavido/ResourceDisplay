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

namespace ArduinoSerial {
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {

        /// <summary>
        /// Maximum time to wait for data in ticks
        /// </summary>
        private const long MAX_WAIT = 1000 * TimeSpan.TicksPerMillisecond;

        private SerialPort sPort;

        public MainWindow() {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e) {

            sPort = AutodetectArduinoPort();
            if (sPort != null) {
                StatusText.Text = "Verbunden mit Arduino an " + sPort.PortName;
            } else {
                StatusText.Text = "Arduino konnte nicht gefunden werden";
            }
        }

        public SerialPort AutodetectArduinoPort() {


            String[] ports = SerialPort.GetPortNames();

            foreach (String s in ports) {



                try {
                    Debug.WriteLine("Trying port: " + s);

                    SerialPort sp = new SerialPort(s, 9600);
                    sp.Open();
                    sp.DiscardInBuffer();

                    //Create syn challange consisting of two random bytes the arduino has to add
                    byte[] syn = new byte[2];
                    (new Random()).NextBytes(syn);

                    Debug.WriteLine("Sending " + String.Join("; ", syn));

                    sp.Write(syn, 0, 2);
                    sp.DiscardInBuffer();

                    long tStart = DateTime.Now.Ticks;

                    //Busy waiting for one byte in input buffer with timeout
                    while (sp.BytesToRead < 1 && DateTime.Now.Ticks - tStart < MAX_WAIT) {
                        Thread.Sleep(10);
                    }

                    long tEnd = (DateTime.Now.Ticks - tStart) / TimeSpan.TicksPerMillisecond;


                    if (sp.BytesToRead > 0) {


                        //If there was response, test it

                        byte ack = (byte)sp.ReadByte();

                        Debug.WriteLine("Received: " + ack + " after " + tEnd + "ms" + "(should be: "+ (byte)(syn[0] + syn[1]) +")" );

                        if (ack == (byte) (syn[0] + syn[1])) {

                            //SynAck connection to arduino with magic number 42
                            byte[] synack = { 42 };
                            sp.Write(synack, 0, 1);

                            return sp;
                        }
                    } else {
                        Debug.WriteLine("No answer");
                    }

                } catch (Exception e) {
                    Debug.WriteLine("\tCan't open port: " + e.Message);
                }

            }

            return null;
        }

        private void send(int v) {
            byte ones = (byte)(v % 10 + 48);
            v /= 10;

            byte tens = (byte)(v % 10 + 48);
            v /= 10;

            byte hundreds = (byte)(v % 10 + 48);

            byte thousands = (byte)(v / 10 + 48);

            byte[] data = { thousands, hundreds, tens, ones };

            sPort?.Write(data, 0, 4);

        }

        private void ValueSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) {
            int value = (int)ValueSlider.Value;
            send(value);
        }
    }
}

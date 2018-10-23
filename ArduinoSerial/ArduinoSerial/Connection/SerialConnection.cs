using System;
using System.Diagnostics;
using System.IO.Ports;
using System.Threading;

namespace ArduinoSerial.Connection {

    /// <summary>
    /// Represents a serial connection to the arduino
    /// </summary>
    class SerialConnection {

        public enum ConnectionState {

            Disconnected,
            Connected,
            Connecting

        }

        public ConnectionState State { get; private set; } = ConnectionState.Disconnected;

        // The serial port on which the arduino is connected
        private SerialPort connection;

        /// <summary>
        /// Sends a syn signal to the arduino
        /// </summary>
        private void SendSyn(SerialPort sp, byte[] syn) {
            Debug.WriteLine("Sending " + string.Join("; ", syn));

            sp.Write(syn, 0, 2);
            sp.DiscardInBuffer();
        }

        private bool WaitAck(SerialPort sp) {
            var tStart = DateTime.Now.Ticks;

            //Busy waiting for one byte in input buffer with timeout
            while (sp.BytesToRead < 1 && DateTime.Now.Ticks - tStart < Properties.Settings.Default.ConnectionTimeout) {
                Thread.Sleep(10);
            }

            //long tEnd = (DateTime.Now.Ticks - tStart) / TimeSpan.TicksPerMillisecond;

            return sp.BytesToRead > 0;
        }

        /// <summary>
        /// Find & connect to Arduino
        /// </summary>
        public bool Connect() {
            State = ConnectionState.Connecting;

            var ports = SerialPort.GetPortNames();

            foreach (var s in ports) {
                try {
                    Debug.WriteLine("Trying port: " + s);

                    var sp = new SerialPort(s, Properties.Settings.Default.BaudRate);
                    sp.Open();

                    //Create syn challange consisting of two random bytes the arduino has to add
                    var syn = new byte[2];
                    new Random().NextBytes(syn);

                    SendSyn(sp, syn);

                    if (WaitAck(sp)) {
                        //If there was response, test it

                        var ack = (byte) sp.ReadByte();

                        if (ack == (byte) (syn[0] + syn[1])) {
                            //SynAck connection to arduino with magic number 42
                            byte[] synack = {42};
                            sp.Write(synack, 0, 1);
                            connection = sp;

                            return true;
                        }
                    } else {
                        Debug.WriteLine("No answer");
                    }
                } catch (Exception e) {
                    Debug.WriteLine($"\tCan't open port: {e.Message}");
                }
            }

            return false;
        }

        /// <summary>
        /// Checks wether a connection is established
        /// </summary>
        /// <returns>True if connected to arduino</returns>
        private bool IsConnected() {
            return State == ConnectionState.Connected;
        }

        private void SendCommand(Command command, byte param) {
            byte[] cmd = new[] {(byte) ((byte) command ^ param)};
            if (IsConnected()) {
                connection.Write(cmd, 0, 1);
            }
        }

        /// <summary>
        /// Sends a print command and then the array of bytes
        /// </summary>
        /// <param name="d"></param>
        /// <param name="addr">The number of the display to print on (0-3)</param>
        private void SendBytes(byte[] d, byte addr) {
            var command = (byte) ((byte) Command.Write | addr);

            var allData = new byte[5];
            allData[0] = command;
            Array.Copy(d, 0, allData, 1, 4);

            Debug.WriteLine("Sending data:");
            foreach (var b in allData) {
                Debug.WriteLine(Convert.ToString(b, 2).PadLeft(8, '0'));
            }

            connection.Write(allData, 0, 5);
        }

        /// <summary>
        /// Gives some info about the connection
        /// </summary>
        /// <returns>A description of the connection state for display in the UI</returns>
        public string GetConnectionInfo() {
            switch (State) {
                case ConnectionState.Connected:
                    return "Connected to Arduino on " + connection.PortName;
                case ConnectionState.Disconnected:
                    return "Not connected";
                case ConnectionState.Connecting:
                    return "Searching for Arduino...";
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Closes the connection to the arduino
        /// </summary>
        public void CloseConnection() {
            if (!IsConnected()) return;

            SendCommand(Command.Disconnect, 0);
            State = ConnectionState.Disconnected;
            connection.Close();
        }

    }

}
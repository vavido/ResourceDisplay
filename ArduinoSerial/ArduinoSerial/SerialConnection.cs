using System;
using System.Diagnostics;
using System.IO.Ports;
using System.Threading;

namespace ArduinoSerial.Connection {


    /// <summary>
    /// Represents a serial connection to the arduino
    /// </summary>
    class SerialConnection {

        // Singleton instance
        private static SerialConnection instance;


        /// <summary>
        /// Get the instance of the singleton, create a new one if not present
        /// </summary>
        /// <returns></returns>
        public static SerialConnection GetInstance() {
            if (instance == null) {
                instance = new SerialConnection();
            }
            return instance;
        }

        private SerialConnection() { }

        /// <summary>
        /// Maximum time to wait for data in ticks
        /// </summary>
        private const long MAX_WAIT = 1000 * TimeSpan.TicksPerMillisecond;

        private const int BAUDRATE = 9600;

        private const String printableChars = "0123456789AaBbCcDdEeFfHhLlPp-.,_ ";

        private const byte CMD_WRITE = 0x10, CMD_SLEEP = 0x20, CMD_WAKE = 0x30;

        // The serial port on which the arduino is connected
        private SerialPort connection;

        /// <summary>
        /// Sends a syn signal to the arduino
        /// </summary>
        private void sendSyn(SerialPort sp) {

            //Create syn challange consisting of two random bytes the arduino has to add
            byte[] syn = new byte[2];
            new Random().NextBytes(syn);

            Debug.WriteLine("Sending " + String.Join("; ", syn));

            sp.Write(syn, 0, 2);
            sp.DiscardInBuffer();

        }

        private bool waitAck(SerialPort sp) {

            long tStart = DateTime.Now.Ticks;

            //Busy waiting for one byte in input buffer with timeout
            while (sp.BytesToRead < 1 && DateTime.Now.Ticks - tStart < MAX_WAIT) {
                Thread.Sleep(10);
            }

            //long tEnd = (DateTime.Now.Ticks - tStart) / TimeSpan.TicksPerMillisecond;


            return sp.BytesToRead > 0;
        }

        /// <summary>
        /// Find & connect to Arduino
        /// </summary>
        public bool Connect() {

            String[] ports = SerialPort.GetPortNames();

            foreach (String s in ports) {

                try {
                    Debug.WriteLine("Trying port: " + s);

                    SerialPort sp = new SerialPort(s, BAUDRATE);
                    sp.Open();

                    sendSyn(sp);

                    if (waitAck(sp)) {

                        //If there was response, test it

                        byte ack = (byte)sp.ReadByte();

                        Debug.WriteLine("Received: " + ack + " after " + tEnd + "ms" + "(should be: " + (byte)(syn[0] + syn[1]) + ")");

                        if (ack == (byte)(syn[0] + syn[1])) {

                            //SynAck connection to arduino with magic number 42
                            byte[] synack = { 42 };
                            sp.Write(synack, 0, 1);
                            this.connection = sp;

                            return true;
                        }
                    } else {
                        Debug.WriteLine("No answer");
                    }

                } catch (Exception e) {
                    Debug.WriteLine("\tCan't open port: " + e.Message);
                }

            }

            return false;
        }

        /// <summary>
        /// Checks wether a connection is established
        /// </summary>
        /// <returns>True if connected to arduino</returns>
        public bool IsConnected() {
            return connection != null;
        }

        /// <summary>
        /// Prints a float value to the display
        /// </summary>
        /// <param name="f">The float value to print</param>
        /// <param name="addr">The address of the display to use</param>
        public void PrintFloat(float f, byte addr) {

            string floatValue = f.ToString("0.###");
            byte[] dps = { 0, 0, 0, 0 };

            if (floatValue.Contains(".")) {
                if (floatValue.Length < 5) {

                    leftpad(floatValue, " ", 5);
                }
                dps[floatValue.IndexOf('.') - 1] = 1;
                floatValue.Replace(".", "");
            } else {
                leftpad(floatValue, " ", 4);
            }

            PrintString(floatValue, dps, addr);

        }
        /// <summary>
        /// Prints a string to the display with the specified address
        /// </summary>
        /// <param name="s">the string to print</param>
        /// <param name="dps">indicates the decimal points</param>
        /// <param name="addr">the address of the display</param>
        public void PrintString(String s, byte[] dps, byte addr) {

            if (dps.Length != 4) {
                throw new ArgumentException("DP array must have exactly 4 elements");
            }

            byte[] data = Encode(s.ToCharArray(), dps);

            PrintBytes(data, addr);
        }

        /// <summary>
        /// Encodes data to send to the Arduino
        /// </summary>
        /// <param name="chars">Chars to send</param>
        /// <param name="dps">Indicats placement of decimal points, has to be 4 items </param>
        /// <returns></returns>
        private byte[] Encode(char[] chars, byte[] dps) {

            if (chars.Length != 4 || dps.Length != 4) throw new ArgumentException("Data to encode must be 4 digits long");

            byte[] res = new byte[chars.Length];

            for (int i = 0; i < 4; i++) {

                byte c = (byte)chars[i];
                byte dp = (dps[i]);

                res[i] = (byte)(c | dp << 7);
            }

            return res;

        }

        /// <summary>
        /// Sends a print command and then the array of bytes
        /// </summary>
        /// <param name="d"></param>
        /// <param name="addr">The number of the display to print on (0-3)</param>
        private void PrintBytes(byte[] d, byte addr) {

            byte command = (byte)(CMD_WRITE | addr);

            byte[] allData = new byte[5];
            allData[0] = command;
            Array.Copy(d, 0, allData, 1, 4);

            Debug.WriteLine("Sending data:");
            foreach (byte b in allData) {
                Debug.WriteLine(Convert.ToString(b, 2).PadLeft(8, '0'));
            }

            connection.Write(allData, 0, 5);
        }

        /// <summary>
        /// Checks if a string only consists of printable chars
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        private bool CheckPrintableChars(String s) {
            foreach (char c in s) {
                if (printableChars.IndexOf(c) == -1) {
                    return false;
                }
            }
            return true;
        }


        /// <summary>
        /// Gives some info about the connection
        /// </summary>
        /// <returns>A description of the connection state for display in the UI</returns>
        public String GetConnectionInfo() {
            if (IsConnected()) {
                return "Connected to Arduino on " + connection.PortName;
            } else {
                return "Not connected";
            }
        }

        /// <summary>
        /// Closes the connection to the arduino
        /// </summary>
        public void CloseConnection() {
            if (IsConnected()) {
                connection.Close();
            }
        }

        /// <summary>
        /// Adds chars to the left of a string to reach a given length
        /// </summary>
        /// <param name="s">The string that needs padding</param>
        /// <param name="padding">the char to use for padding</param>
        /// <param name="destlenght">the desired length</param>
        private void leftpad(string s, string padding, int destlenght) {

            while (s.Length > destlenght) {
                s = padding + s;
            }

        }
    }
}

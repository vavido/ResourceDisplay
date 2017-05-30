using System;
using System.Diagnostics;
using System.IO.Ports;
using System.Threading;

namespace ArduinoSerial {


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

        private const int BAUDRATE = 115200;

        private const String printableChars = "0123456789AaBbCcDdEeFfHhLlPp-.,_ ";

        private const byte CMD_WRITE = 0x10, CMD_SLEEP = 0x20, CMD_WAKE = 0x30;

        // The serial port on which the arduino is connected
        private SerialPort connection;

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

        public void PrintFloat(float f, byte addr) {
            PrintString(f.ToString("0.###"), addr);
        }

        public void PrintString(String s, byte addr) {

            if (s.Length > 8) {
                throw new ArgumentException("Can't print String longer than 4 chars and 4 decimal points");
            }
            if (!CheckPrintableChars(s)) {
                throw new ArgumentException("String contains unprintable chars");
            }

            char[] chars = { ' ', ' ', ' ', ' ' };
            byte[] dps = { 0, 0, 0, 0 };

            int count = 1;
            char[] input = s.ToCharArray();

            for (int i = s.Length - 1; i >= 0; i--) {
                if (input[i] == '.' || input[i] == ',') {
                    dps[dps.Length - count] = 1;
                    i--;
                }
                chars[chars.Length - count] = input[i];
                count++;
            }

            byte[] data = Encode(chars, dps);

            PrintBytes(data, addr);

        }

        private byte[] Encode(char[] chars, byte[] dps) {

            if (chars.Length != 4 || dps.Length != 4) throw new ArgumentException("Data to encode must be 4 digits long");

            byte[] res = new byte[chars.Length];

            for(int i = 0; i < 4; i++){

                byte c = (byte)chars[i];
                byte dp = (dps[i]);

                res[i] = (byte) (c | dp << 7);
            }

            return res;

        }

        /// <summary>
        /// Sends a print command and then the array of bytes
        /// </summary>
        /// <param name="d"></param>
        /// <param name="addr">The number of the display to print on (0-3)</param>
        private void PrintBytes(byte[] d, byte addr) {

            byte command = (byte) (CMD_WRITE | addr);

            byte[] allData = new byte[5];
            allData[0] = command;
            Array.Copy(d, 0, allData, 1, 4);

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

        public String GetConnectionInfo() {
            if (IsConnected()) {
                return "Connected to Arduino on " + connection.PortName;
            } else {
                return "Not connected";
            }
        }

        public void CloseConnection() {
            if (IsConnected()) {
                connection.Close();
            }
        }
    }
}

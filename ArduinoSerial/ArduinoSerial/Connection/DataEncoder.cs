using System;
using System.Collections.Generic;

namespace ArduinoSerial.Connection {

    public class DataEncoder {

        private const string PrintableChars = "0123456789AaBbCcDdEeFfHhLlPp-.,_ ";

        /// <summary>
        /// Prints a float value to the display
        /// </summary>
        /// <param name="f">The float value to print</param>
        /// <param name="addr">The address of the display to use</param>
        public static byte[] EncodeFloat(float f, byte addr) {
            var floatValue = f < 1000 ? $"{f,5:#.0}" : $"{f,4:#}";

            byte[] dps = {0, 0, 0, 0};

            if (floatValue.Contains(",")) {
                dps[floatValue.IndexOf(',') - 1] = 1;
                floatValue = floatValue.Replace(",", "");
            }

            return EncodeString(floatValue, dps, addr);
        }

        /// <summary>
        /// Prints a string to the display with the specified address
        /// </summary>
        /// <param name="s">the string to print</param>
        /// <param name="dps">indicates the decimal points</param>
        /// <param name="addr">the address of the display</param>
        public static byte[] EncodeString(string s, byte[] dps, byte addr) {
            if (!CheckPrintableChars(s))
                throw new ArgumentException(
                    $"The string contains unprintable chars! Only {PrintableChars} can be used");

            if (dps.Length != 4) {
                throw new ArgumentException("DP array must have exactly 4 elements");
            }

            var data = Encode(s.ToCharArray(), dps);

            return data;
        }

        /// <summary>
        /// Encodes data to send to the Arduino
        /// </summary>
        /// <param name="chars">Chars to send</param>
        /// <param name="dps">Indicats placement of decimal points, has to be 4 items </param>
        /// <returns></returns>
        private static byte[] Encode(IReadOnlyList<char> chars, IReadOnlyList<byte> dps) {
            if (chars.Count != 4 || dps.Count != 4)
                throw new ArgumentException("Data to encode must be 4 digits long");

            var res = new byte[chars.Count];

            for (var i = 0; i < 4; i++) {
                var c = (byte) chars[i];
                var dp = (dps[i]);

                res[i] = (byte) (c | dp << 7);
            }

            return res;
        }

        /// <summary>
        /// Checks if a string only consists of printable chars
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        private static bool CheckPrintableChars(string s) {
            foreach (var c in s) {
                if (PrintableChars.IndexOf(c) == -1) {
                    return false;
                }
            }

            return true;
        }

    }

}
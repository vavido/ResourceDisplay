using System.Diagnostics;
using System.Timers;
using ArduinoSerial.Connection;

namespace ArduinoSerial {

    internal class SystemMonitor {

        private readonly SerialConnection connection;
        private readonly Timer timer;
        private readonly PerformanceCounter cpuCounter;

        public SystemMonitor(int bufferFreq, int bufferSamples, SerialConnection connection) {
            this.connection = connection;
            cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");

            timer = new Timer() {Interval = 1000f / bufferFreq, Enabled = true, AutoReset = true};
            timer.Elapsed += TimerOnElapsed;
        }

        public void StartMonitoring() {
            if (connection.Connect()) {
                timer.Start();
            }
        }

        private void TimerOnElapsed(object sender, ElapsedEventArgs elapsedEventArgs) {
            connection.PrintFloat(cpuCounter.NextValue(), 0);
        }

        public void StopMonitoring() {
            timer.Stop();
            if (connection.IsConnected()) {
                connection.CloseConnection();
            }
        }

    }

}
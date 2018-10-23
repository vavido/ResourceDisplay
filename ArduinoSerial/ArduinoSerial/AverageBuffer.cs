using System.Linq;

namespace ArduinoSerial {

    class AverageBuffer {

        public int Size { get; }

        private readonly float[] bufferedValues;

        private int currentIndex;

        public AverageBuffer(int size) {
            Size = size;
            bufferedValues = new float[size];
        }

        public void Put(float f) {
            bufferedValues[currentIndex] = f;
            currentIndex = (currentIndex++ % Size);
        }

        public float GetAverage() {
            return bufferedValues.Average();
        }

    }

}
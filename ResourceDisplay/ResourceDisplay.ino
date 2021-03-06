#include <LedControl.h>


LedControl lc = LedControl(11, 13, 12, 2);

// Indicates wether the arduino is connected to a PC
bool conn = false;

//Command codes
const byte CMD_WRITE = 1;
const byte CMD_SLEEP = 2;
const byte CMD_WAKEUP = 3;
const byte CMD_DISCONN = 15;

const int BAUDRATE = 9600;

//for waiting animation
long lasttime = 0;
int digit = 0;

void setup() {

  Serial.begin(BAUDRATE);

  lc.shutdown(0, false);
  lc.setIntensity(0, 15);
  lc.clearDisplay(0);

  lc.shutdown(1, false);
  lc.setIntensity(1, 15);
  lc.clearDisplay(1);

  lc.setChar(0, 0, '_', false);
  delay(250);
  lc.setChar(0, 0, '-', false);

}

byte decodeCommand(byte c) {
  return c >> 4;
}

byte decodeAddress(byte c) {
  return c & 0xF;
}

/*
   Format:
    First bit set <=> decimal point set

    Rest of the byte: ascii encoding of the char
*/

void printDigits(byte addr, byte data[]) {
  int ic = addr > 2 ? 1 : 0;
  int s = addr % 2 == 0 ? 0 : 4;

  for (int i = 0; i < 4; i++) {
    bool dp = data[i] >> 7; // Most significat bit indicates decimal point
    char c = data[i] & 0x7F; // 7 least significant bytes are the char data

    lc.setChar(ic, s + i, c, dp);
  }
}

void loop() {

  if (conn) {

    if (Serial.available()) {

      byte command = Serial.read();

      switch (decodeCommand(command)) {

        case CMD_WRITE: {

            byte address = decodeAddress(command);

            byte data[4];
            while (Serial.available() < 4) {} //Wait for the rest of the package

            for (int i = 0; i < 4; i++) {
              data[i] = Serial.read();
            }

            printDigits(address, data);

          } break;

        case CMD_DISCONN: {

            Serial.write((byte)42);
            conn = false;

          } break;
        default:
          break;
      }
    }

    //Code for establishing connection
  } else {

    //Wait for syn
    if (Serial.available() >= 2) {

      byte a = Serial.read();
      byte b = Serial.read();

      //Send ack: add a and b
      Serial.write(a + b);

      //Wait for synack
      while (!Serial.available()) {}

      if (Serial.read() == (byte)42) {

        //Correct synack received, connection established
        conn = true;

        lc.clearDisplay(0);
        lc.clearDisplay(1);

      }
    } else {
      if (millis() - lasttime > 50) {
        lc.setChar(floor(digit / 8), digit % 8, ' ', false);
        digit = (digit + 1) % 16;
        lc.setChar(floor(digit / 8), digit % 8, '-', false);
        lasttime = millis();
      }
    }
  }
}


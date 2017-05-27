#include <LedControl.h>

LedControl lc = LedControl(11, 13, 12, 1);

// Indicates wether the arduino is connected to a PC
bool conn = false;

//Command codes
const byte CMD_WRITE = 1;
const byte CMD_SLEEP = 2;
const byte CMD_WAKEUP = 3;

void setup() {

  Serial.begin(9600);

  lc.shutdown(0, false);
  lc.setIntensity(0, 8);
  lc.clearDisplay(0);

  lc.setChar(0, 3, '_', false);

}

byte decodeCommand(byte c) {
  return c >> 4;
}

byte decodeAddress(byte c) {
  return c & 0xF;
}


/*
 * Format:
 *  First bit set <=> decimal point set
 *  
 *  Rest of the byte: number of the char to display
 *  Order: 0-9, other chars, space
 */
char decodeChar(byte b){
  
}

void printDigits(byte addr,  char[] data){
  int ic = addr > 2 ? 1 : 0;
  int s = addr % 2 == 0 ? 0 : 4;

  
}

void loop() {

  if (conn) {

    if (Serial.available()) {
      byte command = Serial.read();
      switch (decodeCommand(command)) {
        case CMD_WRITE:
          byte address = decodeAddress(command);

          char chars[4];
          for (int i = 0; i < 4; i++) {
            data[i] = decodeChar(Serial.read());
          }

          printDigits(address, data);
          break;

      case default:
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

      }
    }
  }
}


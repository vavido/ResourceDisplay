#include <LedControl.h>

LedControl lc = LedControl(11, 13, 12, 2);

void setup() {


  lc.setIntensity(0, 15);
  lc.clearDisplay(0);
  lc.clearDisplay(1);

  lc.setChar(0, 0, 'a', false);
  lc.setChar(0, 1, 'b', false);
  lc.setChar(0, 2, 'c', false);
  lc.setChar(0, 3, 'd', false);
  lc.setChar(0, 4, 'e', false);
  lc.setChar(0, 5, 'f', false);
  lc.setChar(0, 6, 'g', false);
  lc.setChar(0, 7, 'h', false);

  
  lc.setChar(1, 0, 'a', false);
  lc.setChar(1, 1, 'b', false);
  lc.setChar(1, 2, 'c', false);
  lc.setChar(1, 3, 'd', false);
  lc.setChar(1, 4, 'e', false);
  lc.setChar(1, 5, 'f', false);
  lc.setChar(1, 6, 'g', false);
  lc.setChar(1, 7, 'h', false);
}

void loop() {

}


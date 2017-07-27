#include <LedControl.h>

LedControl lc = LedControl(11, 13, 12, 1);

int number = 0;

void setup() {



  lc.shutdown(0, false);
  lc.shutdown(1, false);

  lc.setIntensity(0, 15);
  lc.setIntensity(1, 15);

  lc.clearDisplay(0);
  //lc.clearDisplay(1);

  lc.setDigit(0, 0, 1, true);
  lc.setDigit(0, 1, 2, true);
  lc.setDigit(0, 2, 3, true);
  lc.setDigit(0, 3, 4, true);
  lc.setDigit(0, 4, 5, true);
  lc.setDigit(0, 5, 6, true);
  lc.setDigit(0, 6, 7, true);
  lc.setDigit(0, 7, 8, true);

  /*
  lc.setChar(1, 0, 'a', false);
  lc.setChar(1, 1, 'b', false);
  lc.setChar(1, 2, 'c', false);
  lc.setChar(1, 3, 'd', false);
  lc.setChar(1, 4, 'e', false);
  lc.setChar(1, 5, 'f', false);
  lc.setChar(1, 6, 'g', false);
  lc.setChar(1, 7, 'h', false);
  */
}

void printNumber(int v) {  
    int ones;  
    int tens;  
    int hundreds; 

    boolean negative=false;

    if(v < -999 || v > 999)  
        return;  
    if(v<0) {  
        negative=true; 
        v=v*-1;  
    }
    ones=v%10;  
    v=v/10;  
    tens=v%10;  
    v=v/10; hundreds=v;  
    if(negative) {  
        //print character '-' in the leftmost column  
        lc.setChar(0,3,'-',false);  } 
    else {
        //print a blank in the sign column  
        lc.setChar(0,3,' ',false);  
    }  
    //Now print the number digit by digit 
    lc.setDigit(0,2,(byte)hundreds,false);
    lc.setDigit(0,1,(byte)tens,false); 
    lc.setDigit(0,0,(byte)ones,false); 
} 

void loop() {
int number = (number +1) % 1000;
printNumber(number);
}


#Resource Display (working name)

##Protocol

###Establishing a connection

The PC sends two random bytes. The arduino has to add the bytes and send the result back.
When the PC receives the correct sum it acknowledges this with the magic number 42

###Commands

All commands have the size of one byte, but only the first 4 bits are used to encode the command. The four least signigicant bits encode the number of the display the command should be applied to or other parameters.  Depending on the command, additional bytes may follow to indicate further instructions

* __Write:__ Value: 0x1 | Paramters: Display Address 

    The write command is followed by four bytes which encode the chars to write to the display. The most significant bit in each byte indicates if the decimal point should be lit, (set bit = decimal point), the remaining bits are the char itself in ASCII encoding. 
    
* __Sleep:__ Value: 0x2 | Paramters: none

    Puts the driver ICs into sleep mode, turns of the displays
    
* __Wake:__ Value: 0x3 | Parameters: none

    Wakes the displays from sleep mode

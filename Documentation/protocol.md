## Protocol

### Establishing a connection

The PC sends two random bytes. The arduino has to add the bytes and send the result back.
When the PC receives the correct sum it acknowledges this with the magic number **42**

### Commands

All commands have the size of one byte, but only the **first 4 bits are used to encode the command**. The four least signigicant bits encode the number of the display the command should be applied to or other parameters.  Depending on the command, additional bytes may follow to indicate further instructions

| Command                                    | Hex Value | Parameters                                                   |
| ------------------------------------------ | --------- | ------------------------------------------------------------ |
| **Write**                                  | 0x10      | The write command is followed by four bytes which encode the chars to write to the display. The most significant bit in each byte indicates if the decimal point should be lit, (set bit = decimal point), the remaining bits are the char itself in ASCII encoding. |
| **Sleep**                                  | 0x20      | Puts the driver ICs into sleep mode, turns of the displays   |
| **Wake**                                   | 0x30      | Wakes the displays from sleep mode                           |
| **Disconnect** (currently not implemented) | 0xF0      | Terminates the connection; Arduino starts listening for connections again |


namespace ArduinoSerial.Connection {

    public enum Command : byte {

        Write = 0x10,
        Sleep = 0x20,
        Wake = 0x30,
        Disconnect = 0xF0

    }

}
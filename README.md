#Resource Display (working name)

##Protocol

###Establishing a connection

The PC sends two random bytes. The arduino has to add the bytes and send the result back. 

When the PC receives the correct sum it acknowledges this with the magic number 42

###Commands

All commands have the size of one byte. Depending on the command, additional bytes may follow to indicate further instructions




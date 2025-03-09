
# Knights of the Old Republic Message Injector
This class library privdes a .Net interace allowing for the injection of `CSWMessage` events within either KotOR game. This is done by hooking onto the process with Windows API, building out a message (using reverse engineered schema), and injecting this message into the process by allocating remote memory to store it. This message is then sent by allocating and writing some brief assembly that will call the relevant "send message" function submitting our message. This assembly is run by spawning a remote thread within the process.

This Library contains serveral classes, listed below.

## ProcessAPI

This just wraps a variaty of common Windows API functions that will be needed for manipulating the live program.

### Example Usage

```cs
// Writing process Memory
UIntPtr bytesWritten;
processHandle = ProcessAPI.OpenProcessByName("swkotor.exe");
IntPtr address = (IntPtr)0x0056f23c;
byte[] buffer = new byte[] {0x1, 0x2, 0x3, 0x4};
ProcessAPI.WriteProcessMemory(processHandle, address, buffer, 4, out bytesWritten);
```

```cs
// Allocating Virtual Space
processHandle = ProcessAPI.OpenProcessByName("swkotor.exe");
IntPtr remoteMemory = ProcessAPI.VirtualAllocEx
(
    processHandle,
    (IntPtr)null,
    ProcessAPI.SHELLCODE_SPACE,
    ProcessAPI.MEM_COMMIT | ProcessAPI.MEM_RESERVE,
    ProcessAPI.PAGE_READWRITE
);
```

_See `Injector` implementation for more examples of using this class._

## KotorHelpers

This static class is just a set of helpful functions and values for use with manipulating KotOR.

### Constants

This class holds various constants.
	- Constants prefixed with `KOTOR_OFFSET_` are pointer offset constants that are shared by all versions of the game.
	- Constants prefixed with `KOTOR_1_` are specific to KotOR1
	- Constants prefixed with `KOTOR_2_` are specific to KotOR2
	- Constants labeled `MODULE_SIZE` are the sizes of the process module as obtained from ProcessAPI.GetModuleSize(). This is usful for determining which game is attached
	- `GAME_OBJECT_TYPES` are the various types of game objects (creatures, doors, etc) many of these messages can target. See `Message.PlayerMessageTypes.GAME_OBJ_UPDATE`
	
### Example Usage

```cs
// Resolve Game version
bool isSteam;
int gameVersion = KotorHelpers.getGameVersion(processHandle, out isSteam);
```

```cs
// Get various player/creature IDs
IntPtr pHandle = ProcessAPI.OpenProcessByName("swkotor2.exe");
uint playerServerId = KotorHelpers.getPlayerServerID(pHandle);
uint playerClientId = KotorHelpers.getPlayerClientID(pHandle);
uint lookingAtServerId = KotorHelpers.getLookingAtServerID(pHandle);
uint lookingAtClientId = KotorHelpers.getLookingAtClientID(pHandle);
```

## Message

This data type is used for constructing the message that will be injected into the game.

### Parts of a Message

#### Source

See `Message.source`
Every message has a source. This can be one of the following:
	- The Player (See `MessageSources.PLAYER_TO_SERVER`)
	- The SysAdmin (See `MessageSources.SYSADMIN_TO_SERVER`)
	- The Server sending to the player (See `MessageSources.SERVER_TO_PLAYER`)
	- The Server sending to the SysAdmin (See `MessageSources.SERVER_TO_SYSADMIN`)

SysAdmin messages go largely unused in the base game, and are not currently implementated in this library. Though this may be a future effort.

Under-the-hood the source sets the first byte of the message to a character corresponding to the source. See `enum MessageSources` for details.

#### Type

See `Message.typePlayer` and `Message.typeSysAdmin`
Every message hasa type. There are around 50 of these, though not all are in use by the game. For the full list of player message types, see `enum PlayerMessageTypes`. SysAdmin messaging is currently not supported.

Under-the-hood the type sets the second byte in the raw message.

#### SubType

See `Message.subtype`
All messages require a subtype, though these aren't always used by the underlying message, and for many the default of `1` works fine. This varies depending on the schema of the message being sent. (Note these schema are not yet well documented, and have primarily been reverse engineered)

Under-the-hood the type sets the third byte in the raw message.

#### Content

Many (but not all) messages include some amount of message content. This can be added to using one of the many `write` commands:
	- `writeByte`
	- `writeUint`
	- `writeInt`
	- `writeUshort`
	- `writeShort`
	- `writeFloat`
	- `writeVector` (3 32-bit floats)
	- `writeBool` (Actually written as a Uint)
	- `writeCResRef` (Writes a BioWare Aurora Style Resource Reference, <16 characters)
	- `writeCExoString` (Writes a BioWare Aurora Style String, any number of characters)
	- `writeVoid` (just raw bytes)
	- `writeCExoLocString` (Writes a BioWare Aurora Style Localized string, either with a custom string, or with a string reference number and language)
	
### Example Usage

```cs
//Teleporting Main Character to (x, y, z) = (30, -38, 0)
IntPtr pHandle = ProcessAPI.OpenProcessByName("swkotor2.exe");
uint playerClientId = getPlayerClientID(pHandle);
Message msg = new Message(Message.PlayerMessageTypes.GAME_OBJ_UPDATE, 1, false);
msg.writeByte(0x55);
msg.writeByte(KotorHelpers.GAME_OBJECT_TYPES.CREATURE);
msg.writeUint(playerClientId);
msg.writeUint(KotorHelpers.CLIENT_OBJECT_UPDATE_FLAGS.POSITION);
msg.writeFloat(30f);
msg.writeFloat(-38f);
msg.writeFloat(0.0f);
```

```cs
//Heal Cheat
IntPtr pHandle = ProcessAPI.OpenProcessByName("swkotor.exe");
Message msg = new Message(Message.PlayerMessageTypes.CHEAT, 2);
```

```cs
//Run Transit Fail Script
IntPtr pHandle = ProcessAPI.OpenProcessByName("swkotor2.exe");
Message msg = new Message(Message.PlayerMessageTypes.CHEAT, 8);
KotorHelpers.setServerDebugMode(true, pHandle); // Debug mode must be on to run scripts
msg.writeCExoString("k_trg_transfail1");
```

```cs
// Apply Instant Death to Target
IntPtr pHandle = ProcessAPI.OpenProcessByName("swkotor.exe");
Message msg = new Message(Message.PlayerMessageTypes.CHEAT, 0x7);
KotorHelpers.setServerDebugMode(true, pHandle);
msg.writeUint(playerServerId);
msg.writeUint(lookingAtServerId);
```
	
```cs
// Activate Freecam
IntPtr pHandle = ProcessAPI.OpenProcessByName("swkotor.exe");
Message msg = new Message(Message.PlayerMessageTypes.CAMERA, 2, false);
msg.writeByte(7); // Camera Mode 7 is Free Cam
```

## SendMessageShellcode

This class handles the version specific creation of the assembly code that will be injected into and run in the process. This class is internal to the `Injector` and is used as such.
	
## Injector

This class Handles the actual injecting and sending of messages to the game.

### Example Usage

```cs
IntPtr pHandle = ProcessAPI.OpenProcessByName("swkotor2.exe");

uint size = ProcessAPI.GetModuleSize(pHandle);
Console.WriteLine($"Module Size = {size}");

bool isSteam;
int version = KotorHelpers.getGameVersion(pHandle, out isSteam);
Console.WriteLine($"Game Version: KotOR{version} {(isSteam ? "STEAM" : "")}");

Injector i = new Injector(pHandle);
uint lookingAtClientId = KotorHelpers.getLookingAtClientID(pHandle);

Message msg;

//Swap / Recruit Targetted Creature
msg = new Message(Message.PlayerMessageTypes.GAME_OBJ_UPDATE, 2, false);
msg.writeUint(lookingAtClientId);

Console.WriteLine($"Sending Message:\n{msg}");
i.sendMessage(msg);
```

```cs
// Injectors can also be constructed with a process name
Injector i = new Injector("swkotor.exe");

// Activate Cinematic Cam
Message msg = new Message(Message.PlayerMessageTypes.CAMERA, 2, false);
msg.writeByte(1); 

Console.WriteLine($"Sending Message:\n{msg}");
i.sendMessage(msg);
```

## Videos
- https://www.youtube.com/watch?v=yIynqfQO_wU
- https://www.youtube.com/watch?v=Hljc0sBWnRc
- https://www.youtube.com/watch?v=KAyYkPgnkxI
- https://www.youtube.com/watch?v=dIN6MFFF-nM
- https://www.youtube.com/watch?v=TV21RnuAZro
- https://www.youtube.com/watch?v=7sAWLj3aYhs

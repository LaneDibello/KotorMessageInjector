using System;
using System.Collections.Generic;

namespace KotorMessageInjector
{
    class SendMessageShellcode
    {
        enum SendFunctions : uint
        {
            K1_PLAYER_TO_SERVER   = 0x00677410,
            K1_SERVER_TO_PLAYER   = 0x00567e60,
            K1_SYSADMIN_TO_SERVER = 0x00675690, //Not yet supported
            K1_SERVER_TO_SYSADMIN = 0x00000000, //More RE to be done here

            K2_PLAYER_TO_SERVER   = 0x00505c30,
            K2_SERVER_TO_PLAYER   = 0x007dd770,
            K2_SYSADMIN_TO_SERVER = 0x00000000, //Not yet supported
            K2_SERVER_TO_SYSADMIN = 0x00000000, //More RE to be done here
        }

        private List<byte> shellcode = new List<byte>{
            0x55,                         // push ebp               | 0
            0x8B, 0xEC,                   // mov ebp, esp

            // Get the this pointer through the chain of pointers
            0xA1, 0x00, 0x00, 0x00, 0x00, // mov eax, [0x00000000]  | 3
            0x83, 0xC0, 0x04,             // add eax, 0x4           | 8
            0x8B, 0x00,                   // mov eax, [eax]
            0x83, 0xC0, 0x04,             // add eax, 0x4           | 13
            0x8B, 0x00,                   // mov eax, [eax]
            0x05, 0x00, 0x00, 0x00, 0x00, // add eax, 0x00000000    | 18
            0x8B, 0xC8,                   // mov ecx, eax ; ECX now holds the this pointer
        };

        private List<byte> footer = new List<byte>
        {
            0xFF, 0xD0,                   // call eax

            // Cleanup and return
            0x83, 0xC4, 0x10,             // add esp, 16 (clean up 4 parameters)
            0x8B, 0xE5,                   // mov esp, ebp
            0x5D,                         // pop ebp
            0xC3                          // ret
        };

        private int gameVersion;
        private bool isSteam;
        private bool sourceIsClient;

        public SendMessageShellcode(Message msg, IntPtr remoteMessageData, int gameVersion = 1, bool isSteam = false)
        {
            this.gameVersion = gameVersion;
            this.isSteam = isSteam;
            setAppManager((uint)KotorHelpers.getGameAppmanager(gameVersion, isSteam));

            SendFunctions sendFunction;
            switch (msg.source)
            {
                case Message.MessageSources.PLAYER_TO_SERVER:
                    sendFunction = gameVersion == 1 ? SendFunctions.K1_PLAYER_TO_SERVER : SendFunctions.K2_PLAYER_TO_SERVER;
                    sourceIsClient = true;
                    break;
                case Message.MessageSources.SERVER_TO_PLAYER:
                    sendFunction = gameVersion == 1 ? SendFunctions.K1_SERVER_TO_PLAYER : SendFunctions.K2_SERVER_TO_PLAYER;
                    sourceIsClient = false;
                    break;
                case Message.MessageSources.SYSADMIN_TO_SERVER:
                    sendFunction = gameVersion == 1 ? SendFunctions.K1_SYSADMIN_TO_SERVER : SendFunctions.K2_SYSADMIN_TO_SERVER;
                    sourceIsClient = true;
                    break;
                case Message.MessageSources.SERVER_TO_SYSADMIN:
                    sendFunction = gameVersion == 1 ? SendFunctions.K1_SERVER_TO_SYSADMIN : SendFunctions.K2_SERVER_TO_SYSADMIN;
                    sourceIsClient = false;
                    break;
                default:
                    throw new MessageNotInitializedException($"Unknown Message Source {msg.source}");
            }
            setMessageHandlerOffset();

            addStackPush4Bytes(msg.length);
            addStackPush4Bytes((uint)remoteMessageData);
            addStackPush1Byte(msg.subtype);
            addStackPush1Byte((byte)msg.typePlayer); // Will need to be more complicated to support sysadmin messaging
            if (!sourceIsClient) addStackPush4Bytes(0);
            addMovEAX4Bytes((uint)sendFunction);
            shellcode.AddRange(footer);
        }

        private void addStackPush4Bytes(uint value)
        {
            shellcode.Add(0x68);
            shellcode.Add((byte)(value & 0xFF));
            shellcode.Add((byte)((value >> 8) & 0xFF));
            shellcode.Add((byte)((value >> 16) & 0xFF));
            shellcode.Add((byte)((value >> 24) & 0xFF));
        }

        private void addStackPush1Byte(byte value)
        {
            shellcode.Add(0x6A);
            shellcode.Add(value);
        }

        private void addMovEAX4Bytes(uint value)
        {
            shellcode.Add(0xB8);
            shellcode.Add((byte)(value & 0xFF));
            shellcode.Add((byte)((value >> 8) & 0xFF));
            shellcode.Add((byte)((value >> 16) & 0xFF));
            shellcode.Add((byte)((value >> 24) & 0xFF));
        }

        private void setAppManager(uint value)
        {
            shellcode[4] = (byte)(value & 0xFF);
            shellcode[5] = (byte)((value >> 8) & 0xFF);
            shellcode[6] = (byte)((value >> 16) & 0xFF);
            shellcode[7] = (byte)((value >> 24) & 0xFF);
        }
        
        private void setMessageHandlerOffset()
        {
            if (gameVersion == 1)
            {
                if (sourceIsClient)
                {
                    //Client Address
                    shellcode[10] = 0x4;  //ClientExoApp
                    shellcode[19] = 0x4c; //Client Message Handler offset
                    shellcode[20] = 0x01;
                    shellcode[21] = 0x00;
                    shellcode[22] = 0x00;
                }
                else
                {
                    //Server Address
                    shellcode[10] = 0x8; //ServerExoApp
                    shellcode[19] = 0x10; //Server Message Handler offset
                    shellcode[20] = 0x00;
                    shellcode[21] = 0x01;
                    shellcode[22] = 0x00;
                }
            }
            else if (gameVersion == 2)
            {
                if (sourceIsClient)
                {
                    //Client Address
                    shellcode[10] = 0x4;  //ClientExoApp
                    shellcode[19] = 0x50; //Client Message Handler offset
                    shellcode[20] = 0x01;
                    shellcode[21] = 0x00;
                    shellcode[22] = 0x00;
                }
                else
                {
                    //Server Address
                    shellcode[10] = 0x8; //ServerExoApp
                    shellcode[19] = 0x10; //Server Message Handler offset
                    shellcode[20] = 0x00;
                    shellcode[21] = 0x01;
                    shellcode[22] = 0x00;
                }
            }
            else
            {
                throw new ArgumentException($"Cannot find game version {gameVersion}");
            }
        }

        public byte[] code
        {
            get
            {
                return shellcode.ToArray();
            }
        }

        public uint length
        {
            get
            {
                return (uint)shellcode.Count;
            }
        }
    }
}

using System;
using static KotorMessageInjector.ProcessAPI;

namespace KotorMessageInjector
{
    public static class KotorHelpers
    {
        private static IntPtr KOTOR_1_APPMANAGER = (IntPtr)0x007a39fc;
        private static IntPtr KOTOR_1_DEACTIVATE_RENDER_WINDOW = (IntPtr)0x00401d90;

        public static class CLIENT_OBJECT_UPDATE_FLAGS
        {
            public const uint POSITION            = 0b00000000000000000000000000000001;
            public const uint ORIENTATION         = 0b00000000000000000000000000000010;
            public const uint ANIMATION           = 0b00000000000000000000000000000100;
            public const uint VFX                 = 0b00000000000000000000000000001000;
            public const uint OBJECT_INTERACTION  = 0b00000000000000000000000000010000; //Specific to Doors, Placeables, and Triggers
            public const uint PORTRAIT            = 0b00000000000000000000000000100000;
            // Below are specific to Creatures
            // TODO: Fill these out

        }

        public static class GAME_OBJECT_TYPES
        {
            public const byte OBJECT_0 = 0;
            public const byte OBJECT_1 = 1;
            public const byte OBJECT_2 = 2;
            public const byte MODULE = 3;
            public const byte AREA = 4;
            public const byte CREATURE = 5;
            public const byte ITEM = 6;
            public const byte TRIGGER = 7;
            public const byte PROJECTILE = 8;
            public const byte PLACEABLE = 9;
            public const byte DOOR = 10;
            public const byte AREAOFEFFECT = 11;
            public const byte WAYPOINT = 12;
            public const byte ENCOUNTER = 13;
            public const byte STORE = 14;
            public const byte OBJECT_f = 15;
            public const byte SOUND = 16; 
        }
        
        public static void disableClickOutPausing(IntPtr processHandle)
        {
            UIntPtr outPtr;
            WriteProcessMemory(processHandle, KOTOR_1_DEACTIVATE_RENDER_WINDOW, new byte[] { 0xc3 }, 1, out outPtr);
        }
        
        private static uint getClientInternal(IntPtr processHandle)
        {
            byte[] outBytes = new byte[4];
            UIntPtr outPtr;

            ReadProcessMemory(processHandle, KOTOR_1_APPMANAGER, outBytes, 4, out outPtr);
            uint appmanager = BitConverter.ToUInt32(outBytes, 0);

            ReadProcessMemory(processHandle, (IntPtr)(appmanager + 4), outBytes, 4, out outPtr);
            uint client = BitConverter.ToUInt32(outBytes, 0);

            ReadProcessMemory(processHandle, (IntPtr)(client + 4), outBytes, 4, out outPtr);
            return BitConverter.ToUInt32(outBytes, 0);
        }

        private static uint getServerInternal(IntPtr processHandle)
        {
            byte[] outBytes = new byte[4];
            UIntPtr outPtr;

            ReadProcessMemory(processHandle, KOTOR_1_APPMANAGER, outBytes, 4, out outPtr);
            uint appmanager = BitConverter.ToUInt32(outBytes, 0);

            ReadProcessMemory(processHandle, (IntPtr)(appmanager + 8), outBytes, 4, out outPtr);
            uint server = BitConverter.ToUInt32(outBytes, 0);

            ReadProcessMemory(processHandle, (IntPtr)(server + 4), outBytes, 4, out outPtr);
            return BitConverter.ToUInt32(outBytes, 0);
        }


        public static void setServerDebugMode(bool debugOn, IntPtr processHandle)
        {
            byte[] inBytes = new byte[4] { (byte)(debugOn ? 0x1 : 0x0), 0x0, 0x0, 0x0 };
            UIntPtr outPtr;

            uint serverInternal = getServerInternal(processHandle);

            WriteProcessMemory(processHandle, (IntPtr)(serverInternal + 0x1006c), inBytes, 4, out outPtr);
        }

        public static uint getPlayerClientID(IntPtr processHandle)
        {
            byte[] outBytes = new byte[4];
            UIntPtr outPtr;

            uint clientInternal = getClientInternal(processHandle);

            ReadProcessMemory(processHandle, (IntPtr)(clientInternal + 0x20), outBytes, 4, out outPtr);
            return BitConverter.ToUInt32(outBytes, 0);
        }

        public static uint getPlayerServerID(IntPtr processHandle)
        {
            return clientToServerId(getPlayerClientID(processHandle));
        }

        public static uint clientToServerId(uint clientId)
        {
            if (clientId != 0x7f000000)
            {
                return clientId & 0x7fffffff;
            }
            return 0x7f000000;
        }

        public static uint serverToClientId(uint serverId)
        {
            if (serverId != 0x7f000000)
            {
                return serverId | 0x80000000;
            }
            return 0x7f000000;
        }

        public static uint getLookingAtClientID(IntPtr processHandle)
        {
            byte[] outBytes = new byte[4];
            UIntPtr outPtr;

            uint clientInternal = getClientInternal(processHandle);

            ReadProcessMemory(processHandle, (IntPtr)(clientInternal + 0x2B4), outBytes, 4, out outPtr);
            return BitConverter.ToUInt32(outBytes, 0);
        }

        public static uint getLookingAtServerID(IntPtr processHandle)
        {
            return clientToServerId(getLookingAtClientID(processHandle));
        }

        public static void reverseLoadBar(IntPtr processHandle)
        {
            byte[] outBytes = new byte[4];
            UIntPtr outPtr;

            uint clientInternal = getClientInternal(processHandle);

            ReadProcessMemory(processHandle, (IntPtr)(clientInternal + 0x278), outBytes, 4, out outPtr);
            uint loadScreen = BitConverter.ToUInt32(outBytes, 0);

            ReadProcessMemory(processHandle, (IntPtr)(loadScreen + 0xc8), outBytes, 4, out outPtr);
            uint loadBar = BitConverter.ToUInt32(outBytes, 0);

            loadBar &= ~1u;

            writeUint(loadBar, (IntPtr)(loadScreen + 0xc8), processHandle);
        }

        public static void writeUint(uint value, IntPtr addr, IntPtr processHandle)
        {
            UIntPtr outPtr;
            byte[] data = new byte[4] { (byte)(value & 0xFF), (byte)((value >> 8) & 0xFF), (byte)((value >> 16) & 0xFF), (byte)((value >> 24) & 0xFF) };
            WriteProcessMemory(processHandle, addr, data, 4, out outPtr);
        }
    }
}

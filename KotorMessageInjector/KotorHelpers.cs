using System;

namespace KotorMessageInjector
{
    public static class KotorHelpers
    {
        private static IntPtr KOTOR_1_APPMANAGER = (IntPtr)0x007a39fc;

        public enum ClientObjectUpdateFlags : uint
        {
            POSITION            = 0b00000000000000000000000000000001,
            ORIENTATION         = 0b00000000000000000000000000000010,
            ANIMATION           = 0b00000000000000000000000000000100,
            VFX                 = 0b00000000000000000000000000001000,
            OBJECT_INTERACTION  = 0b00000000000000000000000000010000, //Specific to Doors, Placeables, and Triggers
            PORTRAIT            = 0b00000000000000000000000000100000,
            // Below are specific to Creatures
            // TODO: Fill these out


        }

        public enum GameObjectTypes : byte
        {
            OBJECT_0 = 0,
            OBJECT_1 = 1,
            OBJECT_2 = 2,
            MODULE = 3,
            AREA = 4,
            CREATURE = 5,
            ITEM = 6,
            TRIGGER = 7,
            PROJECTILE = 8,
            PLACEABLE = 9,
            DOOR = 10,
            AREAOFEFFECT = 11,
            WAYPOINT = 12,
            ENCOUNTER = 13,
            STORE = 14,
            OBJECT_f = 15,
            SOUND = 16
        }
        
        private static uint getClientInternal(IntPtr processHandle)
        {
            byte[] outBytes = new byte[4];
            UIntPtr outPtr;

            ProcessAPI.ReadProcessMemory(processHandle, KOTOR_1_APPMANAGER, outBytes, 4, out outPtr);
            uint appmanager = BitConverter.ToUInt32(outBytes, 0);

            ProcessAPI.ReadProcessMemory(processHandle, (IntPtr)(appmanager + 4), outBytes, 4, out outPtr);
            uint client = BitConverter.ToUInt32(outBytes, 0);

            ProcessAPI.ReadProcessMemory(processHandle, (IntPtr)(client + 4), outBytes, 4, out outPtr);
            return BitConverter.ToUInt32(outBytes, 0);
        }

        private static uint getServerInternal(IntPtr processHandle)
        {
            byte[] outBytes = new byte[4];
            UIntPtr outPtr;

            ProcessAPI.ReadProcessMemory(processHandle, KOTOR_1_APPMANAGER, outBytes, 4, out outPtr);
            uint appmanager = BitConverter.ToUInt32(outBytes, 0);

            ProcessAPI.ReadProcessMemory(processHandle, (IntPtr)(appmanager + 8), outBytes, 4, out outPtr);
            uint server = BitConverter.ToUInt32(outBytes, 0);

            ProcessAPI.ReadProcessMemory(processHandle, (IntPtr)(server + 4), outBytes, 4, out outPtr);
            return BitConverter.ToUInt32(outBytes, 0);
        }


        public static void setServerDebugMode(bool debugOn, IntPtr processHandle)
        {
            byte[] inBytes = new byte[4] { (byte)(debugOn ? 0x1 : 0x0), 0x0, 0x0, 0x0 };
            UIntPtr outPtr;

            uint serverInternal = getServerInternal(processHandle);

            ProcessAPI.WriteProcessMemory(processHandle, (IntPtr)(serverInternal + 0x1006c), inBytes, 4, out outPtr);
        }

        public static uint getPlayerClientID(IntPtr processHandle)
        {
            byte[] outBytes = new byte[4];
            UIntPtr outPtr;

            uint clientInternal = getClientInternal(processHandle);

            ProcessAPI.ReadProcessMemory(processHandle, (IntPtr)(clientInternal + 0x20), outBytes, 4, out outPtr);
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

            ProcessAPI.ReadProcessMemory(processHandle, (IntPtr)(clientInternal + 0x2B4), outBytes, 4, out outPtr);
            return BitConverter.ToUInt32(outBytes, 0);
        }

        public static void reverseLoadBar(IntPtr processHandle)
        {
            byte[] outBytes = new byte[4];
            UIntPtr outPtr;

            uint clientInternal = getClientInternal(processHandle);

            ProcessAPI.ReadProcessMemory(processHandle, (IntPtr)(clientInternal + 0x278), outBytes, 4, out outPtr);
            uint loadScreen = BitConverter.ToUInt32(outBytes, 0);

            ProcessAPI.ReadProcessMemory(processHandle, (IntPtr)(loadScreen + 0xc8), outBytes, 4, out outPtr);
            uint loadBar = BitConverter.ToUInt32(outBytes, 0);

            loadBar &= ~1u;

            writeUint(loadBar, (IntPtr)(loadScreen + 0xc8), processHandle);
        }

        public static void writeUint(uint value, IntPtr addr, IntPtr processHandle)
        {
            UIntPtr outPtr;
            byte[] data = new byte[4] { (byte)(value & 0xFF), (byte)((value >> 8) & 0xFF), (byte)((value >> 16) & 0xFF), (byte)((value >> 24) & 0xFF) };
            ProcessAPI.WriteProcessMemory(processHandle, addr, data, 4, out outPtr);
        }
    }
}

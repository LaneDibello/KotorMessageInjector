using System;
using static KotorMessageInjector.ProcessAPI;

namespace KotorMessageInjector
{
    public class KotorVersionNotFoundException : Exception
    {
        public KotorVersionNotFoundException(string message) : base(message) { }
    }

    public static class KotorHelpers
    {
        private const uint KOTOR_OFFSET_SERVER_DEBUG_MODE = 0x1006c;
        private const uint KOTOR_OFFSET_CLIENT_PLAYER_ID = 0x20;
        private const uint KOTOR_OFFSET_CLIENT = 0x4;
        private const uint KOTOR_OFFSET_SERVER = 0x8;
        private const uint KOTOR_OFFSET_INTERNAL = 0x4;
        private const uint KOTOR_OFFSET_LOADBAR = 0x278;
        private const uint KOTOR_OFFSET_LAST_TARGET = 0x2B4;


        private static IntPtr KOTOR_1_APPMANAGER = (IntPtr)0x007a39fc;
        private static IntPtr KOTOR_1_DEACTIVATE_RENDER_WINDOW = (IntPtr)0x00401d90; 
        private const uint KOTOR_1_GOG_MODULE_SIZE = 4640768;
        private const uint KOTOR_1_STEAM_MODULE_SIZE = 4993024;
        private const uint KOTOR_1_LOAD_DIRECTION = 0xc8;

        private static IntPtr KOTOR_2_APPMANAGER = (IntPtr)0x00a11c04;
        private static IntPtr KOTOR_2_STEAM_APPMANAGER = (IntPtr)0x00a1b4a4;
        private const uint KOTOR_2_STEAM_MODULE_SIZE = 7049216;
        private const uint KOTOR_2_GOG_MODULE_SIZE = 7012352;
        private const uint KOTOR_2_LOAD_DIRECTION = 0xd0;

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
        
        public static IntPtr getGameAppmanager(int gameVersion, bool isSteam)
        {
            if (gameVersion == 1)
            {
                return KOTOR_1_APPMANAGER;
            }
            else if (gameVersion == 2)
            {
                if (isSteam)
                {
                    return KOTOR_2_STEAM_APPMANAGER;
                }
                else
                {
                    return KOTOR_2_APPMANAGER;
                }
            }
            else
            {
                throw new ArgumentException($"Cannot find App Manager for kotor version: {gameVersion}");
            }
        }

        public static int getGameVersion(IntPtr processHandle, out bool isSteam)
        {
            uint moduleSize = GetModuleSize(processHandle);
            switch(moduleSize)
            {
                case KOTOR_1_GOG_MODULE_SIZE:
                    isSteam = false;
                    return 1;
                case KOTOR_1_STEAM_MODULE_SIZE:
                    isSteam = true;
                    return 1;
                case KOTOR_2_GOG_MODULE_SIZE:
                    isSteam = false;
                    return 2;
                case KOTOR_2_STEAM_MODULE_SIZE:
                    isSteam = true;
                    return 2;
                default:
                    throw new KotorVersionNotFoundException($"Could not find kotor version with module size: {moduleSize}");
            }
        }
        
        public static void disableClickOutPausing(IntPtr processHandle)
        {
            UIntPtr outPtr;
            WriteProcessMemory(processHandle, KOTOR_1_DEACTIVATE_RENDER_WINDOW, new byte[] { 0xc3 }, 1, out outPtr);
        }
        
        public static uint getClientInternal(IntPtr processHandle)
        {
            byte[] outBytes = new byte[4];
            UIntPtr outPtr;
            bool isSteam;
            int gameVersion = getGameVersion(processHandle, out isSteam);

            ReadProcessMemory(processHandle, getGameAppmanager(gameVersion, isSteam), outBytes, 4, out outPtr);
            uint appmanager = BitConverter.ToUInt32(outBytes, 0);

            ReadProcessMemory(processHandle, (IntPtr)(appmanager + KOTOR_OFFSET_CLIENT), outBytes, 4, out outPtr);
            uint client = BitConverter.ToUInt32(outBytes, 0);

            ReadProcessMemory(processHandle, (IntPtr)(client + KOTOR_OFFSET_INTERNAL), outBytes, 4, out outPtr);
            return BitConverter.ToUInt32(outBytes, 0);
        }

        public static uint getServerInternal(IntPtr processHandle)
        {
            byte[] outBytes = new byte[4];
            UIntPtr outPtr;
            bool isSteam;
            int gameVersion = getGameVersion(processHandle, out isSteam);

            ReadProcessMemory(processHandle, getGameAppmanager(gameVersion, isSteam), outBytes, 4, out outPtr);
            uint appmanager = BitConverter.ToUInt32(outBytes, 0);

            ReadProcessMemory(processHandle, (IntPtr)(appmanager + KOTOR_OFFSET_SERVER), outBytes, 4, out outPtr);
            uint server = BitConverter.ToUInt32(outBytes, 0);

            ReadProcessMemory(processHandle, (IntPtr)(server + KOTOR_OFFSET_INTERNAL), outBytes, 4, out outPtr);
            return BitConverter.ToUInt32(outBytes, 0);
        }

        public static uint getServerPartyTable(IntPtr processHandle)
        {
            byte[] outBytes = new byte[4];
            UIntPtr outPtr;

            // TODO: convert offset to constant
            return getServerInternal(processHandle) + 0x1b770;
        }

        public static void setNPCAvail(IntPtr processHandle, int id)
        {
            byte[] inBytes = new byte[4] { 0x1, 0x0, 0x0, 0x0};
            UIntPtr outPtr;

            uint partyTable = getServerPartyTable(processHandle);

            // TODO: convert offset to constant
            WriteProcessMemory(processHandle, (IntPtr)(partyTable + 0x30 + (4 * id)), inBytes, 4, out outPtr);
        }

        public static void setServerDebugMode(bool debugOn, IntPtr processHandle)
        {
            byte[] inBytes = new byte[4] { (byte)(debugOn ? 0x1 : 0x0), 0x0, 0x0, 0x0 };
            UIntPtr outPtr;

            uint serverInternal = getServerInternal(processHandle);

            WriteProcessMemory(processHandle, (IntPtr)(serverInternal + KOTOR_OFFSET_SERVER_DEBUG_MODE), inBytes, 4, out outPtr);
        }

        public static uint getPlayerClientID(IntPtr processHandle)
        {
            byte[] outBytes = new byte[4];
            UIntPtr outPtr;

            uint clientInternal = getClientInternal(processHandle);

            ReadProcessMemory(processHandle, (IntPtr)(clientInternal + KOTOR_OFFSET_CLIENT_PLAYER_ID), outBytes, 4, out outPtr);
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

            ReadProcessMemory(processHandle, (IntPtr)(clientInternal + KOTOR_OFFSET_LAST_TARGET), outBytes, 4, out outPtr);
            return BitConverter.ToUInt32(outBytes, 0);
        }

        public static uint getLookingAtServerID(IntPtr processHandle)
        {
            return clientToServerId(getLookingAtClientID(processHandle));
        }

        public static uint getClientModule(IntPtr processHandle)
        {
            byte[] outBytes = new byte[4];
            UIntPtr outPtr;

            uint clientInternal = getClientInternal(processHandle);

            // TODO: convert offset to constant
            ReadProcessMemory(processHandle, (IntPtr)(clientInternal + 0x18), outBytes, 4, out outPtr);
            return BitConverter.ToUInt32(outBytes, 0);
        }

        public static uint getClientArea(IntPtr processHandle)
        {
            byte[] outBytes = new byte[4];
            UIntPtr outPtr;

            uint module = getClientModule(processHandle);

            // TODO: convert offset to constant
            ReadProcessMemory(processHandle, (IntPtr)(module + 0x48), outBytes, 4, out outPtr);
            return BitConverter.ToUInt32(outBytes, 0);
        }

        public static void reverseLoadBar(IntPtr processHandle)
        {
            byte[] outBytes = new byte[4];
            UIntPtr outPtr;
            bool isSteam;
            int gameVersion = getGameVersion(processHandle, out isSteam);
            uint loadDirection = gameVersion == 1 ? KOTOR_1_LOAD_DIRECTION : KOTOR_2_LOAD_DIRECTION;

            uint clientInternal = getClientInternal(processHandle);

            ReadProcessMemory(processHandle, (IntPtr)(clientInternal + KOTOR_OFFSET_LOADBAR), outBytes, 4, out outPtr);
            uint loadScreen = BitConverter.ToUInt32(outBytes, 0);

            ReadProcessMemory(processHandle, (IntPtr)(loadScreen + loadDirection), outBytes, 4, out outPtr);
            uint loadBar = BitConverter.ToUInt32(outBytes, 0);

            loadBar &= ~1u;

            writeUint(loadBar, (IntPtr)(loadScreen + loadDirection), processHandle);
        }

        public static void writeUint(uint value, IntPtr addr, IntPtr processHandle)
        {
            UIntPtr outPtr;
            byte[] data = new byte[4] { (byte)(value & 0xFF), (byte)((value >> 8) & 0xFF), (byte)((value >> 16) & 0xFF), (byte)((value >> 24) & 0xFF) };
            WriteProcessMemory(processHandle, addr, data, 4, out outPtr);
        }
    }
}

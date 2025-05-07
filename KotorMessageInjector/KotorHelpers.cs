using System;
using static KotorMessageInjector.ProcessAPI;
using System.Collections.Generic;

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
        private const uint KOTOR_OFFSET_MODULE = 0x18;
        private const uint KOTOR_OFFSET_AREA = 0x48;
        private const uint KOTOR_OFFSET_FACTION_MANAGER = 0x10054;
        private const uint KOTOR_OFFSET_CLIENT_ANIM_BASE = 0x68;
        private const uint KOTOR_OFFSET_ANIM_BASE_GOB = 0xb8;

        private static IntPtr KOTOR_1_APPMANAGER = (IntPtr)0x007a39fc;
        private static IntPtr KOTOR_1_DEACTIVATE_RENDER_WINDOW = (IntPtr)0x00401d90;
        private const uint KOTOR_1_GOG_MODULE_SIZE = 4640768;
        private const uint KOTOR_1_STEAM_MODULE_SIZE = 4993024;
        private const uint KOTOR_1_LOAD_DIRECTION = 0xc8;
        private const uint KOTOR_1_OFFSET_PARTY_TABLE = 0x1b770;
        private const uint KOTOR_1_OFFSET_CHEAT_USED = 0x194;
        private const uint KOTOR_1_OFFSET_SCENE = 0x184;
        private const uint KOTOR_1_OFFSET_CREATURE_STATS = 0xa74;
        private const uint KOTOR_1_OFFSET_CREATURE_STATS_RUNRATE = 0x198;

        private static IntPtr KOTOR_2_APPMANAGER = (IntPtr)0x00a11c04;
        private static IntPtr KOTOR_2_STEAM_APPMANAGER = (IntPtr)0x00a1b4a4;
        private const uint KOTOR_2_STEAM_MODULE_SIZE = 7049216;
        private const uint KOTOR_2_GOG_MODULE_SIZE = 7012352;
        private const uint KOTOR_2_LOAD_DIRECTION = 0xd0;
        private const uint KOTOR_2_GOG_OFFSET_PARTY_TABLE = 0x1f0b4;
        private const uint KOTOR_2_OFFSET_CHEAT_USED = 0x23c;
        private const uint KOTOR_2_OFFSET_SCENE = 0x188;
        private const uint KOTOR_2_OFFSET_CREATURE_STATS = 0x1198;
        private const uint KOTOR_2_OFFSET_CREATURE_STATS_RUNRATE = 0x1a8;

        [Flags]
        public enum CLIENT_OBJECT_UPDATE_FLAGS : uint
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

        public enum GAME_OBJECT_TYPES : byte
        {
            OBJECT_0      = 0,
            OBJECT_1      = 1,
            OBJECT_2      = 2,
            MODULE        = 3,
            AREA          = 4,
            CREATURE      = 5,
            ITEM          = 6,
            TRIGGER       = 7,
            PROJECTILE    = 8,
            PLACEABLE     = 9,
            DOOR          = 10,
            AREAOFEFFECT  = 11,
            WAYPOINT      = 12,
            ENCOUNTER     = 13,
            STORE         = 14,
            OBJECT_f      = 15,
            SOUND         = 16,
        }

        public static IntPtr getRunningKotor()
        {
            IntPtr pHandle = OpenProcessByName("swkotor.exe");

            if (pHandle != (IntPtr)0)
            {
                return pHandle;
            }

            pHandle = OpenProcessByName("swkotor2.exe");

            if (pHandle != (IntPtr)0)
            {
                return pHandle;
            }

            throw new KotorVersionNotFoundException($"Could not find a running instance of kotor");
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

        public static Dictionary<Function, uint> getFuncLibrary(IntPtr processHandle)
        {
            var version = getGameVersion(processHandle, out bool isSteam);
            return version == 1
                ? RemoteFunctionLibrary.k1Functions
                : version == 2
                    ? isSteam
                        ? RemoteFunctionLibrary.k2SteamFunctions
                        : RemoteFunctionLibrary.k2Functions
                    : throw new ArgumentException($"Cannot find App Manager for kotor version: {version}");
        }

        public static int getGameVersion(IntPtr processHandle, out bool isSteam)
        {
            uint moduleSize = GetModuleSize(processHandle);
            switch (moduleSize)
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

        public static int getGameVersion(IntPtr processHandle)
        {
            bool isSteam;
            return getGameVersion(processHandle, out isSteam);
        }

        public static uint getCurrentScene(IntPtr processHandle)
        {
            byte[] outBytes = new byte[4];
            UIntPtr outPtr;

            bool isSteam;
            int version = getGameVersion(processHandle, out isSteam);

            uint area = getClientArea(processHandle);

            ReadProcessMemory(processHandle, (IntPtr)(area + (version == 1 ? KOTOR_1_OFFSET_SCENE : KOTOR_2_OFFSET_SCENE)), outBytes, 4, out outPtr);
            return BitConverter.ToUInt32(outBytes, 0);
        }

        public static void disableClickOutPausing(IntPtr processHandle)
        {
            UIntPtr outPtr;
            WriteProcessMemory(processHandle, KOTOR_1_DEACTIVATE_RENDER_WINDOW, new byte[] { 0xc3 }, 1, out outPtr);
        }

        public static uint getClient(IntPtr processHandle)
        {
            byte[] outBytes = new byte[4];
            UIntPtr outPtr;
            bool isSteam;
            int gameVersion = getGameVersion(processHandle, out isSteam);

            ReadProcessMemory(processHandle, getGameAppmanager(gameVersion, isSteam), outBytes, 4, out outPtr);
            uint appmanager = BitConverter.ToUInt32(outBytes, 0);

            ReadProcessMemory(processHandle, (IntPtr)(appmanager + KOTOR_OFFSET_CLIENT), outBytes, 4, out outPtr);
            return BitConverter.ToUInt32(outBytes, 0);
        }


        public static uint getClientInternal(IntPtr processHandle)
        {
            byte[] outBytes = new byte[4];
            UIntPtr outPtr;
            bool isSteam;
            int gameVersion = getGameVersion(processHandle, out isSteam);

            uint client = getClient(processHandle);

            ReadProcessMemory(processHandle, (IntPtr)(client + KOTOR_OFFSET_INTERNAL), outBytes, 4, out outPtr);
            return BitConverter.ToUInt32(outBytes, 0);
        }

        public static uint getServer(IntPtr processHandle)
        {
            byte[] outBytes = new byte[4];
            UIntPtr outPtr;
            bool isSteam;
            int gameVersion = getGameVersion(processHandle, out isSteam);

            ReadProcessMemory(processHandle, getGameAppmanager(gameVersion, isSteam), outBytes, 4, out outPtr);
            uint appmanager = BitConverter.ToUInt32(outBytes, 0);

            ReadProcessMemory(processHandle, (IntPtr)(appmanager + KOTOR_OFFSET_SERVER), outBytes, 4, out outPtr);
            return BitConverter.ToUInt32(outBytes, 0);
        }

        public static uint getServerInternal(IntPtr processHandle)
        {
            byte[] outBytes = new byte[4];
            UIntPtr outPtr;
            bool isSteam;
            int gameVersion = getGameVersion(processHandle, out isSteam);

            uint server = getServer(processHandle);

            ReadProcessMemory(processHandle, (IntPtr)(server + KOTOR_OFFSET_INTERNAL), outBytes, 4, out outPtr);
            return BitConverter.ToUInt32(outBytes, 0);
        }

        public static uint getServerPartyTable(IntPtr processHandle)
        {
            byte[] outBytes = new byte[4];
            bool isSteam;
            int version = getGameVersion(processHandle, out isSteam);

            if (version == 1)
            {
                return getServerInternal(processHandle) + KOTOR_1_OFFSET_PARTY_TABLE;
            }
            else
            {
                return getServerInternal(processHandle) + KOTOR_2_GOG_OFFSET_PARTY_TABLE;
            }
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

        public static uint getFactionManager(IntPtr processHandle)
        {
            byte[] outBytes = new byte[4];
            UIntPtr outPtr;

            uint serverInternal = getServerInternal(processHandle);

            ReadProcessMemory(processHandle, (IntPtr)(serverInternal + KOTOR_OFFSET_FACTION_MANAGER), outBytes, 4, out outPtr);
            return BitConverter.ToUInt32(outBytes, 0);
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

            ReadProcessMemory(processHandle, (IntPtr)(clientInternal + KOTOR_OFFSET_MODULE), outBytes, 4, out outPtr);
            return BitConverter.ToUInt32(outBytes, 0);
        }

        public static uint getClientArea(IntPtr processHandle)
        {
            byte[] outBytes = new byte[4];
            UIntPtr outPtr;

            uint module = getClientModule(processHandle);

            ReadProcessMemory(processHandle, (IntPtr)(module + KOTOR_OFFSET_AREA), outBytes, 4, out outPtr);
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

        public static (float, float, float, float) readQuaternion(IntPtr processHandle, IntPtr addr)
        {
            byte[] outBytes = new byte[16];
            ReadProcessMemory(processHandle, addr, outBytes, 16, out _);

            return
            (
                BitConverter.ToSingle(outBytes, 0),
                BitConverter.ToSingle(outBytes, 4),
                BitConverter.ToSingle(outBytes, 8),
                BitConverter.ToSingle(outBytes, 12)
            );
        }

        public static uint getClientObjectGob(IntPtr processHandle, uint clientObject)
        {
            byte[] outBytes = new byte[4];

            ReadProcessMemory(processHandle, (IntPtr)(clientObject + KOTOR_OFFSET_CLIENT_ANIM_BASE), outBytes, 4, out _);
            uint animBase = BitConverter.ToUInt32(outBytes, 0);

            ReadProcessMemory(processHandle, (IntPtr)(animBase + KOTOR_OFFSET_ANIM_BASE_GOB), outBytes, 4, out _);
            return BitConverter.ToUInt32(outBytes, 0);
        }

        public static uint getCreatureStats(IntPtr processHandle, uint serverCreature)
        {
            byte[] outBytes = new byte[4];

            int version = getGameVersion(processHandle);
            uint offset = version == 1 ? KOTOR_1_OFFSET_CREATURE_STATS : KOTOR_2_OFFSET_CREATURE_STATS;

            ReadProcessMemory(processHandle, (IntPtr)(serverCreature + offset), outBytes, 4, out _);
            return BitConverter.ToUInt32(outBytes, 0);
        }

        // Default runrate for PCs is 5.4
        public static void setRunrate(IntPtr processHandle, uint serverCreature, float runrate)
        {
            byte[] inBytes = BitConverter.GetBytes(runrate);

            int version = getGameVersion(processHandle);
            uint offset = version == 1 ? KOTOR_1_OFFSET_CREATURE_STATS_RUNRATE : KOTOR_2_OFFSET_CREATURE_STATS_RUNRATE;

            uint creatureStats = getCreatureStats(processHandle, serverCreature);

            WriteProcessMemory(processHandle, (IntPtr)(creatureStats + offset), inBytes, 4, out _);
        }

        public static void setCheatUsed(IntPtr processHandle, bool cheatUsed)
        {
            byte[] inBytes = BitConverter.GetBytes(cheatUsed ? 1 : 0);

            int version = getGameVersion(processHandle);
            uint offset = version == 1 ? KOTOR_1_OFFSET_CHEAT_USED : KOTOR_2_OFFSET_CHEAT_USED;

            var partyTable = getServerPartyTable(processHandle);

            WriteProcessMemory(processHandle, (IntPtr)(partyTable + offset), inBytes, 4, out _);
        }
    }
}

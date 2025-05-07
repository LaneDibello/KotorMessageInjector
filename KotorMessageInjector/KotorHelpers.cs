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

        public enum ATTRIBUTES
        {
            STR,
            DEX,
            CON,
            INT,
            WIS,
            CHA
        }

        public enum SKILLS : byte
        {
            COMPUTER_USE = 0,
            DEMOLITIONS = 1,
            STEALTH = 2,
            AWARNESS = 3,
            PERSUADE = 4,
            REPAIR = 5,
            SECURITY = 6,
            TREAT_INJURY = 7,
        }

        public enum FEATS : ushort
        {
            UNDEFINED = 0,
            ADVANCED_JEDI_DEFENSE = 1,
            XXXX_ADVANCED_GUARD_STANCE = 2,
            TWO_WEAPON_FIGHTING = 3,
            ARMOUR_PROF_HEAVY = 4,
            ARMOUR_PROF_LIGHT = 5,
            ARMOUR_PROF_MEDIUM = 6,
            CAUTIOUS = 7,
            CRITICAL_STRIKE = 8,
            TWO_WEAPON_ADVANCED = 9,
            EMPATHY = 10,
            FLURRY = 11,
            GEAR_HEAD = 12,
            CONDITIONING = 13,
            IMPLANT_LEVEL_1 = 14,
            IMPLANT_LEVEL_2 = 15,
            IMPLANT_LEVEL_3 = 16,
            IMPROVED_POWER_ATTACK = 17,
            IMPROVED_POWER_BLAST = 18,
            IMPROVED_CRITICAL_STRIKE = 19,
            IMPROVED_SNIPER_SHOT = 20,
            IMPROVED_CONDITIONING = 21,
            MASTER_CONDITIONING = 22,
            MASTER_JEDI_DEFENSE = 24,
            XXXX_MASTER_GUARD_STANCE = 25,
            MULTI_SHOT = 26,
            XXXX_PERCEPTIVE = 27,
            POWER_ATTACK = 28,
            POWER_BLAST = 29,
            RAPID_SHOT = 30,
            SNIPER_SHOT = 31,
            WEAPON_FOCUS_BLASTER = 32,
            WEAPON_FOCUS_BLASTER_RIFLE = 33,
            XXXX_WEAPON_FOCUS_GRENADE = 34,
            WEAPON_FOCUS_HEAVY_WEAPONS = 35,
            WEAPON_FOCUS_LIGHTSABER = 36,
            WEAPON_FOCUS_MELEE_WEAPONS = 37,
            XXXX_WEAPON_FOCUS_SIMPLE_WEAPONS = 38,
            WEAPON_PROF_BLASTER = 39,
            WEAPON_PROF_BLASTER_RIFLE = 40,
            XXXX_WEAPON_PROF_GRENADE = 41,
            WEAPON_PROF_HEAVY_WEAPONS = 42,
            WEAPON_PROF_LIGHTSABER = 43,
            WEAPON_PROF_MELEE_WEAPONS = 44,
            XXXX_WEAPON_PROF_SIMPLE_WEAPONS = 45,
            WEAPON_SPEC_BLASTER = 46,
            WEAPON_SPEC_BLASTER_RIFLE = 47,
            XXXX_WEAPON_SPEC_GRENADE = 48,
            WEAPON_SPEC_HEAVY_WEAPONS = 49,
            WEAPON_SPEC_LIGHTSABER = 50,
            WEAPON_SPEC_MELEE_WEAPONS = 51,
            XXXX_WEAPON_SPEC_SIMPLE_WEAPONS = 52,
            WHIRLWIND_ATTACK = 53,
            XXXX_GUARD_STANCE = 54,
            JEDI_DEFENSE = 55,
            UNCANNY_DODGE_1 = 56,
            UNCANNY_DODGE_2 = 57,
            XXXX_SKILL_FOCUS_COMPUTER_USE = 58,
            SNEAK_ATTACK_1D6 = 60,
            SNEAK_ATTACK_2D6 = 61,
            SNEAK_ATTACK_3D6 = 62,
            SNEAK_ATTACK_4D6 = 63,
            SNEAK_ATTACK_5D6 = 64,
            SNEAK_ATTACK_6D6 = 65,
            SNEAK_ATTACK_7D6 = 66,
            SNEAK_ATTACK_8D6 = 67,
            SNEAK_ATTACK_9D6 = 68,
            SNEAK_ATTACK_10D6 = 69,
            XXXX_SKILL_FOCUS_DEMOLITIONS = 70,
            XXXX_SKILL_FOCUS_STEALTH = 71,
            XXXX_SKILL_FOCUS_AWARENESS = 72,
            XXXX_SKILL_FOCUS_PERSUADE = 73,
            XXXX_SKILL_FOCUS_REPAIR = 74,
            XXXX_SKILL_FOCUS_SECURITY = 75,
            XXXX_SKILL_FOCUS_TREAT_INJURY = 76,
            MASTER_SNIPER_SHOT = 77,
            DROID_UPGRADE_1 = 78,
            DROID_UPGRADE_2 = 79,
            DROID_UPGRADE_3 = 80,
            MASTER_CRITICAL_STRIKE = 81,
            MASTER_POWER_BLAST = 82,
            MASTER_POWER_ATTACK = 83,
            TOUGHNESS = 84,
            TWO_WEAPON_MASTERY = 85,
            XXXX_FORCE_FOCUS_ALTER = 86,
            XXXX_FORCE_FOCUS_CONTROL = 87,
            FORCE_FOCUS = 88,
            FORCE_FOCUS_ADVANCED = 89,
            FORCE_FOCUS_MASTERY = 90,
            IMPROVED_FLURRY = 91,
            IMPROVED_RAPID_SHOT = 92,
            PROFICIENCY_ALL = 93,
            BATTLE_MEDITATION = 94,
            WOOKIE_ENDURANCE = 95,
            BLASTER_INTEGRATION = 96,
            FORCE_CAMOFLAGE = 97,
            FORCE_IMMUNITY_FEAR = 98,
            FORCE_IMMUNITY_STUN = 99,
            FORCE_IMMUNITY_PARALYSIS = 100,
            FORCE_JUMP = 101,
            FORCE_JUMP_ADVANCED = 102,
            FORCE_JUMP_MASTERY = 103,
            SCOUNDRELS_LUCK = 104,
            IMPROVED_SCOUNDRELS_LUCK = 105,
            MASTER_SCOUNDRELS_LUCK = 106,
            JEDI_SENSE = 107,
            KNIGHT_SENSE = 108,
            MASTER_SENSE = 109,
            LOGIC_UPGRADE_COMBAT = 110,
            LOGIC_UPGRADE_TACTICIAN = 111,
            LOGIC_UPGRADE_BATTLE_DROID = 112,
            DUELING = 113,
            ADVANCED_DUELING = 114,
            MASTER_DUELING = 115,
            FORCE_SENSITIVE = 116,
            IMPROVED_CAUTION = 117,
            MASTER_CAUTION = 118,
            GEAR_HEAD_ADEPT = 119,
            GEAR_HEAD_MASTER = 120,
            IMPROVED_EMPATHY = 121,
            MASTER_EMPATHY = 122,
            IMPROVED_TOUGHNESS = 123,
            MASTER_TOUGHNESS = 124,
            //KOTOR 2 EXCLUSIVE FEATS
            EVASION = 125,
            TARGETING_1 = 126,
            TARGETING_2 = 127,
            TARGETING_3 = 128,
            TARGETING_4 = 129,
            TARGETING_5 = 130,
            TARGETING_6 = 131,
            TARGETING_7 = 132,
            TARGETING_8 = 133,
            XXXX_TARGETING_9 = 134,
            XXXX_TARGETING_10 = 135,
            XXXPRECISE_SHOT_I = 136,
            XXXPRECISE_SHOT_II = 137,
            XXXPRECISE_SHOT_III = 138,
            CLOSE_COMBAT = 139,
            IMPROVED_CLOSE_COMBAT = 140,
            XXXIMPROVED_FORCE_CAMOUFLAGE = 141,
            XXXMASTER_FORCE_CAMOUFLAGE = 142,
            REGENERATE_FORCE_POINTS = 143,
            XXXX_CRUSH_OPPOSITION_I = 144,
            XXXX_CRUSH_OPPOSITION_II = 145,
            XXXX_CRUSH_OPPOSITION_III = 146,
            XXXX_CRUSH_OPPOSITION_IV = 147,
            XXXX_CRUSH_OPPOSITION_V = 148,
            DARK_SIDE_CORRUPTION = 149,
            IGNORE_PAIN_I = 150,
            IGNORE_PAIN_II = 151,
            IGNORE_PAIN_III = 152,
            INCREASE_COMBAT_DAMAGE_I = 153,
            INCREASE_COMBAT_DAMAGE_II = 154,
            INCREASE_COMBAT_DAMAGE_III = 155,
            SUPER_WEAPON_FOCUS_LIGHTSABER_I = 156,
            SUPER_WEAPON_FOCUS_LIGHTSABER_II = 157,
            SUPER_WEAPON_FOCUS_LIGHTSABER_III = 158,
            SUPER_WEAPON_FOCUS_2_WEAPON_I = 159,
            SUPER_WEAPON_FOCUS_2_WEAPON_II = 160,
            SUPER_WEAPON_FOCUS_2_WEAPON_III = 161,
            XXXX_INSPIRE_FOLLOWERS_I = 162,
            XXXX_INSPIRE_FOLLOWERS_II = 163,
            XXXX_INSPIRE_FOLLOWERS_III = 164,
            XXXX_INSPIRE_FOLLOWERS_IV = 165,
            XXXX_INSPIRE_FOLLOWERS_V = 166,
            LIGHT_SIDE_ENLIGHTENMENT = 167,
            DEFLECT = 168,
            INNER_STRENGTH_I = 169,
            INNER_STRENGTH_II = 170,
            INNER_STRENGTH_III = 171,
            INCREASE_MELEE_DAMAGE_I = 172,
            INCREASE_MELEE_DAMAGE_II = 173,
            INCREASE_MELEE_DAMAGE_III = 174,
            CRAFT = 175,
            MASTERCRAFT_WEAPONS_I = 176,
            MASTERCRAFT_WEAPONS_II = 177,
            MASTERCRAFT_WEAPONS_III = 178,
            MASTERCRAFT_ARMOR_I = 179,
            MASTERCRAFT_ARMOR_II = 180,
            MASTERCRAFT_ARMOR_III = 181,
            DROID_INTERFACE = 182,
            CLASS_SKILL_AWARENESS = 183,
            CLASS_SKILL_COMPUTER_USE = 184,
            CLASS_SKILL_DEMOLITIONS = 185,
            CLASS_SKILL_REPAIR = 186,
            CLASS_SKILL_SECURITY = 187,
            CLASS_SKILL_STEALTH = 188,
            CLASS_SKILL_TREAT_INJURY = 189,
            DUAL_STRIKE = 190,
            IMPROVED_DUAL_STRIKE = 191,
            MASTER_DUAL_STRIKE = 192,
            FINESSE_LIGHTSABERS = 193,
            FINESSE_MELEE_WEAPONS = 194,
            XXXX_MOBILITY = 195,
            REGENERATE_VITALITY_POINTS = 196,
            STEALTH_RUN = 197,
            KINETIC_COMBAT = 198,
            SURVIVAL = 199,
            MANDALORIAN_COURAGE = 200,
            PERSONAL_CLOAKING_SHIELD = 201,
            MENTOR = 202,
            IMPLANT_SWITCHING = 203,
            SPIRIT = 204,
            FORCE_CHAIN = 205,
            WAR_VETERAN = 206,
            COMPLEX_UNARMED_ANIMS = 207,
            WEAPON_PROF_WRIST_MOUNTED = 208,
            ECHANI_STRIKE_I = 209,
            ECHANI_STRIKE_II = 210,
            ECHANI_STRIKE_III = 211,
            UNARMED_SPECIALIST_I = 212,
            UNARMED_SPECIALIST_II = 213,
            UNARMED_SPECIALIST_III = 214,
            UNARMED_SPECIALIST_IV = 215,
            UNARMED_SPECIALIST_V = 216,
            UNARMED_SPECIALIST_VI = 217,
            UNARMED_SPECIALIST_VII = 218,
            UNARMED_SPECIALIST_VIII = 219,
            SHIELD_BREAKER = 220,
            REPULSOR_STRIKE = 221,
            ELECTRICAL_STRIKE = 222,
            GRAVITONIC_STRIKE = 223,
            WOOKIEE_TOUGHNESS_II = 224,
            WOOKIEE_TOUGHNESS_III = 225,
            XXXPRECISE_SHOT_IV = 226,
            XXXPRECISE_SHOT_V = 227,
            ASSASSIN_PROTOCOL_I = 228,
            ASSASSIN_PROTOCOL_II = 229,
            ASSASSIN_PROTOCOL_III = 230,
            WOOKIEE_RAGE_I = 231,
            WOOKIEE_RAGE_II = 232,
            WOOKIEE_RAGE_III = 233,
            DROID_TRICK = 234,
            DROID_CONFUSION = 235,
            FIGHTING_SPIRIT = 236,
            HEROIC_RESOLVE = 237,
            MINE_IMMUNITY = 238,
            POINT_GUARD = 239,
            PRECISE_SHOT_I = 240,
            PRECISE_SHOT_II = 241,
            PRECISE_SHOT_III = 242,
            PRECISE_SHOT_IV = 243,
            PRECISE_SHOT_V = 244,

        }

        public enum CLASSES : byte
        {
            SOLDIER = 0,
            SCOUT = 1,
            SCOUNDREL = 2,
            JEDI_GUARDIAN = 3,
            JEDI_CONSULAR = 4,
            JEDI_SENTINEL = 5,
            COMBAT_DROID = 6,
            EXPERT_DROID = 7,
            MINION = 8,
            // KotOR 2 Exclusive Classes
            TECH_SPECIALIST = 9,
            BOUNTY_HUNTER = 10,
            JEDI_WEAPONMASTER = 11,
            JEDI_MASTER = 12,
            JEDI_WATCHMAN = 13,
            SITH_MARAUDER = 14,
            SITH_LORD = 15,
            SITH_ASSASSIN = 16
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

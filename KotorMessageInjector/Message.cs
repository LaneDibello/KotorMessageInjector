using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KotorMessageInjector
{
    #region exceptions
    class MessageNotInitializedException : Exception
    {
        public MessageNotInitializedException(string message) : base(message) { }
    }

    class MessageOfWrongSourceException : Exception
    {
        public MessageOfWrongSourceException(string message) : base(message) { }
    }
    #endregion

    public class Message
    {
        #region enums
        public enum MessageSources : byte
        {
            PLAYER_TO_SERVER = (byte)'p',
            SYSADMIN_TO_SERVER = (byte)'s',
            SERVER_TO_PLAYER = (byte)'P',
            SERVER_TO_SYSADMIN = (byte)'S'
        }

        public enum PlayerMessageTypes : byte
        {
            UNKNOWN_0 = 0,
            SERVER_STATUS = 1,
            LOGIN = 2,
            MODULE = 3,
            AREA = 4,
            GAME_OBJ_UPDATE = 5,
            INPUT = 6,
            UNKNOWN_7 = 7,
            GOLD = 8,
            CHAT = 9,
            PLAYER_LIST = 10,
            CHAT_DISABLED = 11,
            INVENTORY = 12,
            GUI_INVENTORY = 13,
            PARTY = 14,
            CHEAT = 15,
            CAMERA = 16,
            CHAR_LIST = 17,
            CLIENT_SIDE_MESSAGE = 18,
            COMBAT_ROUND = 19,
            DIALOG = 20,
            GUI_CHARACTER_SHEET = 21,
            QUICK_CHAT = 22,
            SOUND = 23,
            ITEM_PROPERTY = 24,
            GUI_CONTAINER = 25,
            VOICE_CHAT = 26,
            GUI_INFO_POPUP = 27,
            JOURNAL = 28,
            LEVEL_UP = 29,
            GUI_QUICKBAR = 30,
            UNKNOWN_1F = 31,
            MAP_PIN = 32,
            DEBUG_INFO = 33,
            SAFE_PROJECTILE = 34,
            UNKNOWN_23 = 35,
            POP_UP_GUI_PANEL = 36,
            DEATH = 37,
            UNKNOWN_26 = 38,
            UNKNOWN_27 = 39,
            AMBIENT = 40,
            UNKNOWN_29 = 41,
            UNKNOWN_2A = 42,
            CHARACTER_DOWNLOAD = 43,
            LOAD_BAR = 44,
            SAVE_LOAD = 45,
            UNKNOWN_2e = 46,
            SHUT_DOWN_SERVER = 47,
            UNKNOWN_30 = 48,
            PLAY_MODULE_CHARACTER_LIST = 49,
            CUSTOM_TOKEN = 50
        }

        #endregion

        private List<byte> raw;

        #region constructors
        public Message()
        {
            raw = new List<byte>() { 0, 0, 0 };
        }

        public Message(MessageSources source)
        {
            raw = new List<byte>() { (byte)source, 0, 0 };
        }

        public Message(PlayerMessageTypes type, byte subtype, bool playerToServer = true)
        {
            raw = new List<byte>() { 
                (byte)(playerToServer ? MessageSources.PLAYER_TO_SERVER : MessageSources.SERVER_TO_PLAYER), 
                (byte)type, 
                subtype 
            };
        }
        #endregion

        public override string ToString()
        {
            if (raw.Count < 3) return "Unititialized Message";

            string header = $"Source:{source} | Type:{raw[1]} | Subtype:{raw[2]}\n";
            string line = "----------------------------------------------------\n";

            StringBuilder hex = new StringBuilder(raw.Count * 2);
            foreach (byte b in raw.ToArray())
                hex.AppendFormat("{0:x2}", b);

            return header + line + hex + "\n";
        }

        #region properties
        public MessageSources source
        {
            get
            {
                if (raw.Count < 1 || raw[0] == 0)
                {
                    throw new MessageNotInitializedException($"Could Not Find Message Source, raw message length {raw.Count}");
                }
                return (MessageSources)raw[0];
            }
            set
            {
                raw[0] = (byte)value;
            }
        }

        public PlayerMessageTypes typePlayer
        {
            get
            {
                if (source != MessageSources.PLAYER_TO_SERVER && source != MessageSources.SERVER_TO_PLAYER)
                {
                    throw new MessageOfWrongSourceException($"Player Message Types are not valid for source {source}");
                }
                if (raw.Count < 2)
                {
                    throw new MessageNotInitializedException($"Could Not Find Message Type, raw message length {raw.Count}");
                }
                return (PlayerMessageTypes)raw[1];
            }
            set
            {
                if (source != MessageSources.PLAYER_TO_SERVER && source != MessageSources.SERVER_TO_PLAYER)
                {
                    throw new MessageOfWrongSourceException($"Player Message Types are not valid for source {source}");
                }
                raw[1] = (byte)value;
            }
        }

        public byte typeSysAdmin
        {
            get
            {
                if (raw.Count < 2)
                {
                    throw new MessageNotInitializedException($"Could Not Find Message Type, raw message length {raw.Count}");
                }
                return raw[1];
            }
            set
            {
                raw[1] = value;
            }
        }

        public byte subtype
        {
            get
            {
                if (raw.Count < 3)
                {
                    throw new MessageNotInitializedException($"Could Not Find Message Subtype, raw message length {raw.Count}");
                }
                return raw[2];
            }
            set
            {
                raw[2] = value;
            }
        }
        public byte[] message 
        { 
            get
            {
                return raw.ToArray();
            }
        }

        public uint length
        {
            get
            {
                return (uint)raw.Count;
            }
        }

        #endregion

        #region message writing
        public void writeByte(byte value)
        {
            raw.Add(value);
        }

        public void writeUint(uint value)
        {
            raw.Add((byte)(value & 0xFF));                 
            raw.Add((byte)((value >> 8) & 0xFF));          
            raw.Add((byte)((value >> 16) & 0xFF));         
            raw.Add((byte)((value >> 24) & 0xFF));         
        }

        public void writeInt(int value)
        {
            raw.Add((byte)(value & 0xFF));        
            raw.Add((byte)((value >> 8) & 0xFF)); 
            raw.Add((byte)((value >> 16) & 0xFF));
            raw.Add((byte)((value >> 24) & 0xFF));
        }

        public void writeUshort(ushort value)
        {
            raw.Add((byte)(value & 0xFF));       
            raw.Add((byte)((value >> 8) & 0xFF));
        }

        public void writeShort(short value)
        {
            raw.Add((byte)(value & 0xFF));       
            raw.Add((byte)((value >> 8) & 0xFF));
        }

        public void writeFloat(float value)
        {
            // Convert float to its binary representation
            byte[] bytes = BitConverter.GetBytes(value);

            // Check if system is little-endian
            if (BitConverter.IsLittleEndian)
            {
                raw.Add(bytes[0]);
                raw.Add(bytes[1]);
                raw.Add(bytes[2]);
                raw.Add(bytes[3]);
            }
            else
            {
                raw.Add(bytes[3]);
                raw.Add(bytes[2]);
                raw.Add(bytes[1]);
                raw.Add(bytes[0]);
            }
        }

        public void writeBool(bool value)
        {
            raw.Add(value ? (byte)1 : (byte)0);
        }

        public void writeCResRef(string resref)
        {
            // 16 character resource reference
            for (int i = 0; i < 16; i++)
            {
                if (i >= resref.Length)
                {
                    raw.Add(0x0);
                }
                else
                {
                    raw.Add((byte)resref[i]);
                }
            }
        }

        // TODO: Add support for VOID* and CExoString

        #endregion
    }
}

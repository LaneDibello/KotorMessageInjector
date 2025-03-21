﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static KotorMessageInjector.KotorHelpers;

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

    class MessageBadAllocationException : Exception
    {
        public MessageBadAllocationException(string message) : base(message) { }
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

        /// <summary>
        /// Constructs a Message with `MessageSources` source
        /// </summary>
        /// <param name="source">The source of the Message</param>
        public Message(MessageSources source)
        {
            raw = new List<byte>() { (byte)source, 0, 0 };
        }

        /// <summary>
        /// Constructs a player message witht eh following type and subtype
        /// </summary>
        /// <param name="type">The type of player message to be sent</param>
        /// <param name="subtype">The subtype of this message</param>
        /// <param name="playerToServer">True if this Message source is PLAYER_TO_SERVER, false for SERVER_TO_PLAYER</param>
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
        public Message writeByte(GAME_OBJECT_TYPES value) => writeByte((byte)value);

        public Message writeByte(byte value)
        {
            raw.Add(value);
            return this;
        }

        public Message writeUint(CLIENT_OBJECT_UPDATE_FLAGS value) => writeUint((uint)value);

        public Message writeUint(uint value)
        {
            raw.Add((byte)(value & 0xFF));
            raw.Add((byte)((value >> 8) & 0xFF));
            raw.Add((byte)((value >> 16) & 0xFF));
            raw.Add((byte)((value >> 24) & 0xFF));
            return this;
        }

        public Message writeInt(int value)
        {
            raw.Add((byte)(value & 0xFF));
            raw.Add((byte)((value >> 8) & 0xFF));
            raw.Add((byte)((value >> 16) & 0xFF));
            raw.Add((byte)((value >> 24) & 0xFF));
            return this;
        }

        public Message writeUshort(ushort value)
        {
            raw.Add((byte)(value & 0xFF));
            raw.Add((byte)((value >> 8) & 0xFF));
            return this;
        }

        public Message writeShort(short value)
        {
            raw.Add((byte)(value & 0xFF));
            raw.Add((byte)((value >> 8) & 0xFF));
            return this;
        }

        public Message writeFloat(float value)
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

            return this;
        }
        /// <summary>
        /// Writes 3 floats, x, y, z to the message contents
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public Message writeVector(float x, float y, float z)
        {
            writeFloat(x);
            writeFloat(y);
            writeFloat(z);
            return this;
        }

        public Message writeBool(bool value)
        {
            writeUint(value ? (byte)1 : (byte)0);
            return this;
        }

        /// <summary>
        /// Write a Bioware Aurora Style Resource Reference to the message contents
        /// </summary>
        /// <param name="resref">A 16 character or less reference to some KotOR file/resource</param>
        public Message writeCResRef(string resref)
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
            return this;
        }

        /// <summary>
        /// Write a Bioware Aurora Style string to the message contents
        /// </summary>
        /// <param name="s">The string to be written</param>
        public Message writeCExoString(string s)
        {
            writeInt(s.Length);
            for (int i = 0; i < s.Length; i++)
            {
                writeByte((byte)s[i]);
            }
            return this;
        }

        /// <summary>
        /// Write arbitrary bytes to message contents
        /// </summary>
        /// <param name="bytes">The bytes to be written</param>
        public Message writeVoid(byte[] bytes)
        {
            foreach (byte b in bytes)
            {
                writeByte(b);
            }
            return this;
        }

        /// <summary>
        /// Write a Bioware Aurora Style custom localized string to message contents
        /// </summary>
        /// <param name="s">The custom string to be written</param>
        public Message writeCExoLocString(string s)
        {
            writeBool(false); // isStrRef
            writeCExoString(s);
            return this;
        }

        /// <summary>
        /// Write a localized string using a TLK table reference
        /// </summary>
        /// <param name="strref">A numeral reference to a particular string on the TLK table</param>
        /// <param name="language">The language to use (0 for English)</param>
        public Message writeCExoLocString(uint strref, byte language = 0)
        {
            writeBool(true); // isStrRef
            writeByte(language); // Language ID
            writeUint(strref);
            return this;
        }

        #endregion
    }
}

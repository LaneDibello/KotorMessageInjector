using KotorMessageInjector;
using static KotorMessageInjector.KotorHelpers;
using static KotorMessageInjector.Message;


namespace testApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            IntPtr pHandle = ProcessAPI.OpenProcessByName("swkotor.exe");

            Injector i = new Injector(pHandle);

            uint playerServerId = getPlayerServerID(pHandle);
            uint playerClientId = getPlayerClientID(pHandle);
            uint lookingAtServerId = getLookingAtServerID(pHandle);
            uint lookingAtClientId = getLookingAtClientID(pHandle);

            disableClickOutPausing(pHandle);

            Message msg;

            ////Teleporting
            //msg = new Message(PlayerMessageTypes.GAME_OBJ_UPDATE, 1, false);
            //msg.writeByte(0x55);
            //msg.writeByte(GAME_OBJECT_TYPES.CREATURE);
            //msg.writeUint(playerClientId);
            //msg.writeUint(CLIENT_OBJECT_UPDATE_FLAGS.POSITION);
            //msg.writeFloat(180f);
            //msg.writeFloat(100f);
            //msg.writeFloat(0.0f);

            //// Swap/Recruit Creatures
            //msg = new Message(PlayerMessageTypes.GAME_OBJ_UPDATE, 2, false);
            //msg.writeUint(lookingAtClientId);

            ////Heal
            //msg = new Message(PlayerMessageTypes.CHEAT, 2);

            ////Run Scripts
            //msg = new Message(PlayerMessageTypes.CHEAT, 8);
            //setServerDebugMode(true, pHandle);
            //msg.writeCExoString("k_trg_transfail1");

            //// Take Control/Add to party
            //msg = new Message(PlayerMessageTypes.GAME_OBJ_UPDATE, 2, false);
            //msg.writeUint(lookingAtClientId);

            //// Update door properties
            //msg = new Message(PlayerMessageTypes.GAME_OBJ_UPDATE, 1, false);
            //msg.writeByte(0x55);
            //msg.writeByte(GAME_OBJECT_TYPES.DOOR);
            //msg.writeUint(lookingAtClientId);
            //msg.writeUint(CLIENT_OBJECT_UPDATE_FLAGS.OBJECT_INTERACTION | CLIENT_OBJECT_UPDATE_FLAGS.POSITION);
            //msg.writeFloat(188f);
            //msg.writeFloat(80f);
            //msg.writeFloat(0f);
            //msg.writeBool(false); // field 0x114 | Hostile
            //msg.writeBool(true); // Do update?
            //msg.writeBool(false); // field 0x108 | Bash/Security
            //msg.writeBool(false); // field 0x104 | no Bash

            //// Update placeable Properties
            //msg = new Message(PlayerMessageTypes.GAME_OBJ_UPDATE, 1, false);
            //msg.writeByte(0x55);
            //msg.writeByte(GAME_OBJECT_TYPES.PLACEABLE);
            //msg.writeUint(lookingAtClientId);
            //msg.writeUint(CLIENT_OBJECT_UPDATE_FLAGS.OBJECT_INTERACTION);
            //msg.writeBool(false); // field 0x124 | Hostile
            //msg.writeBool(true); // field 0x11c | Do Update?
            //msg.writeBool(true); // field 0x108 | Bash/Security
            //msg.writeBool(true); // field 0x104 | no Bash

            //// Delete Door
            //msg = new Message(PlayerMessageTypes.GAME_OBJ_UPDATE, 1, false);
            //msg.writeByte(0x44);
            //msg.writeByte(GAME_OBJECT_TYPES.DOOR);
            //msg.writeUint(lookingAtClientId);

            //// Death Note
            //msg = new Message(PlayerMessageTypes.CHEAT, 0x7);
            //setServerDebugMode(true, pHandle);
            //msg.writeUint(playerServerId);
            //msg.writeUint(lookingAtClientId);

            //// Play Creature Sound effect
            //msg = new Message(PlayerMessageTypes.VOICE_CHAT, 1, false);
            //msg.writeUint(playerClientId);
            //msg.writeByte(7 ); // Sound Set Index

            //// Peak Container Contents (Crashes the game if you aren't loking at a placeable)
            //msg = new Message(PlayerMessageTypes.GUI_CONTAINER, 1, false);
            //msg.writeUint(lookingAtClientId);
            //msg.writeInt(0);

            //// Disable Level Up temporarily
            //msg = new Message(PlayerMessageTypes.LEVEL_UP, 1, false);

            //// Free Cam
            //msg = new Message(PlayerMessageTypes.CAMERA, 2, false);
            //msg.writeByte(7); // Mode 7 is Free Cam


            Console.WriteLine($"Sending Message:\n{msg}");

            //Thread.Sleep(2000);
            
            i.sendMessage(msg);
            
            //Console.ReadKey();
        }
    }
}

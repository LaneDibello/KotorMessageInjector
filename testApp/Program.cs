using KotorMessageInjector;
using static KotorMessageInjector.KotorHelpers;

namespace testApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Injector i = new Injector("swkotor.exe");

            ////Teleporting
            //Message msg = new Message(Message.PlayerMessageTypes.GAME_OBJ_UPDATE, 1, false);
            //msg.writeByte(0x55);
            //msg.writeByte(0x5);
            //uint id = getPlayerClientID(i.processHandle);
            //msg.writeUint(id);
            //msg.writeUint((uint)ClientObjectUpdateFlags.POSITION); 
            //msg.writeFloat(180f);
            //msg.writeFloat(100f);
            //msg.writeFloat(0.0f);

            //// Swap/Recruit Creatures
            //Message msg = new Message(Message.PlayerMessageTypes.GAME_OBJ_UPDATE, 2, false);
            //uint id = getLookingAtClientID(i.processHandle);
            //msg.writeUint(id);  

            ////Heal
            //Message msg = new Message(Message.PlayerMessageTypes.CHEAT, 2);

            ////Run Scripts
            //Message msg = new Message(Message.PlayerMessageTypes.CHEAT, 8);
            //setServerDebugMode(true, i.processHandle);
            //msg.writeCExoString("k_trg_transfail1");

            //// Take Control/Add to party
            //Message msg = new Message(Message.PlayerMessageTypes.GAME_OBJ_UPDATE, 2, false);
            //uint id = getLookingAtClientID(i.processHandle);
            //msg.writeUint(id);

            //// Update door properties
            //Message msg = new Message(Message.PlayerMessageTypes.GAME_OBJ_UPDATE, 1, false);
            //msg.writeByte(0x55);
            //msg.writeByte(0xA);
            //uint id = getLookingAtClientID(i.processHandle);
            //msg.writeUint(id);
            //msg.writeUint((uint)ClientObjectUpdateFlags.OBJECT_INTERACTION | (uint)ClientObjectUpdateFlags.POSITION);
            //msg.writeFloat(188f);
            //msg.writeFloat(80f);
            //msg.writeFloat(0f);
            //msg.writeBool(false); // field 0x114 | Hostile
            //msg.writeBool(true); // Do update?
            //msg.writeBool(false); // field 0x108 | Bash/Security
            //msg.writeBool(false); // field 0x104 | no Bash

            //// Update placeable Properties
            //Message msg = new Message(Message.PlayerMessageTypes.GAME_OBJ_UPDATE, 1, false);
            //msg.writeByte(0x55);
            //msg.writeByte(0x9);
            //uint id = getLookingAtClientID(i.processHandle);
            //msg.writeUint(id);
            //msg.writeUint((uint)ClientObjectUpdateFlags.OBJECT_INTERACTION);
            //msg.writeBool(false); // field 0x124 | Hostile
            //msg.writeBool(true); // field 0x11c | Do Update?
            //msg.writeBool(true); // field 0x108 | Bash/Security
            //msg.writeBool(true); // field 0x104 | no Bash

            //// Delete Door
            //Message msg = new Message(Message.PlayerMessageTypes.GAME_OBJ_UPDATE, 1, false);
            //msg.writeByte(0x44);
            //msg.writeByte((byte)GameObjectTypes.DOOR);
            //uint id = getLookingAtClientID(i.processHandle);
            //msg.writeUint(id);

            //// Death Note
            //Message msg = new Message(Message.PlayerMessageTypes.CHEAT, 0x7);
            //setServerDebugMode(true, i.processHandle);
            //uint id = getPlayerServerID(i.processHandle);
            //msg.writeUint(id);
            //id = getLookingAtClientID(i.processHandle);
            //msg.writeUint(id);

            Console.WriteLine($"Sending Message:\n{msg}");
            
            i.sendMessage(msg);
            
            //Console.ReadKey();
        }
    }
}

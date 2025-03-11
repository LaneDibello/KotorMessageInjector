using KotorMessageInjector;
using System.Runtime.Serialization;
using static KotorMessageInjector.KotorHelpers;
using static KotorMessageInjector.Message;


namespace testApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            IntPtr pHandle = ProcessAPI.OpenProcessByName("swkotor.exe");

            uint size = ProcessAPI.GetModuleSize(pHandle);
            Console.WriteLine($"Module Size = {size}");

            bool isSteam;
            int version = getGameVersion(pHandle, out isSteam);
            Console.WriteLine($"Game Version: KotOR{version} {(isSteam ? "STEAM" : "")}");

            Injector i = new Injector(pHandle);

            uint playerServerId = getPlayerServerID(pHandle);
            uint playerClientId = getPlayerClientID(pHandle);
            uint lookingAtServerId = getLookingAtServerID(pHandle);
            uint lookingAtClientId = getLookingAtClientID(pHandle);
            uint partyTable = getServerPartyTable(pHandle);

            ////Game keeps running even when you click out
            //disableClickOutPausing(pHandle);

            Message msg;

            ////Teleporting
            //msg = new Message(PlayerMessageTypes.GAME_OBJ_UPDATE, 1, false);
            //msg.writeByte(0x55);
            //msg.writeByte(GAME_OBJECT_TYPES.CREATURE);
            //msg.writeUint(playerClientId);
            //msg.writeUint(CLIENT_OBJECT_UPDATE_FLAGS.POSITION);
            //msg.writeFloat(112f);
            //msg.writeFloat(97f);
            //msg.writeFloat(0.0f);

            ////Swap / Recruit Creatures
            //msg = new Message(PlayerMessageTypes.GAME_OBJ_UPDATE, 2, false);
            //msg.writeUint(lookingAtClientId);

            ////Heal
            //msg = new Message(PlayerMessageTypes.CHEAT, 2);

            ////Run Scripts
            //msg = new Message(PlayerMessageTypes.CHEAT, 8);
            //setServerDebugMode(true, pHandle); // Debug mode must be on to run scripts
            //msg.writeCExoString("k_trg_transfail");

            //// Take Control/Add to party
            //msg = new Message(PlayerMessageTypes.GAME_OBJ_UPDATE, 2, false);
            //msg.writeUint(lookingAtClientId);

            //// Update door properties
            //msg = new Message(PlayerMessageTypes.GAME_OBJ_UPDATE, 1, false);
            //msg.writeByte(0x55);
            //msg.writeByte(GAME_OBJECT_TYPES.DOOR);
            //msg.writeUint(lookingAtClientId);
            //msg.writeUint(CLIENT_OBJECT_UPDATE_FLAGS.OBJECT_INTERACTION);
            //msg.writeBool(true); // field 0x114 | Hostile
            //msg.writeBool(true); // Do update?
            //msg.writeBool(true); // field 0x108 | Bash/Security
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
            //msg.writeUint(lookingAtServerId);

            //// Play Creature Sound effect
            //msg = new Message(PlayerMessageTypes.VOICE_CHAT, 1, false);
            //msg.writeUint(playerClientId);
            //msg.writeByte(7); // Sound Set Index

            //// Peak Container Contents (Crashes the game if you aren't loking at a placeable)
            //msg = new Message(PlayerMessageTypes.GUI_CONTAINER, 1, false);
            //msg.writeUint(lookingAtClientId);
            //msg.writeInt(0);

            //// Disable Level Up temporarily
            //msg = new Message(PlayerMessageTypes.LEVEL_UP, 1, false);

            //// Free Cam
            //// You can adjust the speed of the camera with the float at 0x007455c8
            //msg = new Message(PlayerMessageTypes.CAMERA, 2, false);
            //msg.writeByte(7); // Mode 7 is Free Cam

            //msg = new Message(PlayerMessageTypes.GAME_OBJ_UPDATE, 1);
            //msg.writeByte(0x43); // Summon Creature
            //msg.writeUint(0x7f000000); //New Creature ID?
            //msg.writeVector(0f, 0f, 0f); // Spawn position?
            //msg.writeCExoString("c_kinrath");

            //msg = new Message(PlayerMessageTypes.GAME_OBJ_UPDATE, 1, false);
            //msg.writeByte(0x41); // Add Object
            //msg.writeByte(GAME_OBJECT_TYPES.CREATURE);
            //msg.writeUint(serverToClientId(211));
            //msg.writeByte(0);
            //msg.writeFloat(107f);
            //msg.writeFloat(125f);
            //msg.writeFloat(0f);
            //msg.writeFloat(0f);
            //msg.writeFloat(0f);
            //msg.writeFloat(0f);
            //msg.writeUshort(0);

            //Console.WriteLine($"Sending Message:\n{msg}");

            //Thread.Sleep(2000);

            //i.sendMessage(msg);

            ////Get Pointer to Object looking at
            //RemoteFunction rf = new RemoteFunction(0x004b1700);
            //rf.setThis(getServerInternal(pHandle));
            //rf.addParam(lookingAtServerId);
            //uint retval = i.runFunction(rf);
            //Console.WriteLine($"Returned: {retval}");

            ////Adds the rancor to your available parety members in slot 4
            //ObjManager om = new(pHandle);
            //RemoteFunction rf = new RemoteFunction(0x005645f0);
            //rf.setThis(partyTable);
            //rf.addParam((uint)4);
            //rf.addParam(om.createCExoString("g_rancor01"));
            //uint retval = i.runFunction(rf);
            //Console.WriteLine($"Returned: {retval}");

            //spawnCreature(pHandle, "g_rancor01", 112.5f, 97.5f, 0.0f);

            //// Kotor 1 Warp to module
            //ObjManager om = new(pHandle);
            //uint server = getServer(pHandle);
            //RemoteFunction rf = new RemoteFunction(0x004aecd0, false); //SetMoveToModuleString
            //rf.setThis(server);
            //rf.addParam(om.createCExoString("danm13"));
            //i.runFunction(rf);
            //rf = new RemoteFunction(0x004aecc0, false); //SetMoveToModulePending
            //rf.setThis(server);
            //rf.addParam(1);
            //i.runFunction(rf);

            //Console.ReadKey();
        }

        static void spawnCreature(IntPtr pHandle, string resref, float x, float y, float z)
        {
            Injector i = new Injector(pHandle);
            
            ObjManager om = new(pHandle);
            RemoteFunction rf_new = new RemoteFunction(0x006fa7e6); // Operator New
            rf_new.addParam(0xac8);
            uint creatureBuffer = i.runFunction(rf_new);

            RemoteFunction rf_serverCreature = new RemoteFunction(0x004f7a10); // CSWSCreature
            rf_serverCreature.setThis(creatureBuffer);
            rf_serverCreature.addParam((uint)0x7f000000);
            rf_serverCreature.addParam(0);
            creatureBuffer = i.runFunction(rf_serverCreature);

            RemoteFunction rf_loadTemplate = new RemoteFunction(0x005026d0); // LoadFromTemplate
            rf_loadTemplate.setThis(creatureBuffer);
            rf_loadTemplate.addParam(om.createCResRef(resref));
            rf_loadTemplate.addParam(0);
            uint result = i.runFunction(rf_loadTemplate);

            RemoteFunction rf_module = new RemoteFunction(0x004b14f0); // GetModule
            rf_module.setThis(getServerInternal(pHandle));
            uint module = i.runFunction(rf_module);

            RemoteFunction rf_area = new RemoteFunction(0x004c30f0); // GetArea
            rf_area.setThis(module);
            uint area = i.runFunction(rf_area);

            RemoteFunction rf_addToArea = new RemoteFunction(0x004fa100, false); // AddToArea
            rf_addToArea.setThis(creatureBuffer);
            rf_addToArea.addParam(area);
            rf_addToArea.addParam(x);
            rf_addToArea.addParam(y);
            rf_addToArea.addParam(z);
            rf_addToArea.addParam(1);
            rf_addToArea.addParam(1);
            i.runFunction(rf_addToArea);
        }
    }
}

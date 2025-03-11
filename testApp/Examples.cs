using KotorMessageInjector;
using static KotorMessageInjector.Message;
using static KotorMessageInjector.KotorHelpers;

namespace testApp
{
    static class Examples
    {
        const uint KOTOR_1_CSWSCREATURE_SIZE = 0xac8;
        const uint KOTOR_2_CSWSCREATURE_SIZE = 0x1220;


        #region messages
        static public Message teleportPlayer(uint playerClientId, float x, float y, float z)
        {
            Message msg = new Message(PlayerMessageTypes.GAME_OBJ_UPDATE, 1, false);
            msg.writeByte(0x55);
            msg.writeByte(GAME_OBJECT_TYPES.CREATURE);
            msg.writeUint(playerClientId);
            msg.writeUint(CLIENT_OBJECT_UPDATE_FLAGS.POSITION);
            msg.writeFloat(x);
            msg.writeFloat(y);
            msg.writeFloat(z);

            return msg;
        }

        static public Message swapToTarget(uint lookingAtClientId)
        {
            Message msg = new Message(PlayerMessageTypes.GAME_OBJ_UPDATE, 2, false);
            msg.writeUint(lookingAtClientId);

            return msg;
        }

        static public Message heal()
        {
            return new Message(PlayerMessageTypes.CHEAT, 2);
        }

        static public Message runScript(string script, IntPtr processHandle)
        {
            //Run Scripts
            Message msg = new Message(PlayerMessageTypes.CHEAT, 8);
            setServerDebugMode(true, processHandle); // Debug mode must be on to run scripts
            msg.writeCExoString(script);

            return msg;
        }

        static public Message updateTargetDoorProps(uint lookingAtClientId, bool hostile, bool enableQuickActions, bool disableBash)
        {
            Message msg = new Message(PlayerMessageTypes.GAME_OBJ_UPDATE, 1, false);
            msg.writeByte(0x55);
            msg.writeByte(GAME_OBJECT_TYPES.DOOR);
            msg.writeUint(lookingAtClientId);
            msg.writeUint(CLIENT_OBJECT_UPDATE_FLAGS.OBJECT_INTERACTION);
            msg.writeBool(hostile); // field 0x114 | Hostile
            msg.writeBool(true); // Do update?
            msg.writeBool(enableQuickActions); // field 0x108 | Bash/Security
            msg.writeBool(disableBash); // field 0x104 | no Bash

            return msg;
        }

        static public Message updateTargetPlasableProps(uint lookingAtClientId, bool hostile, bool enableQuickActions, bool disableBash)
        {
            Message msg = new Message(PlayerMessageTypes.GAME_OBJ_UPDATE, 1, false);
            msg.writeByte(0x55);
            msg.writeByte(GAME_OBJECT_TYPES.PLACEABLE);
            msg.writeUint(lookingAtClientId);
            msg.writeUint(CLIENT_OBJECT_UPDATE_FLAGS.OBJECT_INTERACTION);
            msg.writeBool(hostile); // field 0x114 | Hostile
            msg.writeBool(true); // Do update?
            msg.writeBool(enableQuickActions); // field 0x108 | Bash/Security
            msg.writeBool(disableBash); // field 0x104 | no Bash

            return msg;
        }

        static public Message deleteTargetDoor(uint lookingAtClientId)
        {
            // Delete Door
            Message msg = new Message(PlayerMessageTypes.GAME_OBJ_UPDATE, 1, false);
            msg.writeByte(0x44);
            msg.writeByte(GAME_OBJECT_TYPES.DOOR);
            msg.writeUint(lookingAtClientId);

            return msg;
        }

        static public Message killTargetCreature(uint playerServerId, uint lookingAtServerId, IntPtr processHandle)
        {
            Message msg = new Message(PlayerMessageTypes.CHEAT, 0x7);
            setServerDebugMode(true, processHandle);
            msg.writeUint(playerServerId);
            msg.writeUint(lookingAtServerId);

            return msg;
        }

        static public Message playCreatureSound(uint creatureClientId, byte sound)
        {
            Message msg = new Message(PlayerMessageTypes.VOICE_CHAT, 1, false);
            msg.writeUint(creatureClientId);
            msg.writeByte(sound); // Sound Set Index

            return msg;
        }

        static public Message peekContainerContents(uint containerClientId)
        {
            Message msg = new Message(PlayerMessageTypes.GUI_CONTAINER, 1, false);
            msg.writeUint(containerClientId);
            msg.writeInt(0);

            return msg;
        }

        static public Message disableLevelUp()
        {
            return new Message(PlayerMessageTypes.LEVEL_UP, 1, false);
        }

        static public Message freeCam()
        {
            Message msg = new Message(PlayerMessageTypes.CAMERA, 2, false);
            msg.writeByte(7); // Mode 7 is Free Cam

            return msg;
        }

        static public Message invulnerability()
        {
            return new Message(PlayerMessageTypes.CHEAT, 4);
        }
        #endregion

        #region functions
        public static void spawnCreature(IntPtr pHandle, string resref, float x, float y, float z)
        {
            Injector i = new Injector(pHandle);

            int version = getGameVersion(pHandle);

            Dictionary<string, uint> funcLibrary = getFunctionLibrary(pHandle);

            ObjManager om = new(pHandle);
            RemoteFunction rf_new = new RemoteFunction(funcLibrary["operator_new"]); 
            rf_new.addParam(version == 1 ? KOTOR_1_CSWSCREATURE_SIZE : KOTOR_2_CSWSCREATURE_SIZE);
            uint creatureBuffer = i.runFunction(rf_new);

            Console.WriteLine("ran operator_new");

            RemoteFunction rf_serverCreature = new RemoteFunction(funcLibrary["CSWSCreature::CSWSCreature"]); 
            rf_serverCreature.setThis(creatureBuffer);
            rf_serverCreature.addParam((uint)0x7f000000);
            rf_serverCreature.addParam(0);
            creatureBuffer = i.runFunction(rf_serverCreature);

            Console.WriteLine("ran CSWSCreature");

            RemoteFunction rf_loadTemplate = new RemoteFunction(funcLibrary["CSWSCreature::LoadFromTemplate"]); 
            rf_loadTemplate.setThis(creatureBuffer);
            rf_loadTemplate.addParam(om.createCResRef(resref));
            rf_loadTemplate.addParam(0);
            uint result = i.runFunction(rf_loadTemplate);

            Console.WriteLine("ran LoadFromTemplate");

            RemoteFunction rf_module = new RemoteFunction(funcLibrary["CServerExoAppInternal::GetModule"]); 
            rf_module.setThis(getServerInternal(pHandle));
            uint module = i.runFunction(rf_module);

            Console.WriteLine("ran GetModule");

            RemoteFunction rf_area = new RemoteFunction(funcLibrary["CSWSModule::GetArea"]);
            rf_area.setThis(module);
            uint area = i.runFunction(rf_area);

            RemoteFunction rf_addToArea = new RemoteFunction(funcLibrary["CSWSCreature::AddToArea"], false); 
            rf_addToArea.setThis(creatureBuffer);
            rf_addToArea.addParam(area);
            rf_addToArea.addParam(x);
            rf_addToArea.addParam(y);
            rf_addToArea.addParam(z);
            rf_addToArea.addParam(1);
            rf_addToArea.addParam(1);
            i.runFunction(rf_addToArea);
        }
        
        public static uint addParty(IntPtr pHandle, string creatureRef, int partySlot)
        {
            Injector i = new Injector(pHandle);
            ObjManager om = new(pHandle);

            Dictionary<string, uint> funcLibrary = getFunctionLibrary(pHandle);

            RemoteFunction rf = new RemoteFunction(funcLibrary["CSWPartyTable::AddNPC"]);
            rf.setThis(getServerPartyTable(pHandle));
            rf.addParam(partySlot);
            rf.addParam(om.createCExoString(creatureRef));

            return i.runFunction(rf);
        }

        public static void warp(IntPtr pHandle, string module)
        {
            Injector i = new Injector(pHandle);
            ObjManager om = new(pHandle);

            Dictionary<string, uint> funcLibrary = getFunctionLibrary(pHandle);

            uint server = getServer(pHandle);
            
            RemoteFunction rf = new RemoteFunction(funcLibrary["CServerExoApp::SetMoveToModuleString"], false);
            rf.setThis(server);
            rf.addParam(om.createCExoString(module));
            i.runFunction(rf);
            
            rf = new RemoteFunction(funcLibrary["CServerExoApp::SetMoveToModulePending"], false);
            rf.setThis(server);
            rf.addParam(1);
            i.runFunction(rf);
        }

        public static void changeFaction(IntPtr pHandle, uint target, int faction)
        {
            Injector i = new Injector(pHandle);

            Dictionary<string, uint> funcLibrary = getFunctionLibrary(pHandle);

            uint facManager = getFactionManager(pHandle);

            RemoteFunction rf_getFac = new RemoteFunction(funcLibrary["CFactionManager::GetFaction"]);
            rf_getFac.setThis(facManager);
            rf_getFac.addParam(faction);
            uint fac = i.runFunction(rf_getFac);

            RemoteFunction rf_addMember = new RemoteFunction(funcLibrary["CSWSFaction::AddMember"], false);
            rf_addMember.setThis(fac);
            rf_addMember.addParam(target);
            rf_addMember.addParam(0);
            i.runFunction(rf_addMember);
        }

        #endregion
    }
}

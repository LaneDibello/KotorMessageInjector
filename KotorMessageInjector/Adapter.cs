﻿using static KotorMessageInjector.KotorHelpers;
using static KotorMessageInjector.Message;
using System;
using System.Runtime.InteropServices;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace KotorMessageInjector
{
    public class AdapterNotImplementedForGameError : Exception
    {
        public AdapterNotImplementedForGameError(string message) : base(message) { }
    }

    public static class Adapter
    {
        private const uint KOTOR_1_CSWSCREATURE_SIZE = 0xac8;
        private const uint KOTOR_2_CSWSCREATURE_SIZE = 0x1220;
        private static Injector _injector = null;

        #region Messages

        public static void SendMessage(IntPtr pHandle, Message message)
        {
            if (_injector == null || _injector.processHandle != pHandle)
                _injector = new Injector(pHandle);
            _injector.sendMessage(message);
        }

        public static Message TeleportPlayer(uint playerClientId, float x, float y, float z)
        {
            return new Message(PlayerMessageTypes.GAME_OBJ_UPDATE, 1, false)
                .writeByte(0x55)
                .writeByte(GAME_OBJECT_TYPES.CREATURE)
                .writeUint(playerClientId)
                .writeUint(CLIENT_OBJECT_UPDATE_FLAGS.POSITION)
                .writeFloat(x)
                .writeFloat(y)
                .writeFloat(z);
        }

        public static Message SwapToTarget(uint lookingAtClientId)
        {
            return new Message(PlayerMessageTypes.GAME_OBJ_UPDATE, 2, false)
                .writeUint(lookingAtClientId);
        }

        public static Message Heal()
        {
            return new Message(PlayerMessageTypes.CHEAT, 2);
        }

        public static Message RunScript(string script, IntPtr processHandle)
        {
            setServerDebugMode(true, processHandle); // Debug mode must be on to run scripts
            return new Message(PlayerMessageTypes.CHEAT, 8)
                .writeCExoString(script);
        }

        public static Message UpdateTargetDoorProps(uint lookingAtClientId, bool hostile, bool enableQuickActions, bool disableBash)
        {
            return new Message(PlayerMessageTypes.GAME_OBJ_UPDATE, 1, false)
                .writeByte(0x55)
                .writeByte(GAME_OBJECT_TYPES.DOOR)
                .writeUint(lookingAtClientId)
                .writeUint(CLIENT_OBJECT_UPDATE_FLAGS.OBJECT_INTERACTION)
                .writeBool(hostile) // field 0x114 | Hostile
                .writeBool(true) // Do update?
                .writeBool(enableQuickActions) // field 0x108 | Bash/Security
                .writeBool(disableBash); // field 0x104 | no Bash
        }

        public static Message UpdateTargetPlaceableProps(uint lookingAtClientId, bool hostile, bool enableQuickActions, bool disableBash)
        {
            return new Message(PlayerMessageTypes.GAME_OBJ_UPDATE, 1, false)
                .writeByte(0x55)
                .writeByte(GAME_OBJECT_TYPES.PLACEABLE)
                .writeUint(lookingAtClientId)
                .writeUint(CLIENT_OBJECT_UPDATE_FLAGS.OBJECT_INTERACTION)
                .writeBool(hostile) // field 0x114 | Hostile
                .writeBool(true) // Do update?
                .writeBool(enableQuickActions) // field 0x108 | Bash/Security
                .writeBool(disableBash); // field 0x104 | no Bash
        }

        public static Message DeleteTargetDoor(uint lookingAtClientId)
        {
            return new Message(PlayerMessageTypes.GAME_OBJ_UPDATE, 1, false)
                .writeByte(0x44)
                .writeByte(GAME_OBJECT_TYPES.DOOR)
                .writeUint(lookingAtClientId);
        }

        public static Message KillTargetCreature(uint playerServerId, uint lookingAtServerId, IntPtr processHandle)
        {
            setServerDebugMode(true, processHandle);
            return new Message(PlayerMessageTypes.CHEAT, 0x7)
                .writeUint(playerServerId)
                .writeUint(lookingAtServerId);
        }

        public static Message PlayCreatureSound(uint creatureClientId, byte sound)
        {
            return new Message(PlayerMessageTypes.VOICE_CHAT, 1, false)
                .writeUint(creatureClientId)
                .writeByte(sound); // Sound Set Index
        }

        public static Message PeekContainerContents(uint containerClientId)
        {
            return new Message(PlayerMessageTypes.GUI_CONTAINER, 1, false)
                .writeUint(containerClientId)
                .writeInt(0);
        }

        public static Message DisableLevelUp()
        {
            return new Message(PlayerMessageTypes.LEVEL_UP, 1, false);
        }

        public static Message FreeCamOn()
        {
            return new Message(PlayerMessageTypes.CAMERA, 2, false)
                .writeByte(7); // Mode 7 is Free Cam
        }

        public static Message FreeCamOff()
        {
            return new Message(PlayerMessageTypes.CAMERA, 2, false)
                .writeByte(3); // Mode 7 is Free Cam
        }

        public static Message Invulnerability()
        {
            return new Message(PlayerMessageTypes.CHEAT, 4);
        }

        #endregion

        #region Functions

        public static void SpawnCreature(IntPtr pHandle, string resref, float x, float y, float z)
        {
            var i = new Injector(pHandle);
            var version = getGameVersion(pHandle);
            var funcLibrary = getFuncLibrary(pHandle);

            var creatureBuffer = i.runFunction(new RemoteFunction(funcLibrary[Function.operator_new])
                .addParam(version == 1 ? KOTOR_1_CSWSCREATURE_SIZE : KOTOR_2_CSWSCREATURE_SIZE));

            creatureBuffer = i.runFunction(new RemoteFunction(funcLibrary[Function.CSWSCreature_CSWSCreature])
                .setThis(creatureBuffer)
                .addParam((uint)0x7f000000)
                .addParam(0));

            _ = i.runFunction(new RemoteFunction(funcLibrary[Function.CSWSCreature_LoadFromTemplate])
                .setThis(creatureBuffer)
                .addParam(new ObjManager(pHandle).createCResRef(resref))
                .addParam(0));

            var module = i.runFunction(new RemoteFunction(funcLibrary[Function.CServerExoAppInternal_GetModule])
                .setThis(getServerInternal(pHandle)));

            var area = i.runFunction(new RemoteFunction(funcLibrary[Function.CSWSModule_GetArea])
                .setThis(module));

            _ = i.runFunction(new RemoteFunction(funcLibrary[Function.CSWSCreature_AddToArea], false)
                .setThis(creatureBuffer)
                .addParam(area)
                .addParam(x)
                .addParam(y)
                .addParam(z)
                .addParam(1)
                .addParam(1));
        }

        public static uint AddParty(IntPtr pHandle, string creatureRef, int partySlot)
        {
            var i = new Injector(pHandle);
            var funcLibrary = getFuncLibrary(pHandle);

            return i.runFunction(new RemoteFunction(funcLibrary[Function.CSWPartyTable_AddNPC])
                .setThis(getServerPartyTable(pHandle))
                .addParam(partySlot)
                .addParam(new ObjManager(pHandle).createCExoString(creatureRef)));
        }

        public static void Warp(IntPtr pHandle, string module)
        {
            var i = new Injector(pHandle);
            var funcLibrary = getFuncLibrary(pHandle);
            var server = getServer(pHandle);

            _ = i.runFunction(new RemoteFunction(funcLibrary[Function.CServerExoApp_SetMoveToModuleString], false)
                .setThis(server)
                .addParam(new ObjManager(pHandle).createCExoString(module)));
            _ = i.runFunction(new RemoteFunction(funcLibrary[Function.CServerExoApp_SetMoveToModulePending], false)
                .setThis(server)
                .addParam(1));
        }

        public static void ChangeFaction(IntPtr pHandle, uint target, int faction)
        {
            var i = new Injector(pHandle);
            var funcLibrary = getFuncLibrary(pHandle);
            var facManager = getFactionManager(pHandle);

            var fac = i.runFunction(new RemoteFunction(funcLibrary[Function.CFactionManager_GetFaction])
                .setThis(facManager)
                .addParam(faction));
            _ = i.runFunction(new RemoteFunction(funcLibrary[Function.CSWSFaction_AddMember], false)
                .setThis(fac)
                .addParam(target)
                .addParam(0));
        }

        public static uint GetPlayerGob(IntPtr pHandle)
        {
            var i = new Injector(pHandle);
            var funcLibrary = getFuncLibrary(pHandle);

            uint playerClientObject = i.runFunction(new RemoteFunction(funcLibrary[Function.CClientExoApp_GetGameObject])
                .setThis(getClient(pHandle))
                .addParam(getPlayerClientID(pHandle)));

            return getClientObjectGob(pHandle, playerClientObject);
        }

        public static uint GetLookingAtGob(IntPtr pHandle)
        {
            var i = new Injector(pHandle);
            var funcLibrary = getFuncLibrary(pHandle);

            uint targetClientObject = i.runFunction(new RemoteFunction(funcLibrary[Function.CClientExoApp_GetGameObject])
                .setThis(getClient(pHandle))
                .addParam(getLookingAtClientID(pHandle)));

            return getClientObjectGob(pHandle, targetClientObject);
        }

        public static uint GetPlayerClientObject(IntPtr pHandle)
        {
            return GetClientObject(pHandle, getPlayerClientID(pHandle));
        }

        public static uint GetPlayerServerObject(IntPtr pHandle)
        {
            return GetServerObject(pHandle, getPlayerServerID(pHandle));
        }

        public static uint GetClientObject(IntPtr pHandle, uint clientId)
        {
            var i = new Injector(pHandle);
            var funcLibrary = getFuncLibrary(pHandle);

            return i.runFunction(new RemoteFunction(funcLibrary[Function.CClientExoApp_GetGameObject])
                .setThis(getClient(pHandle))
                .addParam(clientId));
        }

        public static uint GetServerObject(IntPtr pHandle, uint serverId)
        {
            var i = new Injector(pHandle);
            var funcLibrary = getFuncLibrary(pHandle);

            var server = getServer(pHandle);
            if (server == 0) { return 0; }

            return i.runFunction(new RemoteFunction(funcLibrary[Function.CServerExoApp_GetGameObject])
                .setThis(getServer(pHandle))
                .addParam(serverId));
        }

        public static uint DrawModel(IntPtr pHandle, string model, float scale, float x, float y, float z)
        {
            var om = new ObjManager(pHandle);
            var scene = getCurrentScene(pHandle);
            var i = new Injector(pHandle);
            var funcLibrary = getFuncLibrary(pHandle);

            var gob = i.runFunction(new RemoteFunction(funcLibrary[Function.NewCAurObject])
                .addParam(om.createCStr(model))
                .addParam(om.createCStr(""))
                .addParam(0)
                .addParam(0));
            _ = i.runFunction(new RemoteFunction(funcLibrary[Function.Gob_SetPosition], false)
                .setThis(gob)
                .addParam(om.createVector(0, 0, 0))
                .addParam(x)
                .addParam(y)
                .addParam(z));
            _ = i.runFunction(new RemoteFunction(funcLibrary[Function.Gob_TurnOffShadows], false)
                .setThis(gob));
            _ = i.runFunction(new RemoteFunction(funcLibrary[Function.Gob_SetObjectScale], false)
                .setThis(gob)
                .addParam(scale)
                .addParam(false));
            _ = i.runFunction(new RemoteFunction(funcLibrary[Function.Gob_AttachToScene], false)
                .setThis(gob)
                .addParam(scene));
            return gob;
        }

        public static void MoveModel(IntPtr pHandle, uint gob, float x, float y, float z)
        {
            var om = new ObjManager(pHandle);
            var i = new Injector(pHandle);
            var funcLibrary = getFuncLibrary(pHandle);

            _ = i.runFunction(new RemoteFunction(funcLibrary[Function.Gob_SetPosition], false)
                .setThis(gob)
                .addParam(om.createVector(0, 0, 0))
                .addParam(x)
                .addParam(y)
                .addParam(z));
        }

        // Takes a Yaw, Pitch, and Roll in Degrees. And Converts it to Quaternion Orientation
        // Gob_SetOrientation could theorhetically also be used for skews if you feel like doing the math
        public static void RotateModel(IntPtr pHandle, uint gob, float yaw, float pitch, float roll)
        {
            var om = new ObjManager(pHandle);
            var i = new Injector(pHandle);
            var funcLibrary = getFuncLibrary(pHandle);

            uint orientation = om.createQuaternion(0, 0, 0, 0);

            _ = i.runFunction(new RemoteFunction(funcLibrary[Function.YawPitchRoll], false)
                .addParam(orientation)
                .addParam(yaw)
                .addParam(pitch)
                .addParam(roll));

            var (w, x, y, z) = readQuaternion(pHandle, (IntPtr)orientation);

            _ = i.runFunction(new RemoteFunction(funcLibrary[Function.Gob_SetOrientation], false)
                .setThis(gob)
                .addParam(om.createQuaternion(0, 0, 0, 0))
                .addParam(w)
                .addParam(x)
                .addParam(y)
                .addParam(z));
        }

        public static (float, float, float) Normalize(float x, float y, float z)
        {
            var magnitude = Math.Sqrt(x * x + y * y + z * z);
            if (magnitude < 1) return (x, y, z);
            var xOut = (float)(x / magnitude);
            var yOut = (float)(y / magnitude);
            var zOut = (float)(z / magnitude);
            return (xOut, yOut, zOut);
        }

        public static void ColorizeModel(IntPtr pHandle, uint gob, float r, float g, float b, float a = 1f)
        {
            var i = new Injector(pHandle);
            var funcLibrary = getFuncLibrary(pHandle);
            (r, g, b) = Normalize(r, g, b);

            _ = i.runFunction(new RemoteFunction(funcLibrary[Function.Gob_SetColorShifting], false)
                .setThis(gob)
                .addParam(r)
                .addParam(g)
                .addParam(b)
                .addParam(a)
                .addParam(1));
            _ = i.runFunction(new RemoteFunction(funcLibrary[Function.Gob_SetIllumination], false)
                .setThis(gob)
                .addParam(r)
                .addParam(g)
                .addParam(b)
                .addParam(1));
        }

        public static void DeleteModel(IntPtr pHandle, uint gob)
        {
            var i = new Injector(pHandle);
            var funcLibrary = getFuncLibrary(pHandle);

            _ = i.runFunction(new RemoteFunction(funcLibrary[Function.Gob_AttachToScene], false)
                .setThis(gob)
                .addParam(0));
        }

        public static void SetCreatureAttribute(IntPtr pHandle, uint serverCreature, ATTRIBUTES attribute, byte value)
        {
            var i = new Injector(pHandle);
            var funcLibrary = getFuncLibrary(pHandle);

            if (serverCreature == 0) { return; }

            var creatureStats = getCreatureStats(pHandle, serverCreature);
            int targetValue = (int)value;

            switch (attribute)
            {
                case ATTRIBUTES.STR:
                    _ = i.runFunction(new RemoteFunction(funcLibrary[Function.CSWSCreatureStats_SetSTRBase], false)
                        .setThis(creatureStats)
                        .addParam(targetValue));
                    break;
                case ATTRIBUTES.DEX:
                    _ = i.runFunction(new RemoteFunction(funcLibrary[Function.CSWSCreatureStats_SetDEXBase], false)
                        .setThis(creatureStats)
                        .addParam(targetValue));
                    break;
                case ATTRIBUTES.CON:
                    _ = i.runFunction(new RemoteFunction(funcLibrary[Function.CSWSCreatureStats_SetCONBase], false)
                        .setThis(creatureStats)
                        .addParam(targetValue)
                        .addParam(1)); // Recalculate HP
                    break;
                case ATTRIBUTES.INT:
                    _ = i.runFunction(new RemoteFunction(funcLibrary[Function.CSWSCreatureStats_SetINTBase], false)
                        .setThis(creatureStats)
                        .addParam(targetValue));
                    break;
                case ATTRIBUTES.WIS:
                    _ = i.runFunction(new RemoteFunction(funcLibrary[Function.CSWSCreatureStats_SetWISBase], false)
                        .setThis(creatureStats)
                        .addParam(targetValue));
                    break;
                case ATTRIBUTES.CHA:
                    _ = i.runFunction(new RemoteFunction(funcLibrary[Function.CSWSCreatureStats_SetCHABase], false)
                        .setThis(creatureStats)
                        .addParam(targetValue));
                    break;
            }
        }

        public static void SetCreatureSkill(IntPtr pHandle, uint serverCreature, SKILLS skill, byte value)
        {
            var i = new Injector(pHandle);
            var funcLibrary = getFuncLibrary(pHandle);

            if (serverCreature == 0) { return; }

            var creatureStats = getCreatureStats(pHandle, serverCreature);
            int targetValue = (int)value;

            _ = i.runFunction(new RemoteFunction(funcLibrary[Function.CSWSCreatureStats_SetSkillRank], false)
                .setThis(creatureStats)
                .addParam((int)skill)
                .addParam(targetValue));
        }

        public static void AddCreatureFeat(IntPtr pHandle, uint serverCreature, FEATS feat)
        {
            var i = new Injector(pHandle);
            var funcLibrary = getFuncLibrary(pHandle);
            
            if (serverCreature == 0) { return; }

            var creatureStats = getCreatureStats(pHandle, serverCreature);

            _ = i.runFunction(new RemoteFunction(funcLibrary[Function.CSWSCreatureStats_AddFeat], false)
                .setThis(creatureStats)
                .addParam((int)feat));
        }

        public static void ClearCreatureFeats(IntPtr pHandle, uint serverCreature)
        {
            var i = new Injector(pHandle);
            var funcLibrary = getFuncLibrary(pHandle);

            if (serverCreature == 0) { return; }

            var creatureStats = getCreatureStats(pHandle, serverCreature);

            _ = i.runFunction(new RemoteFunction(funcLibrary[Function.CSWSCreatureStats_ClearFeats], false)
                .setThis(creatureStats));
        }

        public static void SetCreatureFeats(IntPtr pHandle, uint serverCreature, List<FEATS> feats)
        {
            var i = new Injector(pHandle);
            var funcLibrary = getFuncLibrary(pHandle);

            if (serverCreature == 0) { return; }

            var creatureStats = getCreatureStats(pHandle, serverCreature);

            _ = i.runFunction(new RemoteFunction(funcLibrary[Function.CSWSCreatureStats_ClearFeats], false)
                .setThis(creatureStats));

            foreach (var feat in feats)
            {
                _ = i.runFunction(new RemoteFunction(funcLibrary[Function.CSWSCreatureStats_AddFeat], false)
                    .setThis(creatureStats)
                    .addParam((int)feat));
            }
        }

        public static void AddCreatureClass(IntPtr pHandle, uint serverCreature, CLASSES newClass)
        {
            var i = new Injector(pHandle);
            var funcLibrary = getFuncLibrary(pHandle);

            if (serverCreature == 0) { return; }

            var creatureStats = getCreatureStats(pHandle, serverCreature);

            _ = i.runFunction(new RemoteFunction(funcLibrary[Function.CSWSCreatureStats_AddClass], false)
                .setThis(creatureStats)
                .addParam((int)newClass)
                .addParam(0));
        }

        public static void AddCreatureExp(IntPtr pHandle, uint serverCreature, uint experience)
        {
            var i = new Injector(pHandle);
            var funcLibrary = getFuncLibrary(pHandle);

            if (serverCreature == 0) { return; }

            var creatureStats = getCreatureStats(pHandle, serverCreature);

            _ = i.runFunction(new RemoteFunction(funcLibrary[Function.CSWSCreatureStats_AddExperience], false)
                .setThis(creatureStats)
                .addParam(experience));
        }

        public static void AddCreatureSpell(IntPtr pHandle, uint serverCreature, byte classIndex, SPELLS spell)
        {
            // NOTE: If a jedi class was your first class use, then classIndex should be 0
            // If it was your second class, the classIndex will be 1
            // While you can apply force powers to non-jedi classes, the results are inconsistent, and not ideal
            
            // NOTE: Not all of the powers on the Spells list work.
            // Item abilities largely do nothing
            // All spells greater than 131 are kotor 2 exclusive

            var i = new Injector(pHandle);
            var funcLibrary = getFuncLibrary(pHandle);

            if (serverCreature == 0) { return; }

            var creatureStats = getCreatureStats(pHandle, serverCreature);

            _ = i.runFunction(new RemoteFunction(funcLibrary[Function.CSWSCreatureStats_AddKnownSpell], false)
                .setThis(creatureStats)
                .addParam((int)classIndex)
                .addParam((int)spell));
        }

        public static void SetCreatureCredits(IntPtr pHandle, uint serverCreature, int amount)
        {
            // NOTE: the game won't allow you to set a credit count above 999999999
            // However, this does accept nagative values, which gives you effectively more credits

            var i = new Injector(pHandle);
            var funcLibrary = getFuncLibrary(pHandle);

            if (serverCreature == 0) { return; }

            _ = i.runFunction(new RemoteFunction(funcLibrary[Function.CSWSCreature_SetGold], false)
                .setThis(serverCreature)
                .addParam(amount));
        }

        /// <param name="pHandle">The Handle of the Kotor process</param>
        /// <param name="text">The text to appear on the pop-up. May encounter odd behvaior if text is more than 512 characters in length</param>
        /// <param name="cancelButton">Should this box has a "Cancel" Button?</param>
        /// <param name="okCallback">The address of the function to run when "ok" is pressed. Must take a `CSWGuiControl *` 
        /// parameter, though it doesn't have to use it. Theorhetically we could make our own functions to run here.</param>
        public static void CreatePopUp(IntPtr pHandle, string text, bool cancelButton = false, uint okCallback = 0xFFFFFFFF) 
        { 
            var i = new Injector(pHandle);
            var funcLibrary = getFuncLibrary(pHandle);

            // Set whether there is a cancel button on the pop-up
            uint messageBox = getMessageBox(pHandle);
            Console.WriteLine("Starting CSWGuiMessageBox_SetAllowCancel");
            _ = i.runFunction(new RemoteFunction(funcLibrary[Function.CSWGuiMessageBox_SetAllowCancel], false)
                .setThis(messageBox)
                .addParam(cancelButton ? 1 : 0));

            // Set a call-back if a function was specified
            Console.WriteLine("Starting CSWGuiMessageBox_SetCallback");
            if (okCallback != 0xFFFFFFFF)
            {
                messageBox = getMessageBox(pHandle);
                _ = i.runFunction(new RemoteFunction(funcLibrary[Function.CSWGuiMessageBox_SetCallback], false)
                .setThis(messageBox)
                .addParam(messageBox) // The parent GUI, can overwrite the callbacks
                .addParam(okCallback) // The callback function, gets called when the user selects "Ok"
                .addParam(0)); // A GUI Control poiunter that gets passed as a parameter to the callback function
            }

            // Create pop-up text string
            Console.WriteLine("Starting operator_new");
            uint message = i.runFunction(new RemoteFunction(funcLibrary[Function.operator_new], true)
                .addParam(text.Length + 1));
            writeStringToMemory(pHandle, message, text, text.Length + 1);

            // Apply the text as the pop-up message 
            Console.WriteLine("Starting CSWGuiMessageBox_SetMessage");
            messageBox = getMessageBox(pHandle);
            _ = i.runFunction(new RemoteFunction(funcLibrary[Function.CSWGuiMessageBox_SetMessage], false)
                .setThis(messageBox)
                .addParam(message) // The message to appear on the pop-up
                .addParam(text.Length)); // The length of the text

            // Add the message box to the active GUI
            Console.WriteLine("Starting CSWGuiManager_AddPanel");
            uint manager = getGuiManager(pHandle);
            _ = i.runFunction(new RemoteFunction(funcLibrary[Function.CSWGuiManager_AddPanel], false)
                .setThis(manager)
                .addParam(messageBox)
                .addParam(1) // Some bit flags here, seems to have some function over bits 0-4 (0-15)
                .addParam(1)); // Controls whether the pop-up plays a sound
        }

        public static int GetGlobalNumber(IntPtr pHandle, string global)
        {
            var i = new Injector(pHandle);
            var funcLibrary = getFuncLibrary(pHandle);
            var om = new ObjManager(pHandle);

            var server = getServer(pHandle);

            if (server == 0)
            {
                return -1;
            }

            // Get Global Variable Table
            uint globalTable = i.runFunction(new RemoteFunction(funcLibrary[Function.CServerExoApp_GetGlobalVariableTable], true)
                .setThis(server));

            // Get Variable Number
            uint label = om.createCExoString(global);
            uint output = om.createBuffer(4);
            _ = (int)i.runFunction(new RemoteFunction(funcLibrary[Function.CSWGlobalVariableTable_GetValueNumber], false)
                .setThis(globalTable)
                .addParam(label) // The CExoString label for this global
                .addParam(output));

            return readIntFromMemory(pHandle, output);
        }

        public static void SetGlobalNumber(IntPtr pHandle, string global, byte value)
        {
            var i = new Injector(pHandle);
            var funcLibrary = getFuncLibrary(pHandle);
            var om = new ObjManager(pHandle);

            var server = getServer(pHandle);

            if (server == 0) { return; }

            // Get Global Variable Table
            uint globalTable = i.runFunction(new RemoteFunction(funcLibrary[Function.CServerExoApp_GetGlobalVariableTable], true)
                .setThis(server));

            // Get Variable Number
            uint label = om.createCExoString(global);
            _ = (int)i.runFunction(new RemoteFunction(funcLibrary[Function.CSWGlobalVariableTable_SetValueNumber], false)
                .setThis(globalTable)
                .addParam(label) // The CExoString label for this global
                .addParam((int)value));
        }

        public static bool? GetGlobalBoolean(IntPtr pHandle, string global)
        {
            var i = new Injector(pHandle);
            var funcLibrary = getFuncLibrary(pHandle);
            var om = new ObjManager(pHandle);

            var server = getServer(pHandle);

            if (server == 0)
            {
                return null;
            }

            // Get Global Variable Table
            uint globalTable = i.runFunction(new RemoteFunction(funcLibrary[Function.CServerExoApp_GetGlobalVariableTable], true)
                .setThis(server));

            // Get Variable Number
            uint label = om.createCExoString(global);
            uint output = om.createBuffer(4);
            _ = (int)i.runFunction(new RemoteFunction(funcLibrary[Function.CSWGlobalVariableTable_GetValueBoolean], false)
                .setThis(globalTable)
                .addParam(label) // The CExoString label for this global
                .addParam(output));

            return readIntFromMemory(pHandle, output) == 1;
        }

        public static void SetGlobalBoolean(IntPtr pHandle, string global, bool value)
        {
            var i = new Injector(pHandle);
            var funcLibrary = getFuncLibrary(pHandle);
            var om = new ObjManager(pHandle);

            var server = getServer(pHandle);

            if (server == 0)
            {
                return;
            }

            // Get Global Variable Table
            uint globalTable = i.runFunction(new RemoteFunction(funcLibrary[Function.CServerExoApp_GetGlobalVariableTable], true)
                .setThis(server));

            // Get Variable Number
            uint label = om.createCExoString(global);
            _ = (int)i.runFunction(new RemoteFunction(funcLibrary[Function.CSWGlobalVariableTable_SetValueBoolean], false)
                .setThis(globalTable)
                .addParam(label) // The CExoString label for this global
                .addParam(value ? 1 : 0));
        }

        /// <summary>
        /// Opens the party selection GUI
        /// </summary>
        /// <param name="pHandle">The Handle of the Kotor process</param>
        /// <param name="forceNpc1">Forces this NPC (if available) into your party, -1 for open choice</param>
        /// <param name="forceNpc2">Forces this NPC into your party, -1 for open choice</param>
        /// <param name="exitScript">NCS script to be run when the menu closes</param>
        public static void ShowPartySelection(IntPtr pHandle, int forceNpc1 = -1, int forceNpc2 = -1, string exitScript = "")
        {
            var i = new Injector(pHandle);
            var funcLibrary = getFuncLibrary(pHandle);
            var om = new ObjManager(pHandle);

            var guiInGame = getInGameGui(pHandle);

            uint script = om.createCExoString(exitScript);
            _ = i.runFunction(new RemoteFunction(funcLibrary[Function.CGuiInGame_ShowPartySelection], false)
                .setThis(guiInGame)
                .addParam(script) // Script to be run when the menu is closed
                .addParam(1) // Requires the user select 'Ok', must be set to 1 to work everywhere
                .addParam(forceNpc1)
                .addParam(forceNpc2));
        }

        public static void ShowItemCreateMenu(IntPtr pHandle)
        {
            var i = new Injector(pHandle);
            var funcLibrary = getFuncLibrary(pHandle);

            var guiInGame = getInGameGui(pHandle);

            _ = i.runFunction(new RemoteFunction(funcLibrary[Function.CGuiInGame_ShowItemCreateMenu], false)
                .setThis(guiInGame));
        }

        public static string GetClientObjectName(IntPtr pHandle, uint clientId)
        {
            var i = new Injector(pHandle);
            var funcLibrary = getFuncLibrary(pHandle);
            var om = new ObjManager(pHandle);

            var client = getClient(pHandle);

            uint name = om.createCExoString("");
            _ = i.runFunction(new RemoteFunction(funcLibrary[Function.CClientExoApp_GetObjectName], false)
                .setThis(client)
                .addParam(clientId)
                .addParam(name));

            return readCExoStringFromMemory(pHandle, name);
        }

        public static string GetServerObjectName(IntPtr pHandle, uint serverId)
        {
            uint clientId = serverToClientId(serverId);
            return GetClientObjectName(pHandle, clientId);
        }
      
        public static int GetPCInfluenceKotor2(IntPtr pHandle, PARTY_NPCS_K2 npc)
        {
            var i = new Injector(pHandle);
            var funcLibrary = getFuncLibrary(pHandle);

            var partyTable = getServerPartyTable(pHandle);

            if (getGameVersion(pHandle) == 1)
            {
                throw new AdapterNotImplementedForGameError("Tried to get Party Influence in Kotor 1. Kotor 1 does not have an influence system");
            }

            return (int)i.runFunction(new RemoteFunction(funcLibrary[Function.CSWPartyTable_GetInfluence], true)
                .setThis(partyTable)
                .addParam((int)npc));
        }

        public static void SetPCInfluenceKotor2(IntPtr pHandle, PARTY_NPCS_K2 npc, int influence)
        {
            var i = new Injector(pHandle);
            var funcLibrary = getFuncLibrary(pHandle);

            var partyTable = getServerPartyTable(pHandle);

            if (getGameVersion(pHandle) == 1)
            {
                throw new AdapterNotImplementedForGameError("Tried to set Party Influence in Kotor 1. Kotor 1 does not have an influence system");
            }

            _ = i.runFunction(new RemoteFunction(funcLibrary[Function.CSWPartyTable_SetInfluence], false)
                .setThis(partyTable)
                .addParam((int)npc)
                .addParam(influence));
        }

        /// <summary>
        /// Enables Fog in the current scene.
        /// </summary>
        /// <param name="pHandle">The running kotor process</param>
        /// <param name="rangeStart">The distance from the camera the Fog gradient will start</param>
        /// <param name="rangeEnd">The distance fromt the camera the Fog gradient will end</param>
        /// <param name="colorR">Red component of the fog color</param>
        /// <param name="colorG">Green Component of the fog color</param>
        /// <param name="colorB">Blue component of the fog color</param>
        /// <remarks>
        /// The fog is fully transparent at `rangeStart` from the camera, and opaque at `rangeEnd` from the camera.
        /// If `rangeEnd` < `rangeStart`, the fog will be dense near the camera, and fade out at further distances.
        /// If `rangeEnd` = `rangeStart`, the behavior is undefined, and graphical bugs may occur
        /// </remarks>
        public static void SetSceneFogOn(IntPtr pHandle, float rangeStart, float rangeEnd, float colorR = 0.5f, float colorG = 0.5f, float colorB = 0.5f)
        {
            var i = new Injector(pHandle);
            var funcLibrary = getFuncLibrary(pHandle);
            var om = new ObjManager(pHandle);

            uint scene = getCurrentScene(pHandle);
            var color = om.createVector(colorR, colorG, colorB);

            i.runFunction(new RemoteFunction(funcLibrary[Function.Scene_SetFog])
                .setThis(scene)
                .addParam(1));

            i.runFunction(new RemoteFunction(funcLibrary[Function.Scene_SetFogRange])
               .setThis(scene)
               .addParam(rangeStart)
               .addParam(rangeEnd));

            i.runFunction(new RemoteFunction(funcLibrary[Function.Scene_SetFogColor])
               .setThis(scene)
               .addParam(color));
        }

        public static void SetSceneFogOff(IntPtr pHandle)
        {
            var i = new Injector(pHandle);
            var funcLibrary = getFuncLibrary(pHandle);

            uint scene = getCurrentScene(pHandle);

            i.runFunction(new RemoteFunction(funcLibrary[Function.Scene_SetFog])
                .setThis(scene)
                .addParam(0));
        }

        public static void SetVisibilityGraph(IntPtr pHandle, bool on) 
        {
            var i = new Injector(pHandle);
            var funcLibrary = getFuncLibrary(pHandle);

            uint scene = getCurrentScene(pHandle);

            if (on)
            {
                i.runFunction(new RemoteFunction(funcLibrary[Function.Scene_EnableVisibilityGraph])
                    .setThis(scene));
            }
            else
            {
                i.runFunction(new RemoteFunction(funcLibrary[Function.Scene_DisableVisibilityGraph])
                   .setThis(scene));
            }
        }
        #endregion
    }
}

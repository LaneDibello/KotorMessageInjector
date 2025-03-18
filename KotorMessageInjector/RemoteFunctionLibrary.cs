using System;
using System.Collections.Generic;
using System.Text;

namespace KotorMessageInjector
{
    public enum Function
    {
        CFactionManager_GetFaction,
        CServerExoApp_SetMoveToModulePending,
        CServerExoApp_SetMoveToModuleString,
        CServerExoAppInternal_GetModule,
        CSWPartyTable_AddNPC,
        CSWSCreature_CSWSCreature,
        CSWSCreature_AddToArea,
        CSWSCreature_LoadFromTemplate,
        CSWSFaction_AddMember,
        CSWSModule_GetArea,
        Gob_AttachToScene,
        Gob_EnableAlwaysRender,
        Gob_ReplaceTexture,
        Gob_SetColorShifting,
        Gob_SetIllumination,
        Gob_SetObjectScale,
        Gob_SetPosition,
        Gob_SetScene,
        Gob_TurnOffShadows,
        NewCAurObject,
        Operator_New,
        Scene_AddObject,
    }

    public static class RemoteFunctionLibrary
    {
        public static Dictionary<Function, uint> k1Functions = new Dictionary<Function, uint>()
        {
            {Function.Operator_New, 0x006fa7e6},
            {Function.CSWSCreature_CSWSCreature, 0x004f7a10},
            {Function.CSWSCreature_LoadFromTemplate, 0x005026d0},
            {Function.CServerExoAppInternal_GetModule, 0x004b14f0},
            {Function.CSWSModule_GetArea, 0x004c30f0},
            {Function.CSWSCreature_AddToArea, 0x004fa100},
            {Function.CSWPartyTable_AddNPC, 0x005645f0},
            {Function.CServerExoApp_SetMoveToModuleString, 0x004aecd0},
            {Function.CServerExoApp_SetMoveToModulePending, 0x004aecc0},
            {Function.CFactionManager_GetFaction, 0x0052b3b0},
            {Function.CSWSFaction_AddMember, 0x005bfa70},

            {Function.NewCAurObject, 0x00449cc0},
            {Function.Scene_AddObject, 0x00458bd0},
            {Function.Gob_SetPosition, 0x0043f0c0},
            {Function.Gob_SetScene, 0x0043f200},
            {Function.Gob_AttachToScene, 0x0044f7c0},

            {Function.Gob_SetColorShifting, 0x0043ee50},
            {Function.Gob_EnableAlwaysRender, 0x0043e860},
            {Function.Gob_SetObjectScale, 0x00444d90},
            {Function.Gob_ReplaceTexture, 0x00446520},
            {Function.Gob_TurnOffShadows, 0x00449a50},
            {Function.Gob_SetIllumination, 0x0043eea0},
        };

        public static Dictionary<string, uint> kotor1Functions = new Dictionary<string, uint>()
        {
            {"operator_new", 0x006fa7e6},
            {"CSWSCreature::CSWSCreature", 0x004f7a10},
            {"CSWSCreature::LoadFromTemplate", 0x005026d0},
            {"CServerExoAppInternal::GetModule", 0x004b14f0},
            {"CSWSModule::GetArea", 0x004c30f0},
            {"CSWSCreature::AddToArea", 0x004fa100},
            {"CSWPartyTable::AddNPC", 0x005645f0},
            {"CServerExoApp::SetMoveToModuleString", 0x004aecd0},
            {"CServerExoApp::SetMoveToModulePending", 0x004aecc0},
            {"CFactionManager::GetFaction", 0x0052b3b0},
            {"CSWSFaction::AddMember", 0x005bfa70},

            {"NewCAurObject", 0x00449cc0},
            {"Scene::AddObject", 0x00458bd0},
            {"Gob::SetPosition", 0x0043f0c0},
            {"Gob::SetScene", 0x0043f200},
            {"Gob::AttachToScene", 0x0044f7c0},

            {"Gob::SetColorShifting", 0x0043ee50},
            {"Gob::EnableAlwaysRender", 0x0043e860},
            {"Gob::SetObjectScale", 0x00444d90},
            {"Gob::ReplaceTexture", 0x00446520},
            {"Gob::TurnOffShadows", 0x00449a50},
            {"Gob::SetIllumination", 0x0043eea0},
        };

        public static Dictionary<Function, uint> k2Functions = new Dictionary<Function, uint>()
        {
            {Function.Operator_New, 0x00921347},
            {Function.CSWSCreature_CSWSCreature, 0x0067ab60},
            {Function.CSWSCreature_LoadFromTemplate, 0x0068b5a0},
            {Function.CServerExoAppInternal_GetModule, 0x007b2840},
            {Function.CSWSModule_GetArea, 0x0072a6a0},
            {Function.CSWSCreature_AddToArea, 0x00681800},
            {Function.CSWPartyTable_AddNPC, 0x00700170},
            {Function.CServerExoApp_SetMoveToModuleString, 0x0064b8c0},
            {Function.CServerExoApp_SetMoveToModulePending, 0x0064b870},
            {Function.CFactionManager_GetFaction, 0x007ef020},
            {Function.CSWSFaction_AddMember, 0x007e4850},
        };

        public static Dictionary<string, uint> kotor2Functions = new Dictionary<string, uint>()
        {
            {"operator_new", 0x00921347},
            {"CSWSCreature::CSWSCreature", 0x0067ab60},
            {"CSWSCreature::LoadFromTemplate", 0x0068b5a0},
            {"CServerExoAppInternal::GetModule", 0x007b2840},
            {"CSWSModule::GetArea", 0x0072a6a0},
            {"CSWSCreature::AddToArea", 0x00681800},
            {"CSWPartyTable::AddNPC", 0x00700170},
            {"CServerExoApp::SetMoveToModuleString", 0x0064b8c0},
            {"CServerExoApp::SetMoveToModulePending", 0x0064b870},
            {"CFactionManager::GetFaction", 0x007ef020},
            {"CSWSFaction::AddMember", 0x007e4850},
        };

        public static Dictionary<Function, uint> k2SteamFunctions = new Dictionary<Function, uint>()
        {
            {Function.Operator_New, 0x00919723},
            {Function.CSWSCreature_CSWSCreature, 0x00561f30},
            {Function.CSWSCreature_LoadFromTemplate, 0x00572a70},
            {Function.CServerExoAppInternal_GetModule, 0x0052f4c0},
            {Function.CSWSModule_GetArea, 0x00557ef0},
            {Function.CSWSCreature_AddToArea, 0x00568bd0},
            {Function.CSWPartyTable_AddNPC, 0x005fa600},
            {Function.CServerExoApp_SetMoveToModuleString, 0x0051bea0},
            {Function.CServerExoApp_SetMoveToModulePending, 0x0051be50},
            {Function.CFactionManager_GetFaction, 0x00664490},
            {Function.CSWSFaction_AddMember, 0x006d3330},
        };

        public static Dictionary<string, uint> kotor2SteamFunctions = new Dictionary<string, uint>()
        {
            {"operator_new", 0x00919723},
            {"CSWSCreature::CSWSCreature", 0x00561f30},
            {"CSWSCreature::LoadFromTemplate", 0x00572a70},
            {"CServerExoAppInternal::GetModule", 0x0052f4c0},
            {"CSWSModule::GetArea", 0x00557ef0},
            {"CSWSCreature::AddToArea", 0x00568bd0},
            {"CSWPartyTable::AddNPC", 0x005fa600},
            {"CServerExoApp::SetMoveToModuleString", 0x0051bea0},
            {"CServerExoApp::SetMoveToModulePending", 0x0051be50},
            {"CFactionManager::GetFaction", 0x00664490},
            {"CSWSFaction::AddMember", 0x006d3330},
        };
    }
}

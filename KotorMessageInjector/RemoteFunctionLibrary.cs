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
        Gob_SetOrientation,
        Gob_SetScene,
        Gob_TurnOffShadows,
        NewCAurObject,
        operator_new, // Lowercased as this is the C++ `new` operator, and not an actual function
        Scene_AddObject,
        YawPitchRoll,
        CClientExoApp_GetGameObject,
    }

    public static class RemoteFunctionLibrary
    {
        public static Dictionary<Function, uint> k1Functions = new Dictionary<Function, uint>()
        {
            {Function.operator_new, 0x006fa7e6},
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
            {Function.Gob_SetOrientation, 0x0043f0f0},
            {Function.Gob_SetScene, 0x0043f200},
            {Function.Gob_AttachToScene, 0x0044f7c0},

            {Function.Gob_SetColorShifting, 0x0043ee50},
            {Function.Gob_EnableAlwaysRender, 0x0043e860},
            {Function.Gob_SetObjectScale, 0x00444d90},
            {Function.Gob_ReplaceTexture, 0x00446520},
            {Function.Gob_TurnOffShadows, 0x00449a50},
            {Function.Gob_SetIllumination, 0x0043eea0},

            {Function.YawPitchRoll, 0x004acac0},
            {Function.CClientExoApp_GetGameObject, 0x005ed580}

        };

        public static Dictionary<Function, uint> k2Functions = new Dictionary<Function, uint>()
        {
            {Function.operator_new, 0x00921347},
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

            {Function.NewCAurObject, 0x008548b0},
            {Function.Gob_SetPosition, 0x00853a20},
            {Function.Gob_SetOrientation, 0x00853a70},
            {Function.Gob_AttachToScene, 0x0085f680},
            {Function.Gob_SetColorShifting, 0x0084ed30},
            {Function.Gob_SetObjectScale, 0x00855830},
            {Function.Gob_TurnOffShadows, 0x0084be30},
            {Function.Gob_SetIllumination, 0x0084ed90},

            {Function.YawPitchRoll, 0x0080d4b0},
        };

        public static Dictionary<Function, uint> k2SteamFunctions = new Dictionary<Function, uint>()
        {
            {Function.operator_new, 0x00919723},
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

            {Function.NewCAurObject, 0x00462320},
            {Function.Gob_SetPosition, 0x00461490},
            {Function.Gob_SetOrientation, 0x004614e0},
            {Function.Gob_AttachToScene, 0x0046d440},
            {Function.Gob_SetColorShifting, 0x0045c750},
            {Function.Gob_SetObjectScale, 0x00463340},
            {Function.Gob_TurnOffShadows, 0x00459850},
            {Function.Gob_SetIllumination, 0x0045c7b0},

            {Function.YawPitchRoll, 0x00515620},
        };
    }
}

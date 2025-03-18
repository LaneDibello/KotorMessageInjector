using System;
using System.Collections.Generic;
using System.Text;

namespace KotorMessageInjector
{
    public static class RemoteFunctionLibrary
    {
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
            {"Gob::SetPosition", 0x0043f0c0},
            {"Gob::AttachToScene", 0x0044f7c0},
            {"Gob::SetColorShifting", 0x0043ee50},
            {"Gob::SetObjectScale", 0x00444d90},
            {"Gob::TurnOffShadows", 0x00449a50},
            {"Gob::SetIllumination", 0x0043eea0},

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

            {"NewCAurObject", 0x008548b0},
            {"Gob::SetPosition", 0x00853a20},
            {"Gob::AttachToScene", 0x0085f680},
            {"Gob::SetColorShifting", 0x0084ed30},
            {"Gob::SetObjectScale", 0x00855830},
            {"Gob::TurnOffShadows", 0x0084be30},
            {"Gob::SetIllumination", 0x0084ed90},
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

            {"NewCAurObject", 0x00000000},
            {"Gob::SetPosition", 0x00000000},
            {"Gob::AttachToScene", 0x00000000},
            {"Gob::SetColorShifting", 0x00000000},
            {"Gob::SetObjectScale", 0x00000000},
            {"Gob::TurnOffShadows", 0x00000000},
            {"Gob::SetIllumination", 0x00000000},
        };
    }
}

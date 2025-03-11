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
            {"CServerExoApp::SetMoveToModulePending", 0x004aecc0}

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
            {"CServerExoApp::SetMoveToModulePending", 0x0064b870}
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
            {"CServerExoApp::SetMoveToModulePending", 0x0051be50}
        };
    }
}

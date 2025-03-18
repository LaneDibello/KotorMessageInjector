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
            IntPtr pHandle = getRunningKotor();

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

            ////Game keeps running even when you click outs
            //disableClickOutPausing(pHandle);

            //Message msg;

            //msg = Examples.freeCam();

            //Console.WriteLine($"Sending Message:\n{msg}");

            //i.sendMessage(msg);

            //IntPtr targetAddress = (IntPtr)0x00989ee0; // Kotor 2 steam

            //uint oldProtect;
            //ProcessAPI.VirtualProtectEx(pHandle, targetAddress, 8, ProcessAPI.PAGE_READWRITE, out oldProtect);

            //ProcessAPI.WriteProcessMemory(pHandle, targetAddress, BitConverter.GetBytes(100.0), 8, out UIntPtr _);

            //ProcessAPI.VirtualProtectEx(pHandle, targetAddress, 8, oldProtect, out uint _);

            //uint retval = Examples.addParty(pHandle, "c_hutt", 2);
            //Console.WriteLine($"Returned: {retval}");

            //Examples.spawnCreature(pHandle, "p_kreia", 33f, -31f, 0.0f);
            //i.sendMessage(Examples.freeCam());

            //Examples.warp(pHandle, "301NAR");

            //Examples.changeFaction(pHandle, lookingAtServerId, 1);

            // DLZ Marker Example
            //uint gob = Examples.drawModel
            //(
            //    pHandle, "gi_sound_pos", 0.1f,
            //    114.890533f, 122.25f, 0f
            //);
            //Examples.colorizeModel(pHandle, gob, 1f, 1f, 1f);

            //Good candidates:
            // gi_waypoint01 - 04
            // fx_flame01
            // gi_sound_pos
            // plc_marker
            // or_cone01
            // plc_sign

            //string model = "plc_marker";
            //float scale = 10.0f;

            //uint gob = Examples.drawModel
            //(
            //    pHandle, model, scale, 
            //    112.5f, 154.0f, 0f
            //);

            //Examples.colorizeModel(pHandle, gob, 1f, 0f, 0f);

            //gob = Examples.drawModel
            //(
            //    pHandle, model, scale,
            //    112.5f, 159.0f, 0f
            //);

            //Examples.colorizeModel(pHandle, gob, 0f, 1f, 0f);

            //gob = Examples.drawModel
            //(
            //    pHandle, model, scale,
            //    112.5f, 164.0f, 0f
            //);

            //Examples.colorizeModel(pHandle, gob, 0f, 0f, 1f);

            //gob = Examples.drawModel
            //(
            //    pHandle, model, scale,
            //    112.5f, 169.0f, 0f
            //);

            //Examples.colorizeModel(pHandle, gob, 0f, 0f, 0f);

            //gob = Examples.drawModel
            //(
            //    pHandle, model, scale,
            //    112.5f, 174.0f, 0f
            //);

            //Examples.colorizeModel(pHandle, gob, 1f, 1f, 1f);

            //gob = Examples.drawModel
            //(
            //    pHandle, model, scale,
            //    112.5f, 179.0f, 0f
            //);

            //Examples.colorizeModel(pHandle, gob, 0.25f, 0.25f, 0.25f);

            //gob = Examples.drawModel
            //(
            //    pHandle, model, scale,
            //    112.5f, 184.0f, 0f
            //);

            //Examples.colorizeModel(pHandle, gob, 1f, 1f, 0f);

            //gob = Examples.drawModel
            //(
            //    pHandle, model, scale,
            //    112.5f, 189.0f, 0f
            //);

            //Examples.colorizeModel(pHandle, gob, 0f, 1f, 1f);

            //gob = Examples.drawModel
            //(
            //    pHandle, model, scale,
            //    112.5f, 194.0f, 0f
            //);

            //Examples.colorizeModel(pHandle, gob, 1f, 0f, 1f);

            // Exchange DLZ Marker Example
            uint gob = Examples.drawModel
            (
                pHandle, "gi_sound_pos", 0.1f,
                -12.720751f, -19.7f, 0f
            );
            Examples.colorizeModel(pHandle, gob, 1f, 0f, 0f);

            //uint gob = Examples.drawModel
            //(
            //    pHandle, "gi_sound_pos", 1f,
            //    112.5f, 154.0f, 0.5f
            //);

            //Examples.colorizeModel(pHandle, gob, 0f, 1f, 0f);

            Adapter.RotateModel(pHandle, gob, 0.0f, 0.0f, 0.0f);

            Console.ReadKey();

            Examples.deleteModel(pHandle, gob);
        }
    }
}

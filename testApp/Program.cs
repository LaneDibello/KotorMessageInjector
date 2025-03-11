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

            ////Game keeps running even when you click out
            //disableClickOutPausing(pHandle);

            Message msg;

            msg = Examples.invulnerability();

            //Console.WriteLine($"Sending Message:\n{msg}");

            //i.sendMessage(msg);

            //uint retval = Examples.addParty(pHandle, "c_hutt", 2);
            //Console.WriteLine($"Returned: {retval}");

            Examples.spawnCreature(pHandle, "p_kreia", 33f, -31f, 0.0f);
            //i.sendMessage(Examples.freeCam());

            //Examples.warp(pHandle, "301NAR");

            //Console.ReadKey();
        }
    }
}

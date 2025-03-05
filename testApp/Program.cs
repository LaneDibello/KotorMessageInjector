using KotorMessageInjector;

namespace testApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Injector i = new Injector("swkotor.exe");

            Message msg = new Message(Message.PlayerMessageTypes.GAME_OBJ_UPDATE, 1, false);
            msg.writeByte(0x55);
            msg.writeByte(0x5);
            uint id = KotorHelpers.getPlayerClientID(i.processHandle);
            msg.writeUint(id);
            msg.writeUint(1); //Bit Flags
            msg.writeFloat(110.087608f);
            msg.writeFloat(89.0f);
            msg.writeFloat(0.0f);

            //Message msg = new Message(Message.PlayerMessageTypes.CHEAT, 2);

            Console.WriteLine($"Sending Message:\n{msg}");
            
            i.sendMessage(msg);
            
            Console.ReadKey();
        }
    }
}

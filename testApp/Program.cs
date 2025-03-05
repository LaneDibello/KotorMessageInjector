using KotorMessageInjector;

namespace testApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Message msg = new Message(Message.PlayerMessageTypes.GAME_OBJ_UPDATE, 1, false);
            msg.writeByte(0x55);
            msg.writeByte(0x5);
            msg.writeUint(0xFFFFFFFD);
            msg.writeUint(1); //Bit Flags
            msg.writeFloat(130.0f);
            msg.writeFloat(130.0f);
            msg.writeFloat(0.0f);

            //Message msg = new Message(Message.PlayerMessageTypes.CHEAT, 2);

            Injector i = new Injector("swkotor.exe");

            Console.WriteLine($"Sending Message:\n{msg}");
            i.sendMessage(msg);
            
            Console.ReadKey();
        }
    }
}

using KotorMessageInjector;

namespace testApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Message msg = new Message(Message.PlayerMessageTypes.CHEAT, 2);
            Injector i = new Injector("swkotor.exe");

            Console.WriteLine($"Sending Message:\n{msg}");
            i.sendMessage(msg);
            
            Console.ReadKey();
        }
    }
}

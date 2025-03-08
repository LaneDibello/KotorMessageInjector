using System;
using System.Collections.Generic;
using System.Text;

namespace KotorMessageInjector
{
    public class KotorMessenger : Injector
    {
        private Queue<Message> messages;

        public KotorMessenger(string procName = "swkotor.exe") : base(procName)
        {
            messages = new Queue<Message>();
        }

        public void sendNextMessage(int count = 1)
        {
            if (count > messages.Count)
            {
                count = messages.Count;
            }
            
            for (int i = 0; i < count; i++)
            {
                Message msg = messages.Dequeue();
                sendMessage(msg);
            }
        }

        public void sendAllMessages()
        {
            sendNextMessage(int.MaxValue);
        }

        public void pushMessage(Message msg)
        {
            messages.Enqueue(msg);
        }
    }
}

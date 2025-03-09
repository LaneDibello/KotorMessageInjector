using System;
using static KotorMessageInjector.ProcessAPI;

namespace KotorMessageInjector
{
    public class Injector
    {
        private const uint MESSAGE_SPACE = 0x400; // 1 kb
        private const uint SHELLCODE_SPACE = 0x100; // 256 b

        private IntPtr processHandle;
        private IntPtr remoteMessageData;
        private IntPtr remoteShellcode;

        public Injector(IntPtr processHandle)
        {
            this.processHandle = processHandle;

            remoteMessageData = VirtualAllocEx
            (
                processHandle,
                (IntPtr)null,
                MESSAGE_SPACE,
                MEM_COMMIT | MEM_RESERVE,
                PAGE_READWRITE
            );

            remoteShellcode = VirtualAllocEx
            (
                processHandle,
                (IntPtr)null,
                SHELLCODE_SPACE,
                MEM_COMMIT | MEM_RESERVE,
                PAGE_EXECUTE_READWRITE
            );

            KotorHelpers.reverseLoadBar(processHandle);
        }

        public Injector(string procName) : this(OpenProcessByName(procName)) { }

        ~Injector() 
        {
            VirtualFreeEx(processHandle, remoteMessageData, 0, MEM_RELEASE);
            VirtualFreeEx(processHandle, remoteShellcode, 0, MEM_RELEASE);
            CloseHandle(processHandle);
        }

        public void sendMessage (Message msg)
        {
            // Resolve Game version
            bool isSteam;
            int gameVersion = KotorHelpers.getGameVersion(processHandle, out isSteam);

            // Write out Message and shellcode
            UIntPtr bytesWritten;
            WriteProcessMemory(processHandle, remoteMessageData, msg.message, msg.length, out bytesWritten);
            SendMessageShellcode shellcode = new SendMessageShellcode(msg, remoteMessageData, gameVersion, isSteam);
            WriteProcessMemory(processHandle, remoteShellcode, shellcode.code, shellcode.length, out bytesWritten);

            //Send Message
            uint threadId;
            IntPtr thread = CreateRemoteThread(processHandle, (IntPtr)0, 0, remoteShellcode, (IntPtr)0, 0, out threadId);

            //Wait for thread to finish
            WaitForSingleObject(thread, INFINITE);

            //Clean up
            CloseHandle(thread);
        }

    }
}

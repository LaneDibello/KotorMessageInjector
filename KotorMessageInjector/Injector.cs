using System;
using System.Runtime.InteropServices;

namespace KotorMessageInjector
{
    public class Injector
    {
        private const uint MESSAGE_SPACE = 0x400; // 1 kb
        private const uint SHELLCODE_SPACE = 0x100; // 256 b

        public IntPtr processHandle;
        private IntPtr remoteMessageData;
        private IntPtr remoteShellcode;

        public Injector(uint procId)
        {
            processHandle = ProcessAPI.OpenProcess(
                ProcessAPI.PROCESS_ALL_ACCESS,
                false,
                procId
            );

            remoteMessageData = ProcessAPI.VirtualAllocEx(
                processHandle, 
                (IntPtr)null, 
                MESSAGE_SPACE, 
                ProcessAPI.MEM_COMMIT | ProcessAPI.MEM_RESERVE, 
                ProcessAPI.PAGE_READWRITE
            );

            remoteShellcode = ProcessAPI.VirtualAllocEx(
                processHandle,
                (IntPtr)null,
                SHELLCODE_SPACE,
                ProcessAPI.MEM_COMMIT | ProcessAPI.MEM_RESERVE,
                ProcessAPI.PAGE_EXECUTE_READWRITE
            );
        }

        public Injector(string procName) : this(ProcessAPI.GetProcessId(procName)) { }

        public void sendMessage (Message msg)
        {
            // Write out Message and shellcode
            UIntPtr bytesWritten;
            ProcessAPI.WriteProcessMemory(processHandle, remoteMessageData, msg.message, msg.length, out bytesWritten);
            SendMessageShellcode shellcode = new SendMessageShellcode(msg, remoteMessageData);
            ProcessAPI.WriteProcessMemory(processHandle, remoteShellcode, shellcode.code, shellcode.length, out bytesWritten);

            //Send Message
            uint threadId;
            IntPtr thread = ProcessAPI.CreateRemoteThread(processHandle, (IntPtr)0, 0, remoteShellcode, (IntPtr)0, 0, out threadId);

            //Wait for thread to finish
            ProcessAPI.WaitForSingleObject(thread, ProcessAPI.INFINITE);

            //Clean up
            ProcessAPI.CloseHandle(thread);
        }

    }
}

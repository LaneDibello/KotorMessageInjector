using System;
using System.Threading;
using static KotorMessageInjector.ProcessAPI;

namespace KotorMessageInjector
{
    public class InjectedFunctionFailedException : Exception 
    {
        public InjectedFunctionFailedException(string message) : base(message) { }
    }
    
    public class Injector
    {
        private const uint MESSAGE_SPACE = 0x400; // 1 kb
        private const uint SHELLCODE_SPACE = 0x100; // 256 b

        internal IntPtr processHandle;
        private IntPtr remoteMessageData;
        private IntPtr remoteShellcode;

        /// <summary>
        /// Creates an instance of Injector targetting the process opened by processHandle
        /// </summary>
        /// <param name="processHandle">An open 32-bit Windows Process</param>
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

        /// <summary>
        /// Creates an instance of Injector targetting the process named procName
        /// </summary>
        /// <param name="procName">The name of the process to be opened and injected to</param>
        public Injector(string procName) : this(OpenProcessByName(procName)) { }

        /// <summary>
        /// Necessary for freeing the remote memory, and preventing dangling handles
        /// </summary>
        ~Injector()
        {
            VirtualFreeEx(processHandle, remoteMessageData, 0, MEM_RELEASE);
            VirtualFreeEx(processHandle, remoteShellcode, 0, MEM_RELEASE);
            CloseHandle(processHandle);
        }

        /// <summary>
        /// Constructs and sends a message to the attached KotOR Process
        /// </summary>
        /// <param name="msg">The message to be sent to the KotOR Message Handler</param>
        public void sendMessage(Message msg)
        {
            // Resolve Game version
            bool isSteam;
            int gameVersion = KotorHelpers.getGameVersion(processHandle, out isSteam);

            // Write out Message and shellcode
            WriteProcessMemory(processHandle, remoteMessageData, msg.message, msg.length, out _);
            SendMessageShellcode shellcode = new SendMessageShellcode(msg, remoteMessageData, gameVersion, isSteam);
            WriteProcessMemory(processHandle, remoteShellcode, shellcode.code, shellcode.length, out _);

            //Send Message
            uint threadId;
            IntPtr thread = CreateRemoteThread(processHandle, (IntPtr)0, 0, remoteShellcode, (IntPtr)0, 0, out threadId);

            //Wait for thread to finish
            WaitForSingleObject(thread, INFINITE);

            //Clean up
            CloseHandle(thread);
            WriteProcessMemory(processHandle, remoteShellcode, (byte[])Array.CreateInstance(typeof(byte), shellcode.length), shellcode.length, out _);
            WriteProcessMemory(processHandle, remoteMessageData, (byte[])Array.CreateInstance(typeof(byte), msg.length), msg.length, out _);
        }

        public uint runFunction(RemoteFunction func)
        {
            // Build Function Call
            RunFunctionShellcode shellcode = new RunFunctionShellcode(func.functionAddress);
            shellcode.setECX(func.thisPointer);
            if (func.shouldReturn)
            {
                shellcode.setReturnStorage(remoteMessageData);
            }
            foreach (var (param, size) in func.parameters)
            {
                shellcode.addParam(param, size);
            }
            byte[] code = shellcode.generateShellcode();
            WriteProcessMemory(processHandle, remoteShellcode, code, (uint)code.Length, out _);

            // Run Thread
            IntPtr thread = CreateRemoteThread(processHandle, (IntPtr)0, 0, remoteShellcode, (IntPtr)0, 0, out _);

            //Wait for thread to finish
            uint result = WaitForSingleObject(thread, INFINITE);
            
            if (result == 0xFFFFFFFF)
            {
                Console.WriteLine($"Function failed with {GetLastError()}");
            }


            //Clean up
            CloseHandle(thread);

            // Return
            if (func.shouldReturn)
            {
                byte[] outBytes = new byte[4];
                ReadProcessMemory(processHandle, remoteMessageData, outBytes, 4, out _);
                return BitConverter.ToUInt32(outBytes, 0);
            }

            Array.Clear(code, 0, code.Length);
            WriteProcessMemory(processHandle, remoteShellcode, code, (uint)code.Length, out _);

            return 0;
        }

    }
}

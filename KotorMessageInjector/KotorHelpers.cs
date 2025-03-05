using System;

namespace KotorMessageInjector
{
    public static class KotorHelpers
    {
        private static IntPtr KOTOR_1_APPMANAGER = (IntPtr)0x007a39fc;
        
        private static uint getClientInternal(IntPtr processHandle)
        {
            byte[] outBytes = new byte[4];
            UIntPtr outPtr;

            ProcessAPI.ReadProcessMemory(processHandle, KOTOR_1_APPMANAGER, outBytes, 4, out outPtr);
            uint appmanager = BitConverter.ToUInt32(outBytes, 0);

            ProcessAPI.ReadProcessMemory(processHandle, (IntPtr)(appmanager + 4), outBytes, 4, out outPtr);
            uint client = BitConverter.ToUInt32(outBytes, 0);

            ProcessAPI.ReadProcessMemory(processHandle, (IntPtr)(client + 4), outBytes, 4, out outPtr);
            return BitConverter.ToUInt32(outBytes, 0);
        }

        public static uint getPlayerClientID(IntPtr processHandle)
        {
            byte[] outBytes = new byte[4];
            UIntPtr outPtr;


            uint clientInternal = getClientInternal(processHandle);

            ProcessAPI.ReadProcessMemory(processHandle, (IntPtr)(clientInternal + 0x20), outBytes, 4, out outPtr);
            return BitConverter.ToUInt32(outBytes, 0);

        }

        public static uint getLookingAtClientID(IntPtr processHandle)
        {
            byte[] outBytes = new byte[4];
            UIntPtr outPtr;


            uint clientInternal = getClientInternal(processHandle);

            ProcessAPI.ReadProcessMemory(processHandle, (IntPtr)(clientInternal + 0x2B4), outBytes, 4, out outPtr);
            return BitConverter.ToUInt32(outBytes, 0);

        }
    }
}

using System;
using System.Collections.Generic;

namespace KotorMessageInjector
{
    class RunFunctionShellcode : Shellcode
    {
        private List<byte> header = new List<byte>{
            0x55,                         // push ebp
            0x8B, 0xEC,                   // mov ebp, esp
        };

        private static List<byte> footer = new List<byte>
        {
            0x8B, 0xE5,                   // mov esp, ebp
            0x5D,                         // pop ebp
            0xC3                          // ret
        };

        private uint functionAdress;
        private uint returnStorage = 0;
        private uint inECX = 0;
        private Stack<paramEntry> parameters;

        public byte[] generateShellcode()
        {
            // Set up Base Pointer & Stack Pointer
            shellcode.AddRange(header);

            // Set the value in `this`
            movECX4Bytes(inECX);

            // Push parameters onto the stack
            uint paramCount = (uint)parameters.Count;
            for (int i = 0; i < parameters.Count; i++)
            {
                paramEntry pe = parameters.Pop();
                if (pe.size == 4)
                {
                    push4Bytes(pe.param);
                }
                else if (pe.size == 2)
                {
                    push2Bytes((ushort)pe.param);
                }
                else if (pe.size == 1)
                {
                    push1Byte((byte)pe.param);
                }
                else
                {
                    throw new ArgumentException($"Unrecognized Parameter: {pe.param} | Size {pe.size}");
                }
            }

            // Set up function to be called
            movEAX4Bytes(functionAdress);

            // Call the function
            callEAX();

            // If we want a return value from this, get it from EAX
            if (returnStorage != 0)
            {
                movMemoryFromEAX(returnStorage);
            }

            // Clean up Params
            addESP(4 * paramCount);

            // Clean up and return
            shellcode.AddRange(footer);

            return shellcode.ToArray();
        }
        
        struct paramEntry
        {
            internal uint param;
            internal uint size;
        }

        public RunFunctionShellcode(uint function)
        {
            functionAdress = function;
        }

        public void setECX(uint ecx)
        {
            inECX = ecx;
        }

        public void setReturnStorage(IntPtr address)
        {
            returnStorage = (uint)address;
        }

        public void addParam(uint param)
        {
            paramEntry pe;
            pe.param = param;
            pe.size = 4;

            parameters.Push(pe);
        }

        public void addParam(float param)
        {
            paramEntry pe;
            pe.param = BitConverter.ToUInt32(BitConverter.GetBytes(param), 0);
            pe.size = 4;

            parameters.Push(pe);
        }

        public void addParam(ushort param)
        {
            paramEntry pe;
            pe.param = param;
            pe.size = 2;

            parameters.Push(pe);
        }

        public void addParam(byte param)
        {
            paramEntry pe;
            pe.param = param;
            pe.size = 1;

            parameters.Push(pe);
        }

    }
}

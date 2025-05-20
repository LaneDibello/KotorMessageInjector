using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using static KotorMessageInjector.ProcessAPI;
using static System.BitConverter;

namespace KotorMessageInjector
{
    public class ObjManager
    {
        private IntPtr processHandle;
        private IntPtr remoteObjects;
        private IntPtr remoteIndex;
        private uint remoteSize;
        private uint remoteCapacity;

        public ObjManager(IntPtr processHandle, uint remoteCapacity = 0x2000)
        {
            this.processHandle = processHandle;
            this.remoteCapacity = remoteCapacity;
            remoteObjects = VirtualAllocEx
            (
                processHandle,
                (IntPtr)null,
                remoteCapacity,
                MEM_COMMIT | MEM_RESERVE,
                PAGE_READWRITE
            );
            remoteIndex = remoteObjects;
            remoteSize = 0;
        }

        ~ObjManager()
        {
            VirtualFreeEx(processHandle, remoteObjects, 0, MEM_RELEASE);
        }

        private void checkForOverflow()
        {
            if (remoteSize > remoteCapacity)
            {
                throw new OutOfMemoryException
                (
                    $"There is not enough space ({remoteCapacity}) allocated to this ObjectManager. Please use the `remoteCapacity` field in the constructor"
                );
            }
        }
        
        public uint createCExoString(string s)
        {
            uint dataPointer = (uint)remoteIndex;

            List<byte> data = new List<byte>();
            data.AddRange(GetBytes(dataPointer + 8u)); //char *
            data.AddRange(GetBytes((uint)s.Length)); // length
            data.AddRange(Encoding.ASCII.GetBytes(s)); // string
            data.Add(0); // null terminater

            remoteSize += (uint)data.Count;
            checkForOverflow();

            UIntPtr bytesWritten;
            WriteProcessMemory(processHandle, remoteIndex, data.ToArray(), (uint)data.Count, out bytesWritten);
            remoteIndex += data.Count;

            return dataPointer;
        }

        public uint createCResRef(string s)
        {
            uint dataPointer = (uint)remoteIndex;

            List<byte> data = new List<byte>();
            // 16 character resource reference
            for (int i = 0; i < 16; i++)
            {
                if (i >= s.Length)
                {
                    data.Add(0x0);
                }
                else
                {
                    data.Add((byte)s[i]);
                }
            }

            remoteSize += (uint)data.Count;
            checkForOverflow();

            UIntPtr bytesWritten;
            WriteProcessMemory(processHandle, remoteIndex, data.ToArray(), (uint)data.Count, out bytesWritten);
            remoteIndex += data.Count;

            return dataPointer;
        }

        public uint createCStr(string s)
        {
            uint dataPointer = (uint)remoteIndex;

            List<byte> data = new List<byte>();
            data.AddRange(Encoding.ASCII.GetBytes(s));
            data.Add(0); //null terminator

            remoteSize += (uint)data.Count;
            checkForOverflow();

            UIntPtr bytesWritten;
            WriteProcessMemory(processHandle, remoteIndex, data.ToArray(), (uint)data.Count, out bytesWritten);
            remoteIndex += data.Count;

            return dataPointer;
        }

        public uint createBuffer(int size, byte fill = 0)
        {
            uint dataPointer = (uint)remoteIndex;

            List<byte> data = new List<byte>();
            for (int i = 0; i < size; i++) data.Add(fill);

            remoteSize += (uint)data.Count;
            checkForOverflow();

            UIntPtr bytesWritten;
            WriteProcessMemory(processHandle, remoteIndex, data.ToArray(), (uint)data.Count, out bytesWritten);
            remoteIndex += data.Count;

            return dataPointer;
        }

        public uint createVector(float x, float y, float z)
        {
            uint dataPointer = (uint)remoteIndex;

            List<byte> data = new List<byte>();
            data.AddRange(GetBytes(x));
            data.AddRange(GetBytes(y));
            data.AddRange(GetBytes(z));

            remoteSize += (uint)data.Count;
            checkForOverflow();

            UIntPtr bytesWritten;
            WriteProcessMemory(processHandle, remoteIndex, data.ToArray(), (uint)data.Count, out bytesWritten);
            remoteIndex += data.Count;

            return dataPointer;
        }

        public uint createQuaternion(float w, float x, float y, float z)
        {
            uint dataPointer = (uint)remoteIndex;

            List<byte> data = new List<byte>();
            data.AddRange(GetBytes(w));
            data.AddRange(GetBytes(x));
            data.AddRange(GetBytes(y));
            data.AddRange(GetBytes(z));

            remoteSize += (uint)data.Count;
            checkForOverflow();

            UIntPtr bytesWritten;
            WriteProcessMemory(processHandle, remoteIndex, data.ToArray(), (uint)data.Count, out bytesWritten);
            remoteIndex += data.Count;

            return dataPointer;
        }

    }
}

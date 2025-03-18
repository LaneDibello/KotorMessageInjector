using System;
using System.Collections.Generic;
using System.Text;
using static System.BitConverter;

namespace KotorMessageInjector
{
    public class RemoteFunction
    {
        public uint functionAddress;
        public bool shouldReturn;
        public uint thisPointer = 0;

        public List<(uint, uint)> parameters;
        public Dictionary<int, byte[]> objPTRParams;

        public RemoteFunction(uint functionAddress, bool shouldReturn = true)
        {
            this.functionAddress = functionAddress;
            this.shouldReturn = shouldReturn;

            parameters = new List<(uint, uint)>();
            objPTRParams = new Dictionary<int, byte[]>();
        }

        public string stringParameters()
        {
            StringBuilder sb = new StringBuilder();
            foreach (var (param, size) in parameters)
            {
                sb.AppendLine($"{param} : {size} bytes");
            }
            return sb.ToString();
        }

        public RemoteFunction setThis(uint value)
        {
            thisPointer = value;
            return this;
        }
        public RemoteFunction addParam(int value)
        {
            parameters.Add((ToUInt32(GetBytes(value), 0), 4));
            return this;
        }
        public RemoteFunction addParam(uint value)
        {
            parameters.Add((ToUInt32(GetBytes(value), 0), 4));
            return this;
        }
        public RemoteFunction addParam(float value)
        {
            parameters.Add((ToUInt32(GetBytes(value), 0), 4));
            return this;
        }
        public RemoteFunction addParam(IntPtr value)
        {
            parameters.Add((ToUInt32(GetBytes((uint)value), 0), 4));
            return this;
        }
        public RemoteFunction addParam(bool value)
        {
            parameters.Add((ToUInt32(GetBytes(value ? 1 : 0), 0), 4));
            return this;
        }
        public RemoteFunction addParam(short value)
        {
            parameters.Add((ToUInt32(GetBytes(value), 0), 2));
            return this;
        }
        public RemoteFunction addParam(ushort value)
        {
            parameters.Add((ToUInt32(GetBytes(value), 0), 2));
            return this;
        }
        public RemoteFunction addParam(byte value)
        {
            parameters.Add((ToUInt32(GetBytes(value), 0), 1));
            return this;
        }
        public RemoteFunction addParam(char value)
        {
            parameters.Add((ToUInt32(GetBytes(value), 0), 1));
            return this;
        }
    }
}

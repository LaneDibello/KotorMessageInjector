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

        public void setThis(uint value)
        {
            thisPointer = value;
        }
        public void addParam(int value)
        {
            parameters.Add((ToUInt32(GetBytes(value), 0), 4));
        }
        public void addParam(uint value)
        {
            parameters.Add((ToUInt32(GetBytes(value), 0), 4));
        }
        public void addParam(float value)
        {
            parameters.Add((ToUInt32(GetBytes(value), 0), 4));
        }
        public void addParam(IntPtr value)
        {
            parameters.Add((ToUInt32(GetBytes((uint)value), 0), 4));
        }
        public void addParam(bool value)
        {
            parameters.Add((ToUInt32(GetBytes(value), 0), 4));
        }
        public void addParam(short value)
        {
            parameters.Add((ToUInt32(GetBytes(value), 0), 2));
        }
        public void addParam(ushort value)
        {
            parameters.Add((ToUInt32(GetBytes(value), 0), 2));
        }
        public void addParam(byte value)
        {
            parameters.Add((ToUInt32(GetBytes(value), 0), 1));
        }
        public void addParam(char value)
        {
            parameters.Add((ToUInt32(GetBytes(value), 0), 1));
        }
    }
}

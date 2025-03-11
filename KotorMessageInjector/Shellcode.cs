using System;
using System.Collections.Generic;
using System.Text;

namespace KotorMessageInjector
{
    class Shellcode
    {
        protected List<byte> shellcode = new List<byte>();

        // Move immediate values into registers
        protected void movEAX4Bytes(uint value)
        {
            shellcode.Add(0xB8);
            shellcode.Add((byte)(value & 0xFF));
            shellcode.Add((byte)((value >> 8) & 0xFF));
            shellcode.Add((byte)((value >> 16) & 0xFF));
            shellcode.Add((byte)((value >> 24) & 0xFF));
        }

        protected void movECX4Bytes(uint value)
        {
            shellcode.Add(0xB9);
            shellcode.Add((byte)(value & 0xFF));
            shellcode.Add((byte)((value >> 8) & 0xFF));
            shellcode.Add((byte)((value >> 16) & 0xFF));
            shellcode.Add((byte)((value >> 24) & 0xFF));
        }

        protected void movEDX4Bytes(uint value)
        {
            shellcode.Add(0xBA);
            shellcode.Add((byte)(value & 0xFF));
            shellcode.Add((byte)((value >> 8) & 0xFF));
            shellcode.Add((byte)((value >> 16) & 0xFF));
            shellcode.Add((byte)((value >> 24) & 0xFF));
        }

        protected void movEBX4Bytes(uint value)
        {
            shellcode.Add(0xBB);
            shellcode.Add((byte)(value & 0xFF));
            shellcode.Add((byte)((value >> 8) & 0xFF));
            shellcode.Add((byte)((value >> 16) & 0xFF));
            shellcode.Add((byte)((value >> 24) & 0xFF));
        }

        protected void movESP4Bytes(uint value)
        {
            shellcode.Add(0xBC);
            shellcode.Add((byte)(value & 0xFF));
            shellcode.Add((byte)((value >> 8) & 0xFF));
            shellcode.Add((byte)((value >> 16) & 0xFF));
            shellcode.Add((byte)((value >> 24) & 0xFF));
        }

        protected void movEBP4Bytes(uint value)
        {
            shellcode.Add(0xBD);
            shellcode.Add((byte)(value & 0xFF));
            shellcode.Add((byte)((value >> 8) & 0xFF));
            shellcode.Add((byte)((value >> 16) & 0xFF));
            shellcode.Add((byte)((value >> 24) & 0xFF));
        }

        protected void movESI4Bytes(uint value)
        {
            shellcode.Add(0xBE);
            shellcode.Add((byte)(value & 0xFF));
            shellcode.Add((byte)((value >> 8) & 0xFF));
            shellcode.Add((byte)((value >> 16) & 0xFF));
            shellcode.Add((byte)((value >> 24) & 0xFF));
        }

        protected void movEDI4Bytes(uint value)
        {
            shellcode.Add(0xBF);
            shellcode.Add((byte)(value & 0xFF));
            shellcode.Add((byte)((value >> 8) & 0xFF));
            shellcode.Add((byte)((value >> 16) & 0xFF));
            shellcode.Add((byte)((value >> 24) & 0xFF));
        }

        // Move immediate values into 8-bit registers
        protected void movAL1Byte(byte value)
        {
            shellcode.Add(0xB0);
            shellcode.Add(value);
        }

        protected void movCL1Byte(byte value)
        {
            shellcode.Add(0xB1);
            shellcode.Add(value);
        }

        protected void movDL1Byte(byte value)
        {
            shellcode.Add(0xB2);
            shellcode.Add(value);
        }

        protected void movBL1Byte(byte value)
        {
            shellcode.Add(0xB3);
            shellcode.Add(value);
        }

        // Push operations
        protected void push1Byte(byte value)
        {
            shellcode.Add(0x6A); // PUSH imm8
            shellcode.Add(value);
        }

        protected void push4Bytes(uint value)
        {
            shellcode.Add(0x68); // PUSH imm32
            shellcode.Add((byte)(value & 0xFF));
            shellcode.Add((byte)((value >> 8) & 0xFF));
            shellcode.Add((byte)((value >> 16) & 0xFF));
            shellcode.Add((byte)((value >> 24) & 0xFF));
        }

        protected void push2Bytes(ushort value)
        {
            // For 16-bit operands, we need the operand size prefix
            shellcode.Add(0x66); // Operand size prefix
            shellcode.Add(0x68); // PUSH imm16
            shellcode.Add((byte)(value & 0xFF));
            shellcode.Add((byte)((value >> 8) & 0xFF));
        }

        // Push registers
        protected void pushEAX()
        {
            shellcode.Add(0x50);
        }

        protected void pushECX()
        {
            shellcode.Add(0x51);
        }

        protected void pushEDX()
        {
            shellcode.Add(0x52);
        }

        protected void pushEBX()
        {
            shellcode.Add(0x53);
        }

        protected void pushESP()
        {
            shellcode.Add(0x54);
        }

        protected void pushEBP()
        {
            shellcode.Add(0x55);
        }

        protected void pushESI()
        {
            shellcode.Add(0x56);
        }

        protected void pushEDI()
        {
            shellcode.Add(0x57);
        }

        // Pop registers
        protected void popEAX()
        {
            shellcode.Add(0x58);
        }

        protected void popECX()
        {
            shellcode.Add(0x59);
        }

        protected void popEDX()
        {
            shellcode.Add(0x5A);
        }

        protected void popEBX()
        {
            shellcode.Add(0x5B);
        }

        protected void popESP()
        {
            shellcode.Add(0x5C);
        }

        protected void popEBP()
        {
            shellcode.Add(0x5D);
        }

        protected void popESI()
        {
            shellcode.Add(0x5E);
        }

        protected void popEDI()
        {
            shellcode.Add(0x5F);
        }

        // Move memory to register operations
        protected void movEAXFromMemory(uint address)
        {
            shellcode.Add(0xA1); // MOV EAX, [mem32]
            shellcode.Add((byte)(address & 0xFF));
            shellcode.Add((byte)((address >> 8) & 0xFF));
            shellcode.Add((byte)((address >> 16) & 0xFF));
            shellcode.Add((byte)((address >> 24) & 0xFF));
        }

        protected void movECXFromMemory(uint address)
        {
            // MOV ECX, [mem32] doesn't have a direct opcode, so we use the ModR/M form
            shellcode.Add(0x8B); // MOV r32, r/m32
            shellcode.Add(0x0D); // ModR/M byte for ECX and direct address
            shellcode.Add((byte)(address & 0xFF));
            shellcode.Add((byte)((address >> 8) & 0xFF));
            shellcode.Add((byte)((address >> 16) & 0xFF));
            shellcode.Add((byte)((address >> 24) & 0xFF));
        }

        protected void movEDXFromMemory(uint address)
        {
            shellcode.Add(0x8B); // MOV r32, r/m32
            shellcode.Add(0x15); // ModR/M byte for EDX and direct address
            shellcode.Add((byte)(address & 0xFF));
            shellcode.Add((byte)((address >> 8) & 0xFF));
            shellcode.Add((byte)((address >> 16) & 0xFF));
            shellcode.Add((byte)((address >> 24) & 0xFF));
        }

        protected void movEBXFromMemory(uint address)
        {
            shellcode.Add(0x8B); // MOV r32, r/m32
            shellcode.Add(0x1D); // ModR/M byte for EBX and direct address
            shellcode.Add((byte)(address & 0xFF));
            shellcode.Add((byte)((address >> 8) & 0xFF));
            shellcode.Add((byte)((address >> 16) & 0xFF));
            shellcode.Add((byte)((address >> 24) & 0xFF));
        }

        // Move register to memory operations
        protected void movMemoryFromEAX(uint address)
        {
            shellcode.Add(0xA3); // MOV [mem32], EAX
            shellcode.Add((byte)(address & 0xFF));
            shellcode.Add((byte)((address >> 8) & 0xFF));
            shellcode.Add((byte)((address >> 16) & 0xFF));
            shellcode.Add((byte)((address >> 24) & 0xFF));
        }

        protected void movMemoryFromECX(uint address)
        {
            shellcode.Add(0x89); // MOV r/m32, r32
            shellcode.Add(0x0D); // ModR/M byte for ECX and direct address
            shellcode.Add((byte)(address & 0xFF));
            shellcode.Add((byte)((address >> 8) & 0xFF));
            shellcode.Add((byte)((address >> 16) & 0xFF));
            shellcode.Add((byte)((address >> 24) & 0xFF));
        }

        // Register-to-register moves
        protected void movEAXFromECX()
        {
            shellcode.Add(0x89); // MOV r/m32, r32
            shellcode.Add(0xC8); // ModR/M byte for EAX <- ECX
        }

        protected void movECXFromEAX()
        {
            shellcode.Add(0x89); // MOV r/m32, r32
            shellcode.Add(0xC1); // ModR/M byte for ECX <- EAX
        }

        // Common operations
        protected void xorEAXEAX()
        {
            shellcode.Add(0x33); // XOR r32, r/m32
            shellcode.Add(0xC0); // ModR/M byte for EAX, EAX
        }

        protected void xorECXECX()
        {
            shellcode.Add(0x33); // XOR r32, r/m32
            shellcode.Add(0xC9); // ModR/M byte for ECX, ECX
        }

        protected void xorEDXEDX()
        {
            shellcode.Add(0x33); // XOR r32, r/m32
            shellcode.Add(0xD2); // ModR/M byte for EDX, EDX
        }

        // Control flow
        protected void callEAX()
        {
            shellcode.Add(0xff); // Call Register
            shellcode.Add(0xD0); // EAX
        }

        protected void callRelative(uint relativeAddress)
        {
            shellcode.Add(0xE8); // CALL rel32
            shellcode.Add((byte)(relativeAddress & 0xFF));
            shellcode.Add((byte)((relativeAddress >> 8) & 0xFF));
            shellcode.Add((byte)((relativeAddress >> 16) & 0xFF));
            shellcode.Add((byte)((relativeAddress >> 24) & 0xFF));
        }

        // Call to immediate absolute address
        protected void callAbsolute(uint absoluteAddress)
        {
            shellcode.Add(0xFF); // CALL r/m32
            shellcode.Add(0x15); // ModR/M byte for indirect call [address]
            shellcode.Add((byte)(absoluteAddress & 0xFF));
            shellcode.Add((byte)((absoluteAddress >> 8) & 0xFF));
            shellcode.Add((byte)((absoluteAddress >> 16) & 0xFF));
            shellcode.Add((byte)((absoluteAddress >> 24) & 0xFF));
        }

        protected void jmp(uint relativeAddress)
        {
            shellcode.Add(0xE9); // JMP rel32
            shellcode.Add((byte)(relativeAddress & 0xFF));
            shellcode.Add((byte)((relativeAddress >> 8) & 0xFF));
            shellcode.Add((byte)((relativeAddress >> 16) & 0xFF));
            shellcode.Add((byte)((relativeAddress >> 24) & 0xFF));
        }

        protected void ret()
        {
            shellcode.Add(0xC3); // RET
        }

        protected void int3()
        {
            shellcode.Add(0xCC); // INT3 (breakpoint)
        }

        protected void nop()
        {
            shellcode.Add(0x90); // NOP
        }

        // System calls - Windows
        protected void syscall()
        {
            // 0x0F 0x05 is for 64-bit mode in Windows/Linux
            // For 32-bit Windows, we typically use INT 0x2E or SYSENTER
            shellcode.Add(0xCD); // INT
            shellcode.Add(0x2E); // 0x2E
        }

        // Arithmetic operations
        protected void addEAX(uint value)
        {
            if (value <= 0x7F)
            {
                // Optimize for small values (8-bit)
                shellcode.Add(0x83); // ADD r/m32, imm8
                shellcode.Add(0xC0); // ModR/M byte for EAX
                shellcode.Add((byte)(value & 0xFF));
            }
            else
            {
                shellcode.Add(0x05); // ADD EAX, imm32
                shellcode.Add((byte)(value & 0xFF));
                shellcode.Add((byte)((value >> 8) & 0xFF));
                shellcode.Add((byte)((value >> 16) & 0xFF));
                shellcode.Add((byte)((value >> 24) & 0xFF));
            }
        }

        protected void subEAX(uint value)
        {
            if (value <= 0x7F)
            {
                // Optimize for small values (8-bit)
                shellcode.Add(0x83); // SUB r/m32, imm8
                shellcode.Add(0xE8); // ModR/M byte for EAX
                shellcode.Add((byte)(value & 0xFF));
            }
            else
            {
                shellcode.Add(0x2D); // SUB EAX, imm32
                shellcode.Add((byte)(value & 0xFF));
                shellcode.Add((byte)((value >> 8) & 0xFF));
                shellcode.Add((byte)((value >> 16) & 0xFF));
                shellcode.Add((byte)((value >> 24) & 0xFF));
            }
        }
        protected void addESP(uint value)
        {
            if (value <= 0x7F)
            {
                // Optimize for small values (8-bit)
                shellcode.Add(0x83); // ADD r/m32, imm8
                shellcode.Add(0xC4); // ModR/M byte for ESP
                shellcode.Add((byte)(value & 0xFF));
            }
            else
            {
                shellcode.Add(0x81); // ADD r/m32, imm32
                shellcode.Add(0xC4); // ModR/M byte for ESP
                shellcode.Add((byte)(value & 0xFF));
                shellcode.Add((byte)((value >> 8) & 0xFF));
                shellcode.Add((byte)((value >> 16) & 0xFF));
                shellcode.Add((byte)((value >> 24) & 0xFF));
            }
        }

        protected void subESP(uint value)
        {
            if (value <= 0x7F)
            {
                // Optimize for small values (8-bit)
                shellcode.Add(0x83); // SUB r/m32, imm8
                shellcode.Add(0xEC); // ModR/M byte for ESP
                shellcode.Add((byte)(value & 0xFF));
            }
            else
            {
                shellcode.Add(0x81); // SUB r/m32, imm32
                shellcode.Add(0xEC); // ModR/M byte for ESP
                shellcode.Add((byte)(value & 0xFF));
                shellcode.Add((byte)((value >> 8) & 0xFF));
                shellcode.Add((byte)((value >> 16) & 0xFF));
                shellcode.Add((byte)((value >> 24) & 0xFF));
            }
        }

        protected void movEAXFromEBXPlusOffset(byte offset)
        {
            shellcode.Add(0x8B); // MOV r32, r/m32
            shellcode.Add(0x43); // ModR/M byte for EAX, [EBX+disp8]
            shellcode.Add(offset);
        }

        protected void movECXFromEAXPlusOffset(byte offset)
        {
            shellcode.Add(0x8B); // MOV r32, r/m32
            shellcode.Add(0x48); // ModR/M byte for ECX, [EAX+disp8]
            shellcode.Add(offset);
        }
    }
}

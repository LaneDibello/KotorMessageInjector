using System;
using System.Runtime.InteropServices;

namespace KotorMessageInjector
{
    static class ProcessAPI
    {
        // Process Access Rights
        public const int PROCESS_CREATE_THREAD = 0x0002;
        public const int PROCESS_QUERY_INFORMATION = 0x0400;
        public const int PROCESS_VM_OPERATION = 0x0008;
        public const int PROCESS_VM_WRITE = 0x0020;
        public const int PROCESS_VM_READ = 0x0010;
        public const int PROCESS_ALL_ACCESS = 0x001F0FFF;

        // Memory Allocation Types
        public const uint MEM_COMMIT = 0x00001000;
        public const uint MEM_RESERVE = 0x00002000;
        public const uint MEM_RELEASE = 0x00008000;

        // Memory Protection Constants
        public const uint PAGE_READWRITE = 0x04;
        public const uint PAGE_EXECUTE_READWRITE = 0x40;

        // Handle Constants
        public const uint INFINITE = 0xFFFFFFFF;

        // Toolhelp32 Constants
        public const uint TH32CS_SNAPPROCESS = 0x00000002;
        public static readonly IntPtr INVALID_HANDLE_VALUE = new IntPtr(-1);

        // Structure for Process32Next/First
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct PROCESSENTRY32W
        {
            public uint dwSize;
            public uint cntUsage;
            public uint th32ProcessID;
            public IntPtr th32DefaultHeapID;
            public uint th32ModuleID;
            public uint cntThreads;
            public uint th32ParentProcessID;
            public int pcPriClassBase;
            public uint dwFlags;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szExeFile;
        }

        /// <summary>
        /// Finds a process ID by its name (case-insensitive).
        /// </summary>
        /// <param name="processName">The name of the process to find (e.g., "notepad.exe").</param>
        /// <returns>The process ID if found, or 0 if not found.</returns>
        public static uint GetProcessId(string processName)
        {
            uint processId = 0;
            IntPtr snapshot = CreateToolhelp32Snapshot(TH32CS_SNAPPROCESS, 0);

            if (snapshot != INVALID_HANDLE_VALUE)
            {
                PROCESSENTRY32W processEntry = new PROCESSENTRY32W();
                processEntry.dwSize = (uint)Marshal.SizeOf(typeof(PROCESSENTRY32W));

                if (Process32FirstW(snapshot, ref processEntry))
                {
                    do
                    {
                        if (string.Compare(processEntry.szExeFile, processName, StringComparison.OrdinalIgnoreCase) == 0)
                        {
                            processId = processEntry.th32ProcessID;
                            break;
                        }
                    } while (Process32NextW(snapshot, ref processEntry));
                }
                CloseHandle(snapshot);
            }
            return processId;
        }


        /// <summary>
        /// Takes a snapshot of the specified processes.
        /// </summary>
        /// <param name="dwFlags">The portions of the system to be included in the snapshot.</param>
        /// <param name="th32ProcessID">The process identifier of the process to be included in the snapshot.</param>
        /// <returns>If the function succeeds, it returns an open handle to the specified snapshot.</returns>
        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern IntPtr CreateToolhelp32Snapshot(
            uint dwFlags,
            uint th32ProcessID);

        /// <summary>
        /// Retrieves information about the first process encountered in a system snapshot.
        /// </summary>
        /// <param name="hSnapshot">A handle to the snapshot returned from a previous call to CreateToolhelp32Snapshot.</param>
        /// <param name="lppe">A pointer to a PROCESSENTRY32 structure.</param>
        /// <returns>Returns TRUE if the first entry of the process list has been copied to the buffer or FALSE otherwise.</returns>
        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern bool Process32FirstW(
            IntPtr hSnapshot,
            ref PROCESSENTRY32W lppe);

        /// <summary>
        /// Retrieves information about the next process recorded in a system snapshot.
        /// </summary>
        /// <param name="hSnapshot">A handle to the snapshot returned from a previous call to CreateToolhelp32Snapshot.</param>
        /// <param name="lppe">A pointer to a PROCESSENTRY32 structure.</param>
        /// <returns>Returns TRUE if the next entry of the process list has been copied to the buffer or FALSE otherwise.</returns>
        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern bool Process32NextW(
            IntPtr hSnapshot,
            ref PROCESSENTRY32W lppe);

        /// <summary>
        /// Opens an existing local process object.
        /// </summary>
        /// <param name="dwDesiredAccess">The access to the process object.</param>
        /// <param name="bInheritHandle">If this value is TRUE, processes created by this process will inherit the handle. Otherwise, the processes do not inherit this handle.</param>
        /// <param name="dwProcessId">The identifier of the local process to be opened.</param>
        /// <returns>If the function succeeds, the return value is an open handle to the specified process.</returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr OpenProcess(
            int dwDesiredAccess,
            bool bInheritHandle,
            uint dwProcessId);

        /// <summary>
        /// Reserves, commits, or changes the state of a region of memory within the virtual address space of a specified process.
        /// </summary>
        /// <param name="hProcess">A handle to the process in whose virtual address space the memory is to be allocated.</param>
        /// <param name="lpAddress">The pointer that specifies a desired starting address for the region of pages that you want to allocate.</param>
        /// <param name="dwSize">The size of the region of memory to allocate, in bytes.</param>
        /// <param name="flAllocationType">The type of memory allocation.</param>
        /// <param name="flProtect">The memory protection for the region of pages to be allocated.</param>
        /// <returns>If the function succeeds, the return value is the base address of the allocated region of pages.</returns>
        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        public static extern IntPtr VirtualAllocEx(
            IntPtr hProcess,
            IntPtr lpAddress,
            uint dwSize,
            uint flAllocationType,
            uint flProtect);

        /// <summary>
        /// Writes data to an area of memory in a specified process.
        /// </summary>
        /// <param name="hProcess">A handle to the process memory to be modified.</param>
        /// <param name="lpBaseAddress">A pointer to the base address in the specified process to which data is written.</param>
        /// <param name="lpBuffer">A pointer to the buffer that contains data to be written.</param>
        /// <param name="nSize">The number of bytes to be written.</param>
        /// <param name="lpNumberOfBytesWritten">A pointer to a variable that receives the number of bytes transferred.</param>
        /// <returns>If the function succeeds, the return value is nonzero.</returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool WriteProcessMemory(
            IntPtr hProcess,
            IntPtr lpBaseAddress,
            byte[] lpBuffer,
            uint nSize,
            out UIntPtr lpNumberOfBytesWritten);

        /// <summary>
        /// Reads data from an area of memory in a specified process.
        /// </summary>
        /// <param name="hProcess">A handle to the process with memory that is being read.</param>
        /// <param name="lpBaseAddress">A pointer to the base address in the specified process from which to read.</param>
        /// <param name="lpBuffer">A pointer to a buffer that receives the contents from the address space of the specified process.</param>
        /// <param name="nSize">The number of bytes to be read.</param>
        /// <param name="lpNumberOfBytesRead">A pointer to a variable that receives the number of bytes transferred.</param>
        /// <returns>If the function succeeds, the return value is nonzero.</returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool ReadProcessMemory(
            IntPtr hProcess,
            IntPtr lpBaseAddress,
            [Out] byte[] lpBuffer,
            uint nSize,
            out UIntPtr lpNumberOfBytesRead);

        /// <summary>
        /// Waits until the specified object is in the signaled state or the time-out interval elapses.
        /// </summary>
        /// <param name="hHandle">A handle to the object.</param>
        /// <param name="dwMilliseconds">The time-out interval, in milliseconds.</param>
        /// <returns>If the function succeeds, the return value indicates the event that caused the function to return.</returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern uint WaitForSingleObject(
            IntPtr hHandle,
            uint dwMilliseconds);

        /// <summary>
        /// Closes an open object handle.
        /// </summary>
        /// <param name="hObject">A valid handle to an open object.</param>
        /// <returns>If the function succeeds, the return value is nonzero.</returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool CloseHandle(
            IntPtr hObject);

        /// <summary>
        /// Releases, decommits, or releases and decommits a region of memory within the virtual address space of a specified process.
        /// </summary>
        /// <param name="hProcess">A handle to the process in whose virtual address space the memory is to be freed.</param>
        /// <param name="lpAddress">A pointer to the starting address of the region of memory to be freed.</param>
        /// <param name="dwSize">The size of the region of memory to free, in bytes.</param>
        /// <param name="dwFreeType">The type of free operation.</param>
        /// <returns>If the function succeeds, the return value is nonzero.</returns>
        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        public static extern bool VirtualFreeEx(
            IntPtr hProcess,
            IntPtr lpAddress,
            uint dwSize,
            uint dwFreeType);

        /// <summary>
        /// Retrieves the termination status of the specified thread.
        /// </summary>
        /// <param name="hThread">A handle to the thread.</param>
        /// <param name="lpExitCode">A pointer to a variable to receive the thread termination status.</param>
        /// <returns>If the function succeeds, the return value is nonzero.</returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool GetExitCodeThread(
            IntPtr hThread,
            out uint lpExitCode);

        /// <summary>
        /// Creates a thread that runs in the virtual address space of another process.
        /// </summary>
        /// <param name="hProcess">A handle to the process in which the thread is to be created.</param>
        /// <param name="lpThreadAttributes">A pointer to a SECURITY_ATTRIBUTES structure that specifies a security descriptor for the new thread.</param>
        /// <param name="dwStackSize">The initial size of the stack, in bytes.</param>
        /// <param name="lpStartAddress">A pointer to the application-defined function of type LPTHREAD_START_ROUTINE to be executed by the thread.</param>
        /// <param name="lpParameter">A pointer to a variable to be passed to the thread function.</param>
        /// <param name="dwCreationFlags">The flags that control the creation of the thread.</param>
        /// <param name="lpThreadId">A pointer to a variable that receives the thread identifier.</param>
        /// <returns>If the function succeeds, the return value is a handle to the new thread.</returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr CreateRemoteThread(
            IntPtr hProcess,
            IntPtr lpThreadAttributes,
            uint dwStackSize,
            IntPtr lpStartAddress,
            IntPtr lpParameter,
            uint dwCreationFlags,
            out uint lpThreadId);
    }
}
using System.Runtime.InteropServices;

namespace CriFsHook.ReloadedII.CRI
{
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct FileTable
    {
        public bool IsEnabled;
        public int SixIfInvalidFileHandleOrBeyondMaxListSize;
        public int OneIfInvalidFileHandleOrBeyondMaxListSize;
        public int field_C;
        public int field_10;
        public int field_14;
        public char* AlsoDvdrootFullPath;
        public int MaxFileEntryArraySizeInBytes;
        public int FileEntryArraySizeInBytes;
        public char* DvdrootFullPath;
        public int DvdrootFullPathStringLength;
        public int field_2C;
        public int FileEntryCount;
        public int FirstFileEntryPtr;
        public int CurrentFileEntryIteratorPtr;
        public int field_3C;
        public int field_40;

        /* These addresses reference the Sonic Heroes executable (TSonic_win.exe) */

        /// <summary>
        /// 006CFF40
        /// </summary>
        [Reloaded.Hooks.X64.Function(Reloaded.Hooks.X64.CallingConventions.Microsoft)]
        [Reloaded.Hooks.X86.Function(Reloaded.Hooks.X86.CallingConventions.Cdecl)]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate int AddFileToFileTable(NewFileInfoTuple* filePath, FileTable* fileTable);

        /// <summary>
        /// 006D63B0
        /// </summary>
        [Reloaded.Hooks.X64.Function(Reloaded.Hooks.X64.CallingConventions.Microsoft)]
        [Reloaded.Hooks.X86.Function(Reloaded.Hooks.X86.CallingConventions.Cdecl)]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate int BuildFileTable(string folderPath, int decrementsOnNewDirectory, int* a3);

        /// <summary>
        /// 006D6330
        /// </summary>
        [Reloaded.Hooks.X64.Function(Reloaded.Hooks.X64.CallingConventions.Microsoft)]
        [Reloaded.Hooks.X86.Function(Reloaded.Hooks.X86.CallingConventions.Cdecl)]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate int CallsBuildFileTable(char* a1, int a2, int* a3);

        /// <summary>
        /// 006B4040
        /// </summary>
        [Reloaded.Hooks.X64.Function(Reloaded.Hooks.X64.CallingConventions.Microsoft)]
        [Reloaded.Hooks.X86.Function(Reloaded.Hooks.X86.CallingConventions.Cdecl)]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate char* GetDvdrootFullPath(FileTable* fileTable);

        /// <summary>
        /// 006CEFE0
        /// </summary>
        [Reloaded.Hooks.X64.Function(Reloaded.Hooks.X64.CallingConventions.Microsoft)]
        [Reloaded.Hooks.X86.Function(Reloaded.Hooks.X86.CallingConventions.Cdecl)]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate int GetFileEntryCount(FileTable* fileTable);

        /// <summary>
        /// 006D00F0
        /// </summary>
        [Reloaded.Hooks.X64.Function(Reloaded.Hooks.X64.CallingConventions.Microsoft)]
        [Reloaded.Hooks.X86.Function(Reloaded.Hooks.X86.CallingConventions.Cdecl)]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate FileEntry* GetFileEntryFromFilePath(string fullFilePath);

        /// <summary>
        /// 006D01D0
        /// </summary>
        [Reloaded.Hooks.X64.Function(Reloaded.Hooks.X64.CallingConventions.Microsoft)]
        [Reloaded.Hooks.X86.Function(Reloaded.Hooks.X86.CallingConventions.Cdecl)]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate FileEntry* GetNextFileEntry(FileTable* fileTable);

        /// <summary>
        /// 006D00D0
        /// </summary>
        [Reloaded.Hooks.X64.Function(Reloaded.Hooks.X64.CallingConventions.Microsoft)]
        [Reloaded.Hooks.X86.Function(Reloaded.Hooks.X86.CallingConventions.Cdecl)]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate FileEntry* LoadADXFromFileTable(string fullFilePath);

        /// <summary>
        /// 006CFBA0
        /// </summary>
        [Reloaded.Hooks.X64.Function(Reloaded.Hooks.X64.CallingConventions.Microsoft)]
        [Reloaded.Hooks.X86.Function(Reloaded.Hooks.X86.CallingConventions.Cdecl)]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate FileTable* MaybeConstructor(string directoryPath, int a2NormallyMinusOne, char* a3NormallyZero, int a4NormallyZero);

        /// <summary>
        /// 006CF9D0
        /// </summary>
        [Reloaded.Hooks.X64.Function(Reloaded.Hooks.X64.CallingConventions.Microsoft)]
        [Reloaded.Hooks.X86.Function(Reloaded.Hooks.X86.CallingConventions.Cdecl)]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate int ProbablyResetFileTable();
    }
}

using System;
using System.Runtime.InteropServices;

namespace CriFsHook.ReloadedII.CRI;

public unsafe class FileTableFactory
{
    /* These addresses reference the Sonic Heroes executable (TSonic_win.exe) */

    /// <summary>
    /// 0x006C8000
    /// </summary>
    [Reloaded.Hooks.Definitions.X64.Function(Reloaded.Hooks.Definitions.X64.CallingConventions.Microsoft)]
    [Reloaded.Hooks.Definitions.X86.Function(Reloaded.Hooks.Definitions.X86.CallingConventions.Cdecl)]
    [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public delegate FileTable* CreateFromDirectory(char* directoryPath, int a2NormallyMinusOne, int a3NormallyZero, int a4NormallyZero);

    /// <summary>
    /// 0x006D6310
    /// </summary>
    [Reloaded.Hooks.Definitions.X64.Function(Reloaded.Hooks.Definitions.X64.CallingConventions.Microsoft)]
    [Reloaded.Hooks.Definitions.X86.Function(Reloaded.Hooks.Definitions.X86.CallingConventions.Cdecl)]
    [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public delegate int SetFileTablePointer(IntPtr newFunctionPointer, FileTable* fileTablePointer);

    /// <summary>
    /// 0x006CFE70
    /// </summary>
    [Reloaded.Hooks.Definitions.X64.Function(Reloaded.Hooks.Definitions.X64.CallingConventions.Microsoft)]
    [Reloaded.Hooks.Definitions.X86.Function(Reloaded.Hooks.Definitions.X86.CallingConventions.Cdecl)]
    [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public delegate FileTable* SetTablePointersAndBuild(FileTable* fileTablePointer);
}
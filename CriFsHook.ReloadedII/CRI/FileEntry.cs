using System.Runtime.InteropServices;

namespace CriFsHook.ReloadedII.CRI;

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
public unsafe struct FileEntry
{
    public IntPtr FileHandle;
    public uint   FileSize;
    public int    Gap8;
    public FileEntry* NextEntry;
    public char*      FileName;
}
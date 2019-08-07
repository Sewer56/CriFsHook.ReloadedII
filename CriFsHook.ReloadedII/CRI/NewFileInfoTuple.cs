using System.Runtime.InteropServices;

namespace CriFsHook.ReloadedII.CRI
{
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct NewFileInfoTuple
    {
        public char* FileName;
        public int NFileSizeLow;
        public int NFileSizeHigh;
    }
}

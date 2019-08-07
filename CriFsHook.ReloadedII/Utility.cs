using System.IO;

namespace CriFsHook.ReloadedII
{
    public static class Utility
    {
        public static bool IsFileLocked(FileInfo file)
        {
            FileStream stream = null;

            try
            {
                stream = file.Open(FileMode.Open, FileAccess.Read, FileShare.None);
            }
            catch (IOException)
            {
                // The file is unavailable because it is:
                // - Still being written to.
                // - Being processed by another thread.
                // - Does not exist.
                return true;
            }
            finally
            {
                stream?.Close();
            }

            return false;
        }
    }
}

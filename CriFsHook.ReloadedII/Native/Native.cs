using System;
using System.IO;
using System.Runtime.InteropServices;

namespace CriFsHook.ReloadedII.Native
{
    public static class Native
    {
        [SuppressGCTransition]
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr LocalFree(IntPtr hMem);

        [SuppressGCTransition]
        [DllImport("kernel32.dll")]
        public static extern IntPtr LocalAlloc(uint uFlags, UIntPtr uBytes);

        [SuppressGCTransition]
        [DllImport("kernel32.dll")]
        public unsafe static extern IntPtr CreateFileA(
            void* lpFileName,
            FileAccess dwDesiredAccess,
            FileShare dwShareMode,
            [Optional] IntPtr lpSecurityAttributes,
            FileMode dwCreationDisposition,
            FileFlagsAndAttributes dwFlagsAndAttributes,
            [Optional] IntPtr hTemplateFile);

        [SuppressGCTransition]
        [DllImport("kernel32.dll")]
        public static extern uint GetFileSize(IntPtr hFile, out uint lpFileSizeHigh);

        [SuppressGCTransition]
        [DllImport("kernel32.dll")]
        public static extern bool CloseHandle(IntPtr hObject);

        /// <summary>Enumerates the that may apply to files.</summary>
        /// <remarks>These flags may be passed to CreateFile.</remarks>
        [Flags]
        public enum FileAccess : uint
        {
            /// <summary>Read access.</summary>
            GENERIC_READ = 2147483648, // 0x80000000

            /// <summary>Write access.</summary>
            GENERIC_WRITE = 1073741824, // 0x40000000

            /// <summary>Execute access.</summary>
            GENERIC_EXECUTE = 536870912, // 0x20000000

            /// <summary>All possible access rights.</summary>
            GENERIC_ALL = 268435456, // 0x10000000

            /// <summary>
            /// For a file object, the right to read the corresponding file data. For a directory object, the right to read the corresponding directory data.
            /// </summary>
            FILE_READ_DATA = 1,

            /// <summary>For a directory, the right to list the contents of the directory.</summary>
            FILE_LIST_DIRECTORY = FILE_READ_DATA, // 0x00000001

            /// <summary>
            /// For a file object, the right to write data to the file. For a directory object, the right to create a file in the directory ( <see cref="F:Vanara.PInvoke.Kernel32.FileAccess.FILE_ADD_FILE" />).
            /// </summary>
            FILE_WRITE_DATA = 2,

            /// <summary>For a directory, the right to create a file in the directory.</summary>
            FILE_ADD_FILE = FILE_WRITE_DATA, // 0x00000002

            /// <summary>
            /// For a file object, the right to append data to the file. (For local files, write operations will not overwrite existing data if this flag is
            /// specified without <see cref="F:Vanara.PInvoke.Kernel32.FileAccess.FILE_WRITE_DATA" />.) For a directory object, the right to create a subdirectory ( <see cref="F:Vanara.PInvoke.Kernel32.FileAccess.FILE_ADD_SUBDIRECTORY" />).
            /// </summary>
            FILE_APPEND_DATA = 4,

            /// <summary>For a directory, the right to create a subdirectory.</summary>
            FILE_ADD_SUBDIRECTORY = FILE_APPEND_DATA, // 0x00000004

            /// <summary>For a named pipe, the right to create a pipe.</summary>
            FILE_CREATE_PIPE_INSTANCE = FILE_ADD_SUBDIRECTORY, // 0x00000004

            /// <summary>The right to read extended file attributes.</summary>
            FILE_READ_EA = 8,

            /// <summary>The right to write extended file attributes.</summary>
            FILE_WRITE_EA = 16, // 0x00000010

            /// <summary>
            /// For a native code file, the right to execute the file. This access right given to scripts may cause the script to be executable, depending on the
            /// script interpreter.
            /// </summary>
            FILE_EXECUTE = 32, // 0x00000020

            /// <summary>
            /// For a directory, the right to traverse the directory. By default, users are assigned the BYPASS_TRAVERSE_CHECKING privilege, which ignores the
            /// FILE_TRAVERSE access right.
            /// </summary>
            FILE_TRAVERSE = FILE_EXECUTE, // 0x00000020

            /// <summary>For a directory, the right to delete a directory and all the files it contains, including read-only files.</summary>
            FILE_DELETE_CHILD = 64, // 0x00000040

            /// <summary>The right to read file attributes.</summary>
            FILE_READ_ATTRIBUTES = 128, // 0x00000080

            /// <summary>The right to write file attributes.</summary>
            FILE_WRITE_ATTRIBUTES = 256, // 0x00000100
            SPECIFIC_RIGHTS_ALL = 65535, // 0x0000FFFF
            FILE_ALL_ACCESS = 2032127, // 0x001F01FF
            FILE_GENERIC_READ = 1179785, // 0x00120089
            FILE_GENERIC_WRITE = 1179926, // 0x00120116
            FILE_GENERIC_EXECUTE = 1179808, // 0x001200A0
        }

        [StructLayout(LayoutKind.Sequential)]
        public class SECURITY_ATTRIBUTES
        {
            /// <summary>The size, in bytes, of this structure. Set this value to the size of the SECURITY_ATTRIBUTES structure.</summary>
            public int nLength = Marshal.SizeOf(typeof(SECURITY_ATTRIBUTES));

            /// <summary>
            /// A pointer to a SECURITY_DESCRIPTOR structure that controls access to the object. If the value of this member is NULL, the object is assigned the
            /// default security descriptor associated with the access token of the calling process. This is not the same as granting access to everyone by
            /// assigning a NULL discretionary access control list (DACL). By default, the default DACL in the access token of a process allows access only to
            /// the user represented by the access token.
            /// </summary>
            public IntPtr lpSecurityDescriptor;

            /// <summary>
            /// A Boolean value that specifies whether the returned handle is inherited when a new process is created. If this member is TRUE, the new process
            /// inherits the handle.
            /// </summary>
            [MarshalAs(UnmanagedType.Bool)]
            public bool bInheritHandle;
        }

        /// <summary>
        /// File attributes are metadata values stored by the file system on disk and are used by the system and are available to developers via various file I/O APIs.
        /// </summary>
        [Flags]
        public enum FileFlagsAndAttributes : uint
        {
            /// <summary>
            /// A file that is read-only. Applications can read the file, but cannot write to it or delete it. This attribute is not honored on directories. For
            /// more information, see You cannot view or change the Read-only or the System attributes of folders in Windows Server 2003, in Windows XP, in
            /// Windows Vista or in Windows 7.
            /// </summary>
            FILE_ATTRIBUTE_READONLY = 1,

            /// <summary>The file or directory is hidden. It is not included in an ordinary directory listing.</summary>
            FILE_ATTRIBUTE_HIDDEN = 2,

            /// <summary>A file or directory that the operating system uses a part of, or uses exclusively.</summary>
            FILE_ATTRIBUTE_SYSTEM = 4,

            /// <summary>The handle that identifies a directory.</summary>
            FILE_ATTRIBUTE_DIRECTORY = 16, // 0x00000010

            /// <summary>
            /// A file or directory that is an archive file or directory. Applications typically use this attribute to mark files for backup or removal .
            /// </summary>
            FILE_ATTRIBUTE_ARCHIVE = 32, // 0x00000020

            /// <summary>This value is reserved for system use.</summary>
            FILE_ATTRIBUTE_DEVICE = 64, // 0x00000040

            /// <summary>A file that does not have other attributes set. This attribute is valid only when used alone.</summary>
            FILE_ATTRIBUTE_NORMAL = 128, // 0x00000080

            /// <summary>
            /// A file that is being used for temporary storage. File systems avoid writing data back to mass storage if sufficient cache memory is available,
            /// because typically, an application deletes a temporary file after the handle is closed. In that scenario, the system can entirely avoid writing
            /// the data. Otherwise, the data is written after the handle is closed.
            /// </summary>
            FILE_ATTRIBUTE_TEMPORARY = 256, // 0x00000100

            /// <summary>A file that is a sparse file.</summary>
            FILE_ATTRIBUTE_SPARSE_FILE = 512, // 0x00000200

            /// <summary>A file or directory that has an associated reparse point, or a file that is a symbolic link.</summary>
            FILE_ATTRIBUTE_REPARSE_POINT = 1024, // 0x00000400

            /// <summary>
            /// A file or directory that is compressed. For a file, all of the data in the file is compressed. For a directory, compression is the default for
            /// newly created files and subdirectories.
            /// </summary>
            FILE_ATTRIBUTE_COMPRESSED = 2048, // 0x00000800

            /// <summary>
            /// The data of a file is not available immediately. This attribute indicates that the file data is physically moved to offline storage. This
            /// attribute is used by Remote Storage, which is the hierarchical storage management software. Applications should not arbitrarily change this attribute.
            /// </summary>
            FILE_ATTRIBUTE_OFFLINE = 4096, // 0x00001000

            /// <summary>The file or directory is not to be indexed by the content indexing service.</summary>
            FILE_ATTRIBUTE_NOT_CONTENT_INDEXED = 8192, // 0x00002000

            /// <summary>
            /// A file or directory that is encrypted. For a file, all data streams in the file are encrypted. For a directory, encryption is the default for
            /// newly created files and subdirectories.
            /// </summary>
            FILE_ATTRIBUTE_ENCRYPTED = 16384, // 0x00004000

            /// <summary>
            /// The directory or user data stream is configured with integrity (only supported on ReFS volumes). It is not included in an ordinary directory
            /// listing. The integrity setting persists with the file if it's renamed. If a file is copied the destination file will have integrity set if either
            /// the source file or destination directory have integrity set.
            /// <para>
            /// <c>Windows Server 2008 R2, Windows 7, Windows Server 2008, Windows Vista, Windows Server 2003 and Windows XP:</c> This flag is not supported
            /// until Windows Server 2012.
            /// </para>
            /// </summary>
            FILE_ATTRIBUTE_INTEGRITY_STREAM = 32768, // 0x00008000

            /// <summary>This value is reserved for system use.</summary>
            FILE_ATTRIBUTE_VIRTUAL = 65536, // 0x00010000

            /// <summary>
            /// The user data stream not to be read by the background data integrity scanner (AKA scrubber). When set on a directory it only provides
            /// inheritance. This flag is only supported on Storage Spaces and ReFS volumes. It is not included in an ordinary directory listing.
            /// <para>
            /// <c>Windows Server 2008 R2, Windows 7, Windows Server 2008, Windows Vista, Windows Server 2003 and Windows XP:</c> This flag is not supported
            /// until Windows 8 and Windows Server 2012.
            /// </para>
            /// </summary>
            FILE_ATTRIBUTE_NO_SCRUB_DATA = 131072, // 0x00020000

            /// <summary>The file attribute ea</summary>
            FILE_ATTRIBUTE_EA = 262144, // 0x00040000

            /// <summary>
            /// Write operations will not go through any intermediate cache, they will go directly to disk.
            /// <para>For additional information, see the Caching Behavior section of this topic.</para>
            /// </summary>
            FILE_FLAG_WRITE_THROUGH = 2147483648, // 0x80000000

            /// <summary>
            /// The file or device is being opened or created for asynchronous I/O.
            /// <para>
            /// When subsequent I/O operations are completed on this handle, the event specified in the OVERLAPPED structure will be set to the signaled state.
            /// </para>
            /// <para>If this flag is specified, the file can be used for simultaneous read and write operations.</para>
            /// <para>
            /// If this flag is not specified, then I/O operations are serialized, even if the calls to the read and write functions specify an OVERLAPPED structure.
            /// </para>
            /// <para>
            /// For information about considerations when using a file handle created with this flag, see the Synchronous and Asynchronous I/O Handles section of
            /// this topic.
            /// </para>
            /// </summary>
            FILE_FLAG_OVERLAPPED = 1073741824, // 0x40000000

            /// <summary>
            /// The file or device is being opened with no system caching for data reads and writes. This flag does not affect hard disk caching or memory mapped files.
            /// <para>
            /// There are strict requirements for successfully working with files opened with CreateFile using the FILE_FLAG_NO_BUFFERING flag, for details see
            /// File Buffering.
            /// </para>
            /// </summary>
            FILE_FLAG_NO_BUFFERING = 536870912, // 0x20000000

            /// <summary>
            /// Access is intended to be random. The system can use this as a hint to optimize file caching.
            /// <para>This flag has no effect if the file system does not support cached I/O and FILE_FLAG_NO_BUFFERING.</para>
            /// <para>For more information, see the Caching Behavior section of this topic.</para>
            /// </summary>
            FILE_FLAG_RANDOM_ACCESS = 268435456, // 0x10000000

            /// <summary>
            /// Access is intended to be sequential from beginning to end. The system can use this as a hint to optimize file caching.
            /// <para>This flag should not be used if read-behind (that is, reverse scans) will be used.</para>
            /// <para>This flag has no effect if the file system does not support cached I/O and FILE_FLAG_NO_BUFFERING.</para>
            /// <para>For more information, see the Caching Behavior section of this topic.</para>
            /// </summary>
            FILE_FLAG_SEQUENTIAL_SCAN = 134217728, // 0x08000000

            /// <summary>
            /// The file is to be deleted immediately after all of its handles are closed, which includes the specified handle and any other open or duplicated handles.
            /// <para>If there are existing open handles to a file, the call fails unless they were all opened with the FILE_SHARE_DELETE share mode.</para>
            /// <para>Subsequent open requests for the file fail, unless the FILE_SHARE_DELETE share mode is specified.</para>
            /// </summary>
            FILE_FLAG_DELETE_ON_CLOSE = 67108864, // 0x04000000

            /// <summary>
            /// The file is being opened or created for a backup or restore operation. The system ensures that the calling process overrides file security checks
            /// when the process has SE_BACKUP_NAME and SE_RESTORE_NAME privileges. For more information, see Changing Privileges in a Token.
            /// <para>
            /// You must set this flag to obtain a handle to a directory. A directory handle can be passed to some functions instead of a file handle. For more
            /// information, see the Remarks section.
            /// </para>
            /// </summary>
            FILE_FLAG_BACKUP_SEMANTICS = 33554432, // 0x02000000

            /// <summary>
            /// Access will occur according to POSIX rules. This includes allowing multiple files with names, differing only in case, for file systems that
            /// support that naming. Use care when using this option, because files created with this flag may not be accessible by applications that are written
            /// for MS-DOS or 16-bit Windows.
            /// </summary>
            FILE_FLAG_POSIX_SEMANTICS = 16777216, // 0x01000000

            /// <summary>
            /// The file or device is being opened with session awareness. If this flag is not specified, then per-session devices (such as a device using
            /// RemoteFX USB Redirection) cannot be opened by processes running in session 0. This flag has no effect for callers not in session 0. This flag is
            /// supported only on server editions of Windows.
            /// <para><c>Windows Server 2008 R2 and Windows Server 2008:</c> This flag is not supported before Windows Server 2012.</para>
            /// </summary>
            FILE_FLAG_SESSION_AWARE = 8388608, // 0x00800000

            /// <summary>
            /// Normal reparse point processing will not occur; CreateFile will attempt to open the reparse point. When a file is opened, a file handle is
            /// returned, whether or not the filter that controls the reparse point is operational.
            /// <para>This flag cannot be used with the CREATE_ALWAYS flag.</para>
            /// <para>If the file is not a reparse point, then this flag is ignored.</para>
            /// </summary>
            FILE_FLAG_OPEN_REPARSE_POINT = 2097152, // 0x00200000

            /// <summary>
            /// The file data is requested, but it should continue to be located in remote storage. It should not be transported back to local storage. This flag
            /// is for use by remote storage systems.
            /// </summary>
            FILE_FLAG_OPEN_NO_RECALL = 1048576, // 0x00100000

            /// <summary>
            /// If you attempt to create multiple instances of a pipe with this flag, creation of the first instance succeeds, but creation of the next instance
            /// fails with ERROR_ACCESS_DENIED.
            /// </summary>
            FILE_FLAG_FIRST_PIPE_INSTANCE = 524288, // 0x00080000

            /// <summary>Impersonates a client at the Anonymous impersonation level.</summary>
            SECURITY_ANONYMOUS = 0,

            /// <summary>Impersonates a client at the Identification impersonation level.</summary>
            SECURITY_IDENTIFICATION = FILE_ATTRIBUTE_VIRTUAL, // 0x00010000

            /// <summary>
            /// Impersonate a client at the impersonation level. This is the default behavior if no other flags are specified along with the
            /// SECURITY_SQOS_PRESENT flag.
            /// </summary>
            SECURITY_IMPERSONATION = FILE_ATTRIBUTE_NO_SCRUB_DATA, // 0x00020000

            /// <summary>Impersonates a client at the Delegation impersonation level.</summary>
            SECURITY_DELEGATION = SECURITY_IMPERSONATION | SECURITY_IDENTIFICATION, // 0x00030000

            /// <summary>The security tracking mode is dynamic. If this flag is not specified, the security tracking mode is static.</summary>
            SECURITY_CONTEXT_TRACKING = FILE_ATTRIBUTE_EA, // 0x00040000

            /// <summary>
            /// Only the enabled aspects of the client's security context are available to the server. If you do not specify this flag, all aspects of the
            /// client's security context are available.
            /// <para>This allows the client to limit the groups and privileges that a server can use while impersonating the client.</para>
            /// </summary>
            SECURITY_EFFECTIVE_ONLY = FILE_FLAG_FIRST_PIPE_INSTANCE, // 0x00080000

            /// <summary>Include to enable the other SECURITY_ flags.</summary>
            SECURITY_SQOS_PRESENT = FILE_FLAG_OPEN_NO_RECALL, // 0x00100000
        }
    }
}

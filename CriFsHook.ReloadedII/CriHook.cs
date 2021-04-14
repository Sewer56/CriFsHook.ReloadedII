using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using CriFsHook.ReloadedII.CRI;
using Reloaded.Hooks;
using Reloaded.Hooks.Definitions;
using Reloaded.Hooks.ReloadedII.Interfaces;
using Reloaded.Memory.Buffers;
using Reloaded.Memory.Sigscan;
using Reloaded.Memory.Sigscan.Structs;
using Reloaded.Mod.Interfaces;
using Rock.Collections;
using static CriFsHook.ReloadedII.Native.Native;
using IReloadedHooks = Reloaded.Hooks.ReloadedII.Interfaces.IReloadedHooks;

namespace CriFsHook.ReloadedII
{
    public unsafe class CriHook : IDisposable
    {
        private const  int                  BufferSize = 65535;
        private static object               _bufferLock = new object();
        private static MemoryBufferHelper   _helper = new MemoryBufferHelper(Process.GetCurrentProcess());

        private OrderedDictionary<string, IntPtr>           _mappingDictionary = new OrderedDictionary<string, IntPtr>();
        private List<PrivateMemoryBuffer>                   _buffers = new List<PrivateMemoryBuffer>();
        private IHook<FileTable.BuildFileTable>             _buildFileTableHook;
        private IHook<FileTable.GetFileEntryFromFilePath>   _getFileEntryFromFilePathHook;

        private Process                                     _currentProcess;
        private ProcessModule                               _mainModule;
        private ILogger                                     _logger;
        private WeakReference<IReloadedHooks>               _reloadedHooks;

        /* Setup & Teardown */
        public static void Main() { }
        public CriHook(IModLoader modLoader)
        {
            _logger = (ILogger) modLoader.GetLogger();
            _reloadedHooks = modLoader.GetController<IReloadedHooks>();
            CreateNewBuffer();

            _currentProcess = Process.GetCurrentProcess();
            _mainModule     = _currentProcess.MainModule;
            using var scanner = new Scanner(_currentProcess, _mainModule);

            PatternScanResult buildFileTable           = scanner.CompiledFindPattern("81 EC ?? ?? ?? ?? 8D 84 24");
            PatternScanResult getFileEntryFromFilePath = scanner.CompiledFindPattern("8B 44 24 04 81 EC ?? ?? ?? ?? 8D 4C 24 04");

            if (!buildFileTable.Found || !getFileEntryFromFilePath.Found)
            {
                _logger.PrintMessage("[CriFsHook] Not all signatures have been found. Aborting.", _logger.ColorRedLight);
            }
            else
            {
                _buildFileTableHook           = HookAndPrint<FileTable.BuildFileTable>(BuildFileTableImpl, buildFileTable.Offset).Activate();
                _getFileEntryFromFilePathHook = HookAndPrint<FileTable.GetFileEntryFromFilePath>(GetFileEntryFromPathImpl, getFileEntryFromFilePath.Offset).Activate();
            }
        }

        ~CriHook()
        {
            Dispose();
            GC.SuppressFinalize(this);
        }

        public void Dispose()
        {
            foreach (var buffer in _buffers)
                buffer.Dispose();
        }

        private IHook<TFunction> HookAndPrint<TFunction>(TFunction function, int offset)
        {
            var address = (long) (_mainModule.BaseAddress + offset);
            if (_reloadedHooks.TryGetTarget(out var hooks))
            {
                var hook = hooks.CreateHook(function, address).Activate();
                _logger.PrintMessage($"[CriFsHook] Successfully hooked {typeof(TFunction).Name} at {address:X}", _logger.ColorGreenLight);
                return hook;
            }
            else
            {
                _logger.PrintMessage($"[CriFsHook] Reloaded.Hooks Shared Library not found. Not hooking: {typeof(TFunction).Name} at {address:X}", _logger.ColorRed);
                return null;
            }
        }

        /* Hooks */
        private int BuildFileTableImpl(void* folderPath, int decrementsOnNewDirectory, int* a3)
        {
            _logger.PrintMessage($"[CriFsHook] Ignoring Build File Table", _logger.ColorGreenLight);
            return 0;
        }

        private FileEntry* GetFileEntryFromPathImpl(void* fullPath)
        {
            if (fullPath == null)
                return null;

            // Check if our collection already has file.
            string fullFilePath = Marshal.PtrToStringAnsi((IntPtr)fullPath);
            if (_mappingDictionary.TryGetValue(fullFilePath, out var entry))
                return (FileEntry*) entry;

            // Otherwise create new file entry.
            var fileEntry        = new FileEntry();
            fileEntry.FileHandle = CreateFileW(fullFilePath, Native.Native.FileAccess.FILE_GENERIC_READ | Native.Native.FileAccess.FILE_GENERIC_WRITE, FileShare.ReadWrite, new SECURITY_ATTRIBUTES(), FileMode.Open, FileFlagsAndAttributes.FILE_ATTRIBUTE_NORMAL);
            fileEntry.FileSize   = GetFileSize(fileEntry.FileHandle, out var lpFileSizeHigh);
            fileEntry.FileName   = WriteToBuffer(Encoding.ASCII.GetBytes(fullFilePath));

            fileEntry.NextEntry = _mappingDictionary.Values.Count > 0
                ? (FileEntry*) _mappingDictionary.Values.Last()
                : (FileEntry*) 0;

            // Write file entry to unmanaged memory and return
            var fileEntryPtr = WriteToBuffer(ref fileEntry);
            _mappingDictionary[fullFilePath] = (IntPtr) fileEntryPtr;
            return fileEntryPtr;
        }

        /* Utility Functions */
        private char* WriteToBuffer(byte[] item)
        {
            var buffer = _buffers[_buffers.Count - 1];
            lock (_bufferLock)
            {
                if (!buffer.CanItemFit(ref buffer))
                    buffer = CreateNewBuffer();

                return (char*) buffer.Add(item);
            }
        }

        private T* WriteToBuffer<T>(ref T item) where T : unmanaged
        {
            var buffer = _buffers[_buffers.Count - 1];
            lock (_bufferLock)
            {
                if (!buffer.CanItemFit(ref buffer))
                    buffer = CreateNewBuffer();

                return (T*) buffer.Add(ref item);
            }
        }

        // Creates a new buffer and appends it to the current buffer list.
        private PrivateMemoryBuffer CreateNewBuffer()
        {
            var buffer = _helper.CreatePrivateMemoryBuffer(BufferSize);
            _buffers.Add(buffer);
            return buffer;
        }
    }
}

﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using CriFsHook.ReloadedII.CRI;
using Reloaded.Hooks;
using Reloaded.Memory.Buffers;
using Reloaded.Memory.Sigscan;
using Reloaded.Memory.Sigscan.Structs;
using Reloaded.Mod.Interfaces;
using Rock.Collections;
using static CriFsHook.ReloadedII.Native.Native;

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

        /* Setup & Teardown */
        public CriHook(IModLoader modLoader)
        {
            _logger = (ILogger) modLoader.GetLogger();
            CreateNewBuffer();

            _currentProcess = Process.GetCurrentProcess();
            _mainModule     = _currentProcess.MainModule;
            var scanner     = new Scanner(_currentProcess, _mainModule);

            PatternScanResult buildFileTable           = scanner.CompiledFindPattern("81 EC ?? ?? ?? ?? 8D 84 24");
            PatternScanResult getFileEntryFromFilePath = scanner.CompiledFindPattern("8B 44 24 04 81 EC ?? ?? ?? ?? 8D 4C 24 04");

            if (!buildFileTable.Found || !getFileEntryFromFilePath.Found)
            {
                _logger.PrintMessage("[CriFsHook] Not all signatures have been found. Aborting.", _logger.ColorRedLight);
            }
            else
            {
                _buildFileTableHook = HookAndPrint<FileTable.BuildFileTable>(BuildFileTableImpl, buildFileTable.Offset).Activate();
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
            var hook = IntPtr.Size == 4
                ? (IHook<TFunction>) new Reloaded.Hooks.X86.Hook<TFunction>(function, address)
                : new Reloaded.Hooks.X64.Hook<TFunction>(function, address);

            _logger.PrintMessage($"[CriFsHook] Successfully hooked {typeof(TFunction).Name} at {address:X}", _logger.ColorGreenLight);
            return hook;
        }

        /* Hooks */
        private int BuildFileTableImpl(string folderPath, int decrementsOnNewDirectory, int* a3) => 0;

        private FileEntry* GetFileEntryFromPathImpl(string fullFilePath)
        {
            // Check if our collection already has file.
            if (_mappingDictionary.TryGetValue(fullFilePath, out var entry))
                return (FileEntry*) entry;

            // Otherwise create new file entry.
            var fileEntry = new FileEntry();
            fileEntry.FileHandle = CreateFile(fullFilePath, Native.Native.FileAccess.FILE_GENERIC_READ | Native.Native.FileAccess.FILE_GENERIC_WRITE, FileShare.ReadWrite, new SECURITY_ATTRIBUTES(), FileMode.Open, FileFlagsAndAttributes.FILE_ATTRIBUTE_NORMAL);
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
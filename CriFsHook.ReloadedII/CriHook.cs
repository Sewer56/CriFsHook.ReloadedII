using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using CriFsHook.ReloadedII.CRI;
using Reloaded.Hooks.Definitions;
using Reloaded.Hooks.Definitions.X86;
using Reloaded.Memory.Sigscan;
using Reloaded.Memory.Sigscan.Structs;
using Reloaded.Mod.Interfaces;
using static CriFsHook.ReloadedII.Native.Native;

namespace CriFsHook.ReloadedII
{
    public unsafe class CriHook
    {
        private Dictionary<string, IntPtr>                  _mappingDictionary = new Dictionary<string, IntPtr>();
        private IHook<FileTable.BuildFileTableFnPtr>           _buildFileTableHook;
        private IHook<FileTable.GetFileEntryFromFilePathFnPtr> _getFileEntryFromFilePathHook;

        private Process                                     _currentProcess;
        private ProcessModule                               _mainModule;
        private ILogger                                     _logger;
        private IReloadedHooks                              _reloadedHooks;
        private static CriHook                              _this;

        /* Setup & Teardown */
        public static void Main() { }
        public CriHook(IModLoader modLoader)
        {
            _this = this;
            _logger = (ILogger) modLoader.GetLogger();
            modLoader.GetController<IReloadedHooks>().TryGetTarget(out _reloadedHooks);

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
                _buildFileTableHook           = HookAndPrint<FileTable.BuildFileTableFnPtr>(nameof(BuildFileTableImplStatic), buildFileTable.Offset).Activate();
                _getFileEntryFromFilePathHook = HookAndPrint<FileTable.GetFileEntryFromFilePathFnPtr>(nameof(GetFileEntryFromPathImplStatic), getFileEntryFromFilePath.Offset).Activate();
            }
        }

        private IHook<TFunction> HookAndPrint<TFunction>(string functionName, int offset)
        {
            var address = (long) (_mainModule.BaseAddress + offset);
            var hook = _reloadedHooks.CreateHook<TFunction>(typeof(CriHook), functionName, address).Activate();
            _logger.PrintMessage($"[CriFsHook] Successfully hooked {typeof(TFunction).Name} at {address:X}", _logger.ColorGreenLight);
            return hook;
        }

        private IHook<TFunction> HookAndPrint<TFunction>(TFunction target, int offset)
        {
            var address = (long) (_mainModule.BaseAddress + offset);
            var hook = _reloadedHooks.CreateHook<TFunction>(target, address).Activate();
            _logger.PrintMessage($"[CriFsHook] Successfully hooked {typeof(TFunction).Name} at {address:X}", _logger.ColorGreenLight);
            return hook;
        }

        /* Hooks */
        [UnmanagedCallersOnly()]
        private static int BuildFileTableImplStatic(void* folderPath, int decrementsOnNewDirectory, int* a3) => _this.BuildFileTableImpl(folderPath, decrementsOnNewDirectory, a3);
        private int BuildFileTableImpl(void* folderPath, int decrementsOnNewDirectory, int* a3)
        {
            _logger.PrintMessage($"[CriFsHook] Ignoring Build File Table", _logger.ColorGreenLight);
            return 0;
        }

        [UnmanagedCallersOnly()]
        private static FileEntry* GetFileEntryFromPathImplStatic(void* fullPath) => _this.GetFileEntryFromPathImpl(fullPath);
        private FileEntry* GetFileEntryFromPathImpl(void* fullPath)
        {
            if (fullPath == null)
                return null;

            // Check if our collection already has file.
            string fullFilePath = Marshal.PtrToStringAnsi((IntPtr)fullPath);
            if (_mappingDictionary.TryGetValue(fullFilePath, out var entry))
                DisposeEntry((FileEntry*) entry);

            // Otherwise create new file entry.
            var fileEntry        = new FileEntry();
            fileEntry.FileHandle = CreateFileW(fullFilePath, Native.Native.FileAccess.FILE_GENERIC_READ | Native.Native.FileAccess.FILE_GENERIC_WRITE, FileShare.ReadWrite, new SECURITY_ATTRIBUTES(), FileMode.Open, FileFlagsAndAttributes.FILE_ATTRIBUTE_NORMAL);
            fileEntry.FileSize   = GetFileSize(fileEntry.FileHandle, out var lpFileSizeHigh);
            fileEntry.FileName   = (char*) Marshal.StringToHGlobalAnsi(fullFilePath);
            fileEntry.NextEntry  = (FileEntry*) 0;

            // Write file entry to unmanaged memory and return
            var fileEntryPtr = Marshal.AllocHGlobal(sizeof(FileEntry));
            Marshal.StructureToPtr(fileEntry, fileEntryPtr, false);
            _mappingDictionary[fullFilePath] = fileEntryPtr;
            return (FileEntry*) fileEntryPtr;
        }

        /* Utility Functions */
        private void DisposeEntry(FileEntry* entry)
        {
            CloseHandle(entry->FileHandle);
            Marshal.FreeHGlobal((IntPtr) entry->FileName);
            Marshal.FreeHGlobal((IntPtr) entry);
        }
    }
}

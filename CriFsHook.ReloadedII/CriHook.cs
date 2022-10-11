using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using CriFsHook.ReloadedII.CRI;
using Reloaded.Hooks.Definitions;
using Reloaded.Memory.Sigscan;
using Reloaded.Memory.Sigscan.Definitions.Structs;
using Reloaded.Memory.SigScan.ReloadedII.Interfaces;
using Reloaded.Mod.Interfaces;
using static CriFsHook.ReloadedII.Native.Native;

namespace CriFsHook.ReloadedII;

public unsafe class CriHook
{
    private Dictionary<string, IntPtr>                     _mappingDictionary = new();
    private IHook<FileTable.BuildFileTableFnPtr>           _buildFileTableHook = null!;
    private IHook<FileTable.GetFileEntryFromFilePathFnPtr> _getFileEntryFromFilePathHook = null!;

    private readonly ILogger        _logger;
    private readonly IReloadedHooks _reloadedHooks;
    private PatternScanResult? _buildFileTableResult;
    private PatternScanResult? _getEntryFromPathResult;
    
    private static CriHook _this = null!;

    /* Setup & Teardown */
    public CriHook(IModLoader modLoader)
    {
        _this = this;
        _logger = (ILogger) modLoader.GetLogger();
        modLoader.GetController<IReloadedHooks>().TryGetTarget(out _reloadedHooks!);
        modLoader.GetController<IStartupScanner>().TryGetTarget(out var startupScanner);

        var currentProcess = Process.GetCurrentProcess();
        var mainModule     = currentProcess.MainModule!;

        startupScanner!.AddMainModuleScan("81 EC ?? ?? ?? ?? 8D 84 24", result =>
        {
            _buildFileTableResult = result;
            InitIfBothScanned(mainModule);
        });
        
        startupScanner!.AddMainModuleScan("8B 44 24 04 81 EC ?? ?? ?? ?? 8D 4C 24 04", result =>
        {
            _getEntryFromPathResult = result;
            InitIfBothScanned(mainModule);
        });
    }

    private void InitIfBothScanned(ProcessModule mainModule)
    {
        if (_buildFileTableResult == null || _getEntryFromPathResult == null)
            return;

        var fileTableOfs = _buildFileTableResult.Value;
        var fromPathOfs = _getEntryFromPathResult.Value;
        if (!fileTableOfs.Found || !fromPathOfs.Found)
        {
            _logger.PrintMessage("[CriFsHook] Not all signatures have been found. Aborting.", _logger.ColorRedLight);
        }
        else
        {
            _buildFileTableHook           = HookAndPrint<FileTable.BuildFileTableFnPtr>(mainModule, nameof(BuildFileTableImplStatic), fileTableOfs.Offset).Activate();
            _getFileEntryFromFilePathHook = HookAndPrint<FileTable.GetFileEntryFromFilePathFnPtr>(mainModule, nameof(GetFileEntryFromPathImplStatic), fromPathOfs.Offset).Activate();
        }
    }

    private IHook<TFunction> HookAndPrint<TFunction>(ProcessModule module, string functionName, int offset)
    {
        var address = (long) (module.BaseAddress + offset);
        var hook = _reloadedHooks.CreateHook<TFunction>(typeof(CriHook), functionName, address).Activate();
        _logger.PrintMessage($"[CriFsHook] Successfully hooked {typeof(TFunction).Name} at {address:X}", _logger.ColorGreenLight);
        return hook;
    }

    /* Hooks */
    [UnmanagedCallersOnly]
    private static int BuildFileTableImplStatic(void* folderPath, int decrementsOnNewDirectory, int* a3) => _this.BuildFileTableImpl(folderPath, decrementsOnNewDirectory, a3);
    private int BuildFileTableImpl(void* folderPath, int decrementsOnNewDirectory, int* a3)
    {
        _logger.PrintMessage($"[CriFsHook] Ignoring Build File Table", _logger.ColorGreenLight);
        return 0;
    }

    [UnmanagedCallersOnly]
    private static FileEntry* GetFileEntryFromPathImplStatic(void* fullPath)
    {
        return _this.GetFileEntryFromPathImpl(fullPath);
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private FileEntry* GetFileEntryFromPathImpl(void* fullPath)
    {
        if (fullPath == null)
            return null;

        // Check if our collection already has file.
        string managedPath = new string((sbyte*)fullPath);
        if (_mappingDictionary.TryGetValue(managedPath, out var entry))
        {
            LogDebug("DISPOSING");
            DisposeEntry((FileEntry*) entry);
        }

        // Otherwise create new file entry.
        var fileEntry        = new FileEntry();
        fileEntry.FileHandle = CreateFileA((void*)fullPath, Native.Native.FileAccess.FILE_GENERIC_READ | Native.Native.FileAccess.FILE_GENERIC_WRITE, FileShare.ReadWrite, IntPtr.Zero, FileMode.Open, FileFlagsAndAttributes.FILE_ATTRIBUTE_NORMAL);
            
        LogDebug("HANDLE " + fileEntry.FileHandle);
        fileEntry.FileSize = GetFileSize(fileEntry.FileHandle, out var lpFileSizeHigh);
            
        LogDebug("SIZE " + fileEntry.FileSize.ToString("X"));
        fileEntry.FileName = (char*)fullPath;

        LogDebug("NAME " + ((int)fileEntry.FileName).ToString("X"));
        fileEntry.NextEntry = (FileEntry*) 0;

        // Write file entry to unmanaged memory and return
        var fileEntryPtr = LocalAlloc(0, (UIntPtr)sizeof(FileEntry));

        LogDebug("ALLOC " + (fileEntryPtr).ToString("X"));
        Unsafe.WriteUnaligned((void*) fileEntryPtr, fileEntry);
        _mappingDictionary[managedPath] = fileEntryPtr;

        LogDebug("DONE " + (fileEntryPtr).ToString("X") + "\n========");
        return (FileEntry*) fileEntryPtr;
    }

    [Conditional("DEBUG")]
    private void LogDebug(string text) => _logger.WriteLine(text);

    /* Utility Functions */
    private void DisposeEntry(FileEntry* entry)
    {
        CloseHandle(entry->FileHandle);
        LocalFree((IntPtr)entry);
    }
}
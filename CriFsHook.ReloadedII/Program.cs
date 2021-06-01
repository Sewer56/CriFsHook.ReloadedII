using System;
using System.Diagnostics;
using Reloaded.Mod.Interfaces;
using Reloaded.Mod.Interfaces.Internal;

namespace CriFsHook.ReloadedII
{
    public class Program : IMod
    {
        private IModLoader _modLoader;
        private CriHook _criHook;

        public void Start(IModLoaderV1 loader)
        {
            #if DEBUG
            Debugger.Launch();
            #endif
            _modLoader = (IModLoader)loader;

            /* Your mod code starts here. */
            _criHook = new CriHook(_modLoader);
        }

        /* Mod loader actions. */
        public void Suspend() { }
        public void Resume()  { }
        public void Unload()  { }

        public bool CanUnload()  => false;
        public bool CanSuspend() => false;

        /* Automatically called by the mod loader when the mod is about to be unloaded. */
        public Action Disposing { get; }
    }
}

# About This Project

The following project is a [Reloaded II](https://github.com/Reloaded-Project/Reloaded-II) Mod Loader mod that wraps around the "FileSystem" present in CRI Middleware libraries.

The "FileSystem" can be found in various games using CRI middleware released during middle 2000s and is used behind the scenes for loading various CRI formats (such as ADX, AFS, SFD).

While the main benefit (at the time) of such filesystem would have been to allow the streaming of media from discs while only installing the game files to the hard drive, the actual implementation of the "FileSystem" is a bit inconvenient for both modders and people playing on modern hardware.

## Features

### Fast Startup Times
On startup, the CRI filesystem iterates all files in game subfolders both on hard drive and on removable media, opening the files and storing their details for later use. This significantly slows down boot times and makes you unable to edit any file after a game has been started.

Launching a game after a clean reboot or waking up from sleep can take **over 10 seconds** of extra waiting time.

Installing this mod allows games to now boot instantly.

### Allow Better File Redirection
- Due to the way the FileSystem collects the file details, originally CRI files ADX, AFS, SFD cannot be properly used with file redirectors such as [Reloaded Universal Redirector](https://github.com/Reloaded-Project/Reloaded.Mod.Universal.Redirector) when the new/redirected file is larger.  

- The original implementation, due to the file details being collected only once, does not allow to load files not present during game launch, making some advanced modding with code more annoying if you e.g. want to play music from a custom folder.

## Supported Games
- Sonic Heroes  
- Sonic Riders  
- Silent Hill 3  
- Silent Hill 2  

If you know any other affected games or games this mod works with, please let me know.

## How to Use

**A.** Install Reloaded mod as usual.  
**B.** Enable mod and run the game.  

If your mod relies on this mod, remember to set this as a dependency.

## More Technical
A more technical version of this readme can be found in the [old repository](https://github.com/Sewer56/Heroes-CRI-FileTable-Hook), which originally only targeted to fix this issue in Sonic Heroes.

## Acknowledgements

[disc by Muhammad Taufik Sidik from Noun Project](https://thenounproject.com/browse/icons/term/disc/)
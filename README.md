<div align="center">
	<h1>Reloaded II: Cri Filesystem Hook</h1>
	<img src="https://i.imgur.com/BjPn7rU.png" width="150" align="center" />
	<br/> <br/>
	<strong>An Extract from 7th of August 2019</strong>
    <p>So what does CRI do again?<br/>
    *Makes your eyes and cheeks wet*</p>
</div>

# Table of Contents

- [Table of Contents](#table-of-contents)
- [About This Project](#about-this-project)
  - [Features](#features)
      - [Fast Startup Times](#fast-startup-times)
      - [Allow Better File Redirection](#allow-better-file-redirection)
  - [Supported Games](#supported-games)
  - [How to Use](#how-to-use)
  - [More Technical](#more-technical)

# Prerequisites
The CRI FS Hook uses the [Hooks Shared Library](https://github.com/Sewer56/Reloaded.SharedLib.Hooks).
Please download and extract that mod first.

# About This Project

The following project is a [Reloaded II](https://github.com/Reloaded-Project/Reloaded-II) Mod Loader mod that wraps around the "FileSystem" present in CRI Middleware libraries.

The "FileSystem" can be found in various games using CRI middleware released during middle 2000s and is used behind the scenes for loading various CRI formats (such as ADX, AFS, SFD).

While the main benefit (at the time) of such filesystem would have been to allow the streaming of media from discs while only installing the game files to the hard drive, the actual implementation of the "FileSystem" is a bit inconvenient for both modders and people playing on modern hardware.

## Features

#### Fast Startup Times
On startup, the CRI filesystem iterates all files in game subfolders both on hard drive and on removable media, opening the files and storing their details for later use. This significantly slows down boot times and makes you unable to edit any file after a game has been started.

Launching a game after a clean reboot or waking up from sleep can take **over 10 seconds** of extra waiting time.

Installing this mod allows games to now boot instantly.

#### Allow Better File Redirection
- Due to the way the FileSystem collects the file details, originally CRI files ADX, AFS, SFD cannot be properly used with file redirectors such as [ReloadedII Universal Redirector](https://github.com/Reloaded-Project/Reloaded.Mod.Universal.Redirector) when the new/redirected file is larger.
- The original implementation, due to the file details being collected only once, does not allow to load files not present during game launch, making some advanced modding with code more annoying if you e.g. want to play music from a custom folder.

## Supported Games
- Sonic Heroes
- Sonic Riders
- ??

If you know any other affected games or games this mod works with, please let me know.

## How to Use

**A.** Install Reloaded mods as usual. (Extract to mod directory)

**B.** Enable mod and run the game.

## More Technical
A more technical version of this readme can be found in the [old repository](https://github.com/Sewer56/Heroes-CRI-FileTable-Hook), which originally only targeted to fix this issue in Sonic Heroes.

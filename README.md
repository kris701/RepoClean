
<p align="center">
    <img src="https://github.com/kris701/RepoClean/assets/22596587/80338f32-e35c-4278-b6f1-c65a6547ea42" width="200" height="200" />
</p>

[![Build and Publish](https://github.com/kris701/RepoClean/actions/workflows/dotnet-desktop.yml/badge.svg)](https://github.com/kris701/RepoClean/actions/workflows/dotnet-desktop.yml)
![Nuget](https://img.shields.io/nuget/v/RepoClean)
![Nuget](https://img.shields.io/nuget/dt/RepoClean)
![GitHub last commit (branch)](https://img.shields.io/github/last-commit/kris701/RepoClean/main)
![GitHub commit activity (branch)](https://img.shields.io/github/commit-activity/m/kris701/RepoClean)
![Static Badge](https://img.shields.io/badge/Platform-Windows-blue)
![Static Badge](https://img.shields.io/badge/Platform-Linux-blue)
![Static Badge](https://img.shields.io/badge/Framework-dotnet--8.0-green)

# RepoClean

This is a small project to clean bin, obj and .vs folders form a folder or all subfolders.
It is a dotnet tool, and have the following arguments:
```
repoclean [-t|--target <PATH>] [-r|--recursive] [-f|--force] [-s|--show]
```
* `-t|--target` is an argument to the path you want to clean. By default this is the folder the tool is run from.
* `-r|--recursive` clean from the folder and all subfolders.
* `-f|--force` dont ask if they should be removed, just remove them.
* `-s|--show` only show what would be deleted, but dont delete or ask for anything.

This can be found as a package on the [NuGet Package Manager](https://www.nuget.org/packages/RepoClean/) or be installed by the command:
```
dotnet tool install RepoClean
```
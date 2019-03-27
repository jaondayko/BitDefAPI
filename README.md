# BitDefAPI
Using the BitDefender API to access endpoints and if any are currently infected.

The main file is the `BitGrab.cs` which is ran as a console/terminal application. 


# Powershell Script
This script is from Lucas Schill and is his working of the API through Powershell. Using this as a reference to convert into C# and using the API through there.

# Noteable References
Newtonsoft is the primary resource for grabbing the RPC/Client info, but it needs a modified version of it from https://github.com/adamashton/json-rpc-csharp

Which requires you to build the package yourself, then to add the .DLL to the package and then it will work from there. Using the original from NuGet will not work because it is modified to include a few things that BitDefender needs for the API.

With RPC, you must grab every reference sent back from BitDefender, even if the data isn't used. This is a limitation of the json-rpc.

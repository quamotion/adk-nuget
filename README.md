# NuGet packages for the Android SDK (ADK) components.

You may have .NET code that interacts with Android devices. In that scenario, you typically require a
client for the Android Debug Bridge (adb), such as [SharpAdbClient](http://github.com/quamotion/madb),
as well as a large set of Android Development Kit tools that should be installed on your computer.

This project creates NuGet packages for some of the ADK components that you may require. This project
repository only contains the code required to generate the NuGet packages; the packages themselves
are available on NuGet.

You can add these packages to your C# or Visual Basic .NET project. The actual files are available in the `tools\`
folder of the package.

Currently, the following packages are available:

Package Id        | Description
------------------| ---------------------------
[adk-platform-tools](https://www.nuget.org/packages/adk-platform-tools) | Contains the ADK platform tools, such as `adb.exe`


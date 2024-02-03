
DESCRIPTION

This is software that decides statements in symbolic logic.  It supports
predicates on one variable, propositions, identity, boolean operators and
alethic modal operators.  A working version of the web application can be
found at http://www.somerby.net/mack/logic/

INSTRUCTIONS FOR BUILDING THE SOFTWARE

1. Install Microsoft Visual Studio Community for Windows Desktop or
   some equivalent.  If you want to be able to build the web application,
   you will need Microsoft Visual Studio ***2015***; the Saltarelle tool,
   mentioned below, which converts C# .NET to Javascript, is not compatible with later versions of Visual Studio.
   You can download an [ISO for the Visual Studio Community 2015](http://download.microsoft.com/download/b/e/d/bedddfc4-55f4-4748-90a8-ffe38a40e89f/vs2015.3.com_enu.iso)
   or see [this Stack Overlflow post](https://stackoverflow.com/questions/38134857/visual-studio-2015-update-3-offline-installer-iso)
   for other options.  If you use a later version of Visual Studio, you can
   still build the other projects, including the desktop application.

2. Open the Visual Studio solution file named "Logic.sln".

3. Build the solution in the Release configuration.  Don't build it in the
   Debug configuration first; for some reason this messes up the project files
   and the solution fails to build.  When you first build, Visual Studio should
   install Saltarelle automatically.  If it doesn't, run the following commands
   in Visual Studio's package manager console:

```console
> Install-Package Saltarelle.Compiler
> Install-Package Saltarelle.Runtime
> Install-Package Saltarelle.Web
```

4. For Saltarelle to work, you may need to install Microsoft.Build.dll and
   Microsoft.Build.Framework.dll in the GAC.  To do this, open a command prompt
   in administrator mode and use gacutil.exe to install them, e.g.:

```console
> cd "C:\Program Files (x86)\MSBuild\14.0\Bin"
> "C:\Program Files (x86)\Microsoft SDKs\Windows\v10.0A\bin\NETFX 4.8 Tools\gacutil.exe" /i Microsoft.Build.dll
> "C:\Program Files (x86)\Microsoft SDKs\Windows\v10.0A\bin\NETFX 4.8 Tools\gacutil.exe" /i Microsoft.Build.Framework.dll
```

CONTENTS

The files in this project consist of the source code for several .NET
assemblies, .NET applications, and a client-side HTML5/JavaScript web
application.  These applications and assemblies are designed around C# classes
that represent elements of first-order logic such as variables, predicates,
quantifiers and operators.


- Logic

Classes that represent elements of first-order logic.  This assembly serves
as an API for constructing and deciding propositions.  Objects are created
through the class named "Factory".


- Parser

A parser that converts statements in symbolic logic to objects from Logic.
A description of the language that this parser parses can be found in
WebApplication/documentation.html. 


- ConsoleApplication

A simple command-line or console application that uses Logic and Parser to
read and decide statements in symbolic logic.


- Notepad++Plugin

A Notepad++Plugin that decides and depicts statements in symbolic logic.  It
uses Graphviz, if it's installed and on the PATH, to generate depictions.
*This plugin has not been maintained for a long time and probably won't work.*


- WindowsFormsApplication

A simple Windows application that uses Logic and Parser to read and decide
statements in symbolic logic.


- Saltarelle

A Saltarelle project that combines the contents of Logic and Parser and
converts them to JavaScript suitable for use in a web application.


- WebApplication

A web application that uses the output of the project named "Saltarelle" to
decide and depict statements in symbolic logic.


- VerificationTesting

Files containing statements in symbolic logic.  These statements serve as test
cases for verifying that Logic and Parser are functioning correctly.


- UnitTests

A collection of unit tests for Logic and Parser.  They mostly test Logic and
Parser against test cases in VerificationTesting.



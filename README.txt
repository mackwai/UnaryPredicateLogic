
DESCRIPTION

This is software that decides statements in symbolic logic.  It supports
predicates on one variable, propositions, identity, boolean operators and
alethic modal operators.  A working version of the web application can be
found at http://www.somerby.net/mack/logic/

INSTRUCTIONS FOR BUILDING THE SOFTWARE

1. Install Microsoft Visual Studio Express 2013 for Windows Desktop or
   some equivalent.

2. Open the Visual Studio solution file named "Logic.sln".

3. Build the solution in the Release configuration.  Don't build it in the
   Debug configuration first; for some reason this hoses up the project files
   and the solution fails to build.  When you first build, Visual Studio should
   install Saltarelle automatically.  If it doesn't, instructions
   for installing Saltarelle can be found at
   http://www.saltarelle-compiler.com/getting-started.  You only need to
   follow the instructions as far as running the following commands in Visual
   Studio's package manager console:

> Install-Package Saltarelle.Compiler
> Install-Package Saltarelle.Runtime
> Install-Package Saltarelle.Web
 

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



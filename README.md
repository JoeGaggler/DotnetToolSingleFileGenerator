# DotnetToolSingleFileGenerator

A Visual Studio extension that invokes a dotnet tool as a code generator, piping a file in the solution as input to the tool and piping the output to a new file in the solution.
This is structurally identical to other "designer-time" and "code-behind" file pairs in the solution.

# Overview

Adding code generators to Visual Studio often involve authoring, installing, and maintaining an extension for each code generator. 
Creating these extensions require significant overhead, especially if the the extension merely wraps an existing command line utility, such as a dotnet tool.
The extension from this repository facilitates code generation via dotnet tools, which are easily consumed via NuGet packages.


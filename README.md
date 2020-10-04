# DotnetToolSingleFileGenerator

A Visual Studio extension that invokes a dotnet tool as a code generator, piping a file in the solution as input to the tool and piping the output to a new file in the solution.
This is structurally identical to other "designer-time" and "code-behind" file pairs in the solution.

# Overview

Adding code generators to Visual Studio often involve authoring, installing, and maintaining an extension for each code generator. 
Creating these extensions require significant overhead, especially if the the extension merely wraps an existing command line utility, such as a dotnet tool.
The extension from this repository facilitates code generation via dotnet tools, which are easily consumed via NuGet packages.

# Prerequisites
1. This extension is installed in a compatible version of Visual Studio
1. The dotnet tool you want to use is installed, either globally or locally

# Instructions

1. Add a new file to the project to represent the design-time file (i.e. the input to the dotnet tool)
1. Open its file properties and set "Custom Tool" to `Jmg.CodeGen`
1. Create a file called `.codegen.yml` in the same directory
1. Insert this YAML document:

   ```yaml
   files:
   - file: YourDesignerFileName.dat
     tool: sometoolname
     extension: .cs
   ```

1. Replace the `file` value with the name of the design-time.
1. Replace the `tool` value with the command from your dotnet tool.
1. Replace the `extension` value with the file extension that should be generated.
1. Every subsequent update to the design-time file will invoke code generation.

# Notes

See my other repo, `Parsnip`, for an example of a dotnet tool that generates a parser from a grammar file, using this extension to facilitate the code generation in Visual Studio.

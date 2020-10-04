# DotnetToolSingleFileGenerator

A Visual Studio extension that invokes a dotnet tool as a code generator, piping a file in the solution as input to the tool and piping the output to a new file in the solution.
This is structurally identical to other "designer-time" and "code-behind" file pairs in the solution.

# Overview

Adding code generators to Visual Studio often involve authoring, installing, and maintaining an extension for each code generator. 
Creating these extensions require significant overhead, especially if the the extension merely wraps an existing command line utility, such as a dotnet tool.
The extension from this repository facilitates code generation via dotnet tools, which are easily consumed via NuGet packages.

# Instructions

1. Add a new file to the project to represent the input file (i.e. "design-time" file)
1. Open file properties and set "Custom Tool" to `Jmg.CodeGen`
1. Create a file called `.codegen.yml` in the same directory
1. Insert this YAML document:

   ```yaml
   files:
   - file: YourDesignerFileName.dat
     tool: sometoolname
     extension: .cs
   ```

1. Replace `file` with the name of the file you created in the first step
1. Replace `tool` with the command from your dotnet tool
1. Replace `extension` with the file extension that should be generated
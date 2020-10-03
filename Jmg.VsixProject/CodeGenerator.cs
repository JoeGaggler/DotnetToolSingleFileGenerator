using Microsoft.VisualStudio.TextTemplating.VSHost;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Jmg.VsixProject
{
	internal class CodeGenerator : BaseCodeGeneratorWithSite
	{
		private String fileExtension = String.Empty;

		private static readonly Regex safeRegex = new Regex("[^A-Za-z0-9_-]", RegexOptions.Compiled);
		private static readonly Regex fileExtensionRegex = new Regex("[.]?[A-Za-z0-9]([.][A-Za-z0-9]+)*", RegexOptions.Compiled);
		private static readonly Regex nonSafeCharsRegex = new Regex("[^A-Za-z0-9_-]", RegexOptions.Compiled);
		private static readonly String[] metaFileNameCandidates = new[]
		{
			".codegen.yml",
			"codegen.yml",
		};

		public override String GetDefaultExtension() => fileExtension;

		protected override Byte[] GenerateCode(String inputFileName, String inputFileContent)
		{
			this.fileExtension = "." + inputFileContent.Substring(0, 3);
			this.fileExtension = ".cs";

			var result = GenerateCodeCore(inputFilePath: inputFileName, inputFileContent: inputFileContent, fileNamespace: this.FileNamespace);
			var bytes = Encoding.UTF8.GetBytes(result.Contents);
			return bytes;
		}

		private CodeGenResult GenerateCodeCore(String inputFilePath, String inputFileContent, String fileNamespace)
		{
			String fileName, workingDirectory, baseName;
			try
			{
				var fileInfo = new FileInfo(inputFilePath);
				fileName = fileInfo.Name;
				baseName = fileName;
				while (baseName.Contains("."))
				{
					baseName = Path.GetFileNameWithoutExtension(baseName);
				}
				workingDirectory = fileInfo.Directory.FullName;
			}
			catch (Exception exc)
			{
				var message = $"// Unable to access input file: {inputFilePath}";
				return new CodeGenResult(message);
			}

			String metaPath;
			try
			{
				metaPath = metaFileNameCandidates
					.Select(name => Path.Combine(workingDirectory, name))
					.FirstOrDefault(path => File.Exists(path));
			}
			catch (Exception exc)
			{
				var message = $"// Unable to access \"{metaFileNameCandidates.First()}\" in same directory as input: {inputFilePath}";
				return new CodeGenResult(message);
			}

			String toolName;
			if (metaPath != null)
			{
				var metaContents = File.ReadAllText(metaPath);

				var deserializer = new YamlDotNet.Serialization.DeserializerBuilder()
					.Build();

				Yaml.Spec spec;
				try
				{
					spec = deserializer.Deserialize<Yaml.Spec>(metaContents);
				}
				catch (YamlDotNet.Core.YamlException yamlException)
				{
					var start = yamlException.Start;
					String message = $"Unable to parse YAML from {metaPath}\nLine: {start.Line}, Column: {start.Column}\n{yamlException.Message}";
					this.GeneratorErrorCallback(false, 0, message, start.Line, start.Column);
					return new CodeGenResult($"// {message}");
				}

				var file = spec.Files.FirstOrDefault(i => i.FileName == fileName);
				toolName = file.Tool;

				// Set file extension from config
				if (!(file.Extension is String fileExtension))
				{
					fileExtension = ".cs"; // Most code is C#, right? :-D
				}
				else if (!fileExtensionRegex.IsMatch(fileExtension))
				{
					return new CodeGenResult($"// Invalid extension: {fileExtension}");
				}
				else if (!fileExtension.StartsWith("."))
				{
					fileExtension = "." + fileExtension;
				}
				this.fileExtension = fileExtension;
			}
			else if (TryGetCommand(inputFilePath, out toolName, out baseName, out _))
			{
				// Legacy assumption
				this.fileExtension = ".cs";
			}
			else
			{
				return new CodeGenResult($"// Must include a '{metaFileNameCandidates.First()}' file in the same directory as input: {inputFilePath}");
			}

			var outputFileContent = DotnetRunner.Run(
				fileContents: inputFileContent,
				workingDirectory: workingDirectory,
				fileNamespace: fileNamespace,
				toolName: toolName,
				baseName: baseName,
				extension: fileExtension
				);

			return new CodeGenResult(outputFileContent);
		}

		private static Boolean TryGetCommand(String inputFileName, out String command, out String baseName, out String extension)
		{
			extension = Path.GetExtension(inputFileName);

			var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(inputFileName);
			if (String.IsNullOrEmpty(fileNameWithoutExtension))
			{
				command = null;
				baseName = null;
				return false;
			}

			baseName = Path.GetFileNameWithoutExtension(fileNameWithoutExtension);
			var secondExtension = Path.GetExtension(fileNameWithoutExtension);
			if (secondExtension == null || secondExtension.Length < 2)
			{
				command = null;
				return false;
			}

			var unsanitizedToolName = secondExtension.Substring(1);

			command = nonSafeCharsRegex.Replace(unsanitizedToolName, String.Empty);
			return true;
		}

		private class CodeGenResult
		{
			public String Contents { get; set; }

			public CodeGenResult(String contents)
			{
				this.Contents = contents;
			}
		}
	}
}

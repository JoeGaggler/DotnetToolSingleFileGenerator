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

		public override String GetDefaultExtension() => fileExtension;

		protected override Byte[] GenerateCode(String inputFileName, String inputFileContent)
		{
			this.fileExtension = "." + inputFileContent.Substring(0, 3);
			this.fileExtension = ".cs";

			//this.GeneratorErrorCallback(false, 1, "this is a bad", 1, 3);

			var result = GenerateCodeCore(inputFilePath: inputFileName, inputFileContent: inputFileContent, fileNamespace: this.FileNamespace);
			var bytes = Encoding.UTF8.GetBytes(result.Contents);
			return bytes;
		}

		private CodeGenResult GenerateCodeCore(String inputFilePath, String inputFileContent, String fileNamespace)
		{
			var fileInfo = new FileInfo(inputFilePath);
			var workingDirectory = fileInfo.Directory.FullName;

			String toolName;
			var metaPath = Path.Combine(workingDirectory, ".codegen");
			if (File.Exists(metaPath))
			{
				var metaContents = File.ReadAllText(metaPath);
				var san = Regex.Replace(metaContents, "[^A-Za-z0-9_-]", String.Empty);
				toolName = san;
			}
			else
			{
				return new CodeGenResult
				{
					Contents = "No meta",
				};
			}

			var outputFileContent = DotnetRunner.Run(fileContents: inputFileContent, workingDirectory: workingDirectory, fileNamespace: fileNamespace, toolName: toolName);

			//String result = $"NS: {fileNamespace}\r\nPath: {inputFilePath}\r\nNamespace: {fileNamespace}\r\nInput:\r\n{inputFileContent}\r\nworkingPath: {workingDirectory}\r\nOutput:\r\n{outputFileContent}";
			return new CodeGenResult()
			{
				Contents = outputFileContent,
			};
		}

		private class CodeGenResult
		{
			public String Contents { get; set; }
		}
	}
}

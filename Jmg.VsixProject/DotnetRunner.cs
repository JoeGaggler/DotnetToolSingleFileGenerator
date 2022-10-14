using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jmg.VsixProject
{
	static class DotnetRunner
	{
		private const String DotnetExecutableName = "dotnet";

		public static String Run(String fileContents, String workingDirectory, String fileNamespace, String toolName, String baseName, String extension, Boolean runGlobalTool, String inputFilePath)
		{
			var args = $"{toolName} --namespace \"{fileNamespace}\" --name \"{baseName}\" --extension \"{extension}\" --input \"{inputFilePath}\"";
			if (!runGlobalTool)
			{
				// Explicitly call local tool
				args = "tool run " + args;
			}

			var processStartInfo = new ProcessStartInfo(fileName: DotnetExecutableName, arguments: args)
			{
				RedirectStandardInput = true,
				RedirectStandardOutput = true,
				RedirectStandardError = true,
				CreateNoWindow = true,
				ErrorDialog = false,
				StandardOutputEncoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false, throwOnInvalidBytes: true),
				UseShellExecute = false,
				WindowStyle = ProcessWindowStyle.Hidden,
				WorkingDirectory = workingDirectory
			};

			var process = Process.Start(processStartInfo);

			using (var writer = process.StandardInput)
			{
				process.StandardInput.Write(fileContents);
			}

			var outputString = process.StandardOutput.ReadToEnd();
			var errorString = process.StandardError.ReadToEnd();

			errorString += Environment.NewLine + $"Args: {args}";
			errorString += Environment.NewLine + $"Folder: {workingDirectory}";
			errorString += Environment.NewLine + $"";

			var didExit = process.WaitForExit((int)TimeSpan.FromSeconds(10).TotalMilliseconds);
			if (didExit)
			{
				if (process.ExitCode == 0)
				{
					return outputString;
				}
				else
				{
					return GetErrorHeader() + $"#error The generator returned error code {process.ExitCode}{Environment.NewLine}/*{Environment.NewLine}{errorString}{Environment.NewLine}*/";
				}
			}
			else
			{
				process.Kill();
				return GetErrorHeader() + $"#error The generator failed to complete within ten seconds.";
			}
		}

		private static String GetErrorHeader()
		{
			return String.Empty;
		}
	}
}

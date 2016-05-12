using System;
using System.Diagnostics;
using System.IO;
using dumbo.Compiler;

namespace dumbo.WpfApp
{
    public class GccCompiler
    {
        private const string PathEnv = "PATH";
        
        public GccCompiler()
        {
            SetupEnvironment();
        }

        public GccCompilerResult Compile(string sourceFilePath)
        {
            var sourceFileInfo = new FileInfo(sourceFilePath);
            var targetFile = new FileInfo(Path.Combine(sourceFileInfo.DirectoryName, "out.exe"));
            var result = new GccCompilerResult(targetFile.FullName);

            Process process = new Process();
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.WorkingDirectory = Environment.CurrentDirectory;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.FileName = "gcc.exe";
            process.OutputDataReceived += (s, e) => result.AddOutput(e.Data);
            process.ErrorDataReceived += (s, e) => result.AddError(e.Data);
            process.StartInfo.Arguments = $"{sourceFileInfo.Name} -o {targetFile.FullName}";
            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            process.WaitForExit();
            result.StatusCode = process.ExitCode;
            process.Close();

            return result;
        }

        private void SetupEnvironment()
        {
            var baseDir = Utils.GetProjectRootPath();
            var mingwPath = Path.Combine(baseDir, "tools", "MinGW");
            var binPath = Path.Combine(mingwPath, "bin");
            var ming32BinPath = Path.Combine(mingwPath, "mingw32", "bin");
            var msysBinPath = Path.Combine(mingwPath, "msys", "1.0", "bin");

            var currentPath = Environment.GetEnvironmentVariable(PathEnv);
            currentPath += ";" + binPath;
            currentPath += ";" + ming32BinPath;
            currentPath += ";" + msysBinPath;

            Environment.SetEnvironmentVariable(PathEnv, currentPath);
        }
    }
}
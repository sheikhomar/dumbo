using System;
using System.Diagnostics;
using System.IO;

namespace dumbo.Compiler
{
    public class GccCompiler
    {
        private const string PathEnv = "PATH";
        
        public GccCompiler()
        {
            SetupEnvironment();
        }

        public ProcessResult Compile(string sourceFilePath, string targetPath)
        {
            var sourceFileInfo = new FileInfo(sourceFilePath);
            var targetFile = new FileInfo(targetPath);
            if (targetFile.Exists)
            {
                targetFile.Delete();
            }
            var result = new ProcessResult();

            try
            {
                Process process = new Process();
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.WorkingDirectory = Environment.CurrentDirectory;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.FileName = "gcc.exe";
                process.OutputDataReceived += (s, e) => result.AddOutput(e.Data);
                process.ErrorDataReceived += (s, e) => result.AddError(e.Data);
                process.StartInfo.Arguments = $"\"{sourceFileInfo.FullName}\" -o \"{targetFile.FullName}\" -std=c99";
                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
                process.WaitForExit();
                result.StatusCode = process.ExitCode;
                process.Close();
            }
            catch (Exception e)
            {
                result.StatusCode = -1;
                result.AddError($"Unexpected error while compiling with GCC: '{e.Message}'.\n{e.StackTrace}");
            }

            return result;
        }

        private void SetupEnvironment()
        {
            var baseDir = Utils.GetBasePath();
            var mingwPath = Path.Combine(baseDir, "MinGW");
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

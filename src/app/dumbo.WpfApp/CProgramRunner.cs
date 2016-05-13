using System;
using System.Diagnostics;
using System.IO;

namespace dumbo.WpfApp
{
    public class CProgramRunner
    {
        public ProcessResult Run(string binaryPath)
        {
            var targetFile = new FileInfo(binaryPath);
            var result = new ProcessResult();

            Process process = new Process();
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.WorkingDirectory = Environment.CurrentDirectory;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.FileName = targetFile.FullName;
            process.OutputDataReceived += (s, e) => result.AddOutput(e.Data);
            process.ErrorDataReceived += (s, e) => result.AddError(e.Data);
            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            process.WaitForExit();
            result.StatusCode = process.ExitCode;
            process.Close();

            return result;
        }
    }
}
using System;
using System.Diagnostics;
using System.IO;
using dumbo.WpfApp;

namespace dumbo.Compiler
{
    public class CProgramRunner
    {
        public ProcessResult Run(string binaryPath)
        {
            var targetFile = new FileInfo(binaryPath);
            var result = new ProcessResult();

            try
            {
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
            }
            catch (Exception e)
            {
                result.StatusCode = -1;
                result.AddError($"Unexpected error while running the compiled program: '{e.Message}'.\n{e.StackTrace}");
            }
            
            return result;
        }

        // Without running the program inside ILE but in a separate windows
        //public ProcessResult Run(string binaryPath)
        //{
        //    var targetFile = new FileInfo(binaryPath);
        //    var result = new ProcessResult();
        //
        //    Process process = new Process();
        //    process.StartInfo.CreateNoWindow = false;
        //    process.StartInfo.WorkingDirectory = Environment.CurrentDirectory;
        //    process.StartInfo.UseShellExecute = true;
        //    process.StartInfo.RedirectStandardError = false;
        //    process.StartInfo.RedirectStandardOutput = false;
        //    process.StartInfo.FileName = targetFile.FullName;
        //    //process.OutputDataReceived += (s, e) => result.AddOutput(e.Data);
        //    //process.ErrorDataReceived += (s, e) => result.AddError(e.Data);
        //    process.Start();
        //    //process.BeginOutputReadLine();
        //    //process.BeginErrorReadLine();
        //    process.WaitForExit();
        //    result.StatusCode = process.ExitCode;
        //    process.Close();
        //
        //    return result;
        //}
    }
}
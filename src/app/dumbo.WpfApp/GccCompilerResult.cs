using System.Collections.Generic;
using System.IO;

namespace dumbo.WpfApp
{
    public class GccCompilerResult
    {
        public string BinaryPath { get; }
        private readonly List<string> _outputData;
        private readonly List<string> _errorData;

        public IEnumerable<string> OutputData => _outputData;
        public IEnumerable<string> ErrorData => _errorData;
        public int StatusCode { get; set; }

        public GccCompilerResult(string binaryPath)
        {
            BinaryPath = binaryPath;
            _outputData = new List<string>();
            _errorData = new List<string>();
        }

        public void AddError(string error)
        {
            _errorData.Add(error);
        }

        public void AddOutput(string output)
        {
            _outputData.Add(output);
        }
    }
}
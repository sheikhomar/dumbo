using System;
using System.Collections.Generic;
using System.IO;

namespace dumbo.WpfApp
{
    public class ProcessResult
    {
        private readonly List<string> _outputData;
        private readonly List<string> _errorData;

        public IEnumerable<string> OutputData => _outputData;
        public IEnumerable<string> ErrorData => _errorData;
        public int StatusCode { get; set; }

        public ProcessResult()
        {
            _outputData = new List<string>();
            _errorData = new List<string>();
        }

        public void AddError(string error)
        {
            if (error != null) 
                _errorData.Add(error);
        }

        public void AddOutput(string output)
        {
            if (output != null)
                _outputData.Add(output);
        }
    }
}
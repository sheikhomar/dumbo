using System;
using System.IO;

namespace dumbo.Tests
{
    public class TestProgramReader : IDisposable
    {
        public const string CorrectProgramMarker = "//New Program";
        public const string IncorrectProgramMarker = "//New Program Failing";

        private TestProgramFragment _currentTestProgram;
        private bool _endOfFile;
        private int _currentLine;
        private readonly StreamReader _stream;
        private string _previousLine;

        public TestProgramFragment CurrentTestProgram => _currentTestProgram;

        public TestProgramReader(string path)
        {
            _currentLine = 0;
            _stream = new StreamReader(path);
            _endOfFile = false;
            ReadNextLine();
        }

        public bool ReadNext()
        {
            if (_endOfFile)
                return false;
            
            bool startReadingProgram = false;
            while (true)
            {
                if (_previousLine == null)
                {
                    _endOfFile = true;
                    break;
                }

                var isCorrectMarker = CorrectProgramMarker.Equals(_previousLine.Trim());
                var isIncorrectMarker = IncorrectProgramMarker.Equals(_previousLine.Trim());
                var isNewProgramMarker = isCorrectMarker || isIncorrectMarker;

                if (isNewProgramMarker)
                {
                    if (startReadingProgram)
                    {
                        break;
                    }
                    
                    startReadingProgram = true;
                    _currentTestProgram = new TestProgramFragment(_currentLine);
                    _currentTestProgram.ShouldFail = isIncorrectMarker;
                }

                if (!startReadingProgram)
                {
                    ReadNextLine();
                    continue;
                }

                _currentTestProgram.Buffer.AppendLine(_previousLine);
                ReadNextLine();
            }

            return true;
        }

        private void ReadNextLine()
        {
            _previousLine = _stream.ReadLine();
            _currentLine++;
        }
        
        public void Dispose()
        {
            _stream?.Dispose();
        }
    }
}

using System;
using System.IO;

namespace dumbo.Tests.SemanticAnalysis
{
    public class ProgramReader : IDisposable
    {
        public const string CorrectProgramMarker = " //New Program ";
        public const string IncorrectProgramMarker = " //New Program Failing";

        private ProgramFragment _currentProgram;
        private bool _endOfFile;
        private int _currentLine;
        private readonly StreamReader _stream;

        public ProgramFragment CurrentProgram => _currentProgram;

        public ProgramReader(string path)
        {
            _currentLine = 0;
            _stream = new StreamReader(path);
            _endOfFile = false;
        }

        public bool ReadNext()
        {
            if (_endOfFile)
                return false;

            _currentProgram = new ProgramFragment(_currentLine + 1);
            while (true)
            {
                string line = _stream.ReadLine();
                _currentLine++;

                if (line == null)
                {
                    _endOfFile = true;
                    break;
                }

                var isCorrectMarker = CorrectProgramMarker.Equals(line);
                var isIncorrectMarker = IncorrectProgramMarker.Equals(line);
                var isNewProgramMarker = isCorrectMarker || isIncorrectMarker;

                if (isNewProgramMarker)
                {
                    _currentProgram.ShouldFail = isIncorrectMarker;
                    break;
                }
                    

                _currentProgram.Buffer.AppendLine(line);
            }

            return true;
        }
        
        public void Dispose()
        {
            _stream?.Dispose();
        }
    }
}

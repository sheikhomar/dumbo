using System;
using System.IO;

namespace dumbo.Tests.SemanticAnalysis
{
    internal class ProgramReader : IDisposable
    {
        private readonly string _path;
        private readonly string _fileName;
        private ProgramFragment _currentProgram;
        private bool _newProgramStart = true;
        private int _currentLine = 0;
        private StreamReader _stream;
        public const string CorrectProgramMarker = " //New Program ";
        public const string IncorrectProgramMarker = " //New Program Failing";

        public ProgramReader(string path)
        {
            _path = path;

            _currentProgram = new ProgramFragment();
            _currentLine = 0;
            _stream = new StreamReader(_path);
        }

        public bool ReadNext()
        {
            string line;
            while ((line = _stream.ReadLine()) != null)
            {
                if (_newProgramStart)
                {
                    var isCorrectMarker = CorrectProgramMarker.Equals(line);
                    var isIncorrectMarker = IncorrectProgramMarker.Equals(line);
                    var isNewProgramMarker = isCorrectMarker || isIncorrectMarker;

                    if (isNewProgramMarker)
                    {
                        _currentProgram = new ProgramFragment();
                    }
                    continue;
                }

                _currentProgram.Buffer.AppendLine(line);
            }

            return false;
        }

        public ProgramFragment CurrentProgram => _currentProgram;

        public void Dispose()
        {
            _stream?.Dispose();
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;

namespace dumbo.Compiler.SyntaxAnalysis
{
    public class ParserResult
    {
        private readonly IList<LexicalErrorInfo> _lexicalErrors;
        private readonly IList<SyntaxErrorInfo> _syntaxErrors;

        public ParserResult(string errorMessage) 
        {
            _lexicalErrors = new List<LexicalErrorInfo>();
            _syntaxErrors = new List<SyntaxErrorInfo>();
            ErrorMessage = errorMessage;
        }

        public ParserResult() : this(String.Empty)
        {
            
        }

        public IEnumerable<LexicalErrorInfo> LexicalErrors => _lexicalErrors;

        public IEnumerable<SyntaxErrorInfo> SyntaxErrors => _syntaxErrors;

        public string ErrorMessage { get; internal set; }

    }
}
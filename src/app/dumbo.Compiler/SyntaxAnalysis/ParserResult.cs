using System;
using System.Collections.Generic;
using dumbo.Compiler.AST;

namespace dumbo.Compiler.SyntaxAnalysis
{
    public class ParserResult
    {
        private readonly IList<ParserError> _errors;
        private RootNode _root;

        public ParserResult(string errorMessage)
        {
            _errors = new List<ParserError>();
            AddGeneralError(errorMessage);
        }

        public ParserResult() : this(string.Empty)
        {

        }

        public IEnumerable<ParserError> Errors => _errors;

        public bool IsSuccess => _errors.Count == 0;

        public RootNode Root
        {
            get { return _root; }
            set
            {
                if (_root != null)
                    throw new InvalidOperationException("Root node can only set once.");
                _root = value;
            }
        }

        public void AddGeneralError(string message)
        {
            if (!string.IsNullOrWhiteSpace(message))
                _errors.Add(new GeneralParserError(message));
        }

        public void AddLexicalError(LexicalError lexicalError)
        {
            _errors.Add(lexicalError);
        }

        public void AddSyntaxError(SyntaxError syntaxError)
        {
            _errors.Add(syntaxError);
        }
    }
}
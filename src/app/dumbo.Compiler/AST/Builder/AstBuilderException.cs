using System;

namespace dumbo.Compiler.AST.Builder
{
    public class ExceptedTokensNotFoundException : AstBuilderException
    {
        private readonly string _expectedToken;

        public ExceptedTokensNotFoundException(string expectedToken)
        {
            _expectedToken = expectedToken;
        }

        public ExceptedTokensNotFoundException(params string[] expectedTokens)
        {
            _expectedToken = string.Join(", ", expectedTokens);
        }

        public override string Message
        {
            get { return $"A wrong parse tree is provided. Expecting '{_expectedToken}' tokens at this point"; }
        }
    }

    public class RuleNotFoundException : AstBuilderException
    {
        private readonly string _expectedToken;

        public RuleNotFoundException(string expectedToken)
        {
            _expectedToken = expectedToken;
        }

        public override string Message
        {
            get { return $"A wrong parse tree is provided. Expecting an '{_expectedToken}' token at this point"; }
        }
    }

    public class AstBuilderException : InvalidOperationException
    {
        
    }
}
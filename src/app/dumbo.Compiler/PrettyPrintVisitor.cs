using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using dumbo.Compiler.AST;

namespace dumbo.Compiler
{
    public class PrettyPrintVisitor : IVisitor<VisitResult, VisitorArgs>
    {
        private const int TabSize = 2;
        private readonly StringBuilder _buffer;
        private int _currentIdentation = 0;
        private readonly VisitResult _emptyResult;
        private bool _isNewLine;

        public PrettyPrintVisitor()
        {
            _buffer = new StringBuilder();
            _emptyResult = new VisitResult();
            _isNewLine = true;
        }

        public string GetResult()
        {
            return _buffer.ToString();
        }

        public VisitResult Visit(ActualParamListNode node, VisitorArgs arg)
        {
            VisitChildren(node, ", ", arg);

            return _emptyResult;
        }

        public VisitResult Visit(AssignmentStmtNode node, VisitorArgs arg)
        {
            node.Identifiers.Accept(this, arg);
            Write(" := ");
            node.Expressions.Accept(this, arg);
            WriteLine();

            return _emptyResult;
        }

        public VisitResult Visit(BinaryOperationNode node, VisitorArgs arg)
        {
            node.LeftOperand.Accept(this, arg);
            Write(" ");
            Write(OperatorTranslator.BinaryOperatorTypeTranslator(node.Operator));
            Write(" ");
            node.RightOperand.Accept(this, arg);

            return _emptyResult;
        }

        public VisitResult Visit(BreakStmtNode node, VisitorArgs arg)
        {
            WriteLine("Break");

            return _emptyResult;
        }

        public VisitResult Visit(DeclAndAssignmentStmtNode node, VisitorArgs arg)
        {
            Write($"{node.Type} ");
            node.Identifiers.Accept(this, arg);
            Write(" := ");
            node.Expressions.Accept(this, arg);
            WriteLine();

            return _emptyResult;
        }

        public VisitResult Visit(DeclStmtNode node, VisitorArgs arg)
        {
            Write($"{node.Type} ");
            node.Identifiers.Accept(this, arg);
            WriteLine();

            return _emptyResult;
        }

        public VisitResult Visit(ElseIfStmtListNode node, VisitorArgs arg)
        {
            foreach (var item in node)
                item.Accept(this, arg);

            return _emptyResult;
        }

        public VisitResult Visit(ElseIfStmtNode node, VisitorArgs arg)
        {
            Write("Else If (");
            node.Predicate.Accept(this, arg);
            WriteLine(") Then");
            node.Body.Accept(this, arg);

            return _emptyResult;
        }

        public VisitResult Visit(ExpressionListNode node, VisitorArgs arg)
        {
            VisitChildren(node, ", ", arg);

            return _emptyResult;
        }

        public VisitResult Visit(FormalParamListNode node, VisitorArgs arg)
        {
            VisitChildren(node, ", ", arg);

            return _emptyResult;
        }

        public VisitResult Visit(FormalParamNode node, VisitorArgs arg)
        {
            Write($"{node.Type} {node.Name}");

            return _emptyResult;
        }

        public VisitResult Visit(FuncCallExprNode node, VisitorArgs arg)
        {
            Write($"{node.FuncName}(");
            node.Parameters.Accept(this, arg);
            Write(")");

            return _emptyResult;
        }

        public VisitResult Visit(FuncCallStmtNode node, VisitorArgs arg)
        {
            node.CallNode.Accept(this, arg);
            WriteLine();

            return _emptyResult;
        }

        public VisitResult Visit(FuncDeclListNode node, VisitorArgs arg)
        {
            foreach (var item in node)
                item.Accept(this, arg);

            return _emptyResult;
        }

        public VisitResult Visit(FuncDeclNode node, VisitorArgs arg)
        {
            WriteLine();
            Write($"Function {node.Name}(");
            node.Parameters.Accept(this, arg);
            Write(") Returns ");

            if (node.ReturnTypes.Any())
            {
                var retTypes = node.ReturnTypes.Select(rt => rt.ToString());
                Write(string.Join(", ", retTypes));
            }
            else
            {
                Write("Nothing");
            }

            WriteLine();
            node.Body.Accept(this, arg);
            WriteLine("End Function");

            return _emptyResult;
        }

        public VisitResult Visit(IdentifierListNode node, VisitorArgs arg)
        {
            VisitChildren(node, ", ", arg);

            return _emptyResult;
        }

        public VisitResult Visit(IdentifierNode node, VisitorArgs arg)
        {
            Write(node.Name);

            return _emptyResult;
        }

        public VisitResult Visit(IfElseStmtNode node, VisitorArgs arg)
        {
            Write("If (");
            node.Predicate.Accept(this, arg);
            WriteLine(") Then");
            node.Body.Accept(this, arg);
            node.ElseIfStatements.Accept(this, arg);
            WriteLine("Else");
            node.Else.Accept(this, arg);
            WriteLine("End If");

            return _emptyResult;
        }

        public VisitResult Visit(IfStmtNode node, VisitorArgs arg)
        {
            Write("If (");
            node.Predicate.Accept(this, arg);
            WriteLine(") Then");
            node.Body.Accept(this, arg);
            node.ElseIfStatements.Accept(this, arg);
            WriteLine("End If");

            return _emptyResult;
        }

        public VisitResult Visit(LiteralValueNode node, VisitorArgs arg)
        {
            Write(node.Value);

            return _emptyResult;
        }

        public VisitResult Visit(ProgramNode node, VisitorArgs arg)
        {
            WriteLine("Program");
            node.Body.Accept(this, arg);
            WriteLine("End Program");

            return _emptyResult;
        }

        public VisitResult Visit(RepeatStmtNode node, VisitorArgs arg)
        {
            Write("Repeat (");
            node.Number.Accept(this, arg);
            WriteLine(")");
            node.Body.Accept(this, arg);
            WriteLine("End Repeat");

            return _emptyResult;
        }

        public VisitResult Visit(RepeatWhileStmtNode node, VisitorArgs arg)
        {
            Write("Repeat While (");
            node.Predicate.Accept(this, arg);
            WriteLine(")");
            node.Body.Accept(this, arg);
            WriteLine("End Repeat");

            return _emptyResult;
        }

        public VisitResult Visit(ReturnStmtNode node, VisitorArgs arg)
        {
            Write("Return ");
            if (node.Expressions.Count == 0)
                Write("Nothing");
            else
                node.Expressions.Accept(this, arg);
            WriteLine();

            return _emptyResult;
        }

        public VisitResult Visit(RootNode node, VisitorArgs arg)
        {
            node.Program.Accept(this, arg);
            node.FuncDecls.Accept(this, arg);

            return _emptyResult;
        }

        public VisitResult Visit(StmtBlockNode node, VisitorArgs arg)
        {
            if (node.Count > 0)
            {
                Indent();

                foreach (var item in node)
                    item.Accept(this, arg);

                Unindent();
            }
            
            return _emptyResult;
        }

        public VisitResult Visit(UnaryOperationNode node, VisitorArgs arg)
        {
            Write(OperatorTranslator.UnaryOperatorTypeTranslator(node.Operator));
            node.Expression.Accept(this, arg);

            return _emptyResult;
        }
        
        private void VisitChildren<T>(IEnumerable<T> children, string separator, VisitorArgs arg) where T : IVisitable
        {
            bool isFirst = true;
            foreach (var node in children)
            {
                if (!isFirst) Write(separator);

                node.Accept(this, arg);

                isFirst = false;
            }
        }

        private void Write(string input)
        {
            if (_isNewLine)
            {
                _buffer.Append(new string(' ', _currentIdentation * TabSize));
                _isNewLine = false;
            }
            
            _buffer.Append(input);
        }

        public void WriteLine(string input = "")
        {
            Write(input);
            _buffer.AppendLine();
            _isNewLine = true;
        }

        private void Indent()
        {
            _currentIdentation++;
        }

        private void Unindent()
        {
            if (_currentIdentation == 0)
                throw new InvalidOperationException("Cannot unindent beyond zero.");

            _currentIdentation--;    
        }
    }
}
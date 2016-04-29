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
        private int _currentIndentation = 0;
        private VisitResult EmptyResult { get; }
        private bool _isNewLine;

        public PrettyPrintVisitor()
        {
            _buffer = new StringBuilder();
            EmptyResult = new VisitResult();
            _isNewLine = true;
        }

        public string GetResult()
        {
            return _buffer.ToString();
        }

        public VisitResult Visit(ActualParamListNode node, VisitorArgs arg)
        {
            VisitChildren(node, ", ", arg);

            return EmptyResult;
        }

        public VisitResult Visit(AssignmentStmtNode node, VisitorArgs arg)
        {
            node.Identifiers.Accept(this, arg);
            Write(" := ");
            node.Expressions.Accept(this, arg);
            WriteLine();

            return EmptyResult;
        }

        public VisitResult Visit(BinaryOperationNode node, VisitorArgs arg)
        {
            node.LeftOperand.Accept(this, arg);
            Write(" ");
            Write(OperatorTranslator.BinaryOperatorTypeTranslator(node.Operator));
            Write(" ");
            node.RightOperand.Accept(this, arg);

            return EmptyResult;
        }

        public VisitResult Visit(BreakStmtNode node, VisitorArgs arg)
        {
            WriteLine("Break");

            return EmptyResult;
        }

        public VisitResult Visit(BuiltInFuncDeclNode node, VisitorArgs arg)
        {
            return EmptyResult;
        }

        public VisitResult Visit(DeclAndAssignmentStmtNode node, VisitorArgs arg)
        {
            Write($"{node.Type} ");
            node.Identifiers.Accept(this, arg);
            Write(" := ");
            node.Expressions.Accept(this, arg);
            WriteLine();

            return EmptyResult;
        }

        public VisitResult Visit(DeclStmtNode node, VisitorArgs arg)
        {
            Write($"{node.Type} ");
            node.Identifiers.Accept(this, arg);
            WriteLine();

            return EmptyResult;
        }

        public VisitResult Visit(ElseIfStmtListNode node, VisitorArgs arg)
        {
            foreach (var item in node)
                item.Accept(this, arg);

            return EmptyResult;
        }

        public VisitResult Visit(ElseIfStmtNode node, VisitorArgs arg)
        {
            Write("Else If (");
            node.Predicate.Accept(this, arg);
            WriteLine(") Then");
            node.Body.Accept(this, arg);

            return EmptyResult;
        }

        public VisitResult Visit(ExpressionListNode node, VisitorArgs arg)
        {
            VisitChildren(node, ", ", arg);

            return EmptyResult;
        }

        public VisitResult Visit(FormalParamListNode node, VisitorArgs arg)
        {
            VisitChildren(node, ", ", arg);

            return EmptyResult;
        }

        public VisitResult Visit(FormalParamNode node, VisitorArgs arg)
        {
            Write($"{node.Type} {node.Name}");

            return EmptyResult;
        }

        public VisitResult Visit(FuncCallExprNode node, VisitorArgs arg)
        {
            Write($"{node.FuncName}(");
            node.Parameters.Accept(this, arg);
            Write(")");

            return EmptyResult;
        }

        public VisitResult Visit(FuncCallStmtNode node, VisitorArgs arg)
        {
            node.CallNode.Accept(this, arg);
            WriteLine();

            return EmptyResult;
        }

        public VisitResult Visit(FuncDeclListNode node, VisitorArgs arg)
        {
            foreach (var item in node)
                item.Accept(this, arg);

            WriteLine();
            WriteLine();

            return EmptyResult;
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

            return EmptyResult;
        }

        public VisitResult Visit(IdentifierListNode node, VisitorArgs arg)
        {
            VisitChildren(node, ", ", arg);

            return EmptyResult;
        }

        public VisitResult Visit(IdentifierNode node, VisitorArgs arg)
        {
            Write(node.Name);

            return EmptyResult;
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

            return EmptyResult;
        }

        public VisitResult Visit(IfStmtNode node, VisitorArgs arg)
        {
            Write("If (");
            node.Predicate.Accept(this, arg);
            WriteLine(") Then");
            node.Body.Accept(this, arg);
            node.ElseIfStatements.Accept(this, arg);
            WriteLine("End If");

            return EmptyResult;
        }

        public VisitResult Visit(LiteralValueNode node, VisitorArgs arg)
        {
            Write(node.Value);

            return EmptyResult;
        }

        public VisitResult Visit(PrimitiveTypeNode node, VisitorArgs arg)
        {
            Write(node.Type.ToString());

            return EmptyResult;
        }

        public VisitResult Visit(ProgramNode node, VisitorArgs arg)
        {
            WriteLine("Program");
            node.Body.Accept(this, arg);
            WriteLine("End Program");

            return EmptyResult;
        }

        public VisitResult Visit(RepeatStmtNode node, VisitorArgs arg)
        {
            Write("Repeat (");
            node.Number.Accept(this, arg);
            WriteLine(")");
            node.Body.Accept(this, arg);
            WriteLine("End Repeat");

            return EmptyResult;
        }

        public VisitResult Visit(RepeatWhileStmtNode node, VisitorArgs arg)
        {
            Write("Repeat While (");
            node.Predicate.Accept(this, arg);
            WriteLine(")");
            node.Body.Accept(this, arg);
            WriteLine("End Repeat");

            return EmptyResult;
        }

        public VisitResult Visit(ReturnStmtNode node, VisitorArgs arg)
        {
            Write("Return ");
            if (node.Expressions.Count == 0)
                Write("Nothing");
            else
                node.Expressions.Accept(this, arg);
            WriteLine();

            return EmptyResult;
        }

        public VisitResult Visit(RootNode node, VisitorArgs arg)
        {
            node.Program.Accept(this, arg);
            node.FuncDecls.Accept(this, arg);

            return EmptyResult;
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

            return EmptyResult;
        }

        public VisitResult Visit(UnaryOperationNode node, VisitorArgs arg)
        {
            Write(OperatorTranslator.UnaryOperatorTypeTranslator(node.Operator));
            node.Expression.Accept(this, arg);

            return EmptyResult;
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
                _buffer.Append(new string(' ', _currentIndentation * TabSize));
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
            _currentIndentation++;
        }

        private void Unindent()
        {
            if (_currentIndentation == 0)
                throw new InvalidOperationException("Cannot unindent beyond zero.");

            _currentIndentation--;
        }
    }
}

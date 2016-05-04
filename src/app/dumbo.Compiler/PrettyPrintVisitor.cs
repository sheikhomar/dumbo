using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using dumbo.Compiler.AST;

namespace dumbo.Compiler
{
    public class PrettyPrintVisitor : IVisitor<object, VisitorArgs>
    {
        private const int TabSize = 2;
        private readonly StringBuilder _buffer;
        private int _currentIndentation = 0;
        private bool _isNewLine;

        public PrettyPrintVisitor()
        {
            _buffer = new StringBuilder();
            _isNewLine = true;
        }

        public string GetResult()
        {
            return _buffer.ToString();
        }

        public object Visit(ActualParamListNode node, VisitorArgs arg)
        {
            VisitChildren(node, ", ", arg);

            return null;
        }

        public object Visit(ArrayDeclStmtNode node, VisitorArgs arg)
        {
            node.Type.Accept(this, arg);
            Write(" ");
            node.Identifiers.Accept(this, arg);
            WriteLine();

            return null;
        }

        public object Visit(ArrayIdentifierNode node, VisitorArgs arg)
        {
            Write(node.Name);
            Write("[");
            VisitChildren(node.Indices, ",", arg);
            Write("]");
            return null;
        }

        public object Visit(ArrayTypeNode node, VisitorArgs arg)
        {
            Write("Array[");
            VisitChildren(node.Sizes, ", ", arg);
            Write("] of ");
            node.Type.Accept(this, arg);
            return null;
        }

        public object Visit(ArrayValueNode node, VisitorArgs arg)
        {
            WriteNestedListNode(node.Values, arg);

            return null;
        }

        public object Visit(AssignmentStmtNode node, VisitorArgs arg)
        {
            node.Identifiers.Accept(this, arg);
            Write(" := ");
            node.Value.Accept(this, arg);
            WriteLine();

            return null;
        }

        public object Visit(BinaryOperationNode node, VisitorArgs arg)
        {
            node.LeftOperand.Accept(this, arg);
            Write(" ");
            Write(OperatorTranslator.BinaryOperatorTypeTranslator(node.Operator));
            Write(" ");
            node.RightOperand.Accept(this, arg);

            return null;
        }

        public object Visit(BreakStmtNode node, VisitorArgs arg)
        {
            WriteLine("Break");

            return null;
        }

        public object Visit(ContinueStmtNode node, VisitorArgs arg)
        {
            WriteLine("Continue");

            return null;
        }

        public object Visit(ConstDeclListNode node, VisitorArgs arg)
        {
            foreach (var item in node)
            {
                item.Accept(this, arg);
            }

            return null;
        }

        public object Visit(ConstDeclNode node, VisitorArgs arg)
        {
            Write($"Constant {node.Type} {node.Name} ");
            Write(" := ");
            node.Value.Accept(this, arg);
            WriteLine();

            return null;
        }

        public object Visit(BuiltInFuncDeclNode node, VisitorArgs arg)
        {
            return null;
        }

        public object Visit(DeclAndAssignmentStmtNode node, VisitorArgs arg)
        {
            node.Type.Accept(this, arg);
            Write(" ");
            node.Identifiers.Accept(this, arg);
            Write(" := ");
            node.Value.Accept(this, arg);
            WriteLine();

            return null;
        }

        public object Visit(PrimitiveDeclStmtNode node, VisitorArgs arg)
        {
            Write($"{node.Type} ");
            node.Identifiers.Accept(this, arg);
            WriteLine();

            return null;
        }

        public object Visit(ElseIfStmtListNode node, VisitorArgs arg)
        {
            foreach (var item in node)
                item.Accept(this, arg);

            return null;
        }

        public object Visit(ElseIfStmtNode node, VisitorArgs arg)
        {
            Write("Else If (");
            node.Predicate.Accept(this, arg);
            WriteLine(") Then");
            node.Body.Accept(this, arg);

            return null;
        }

        public object Visit(ExpressionListNode node, VisitorArgs arg)
        {
            VisitChildren(node, ", ", arg);

            return null;
        }

        public object Visit(FormalParamListNode node, VisitorArgs arg)
        {
            VisitChildren(node, ", ", arg);

            return null;
        }

        public object Visit(FormalParamNode node, VisitorArgs arg)
        {
            Write($"{node.Type} {node.Name}");

            return null;
        }

        public object Visit(FuncCallExprNode node, VisitorArgs arg)
        {
            Write($"{node.FuncName}(");
            node.Parameters.Accept(this, arg);
            Write(")");

            return null;
        }

        public object Visit(FuncCallStmtNode node, VisitorArgs arg)
        {
            node.CallNode.Accept(this, arg);
            WriteLine();

            return null;
        }

        public object Visit(FuncDeclListNode node, VisitorArgs arg)
        {
            foreach (var item in node)
                item.Accept(this, arg);

            WriteLine();
            WriteLine();

            return null;
        }

        public object Visit(FuncDeclNode node, VisitorArgs arg)
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

            return null;
        }

        public object Visit(IdentifierListNode node, VisitorArgs arg)
        {
            VisitChildren(node, ", ", arg);

            return null;
        }

        public object Visit(IdentifierNode node, VisitorArgs arg)
        {
            Write(node.Name);

            return null;
        }

        public object Visit(IfElseStmtNode node, VisitorArgs arg)
        {
            Write("If (");
            node.Predicate.Accept(this, arg);
            WriteLine(") Then");
            node.Body.Accept(this, arg);
            node.ElseIfStatements.Accept(this, arg);
            WriteLine("Else");
            node.Else.Accept(this, arg);
            WriteLine("End If");

            return null;
        }

        public object Visit(IfStmtNode node, VisitorArgs arg)
        {
            Write("If (");
            node.Predicate.Accept(this, arg);
            WriteLine(") Then");
            node.Body.Accept(this, arg);
            node.ElseIfStatements.Accept(this, arg);
            WriteLine("End If");

            return null;
        }

        public object Visit(LiteralValueNode node, VisitorArgs arg)
        {
            if (node.Type is PrimitiveTypeNode)
            {
                var primType = node.Type as PrimitiveTypeNode;
                switch (primType.Type)
                {
                    case PrimitiveType.Number:
                        Write(node.Value);
                        break;
                    case PrimitiveType.Text:
                        Write($"\"{node.Value}\"");
                        break;
                    case PrimitiveType.Boolean:
                        Write(node.Value);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            return null;
        }

        public object Visit(PrimitiveTypeNode node, VisitorArgs arg)
        {
            Write(node.Type.ToString());

            return null;
        }

        public object Visit(ProgramNode node, VisitorArgs arg)
        {
            WriteLine("Program");
            node.Body.Accept(this, arg);
            WriteLine("End Program");

            return null;
        }

        public object Visit(RepeatStmtNode node, VisitorArgs arg)
        {
            Write("Repeat (");
            node.Number.Accept(this, arg);
            WriteLine(")");
            node.Body.Accept(this, arg);
            WriteLine("End Repeat");

            return null;
        }

        public object Visit(RepeatWhileStmtNode node, VisitorArgs arg)
        {
            Write("Repeat While (");
            node.Predicate.Accept(this, arg);
            WriteLine(")");
            node.Body.Accept(this, arg);
            WriteLine("End Repeat");

            return null;
        }

        public object Visit(ReturnStmtNode node, VisitorArgs arg)
        {
            Write("Return ");
            if (node.Expressions.Count == 0)
                Write("Nothing");
            else
                node.Expressions.Accept(this, arg);
            WriteLine();

            return null;
        }

        public object Visit(RootNode node, VisitorArgs arg)
        {
            node.ConstDecls.Accept(this, arg);
            node.Program.Accept(this, arg);
            node.FuncDecls.Accept(this, arg);

            return null;
        }

        public object Visit(StmtBlockNode node, VisitorArgs arg)
        {
            if (node.Count > 0)
            {
                Indent();

                foreach (var item in node)
                    item.Accept(this, arg);

                Unindent();
            }

            return null;
        }

        public object Visit(UnaryOperationNode node, VisitorArgs arg)
        {
            Write(OperatorTranslator.UnaryOperatorTypeTranslator(node.Operator));
            node.Expression.Accept(this, arg);

            return null;
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

        private void WriteNestedListNode(NestedExpressionListNode node, VisitorArgs arg)
        {
            bool first = true;
            foreach (var item in node)
            {
                if (!first)
                {
                    Write(", ");
                }
                first = false;

                var nestedList = item as NestedExpressionListNode;
                if (nestedList != null && nestedList.Count > 0)
                {
                    Write("(");
                    WriteNestedListNode(nestedList, arg);
                    Write(")");
                }
                else
                {
                    WriteExpressionListNode(item as ExpressionListNode, arg);
                }
            }
        }

        private void WriteExpressionListNode(ExpressionListNode exprList, VisitorArgs arg)
        {
            Write("(");
            VisitChildren(exprList, ", ", arg);
            Write(")");
        }
    }
}

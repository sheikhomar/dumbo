using dumbo.Compiler.AST;
using System;
using System.Text;

namespace dumbo.Compiler.CodeGenerator
{
    public class RuntimeEntity
    {
    }

    public class CodeGeneratorVisitor : IVisitor<RuntimeEntity, VisitorArgs>
    {
        private Program _program;
        private Module _currentModule;
        private Stmt _stmt;

        public CodeGeneratorVisitor()
        {
            _program = new Program();
        }

        public RuntimeEntity Visit(ActualParamListNode node, VisitorArgs arg)
        {
            throw new System.NotImplementedException();
        }

        public RuntimeEntity Visit(AssignmentStmtNode node, VisitorArgs arg)
        {
            

            throw new System.NotImplementedException();
        }

        public RuntimeEntity Visit(BinaryOperationNode node, VisitorArgs arg)
        {
            node.LeftOperand.Accept(this, arg);
            _stmt.Append($" {ConvertBinaryOperator(node.Operator)} ");
            node.RightOperand.Accept(this, arg);

            return null;
        }

        public RuntimeEntity Visit(BreakStmtNode node, VisitorArgs arg)
        {
            _currentModule.Append(new Stmt("Break;"));

            return null;
        }

        public RuntimeEntity Visit(BuiltInFuncDeclNode node, VisitorArgs arg)
        {
            /// Todo -- All predefined functions the user can call
            
            throw new System.NotImplementedException();
        }

        public RuntimeEntity Visit(DeclAndAssignmentStmtNode node, VisitorArgs arg)
        {
            int declAssCount = node.Identifiers.Count;
            _stmt = new Stmt("");
            _stmt.Append($"{ConvertType(node.Type)} ");
            for (int i = 0; i < declAssCount; i++)
            {
                node.Identifiers[i].Accept(this, arg);
                _stmt.Append(" = ");
                node.Expressions[i].Accept(this, arg);
                if (i != 0)
                {
                    _stmt.Append(", ");
                }
            }
            _stmt.Append(";");

            return null;
        }

        public RuntimeEntity Visit(DeclStmtNode node, VisitorArgs arg)
        {
            _stmt = new Stmt("");
            _stmt.Append($"{ConvertType(node.Type)} ");
            node.Identifiers.Accept(this, arg);
            _currentModule.Append(_stmt);

            return null;
        }

        public RuntimeEntity Visit(ElseIfStmtListNode node, VisitorArgs arg)
        {
            foreach (var item in node)
            {
                item.Accept(this, arg);
            }

            return null;
        }

        public RuntimeEntity Visit(ElseIfStmtNode node, VisitorArgs arg)
        {
            _stmt = new Stmt("else if (");
            node.Predicate.Accept(this, arg);
            _stmt.Append(")");
            _currentModule.Append(_stmt);
            node.Body.Accept(this, arg);

            return null;
        }

        public RuntimeEntity Visit(ExpressionListNode node, VisitorArgs arg)
        {
            int count = node.Count;

            for (int i = 0; i < count; i++)
            {
                node[i].Accept(this, arg);
                if (i != 0)
                {
                    _stmt.Append(", ");
                }
            }

            return null;
        }

        public RuntimeEntity Visit(FormalParamListNode node, VisitorArgs arg)
        {
            throw new System.NotImplementedException();
        }

        public RuntimeEntity Visit(FormalParamNode node, VisitorArgs arg)
        {
            throw new System.NotImplementedException();
        }

        public RuntimeEntity Visit(FuncCallExprNode node, VisitorArgs arg)
        {
            throw new System.NotImplementedException();
        }

        public RuntimeEntity Visit(FuncCallStmtNode node, VisitorArgs arg)
        {
            throw new System.NotImplementedException();
        }

        public RuntimeEntity Visit(FuncDeclListNode node, VisitorArgs arg)
        {
            node.Accept(this, arg);
            return null;
        }

        public RuntimeEntity Visit(FuncDeclNode node, VisitorArgs arg)
        {
            FuncVisitorArgs funcArg = arg as FuncVisitorArgs;

            if (funcArg != null && funcArg.VisitBody)
            {
                
            }
            throw new System.NotImplementedException();
        }

        private void FuncHeader(FuncDeclNode node, bool body)
        {
            StringBuilder builder = new StringBuilder();

            //ret type

            //name

            //parameters + ()
            

            if (body)
                builder.Append('{');
            else
                builder.Append(';');

            _currentModule.Append(new Stmt(builder.ToString()));
        }

        public RuntimeEntity Visit(IdentifierListNode node, VisitorArgs arg)
        {
            int length = node.Count;

            for (int i = 0; i < length; i++)
            {
                node[i].Accept(this, arg);
                if (i != 0)
                {
                    _stmt.Append(", ");
                }
            }
            _stmt.Append($";");

            return null;
        }

        public RuntimeEntity Visit(IdentifierNode node, VisitorArgs arg)
        {
            _stmt.Append(node.Name);

            return null;
        }

        public RuntimeEntity Visit(IfElseStmtNode node, VisitorArgs arg)
        {
            _stmt = new Stmt("if (");
            node.Predicate.Accept(this, arg);
            _stmt.Append(")");
            _currentModule.Append(_stmt);
            node.Body.Accept(this, arg);
            node.ElseIfStatements.Accept(this, arg);
            _currentModule.Append(new Stmt($"else"));
            node.Else.Accept(this, arg);

            return null;
        }

        public RuntimeEntity Visit(IfStmtNode node, VisitorArgs arg)
        {
            _stmt = new Stmt("if (");
            node.Predicate.Accept(this, arg);
            _stmt.Append(")");
            _currentModule.Append(_stmt);
            node.Body.Accept(this, arg);
            node.ElseIfStatements.Accept(this, arg);

            return null;
        }

        public RuntimeEntity Visit(LiteralValueNode node, VisitorArgs arg)
        {
            _stmt.Append(node.Value);

            return null;
        }

        public RuntimeEntity Visit(ProgramNode node, VisitorArgs arg)
        {
            _currentModule.Append(new Stmt($"int main()"));
            node.Body.Accept(this, arg);

            return null;
        }

        public RuntimeEntity Visit(RepeatStmtNode node, VisitorArgs arg)
        {
            _stmt = new Stmt("for (int i=0; i<");
            node.Number.Accept(this, arg);
            _stmt.Append(" ; i++;");
            _currentModule.Append(_stmt);
            node.Body.Accept(this, arg);

            return null;
        }

        public RuntimeEntity Visit(RepeatWhileStmtNode node, VisitorArgs arg)
        {
            _stmt = new Stmt("while (");
            node.Predicate.Accept(this, arg);
            _stmt.Append(")");
            _currentModule.Append(_stmt);
            node.Body.Accept(this, arg);

            return null;
        }

        public RuntimeEntity Visit(ReturnStmtNode node, VisitorArgs arg)
        {
            _stmt = new Stmt("return ");
            node.Expressions.Accept(this, arg);
            _stmt.Append(";");

            return null;
        }

        public RuntimeEntity Visit(RootNode node, VisitorArgs arg)
        {
            /// Todo -- Add const + libraries?
            node.FuncDecls.Accept(this, new FuncVisitorArgs(false));
            node.Program.Accept(this, arg);
            node.FuncDecls.Accept(this, new FuncVisitorArgs(true));
            return null;
        }

        public RuntimeEntity Visit(StmtBlockNode node, VisitorArgs arg)
        {
            _currentModule.Append(new Stmt("{"));
            foreach (var item in node)
            {
                item.Accept(this, arg);
            }
            _currentModule.Append(new Stmt("}"));

            return null;
        }

        public RuntimeEntity Visit(UnaryOperationNode node, VisitorArgs arg)
        {
            _stmt.Append(ConvertUnaryOperator(node.Operator));
            node.Expression.Accept(this, arg);

            return null;
        }

        private string ConvertType(HappyType input)
        {
            switch (input)
            {
                case HappyType.Nothing: return "void";
                case HappyType.Number: return "double";
                case HappyType.Text: return "Text";
                case HappyType.Boolean: return "Boolean";
                case HappyType.Error: throw new ArgumentException($"{input} is not a valid type.");
                default: throw new ArgumentException($"{input} is not a valid type.");
            }
        }

        private string ConvertBinaryOperator(BinaryOperatorType input)
        {
            switch (input)
            {
                case BinaryOperatorType.Plus: return "+";
                case BinaryOperatorType.Minus: return "-";
                case BinaryOperatorType.Times: return "*";
                case BinaryOperatorType.Division: return "/";
                case BinaryOperatorType.Modulo: return "%";
                case BinaryOperatorType.Equals: return "==";
                case BinaryOperatorType.GreaterThan: return ">";
                case BinaryOperatorType.GreaterOrEqual: return ">=";
                case BinaryOperatorType.LessThan: return "<";
                case BinaryOperatorType.LessOrEqual: return "<=";
                case BinaryOperatorType.Or: return "||";
                case BinaryOperatorType.And: return "&&";
                default: throw new ArgumentException($"{input} is not a valid binary operator.");
            }
        }

        private string ConvertUnaryOperator(UnaryOperatorType input)
        {
            switch (input)
            {
                case UnaryOperatorType.Not: return "!";
                case UnaryOperatorType.Minus: return "-";
                case UnaryOperatorType.Plus: return "+";
                default: throw new ArgumentException($"{input} is not a valid binary operator.");
            }
        }
    }
}
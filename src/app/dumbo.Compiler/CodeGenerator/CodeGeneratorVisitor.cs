using dumbo.Compiler.AST;
using dumbo.Compiler.CodeGenerator.LHCLib;
using System;
using System.Text;

namespace dumbo.Compiler.CodeGenerator
{
    public class RuntimeEntity
    {
    }

    public class CodeGeneratorVisitor : IVisitor<RuntimeEntity, VisitorArgs>
    {
        private Module _currentModule;
        private Stmt _currentStmt;

        public CodeGeneratorVisitor()
        {
            var libearyReader = new LHCLibReader();
            CProgram = new Program(libearyReader.CreateLHCLIb());
        }

        public Program CProgram { get; }

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
            _currentStmt.Append($" {ConvertBinaryOperator(node.Operator)} ");
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
            return null;
        }

        public RuntimeEntity Visit(DeclAndAssignmentStmtNode node, VisitorArgs arg)
        {
            int declAssCount = node.Identifiers.Count;
            _currentStmt = new Stmt("");
            _currentStmt.Append($"{ConvertType(node.Type)} ");
            for (int i = 0; i < declAssCount; i++)
            {
                node.Identifiers[i].Accept(this, arg);
                _currentStmt.Append(" = ");
                node.Expressions[i].Accept(this, arg);
                if (i != 0)
                {
                    _currentStmt.Append(", ");
                }
            }
            _currentStmt.Append(";");
            _currentModule.Append(_currentStmt);

            return null;
        }

        public RuntimeEntity Visit(DeclStmtNode node, VisitorArgs arg)
        {
            _currentStmt = new Stmt("");
            _currentStmt.Append($"{ConvertType(node.Type)} ");
            node.Identifiers.Accept(this, arg);
            _currentModule.Append(_currentStmt);

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
            _currentStmt = new Stmt("else if (");
            node.Predicate.Accept(this, arg);
            _currentStmt.Append(")");
            _currentModule.Append(_currentStmt);
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
                    _currentStmt.Append(", ");
                }
            }

            return null;
        }

        public RuntimeEntity Visit(FormalParamListNode node, VisitorArgs arg)
        {
            return null;
        }

        public RuntimeEntity Visit(FormalParamNode node, VisitorArgs arg)
        {
            return null;
        }

        public RuntimeEntity Visit(FuncCallExprNode node, VisitorArgs arg)
        {
            //a := ... MyFunction(myInt)+5
            var builder = new StringBuilder();

            builder.Append("_" + node.FuncName + "(");

            foreach (var parameter in node.Parameters)
            {
                builder.Append(parameter.ToString() + ", ");
            }
            RemoveExtraComma(builder);
            _currentStmt.Append(builder.ToString());

            return null;
        }

        public RuntimeEntity Visit(FuncCallStmtNode node, VisitorArgs arg)
        {
            //a,b := MyFunction2(myInt)
            throw new System.NotImplementedException();
        }

        public RuntimeEntity Visit(FuncDeclListNode node, VisitorArgs arg)
        {
            int count = node.Count;

            for (int i = 0; i < count; i++)
            {
                node[i].Accept(this, arg);
            }

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

        private void FuncHeader(FuncDeclNode node, bool includeBody)
        {
            StringBuilder builder = new StringBuilder();

            WriteFunctionHeader(node, builder);

            if (!includeBody)
            {
                builder.Append(";");
                _currentModule.Append(new Stmt(builder.ToString()));
                return;
            }
            
            CreateNewModule();
            _currentModule.Append(new Stmt(builder.ToString()));
            node.Body.Accept(this, new BodyVisitorArgs(node.ReturnTypes));
        }

        

        public RuntimeEntity Visit(IdentifierListNode node, VisitorArgs arg)
        {
            int length = node.Count;

            for (int i = 0; i < length; i++)
            {
                node[i].Accept(this, arg);
                if (i != 0)
                {
                    _currentStmt.Append(", ");
                }
            }
            _currentStmt.Append($";");

            return null;
        }

        public RuntimeEntity Visit(IdentifierNode node, VisitorArgs arg)
        {
            _currentStmt.Append(node.Name);

            return null;
        }

        public RuntimeEntity Visit(IfElseStmtNode node, VisitorArgs arg)
        {
            _currentStmt = new Stmt("if (");
            node.Predicate.Accept(this, arg);
            _currentStmt.Append(")");
            _currentModule.Append(_currentStmt);
            node.Body.Accept(this, arg);
            node.ElseIfStatements.Accept(this, arg);
            _currentModule.Append(new Stmt($"else"));
            node.Else.Accept(this, arg);

            return null;
        }

        public RuntimeEntity Visit(IfStmtNode node, VisitorArgs arg)
        {
            _currentStmt = new Stmt("if (");
            node.Predicate.Accept(this, arg);
            _currentStmt.Append(")");
            _currentModule.Append(_currentStmt);
            node.Body.Accept(this, arg);
            node.ElseIfStatements.Accept(this, arg);

            return null;
        }

        public RuntimeEntity Visit(LiteralValueNode node, VisitorArgs arg)
        {
            _currentStmt.Append(node.Value);

            return null;
        }

        public RuntimeEntity Visit(ProgramNode node, VisitorArgs arg)
        {
            _currentModule = new Module();
            _currentModule.Append(new Stmt($"int main()"));
            node.Body.Accept(this, arg);
            CProgram.AddMainModule(_currentModule);

            return null;
        }

        public RuntimeEntity Visit(RepeatStmtNode node, VisitorArgs arg)
        {
            _currentStmt = new Stmt("for (int i=0; i<");
            node.Number.Accept(this, arg);
            _currentStmt.Append(" ; i++;");
            _currentModule.Append(_currentStmt);
            node.Body.Accept(this, arg);

            return null;
        }

        public RuntimeEntity Visit(RepeatWhileStmtNode node, VisitorArgs arg)
        {
            _currentStmt = new Stmt("while (");
            node.Predicate.Accept(this, arg);
            _currentStmt.Append(")");
            _currentModule.Append(_currentStmt);
            node.Body.Accept(this, arg);

            return null;
        }

        public RuntimeEntity Visit(ReturnStmtNode node, VisitorArgs arg)
        {
            int i = 1;

            _currentModule.Append(new Stmt("{"));
            

            foreach (var ret in node.Expressions)
            { 
                _currentStmt = new Stmt("*ret" + i);
                node.Expressions.Accept(this, arg);
                _currentModule.Append(_currentStmt);
                i++;
            }

            _currentModule.Append(new Stmt("return ;"));
            _currentModule.Append(new Stmt("}"));

            return null;
        }

        public RuntimeEntity Visit(RootNode node, VisitorArgs arg)
        {
            /// Todo -- Add const + libraries?
            _currentModule = new Module();
            node.FuncDecls.Accept(this, new FuncVisitorArgs(false));
            CProgram.AddModule(_currentModule);
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
            _currentStmt.Append(ConvertUnaryOperator(node.Operator));
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


        private void WriteFunctionHeader(FuncDeclNode funcNode, StringBuilder builder)
        {
            bool multiReturn = funcNode.ReturnTypes.Count > 1;

            //Return type
            if (multiReturn)
                builder.Append("void");
            else
                builder.Append(ConvertType(funcNode.ReturnTypes[0]));

            //Name
            builder.Append("_" + funcNode.Name + " ");

            //Parameters
            builder.Append("(");
            foreach (var formalParameter in funcNode.Parameters)
            {
                string type = ConvertType(formalParameter.Type);
                builder.Append(type + " _" + formalParameter.Name + ", ");
            }

            if(multiReturn)
            {
                int i = 1;
                foreach (var retType in funcNode.ReturnTypes)
                {
                    string type = ConvertType(retType);
                    builder.Append(type + " _ ret" + i + ", ");
                    i++;
                }
            }
            RemoveExtraComma(builder);
        }

        private void CreateNewModule()
        {
            var module = new Module();
            CProgram.AddModule(module);
            _currentModule = module;
        }

        private void RemoveExtraComma(StringBuilder builder)
        {
            builder.Remove(builder.Length - 2, 2);
            builder.Append(")");
        }
    }
}
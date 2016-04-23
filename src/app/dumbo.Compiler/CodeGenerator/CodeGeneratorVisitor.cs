using dumbo.Compiler.AST;
using dumbo.Compiler.CodeGenerator.LHCLib;
using System;
using System.Collections.Generic;
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
            int count = node.Count;

            for (int index = 0; index < count; index++)
            {
                node[index].Accept(this, arg);

                if (index < count - 1)
                    _currentStmt.Append(", ");
            }

            return null;
        }

        public RuntimeEntity Visit(AssignmentStmtNode node, VisitorArgs arg)
        {
            bool isFunction = node.Identifiers.Count > node.Expressions.Count;

            if (isFunction)
            {
                int i = 0;
                var funcExp = node.Expressions[0] as FuncCallExprNode;

                if (funcExp == null)
                    throw new Exception("Programming error, should be a function");

                //MyFunction(formalParameters, &ret1, &ret2 ...) | How it looks in C for a function
                _currentStmt = new Stmt("");
                _currentStmt.Append(funcExp.FuncName + "(");
                funcExp.Parameters.Accept(this, arg);

                if (node.Identifiers.Count > 0)
                    _currentStmt.Append(", ");

                foreach (var ret in node.Identifiers)
                {
                    _currentStmt.Append("&" + ret.Name);
                    if (i < node.Identifiers.Count - 1)
                        _currentStmt.Append(", ");
                    i++;
                }
                _currentStmt.Append(");");
                _currentModule.Append(_currentStmt);
            }
            else
            {
                for (int index = 0; index < node.Identifiers.Count; index++)
                {
                    //id = expression;    -- How it looks in C for id
                    _currentStmt = new Stmt("");
                    node.Identifiers[index].Accept(this, arg);
                    _currentStmt.Append(" = ");
                    node.Expressions[index].Accept(this, arg);
                    _currentStmt.Append(";");
                    _currentModule.Append(_currentStmt);
                }
            }

            return null;
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
            bool isFunction = node.Identifiers.Count > node.Expressions.Count;

            if (isFunction)
            {
                int i = 1;
                _currentModule.Append(new Stmt("{"));
                foreach (var identifier in node.Identifiers)
                {
                    //type *ret[i] = &name;    -- How it looks in C
                    _currentStmt = new Stmt("");
                    _currentStmt.Append(ConvertType(node.Type));
                    _currentStmt.Append(" *ret" + i + " = &" + identifier.Name.ToLower());
                    _currentModule.Append(_currentStmt);
                    i++;
                }
                _currentModule.Append(new Stmt("}"));
            }
            else
            {
                int assCount = node.Identifiers.Count;
                _currentStmt = new Stmt("");
                _currentStmt.Append($"{ConvertType(node.Type)} ");
                for (int i = 0; i < assCount; i++)
                {
                    node.Identifiers[i].Accept(this, arg);
                    _currentStmt.Append(" = ");
                    node.Expressions[i].Accept(this, arg);
                    if (i != assCount - 1)
                    {
                        _currentStmt.Append(", ");
                    }
                }
                _currentStmt.Append(";");
                _currentModule.Append(_currentStmt);
            }

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
                if (i < count - 1)
                    _currentStmt.Append(", ");
            }

            return null;
        }

        public RuntimeEntity Visit(FormalParamListNode node, VisitorArgs arg)
        {
            int count = node.Count;

            for (int index = 0; index < count; index++)
            {
                node[index].Accept(this, arg);

                if (index < count - 1)
                {
                    _currentStmt.Append(", ");
                }
            }

            return null;
        }

        public RuntimeEntity Visit(FormalParamNode node, VisitorArgs arg)
        {
            _currentStmt.Append(ConvertType(node.Type) + " " + node.Name.ToLower());

            return null;
        }

        public RuntimeEntity Visit(FuncCallExprNode node, VisitorArgs arg)
        {
            //a := ... MyFunction(myInt)+5
            _currentStmt.Append("_" + node.FuncName.ToLower() + "(");
            node.Parameters.Accept(this, arg);
            _currentStmt.Append(")");

            return null;
        }

        public RuntimeEntity Visit(FuncCallStmtNode node, VisitorArgs arg)
        {
            //void MyFunction2(myInt)
            var actualNode = node.CallNode;

            _currentStmt.Append(actualNode.FuncName.ToLower() + "(");
            actualNode.Parameters.Accept(this, arg);
            _currentStmt.Append(")");

            return null;
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

            if (funcArg == null)
                throw new Exception("FuncDeclNode must have FuncVisitorArgs as arg");

            _currentStmt = new Stmt("");


            if (!funcArg.VisitBody)
            {
                WriteFunctionHeader(node, arg);
                _currentStmt.Append(";");
                return null;
            }

            CreateNewModule();
            _currentStmt = new Stmt("");
            WriteFunctionHeader(node, arg);
            _currentModule.Append(_currentStmt);
            node.Body.Accept(this, new BodyVisitorArgs(node.ReturnTypes));
            return null;
        }

        public RuntimeEntity Visit(IdentifierListNode node, VisitorArgs arg)
        {
            int length = node.Count;

            for (int i = 0; i < length; i++)
            {
                node[i].Accept(this, arg);
                if (i < length - 1)
                    _currentStmt.Append(", ");
            }
            _currentStmt.Append($";");

            return null;
        }

        public RuntimeEntity Visit(IdentifierNode node, VisitorArgs arg)
        {
            _currentStmt.Append(node.Name.ToLower());

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

            var suffix = new List<Stmt>();
            suffix.Add(new Stmt("return 0;"));

            node.Body.Accept(this, new StmtBlockNodeArgs(null, suffix));
            CProgram.AddMainModule(_currentModule);

            return null;
        }

        public RuntimeEntity Visit(RepeatStmtNode node, VisitorArgs arg)
        {
            _currentStmt = new Stmt("for (int i=0; i<");
            node.Number.Accept(this, arg);
            _currentStmt.Append(" ; i++)");
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
            bool isMultipleReturn = node.Expressions.Count > 1;
            int i = 1;


            if (!isMultipleReturn)
            {
                _currentStmt = new Stmt("return ");
                node.Expressions[0].Accept(this, arg);
                _currentStmt.Append(";");
                _currentModule.Append(_currentStmt);
            }
            else
            {
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
            }

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
            var customArgs = arg as StmtBlockNodeArgs;
            if (customArgs != null && customArgs.Handled == false)
                customArgs.Handled = true;
            else
                customArgs = null;

            _currentModule.Append(new Stmt("{"));
            AddItemsToCurrentModule(customArgs?.Prefix);
            foreach (var statement in node)
            {
                statement.Accept(this, customArgs != null ? customArgs : arg);
            }
            AddItemsToCurrentModule(customArgs?.Suffix);
            _currentModule.Append(new Stmt("}"));

            return null;
        }

        public RuntimeEntity Visit(UnaryOperationNode node, VisitorArgs arg)
        {
            _currentStmt.Append(ConvertUnaryOperator(node.Operator));
            node.Expression.Accept(this, arg);

            return null;
        }

        private string ConvertType(TypeNode typeNode)
        {
            var input = typeNode as PrimitiveTypeNode;
            switch (input.Type)
            {
                case PrimitiveType.Number: return "double";
                case PrimitiveType.Text: return "Text";
                case PrimitiveType.Boolean: return "Boolean";
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
        
        private void WriteFunctionHeader(FuncDeclNode funcNode, VisitorArgs arg)
        {
            bool multiReturn = funcNode.ReturnTypes.Count > 1;

            //Return type
            if (multiReturn)
                _currentStmt.Append("void");
            else
                _currentStmt.Append(ConvertType(funcNode.ReturnTypes[0]));

            //Name
            _currentStmt.Append(" _" + funcNode.Name.ToLower());

            //Parameters
            _currentStmt.Append("(");
            funcNode.Parameters.Accept(this, arg);

            if (multiReturn)
            {
                _currentStmt.Append(", ");
                for (int i = 0; i < funcNode.ReturnTypes.Count; i++)
                {
                    string type = ConvertType(funcNode.ReturnTypes[i]);
                    if (i < funcNode.ReturnTypes.Count - 1)
                        _currentStmt.Append(type + " _ret" + (i + 1) + ", ");
                    else
                        _currentStmt.Append(type + " _ret" + (i + 1));
                }
            }
            _currentStmt.Append(")");
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

        private void AddItemsToCurrentModule(IList<Stmt> statements)
        {
            if (statements == null)
                return;

            foreach (var stmt in statements)
            {
                _currentModule.Append(stmt);
            }
        }
    }
}
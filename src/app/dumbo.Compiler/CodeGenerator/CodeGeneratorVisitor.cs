using dumbo.Compiler.AST;
using dumbo.Compiler.CodeGenerator.LHCLib;
using System;
using System.Collections.Generic;
using System.Text;

namespace dumbo.Compiler.CodeGenerator
{
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
                _currentStmt.Append("_" + funcExp.FuncName + "(");
                funcExp.Parameters.Accept(this, arg);

                if (node.Identifiers.Count > 0 && funcExp.Parameters.Count > 0)
                    _currentStmt.Append(", ");

                foreach (var ret in node.Identifiers)
                {
                    if (ret.InferredType.GetFirstAs<PrimitiveTypeNode>().Type == PrimitiveType.Text)
                    {
                        _currentStmt.Append(ret.Name);
                    }
                    else
                    {
                        _currentStmt.Append("&" + ret.Name);
                    }
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
                    PrimitiveTypeNode tempType = node.Identifiers[index].DeclarationNode.Type as PrimitiveTypeNode;
                    _currentStmt = new Stmt("");

                    if (tempType != null && tempType.Type == PrimitiveType.Text)
                    {
                        _currentStmt.Append("UpdateText(");
                        node.Expressions[index].Accept(this, arg);
                        _currentStmt.Append($", ");
                        node.Identifiers[index].Accept(this, arg);
                        _currentStmt.Append(")");
                    }
                    else
                    {
                        //id = expression;    -- How it looks in C for id
                        node.Identifiers[index].Accept(this, arg);
                        _currentStmt.Append(" = ");
                        node.Expressions[index].Accept(this, arg);
                    }
                    _currentStmt.Append(";");
                    _currentModule.Append(_currentStmt);
                }
            }

            return null;
        }

        public RuntimeEntity Visit(BinaryOperationNode node, VisitorArgs arg)
        {
            PrimitiveTypeNode binType = node.InferredType.GetFirstAs<PrimitiveTypeNode>();
            PrimitiveTypeNode leftType = node.LeftOperand.InferredType.GetFirstAs<PrimitiveTypeNode>();
            PrimitiveTypeNode rightType = node.RightOperand.InferredType.GetFirstAs<PrimitiveTypeNode>();
            string binOperator = ConvertBinaryOperator(node.Operator);

            if (binOperator == "+" && (leftType.Type == PrimitiveType.Text || rightType.Type == PrimitiveType.Text))
            {
                if (leftType.Type == PrimitiveType.Text)
                {
                    if (rightType.Type == PrimitiveType.Text)
                    {
                        WriteBinOpNodeFunc("ConcatText", node.LeftOperand, node.RightOperand, arg);
                    }
                    else if (rightType.Type == PrimitiveType.Number)
                    {
                        WriteBinOpNodeFunc("ConcatTextAndNumber", node.LeftOperand, node.RightOperand, arg);
                    }
                    else
                    {
                        WriteBinOpNodeFunc("ConcatTextAndBoolean", node.LeftOperand, node.RightOperand, arg);
                    }
                }
                else if (rightType.Type == PrimitiveType.Text)
                {
                    if (leftType.Type == PrimitiveType.Boolean)
                    {
                        _currentStmt.Append($"ConcatText(ConcatTextAndBoolean(CreateText(\"\"), ");
                        node.LeftOperand.Accept(this, arg);
                        _currentStmt.Append("), ");
                        node.RightOperand.Accept(this, arg);
                        _currentStmt.Append(")");
                    }
                    else
                    {
                        _currentStmt.Append($"ConcatText(ConcatTextAndNumber(CreateText(\"\"), ");
                        node.LeftOperand.Accept(this, arg);
                        _currentStmt.Append("), ");
                        node.RightOperand.Accept(this, arg);
                        _currentStmt.Append(")");
                    }
                }
            }
            else if (binOperator == "%" && binType.Type == PrimitiveType.Number)
            {
                WriteBinOpNodeFunc("modulo", node.LeftOperand, node.RightOperand, arg);
            }
            else if (binOperator == "/" && binType.Type == PrimitiveType.Number)
            {
                WriteBinOpNodeFunc("div", node.LeftOperand, node.RightOperand, arg); 
            }
            else if (binOperator == "==" && leftType.Type == PrimitiveType.Text && rightType.Type == PrimitiveType.Text)
            {
                WriteBinOpNodeFunc("IsEqual", node.LeftOperand, node.RightOperand, arg);
            }
            else
            {
                _currentStmt.Append("(");
                node.LeftOperand.Accept(this, arg);
                _currentStmt.Append($" {binOperator} ");
                node.RightOperand.Accept(this, arg);
                _currentStmt.Append(")");
            }
            
            return null;
        }

        public RuntimeEntity Visit(BreakStmtNode node, VisitorArgs arg)
        {
            _currentModule.Append(new Stmt("break;"));

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
                int i = 0, idenCount = node.Identifiers.Count;
                var funcExp = node.Expressions[0] as FuncCallExprNode;

                if (funcExp == null)
                    throw new Exception("Programming error, should be a function");

                for (int j = 0; j < idenCount; j++)
                {
                    _currentStmt = new Stmt("");
                    node.Type.Accept(this, arg);
                    _currentStmt.Append(" " + node.Identifiers[j].Name + ";");
                    _currentModule.Append(_currentStmt);
                }

                //MyFunction(formalParameters, &ret1, &ret2 ...) | How it looks in C for a function
                _currentStmt = new Stmt("");
                _currentStmt.Append("_" + funcExp.FuncName + "(");
                funcExp.Parameters.Accept(this, arg);

                if (node.Identifiers.Count > 0 && funcExp.Parameters.Count > 0)
                    _currentStmt.Append(", ");

                foreach (var ret in node.Identifiers)
                {
                    if (ret.InferredType.GetFirstAs<PrimitiveTypeNode>().Type == PrimitiveType.Text)
                    {
                        _currentStmt.Append(ret.Name);
                    }
                    else
                    {
                        _currentStmt.Append("&" + ret.Name);
                    }
                    if (i < node.Identifiers.Count - 1)
                        _currentStmt.Append(", ");
                    i++;
                }
                _currentStmt.Append(");");
                _currentModule.Append(_currentStmt);
            }
            else
            {
                //type 
                int assCount = node.Identifiers.Count;
                _currentStmt = new Stmt("");
                node.Type.Accept(this, arg);
                _currentStmt.Append(" ");
                PrimitiveTypeNode declType = node.Type as PrimitiveTypeNode;

                if (declType != null && declType.Type == PrimitiveType.Text)
                {
                    for (int i = 0; i < assCount; i++)
                    {
                        node.Identifiers[i].Accept(this, arg);
                        _currentStmt.Append(";");
                        _currentModule.Append(_currentStmt);
                        _currentStmt = new Stmt("");
                        _currentStmt.Append("UpdateText(");
                        node.Expressions[i].Accept(this, arg);
                        _currentStmt.Append($", ");
                        node.Identifiers[i].Accept(this, arg);
                        _currentStmt.Append(")");
                        if (i != assCount - 1)
                        {
                            _currentStmt.Append(", ");
                        }
                    }
                }
                else
                {
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
                }

                _currentStmt.Append(";");
                _currentModule.Append(_currentStmt);
            }

            return null;
        }

        public RuntimeEntity Visit(DeclStmtNode node, VisitorArgs arg)
        {
            _currentStmt = new Stmt("");
            node.Type.Accept(this, arg);
            _currentStmt.Append(" ");
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
            node.Type.Accept(this, arg);
            _currentStmt.Append(" " + node.Name.ToLower());

            return null;
        }

        public RuntimeEntity Visit(FuncCallExprNode node, VisitorArgs arg)
        {
            //a := ... _MyFunction(myInt)+5
            string prefix = "";
            if (!node.DeclarationNode.IsBuiltIn)
                prefix = "_";

            _currentStmt.Append(prefix + node.FuncName.ToLower() + "(");
            node.Parameters.Accept(this, arg);
            _currentStmt.Append(")");

            return null;
        }

        public RuntimeEntity Visit(FuncCallStmtNode node, VisitorArgs arg)
        {
            //void MyFunction2(myInt)
            var actualNode = node.CallNode;

            _currentStmt = new Stmt("_");
            _currentStmt.Append(actualNode.FuncName.ToLower() + "(");
            actualNode.Parameters.Accept(this, arg);
            _currentStmt.Append(");");
            _currentModule.Append(_currentStmt);

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
                _currentModule.Append(_currentStmt);
                return null;
            }

            CreateNewModule();
            _currentStmt = new Stmt("");
            WriteFunctionHeader(node, arg);
            _currentModule.Append(_currentStmt);

            if (node.ReturnTypes.Count == 0)
            {
                var suffix = new List<Stmt>();
                suffix.Add(new Stmt("return ;"));
                node.Body.Accept(this, new StmtBlockNodeArgs(null, suffix, arg));
            }
            else
                node.Body.Accept(this, arg);
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
            _currentStmt.Append(";");

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
            PrimitiveTypeNode nodeType = node.Type as PrimitiveTypeNode;

            if (nodeType != null)
            {
                switch (nodeType.Type)
                {
                    case PrimitiveType.Number: _currentStmt.Append(node.Value); break;
                    case PrimitiveType.Text: _currentStmt.Append($"CreateText(\"{node.Value}\")"); break;
                    case PrimitiveType.Boolean: _currentStmt.Append(node.Value.ToLower()); break;
                    default: throw new ArgumentException($"{nodeType.Type} is not a valid type");
                }
            }

            return null;
        }

        public RuntimeEntity Visit(PrimitiveTypeNode node, VisitorArgs arg)
        {
            switch (node.Type)
            {
                case PrimitiveType.Number:
                    _currentStmt.Append("double"); break;
                case PrimitiveType.Text:
                    _currentStmt.Append("Text*"); break;
                case PrimitiveType.Boolean:
                    _currentStmt.Append("Boolean"); break;
                default: throw new ArgumentException($"{node.Type} is not a valid type.");
            }

            return null;
        }

        public RuntimeEntity Visit(ProgramNode node, VisitorArgs arg)
        {
            _currentModule = new Module();
            _currentModule.Append(new Stmt($"int main()"));

            var suffix = new List<Stmt>();
            suffix.Add(new Stmt("return 0;"));

            node.Body.Accept(this, new StmtBlockNodeArgs(null, suffix, arg));
            CProgram.AddMainModule(_currentModule);

            return null;
        }

        public RuntimeEntity Visit(RepeatStmtNode node, VisitorArgs arg)
        {
            _currentStmt = new Stmt("int i = 0;");
            _currentModule.Append(_currentStmt);
            _currentStmt = new Stmt("for (i=0; i<");
            node.Number.Accept(this, arg);
            _currentStmt.Append("; i++)");
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
                if (node.Expressions.Count != 0)
                {
                    _currentStmt = new Stmt("return ");

                    node.Expressions[0].Accept(this, arg);
                _currentStmt.Append(";");
                _currentModule.Append(_currentStmt);
                }
            }
            else
            {
                _currentModule.Append(new Stmt("//Return"));
                _currentModule.Append(new Stmt("{"));

                foreach (var ret in node.Expressions)
                {
                    _currentStmt = new Stmt("*_ret" + i);
                    _currentStmt.Append(" = ");
                    ret.Accept(this, arg);
                    _currentStmt.Append(";");
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
            _currentModule = new Module();
            node.FuncDecls.Accept(this, new FuncVisitorArgs(false));
            CProgram.AddUserFuncDeclModule(_currentModule);
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
            _currentStmt.Append(ConvertUnaryOperator(node.Operator) + "(");
            node.Expression.Accept(this, arg);
            _currentStmt.Append(")");

            return null;
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
            bool noReturn = funcNode.ReturnTypes.Count == 0;

            //Return type
            if (multiReturn)
                _currentStmt.Append("void");
            else if (noReturn)
                _currentStmt.Append("void");
            else
                funcNode.ReturnTypes[0].Accept(this, arg);

            //Name
            _currentStmt.Append(" _" + funcNode.Name.ToLower());

            //Parameters
            _currentStmt.Append("(");
            funcNode.Parameters.Accept(this, arg);

            if (multiReturn)
            {
                if (funcNode.Parameters.Count > 0)
                    _currentStmt.Append(", ");

                for (int i = 0; i < funcNode.ReturnTypes.Count; i++)
                {
                    PrimitiveTypeNode retType = funcNode.ReturnTypes[i] as PrimitiveTypeNode;
                    funcNode.ReturnTypes[i].Accept(this, arg);
                    _currentStmt.Append(" " + (retType.Type == PrimitiveType.Text ? "" : "*"));
                    _currentStmt.Append("_ret" + (i + 1));
                    if (i < funcNode.ReturnTypes.Count - 1)
                        _currentStmt.Append(", ");
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

        private void AddItemsToCurrentModule(IList<Stmt> statements)
        {
            if (statements == null)
                return;

            foreach (var stmt in statements)
            {
                _currentModule.Append(stmt);
            }
        }

        private void WriteBinOpNodeFunc(string funcName, ExpressionNode left, ExpressionNode right, VisitorArgs arg)
        {
            _currentStmt.Append(funcName + "(");
            left.Accept(this, arg);
            _currentStmt.Append(", ");
            right.Accept(this, arg);
            _currentStmt.Append(")");
        }
    }
}
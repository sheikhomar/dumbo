using dumbo.Compiler.AST;
using System;
using System.Collections.Generic;
using dumbo.Compiler.CodeGenerator.LHZLib;
using System.Globalization;

namespace dumbo.Compiler.CodeGenerator
{
    public class CodeGeneratorVisitor : IVisitor<RuntimeEntity, VisitorArgs>
    {
        private Module _currentModule;
        private Stmt _currentStmt;

        public CodeGeneratorVisitor()
        {
            var libearyReader = new LHZLibReader();
            CProgram = new Program(libearyReader.CreateLHZLIb());
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

        public RuntimeEntity Visit(ArrayDeclStmtNode node, VisitorArgs arg)
        {
            WriteArrayDecl(node.Type, node.Identifiers, arg);

            return null;
        }

        public RuntimeEntity Visit(ArrayIdentifierNode node, VisitorArgs arg)
        {
            string indexName = $"_indexExpr{node.Name.ToLower()}_{node.SourcePosition.StartLine}_{node.SourcePosition.StartColumn}";
            WriteArrayIndexWithIntPointer(node, indexName, arg);

            PrimitiveType type = node.InferredType.GetFirstAs<PrimitiveTypeNode>().Type;
            _currentStmt.Append($"Read{type}ArrayIndex({node.Name.ToLower()}, {indexName})");

            return null;
        }

        public RuntimeEntity Visit(ArrayTypeNode node, VisitorArgs arg)
        {
            _currentStmt.Append("Array*");
            
            return null;
        }

        public RuntimeEntity Visit(ArrayValueNode node, VisitorArgs arg)
        {
            

            throw new NotImplementedException();
        }

        public RuntimeEntity Visit(AssignmentStmtNode node, VisitorArgs arg)
        {
            bool isMultiAssign = node.Identifiers.Count > 1;

            if (isMultiAssign)
                GenerateMultiAssignment(node, arg);
            else
            {
                //A normal assignment | Id := expression   (note: single return function is an expression)
                PrimitiveTypeNode idType = node.Identifiers[0].DeclarationNode.Type as PrimitiveTypeNode;
                ArrayIdentifierNode arrayType = node.Identifiers[0] as ArrayIdentifierNode;
                _currentStmt = new Stmt("");

                if (idType?.Type == PrimitiveType.Text)
                {
                    _currentStmt.Append("UpdateText(");
                    node.Value.Accept(this, arg);
                    _currentStmt.Append($", ");
                    node.Identifiers.Accept(this, arg);
                    _currentStmt.Append(")");
                    _currentStmt.Append(";");
                    _currentModule.Append(_currentStmt);
                }
                else if (arrayType != null)
                {
                    string indexName = "_index";
                    CreateStmtAndAddToCurrentModule("{");
                    WriteArrayIndexWithIntPointer(arrayType, indexName, arg);

                    PrimitiveType type = arrayType.InferredType.GetFirstAs<PrimitiveTypeNode>().Type;
                    _currentStmt.Append("Update");
                    _currentStmt.Append($"{type}ArrayIndexViaIndex({arrayType.Name.ToLower()}, {indexName}, ");
                    node.Value.Accept(this, arg);
                    _currentStmt.Append(");");
                    AppendCurrentStmtToCurrentModule();
                    CreateStmtAndAddToCurrentModule("}");
                }
                else
                {
                    node.Identifiers.Accept(this, arg);
                    _currentStmt.Append(" = ");
                    node.Value.Accept(this, arg);
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

            if (binOperator == "+" && leftType.Type == PrimitiveType.Text && rightType.Type == PrimitiveType.Text)
            {
                WriteBinOpNodeFunc("ConcatText", node.LeftOperand, node.RightOperand, arg);
            }
            else if (binOperator == "%" && binType.Type == PrimitiveType.Number)
            {
                WriteBinOpNodeFunc("Modulo", node.LeftOperand, node.RightOperand, arg);
            }
            else if (binOperator == "/" && binType.Type == PrimitiveType.Number)
            {
                WriteBinOpNodeFunc("Div", node.LeftOperand, node.RightOperand, arg);
            }
            else if (binOperator == "==" && leftType.Type == PrimitiveType.Text && rightType.Type == PrimitiveType.Text)
            {
                WriteBinOpNodeFunc("IsTextAndTextEqual", node.LeftOperand, node.RightOperand, arg);
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

        public RuntimeEntity Visit(ContinueStmtNode node, VisitorArgs arg)
        {
            CreateStmtAndAddToCurrentModule("continue;");

            return null;
        }

        public RuntimeEntity Visit(ConstDeclListNode node, VisitorArgs arg)
        {
            foreach (var item in node)
            {
                item.Accept(this, arg);
            }

            return null;
        }

        public RuntimeEntity Visit(ConstDeclNode node, VisitorArgs arg)
        {
            _currentStmt = new Stmt("#define ");
            _currentStmt.Append(node.Name.ToLower() + " ");
            node.Value.Accept(this, arg);
            _currentModule.Append(_currentStmt);

            return null;
        }

        public RuntimeEntity Visit(BuiltInFuncDeclNode node, VisitorArgs arg)
        {
            return null;
        }

        public RuntimeEntity Visit(DeclAndAssignmentStmtNode node, VisitorArgs arg)
        {
            bool isMultiAssignmen = node.Identifiers.Count > 1;
            bool isArrayDecl = node.Type is ArrayTypeNode;

            if (isMultiAssignmen)
            {
                //Declare the new LHZ id's
                foreach (var decl in node.Identifiers)
                {
                    WriteDeclWithDefaultValue(decl, arg);
                }

                //Assign to the newly created id's
                GenerateMultiAssignment(node, arg);
            }
            else if (isArrayDecl)
            {
                ArrayTypeNode type = node.Type as ArrayTypeNode;

                WriteArrayDeclAss(type, node.Identifiers, node.Value, arg);
            }
            else
            {
                //Declare and assign to a single id
                WriteDeclWithExpression(node.Identifiers[0], node.Value, arg);
            }
            
            return null;
        }

        public RuntimeEntity Visit(PrimitiveDeclStmtNode node, VisitorArgs arg)
        {
            foreach (var id in node.Identifiers)
            {
                WriteDeclWithDefaultValue(id, arg);
            }

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
            //MyFunction(myInt) in some stmt | can have a ret value or not
            if (node.DeclarationNode.IsBuiltIn)
            {
                _currentStmt.Append(node.DeclarationNode.Name + "(");
            }
            else
                _currentStmt.Append("_" + node.DeclarationNode.Name + "(");

            node.Parameters.Accept(this, arg);
            _currentStmt.Append(")");

            return null;
        }

        public RuntimeEntity Visit(FuncCallStmtNode node, VisitorArgs arg)
        {
            _currentStmt = new Stmt("");
            node.CallNode.Accept(this, arg);
            _currentStmt.Append(";");
            _currentModule.Append(_currentStmt);
            return null;
        }

        public RuntimeEntity Visit(FuncDeclListNode node, VisitorArgs arg)
        {
            foreach (var item in node)
            {
                item.Accept(this, arg);
            }
            return null;
        }

        public RuntimeEntity Visit(FuncDeclNode node, VisitorArgs arg)
        {
            FuncVisitorArgs funcArg = arg as FuncVisitorArgs;
            VisitorArgs retArg = arg;
            var prefix = new List<Stmt>();
            var suffix = new List<Stmt>();

            if (funcArg == null)
                throw new Exception("FuncDeclNode must have FuncVisitorArgs as arg");

            _currentStmt = new Stmt("");

            if (!funcArg.VisitBody)
            {
                WriteFunctionHeader(node, retArg);
                _currentStmt.Append(";");
                _currentModule.Append(_currentStmt);
                return null;
            }

            CreateNewModule();
            _currentStmt = new Stmt("");
            WriteFunctionHeader(node, retArg);
            _currentModule.Append(_currentStmt);

            for (int i = 0; i < node.Parameters.Count; i++)
            {
                PrimitiveTypeNode parType = node.Parameters[i].Type as PrimitiveTypeNode;
                ArrayTypeNode arrType = node.Parameters[i].Type as ArrayTypeNode;
                IdentifierNode arrSizeType;

                if (parType != null && parType.Type == PrimitiveType.Text)
                {
                    prefix.Add(new Stmt($"{node.Parameters[i].Name.ToLower()} = TextDup({node.Parameters[i].Name.ToLower()});"));
                }
                else if (arrType != null)
                {
                    prefix.Add(new Stmt($"{node.Parameters[i].Name.ToLower()} = ArrayDup({node.Parameters[i].Name.ToLower()});"));
                    for (int j = 0; j < arrType.Sizes.Count; j++)
                    {
                        arrSizeType = arrType.Sizes[j] as IdentifierNode;
                        if (arrSizeType != null)
                        {
                            _currentStmt = new Stmt("int ");
                            arrType.Sizes[j].Accept(this, retArg);
                            _currentStmt.Append($" = GetArrayDimSize({node.Parameters[i].Name.ToLower()}, {j});");
                            prefix.Add(_currentStmt);
                        }
                    }
                }
            }

            ArrayTypeNode arrTypeNode;
            IList<TypeNode> retTypeList = new List<TypeNode>();
            foreach (var item in node.ReturnTypes)
            {
                arrTypeNode = item as ArrayTypeNode;
                if (arrTypeNode != null)
                {
                    retTypeList.Add(arrTypeNode);
                }
                else
                {
                    retTypeList.Add(item);
                }
            }
            retArg = new ReturnStmtNodeArgs(retTypeList, arg);

            if (node.ReturnTypes.Count == 0)
            {
                suffix.Add(new Stmt("return ;"));
            }
            
            node.Body.Accept(this, new StmtBlockNodeArgs(prefix, suffix, retArg));
            return null;
        }

        public RuntimeEntity Visit(IdentifierListNode node, VisitorArgs arg)
        {
            for (int i = 0; i < node.Count; i++)
            {
                node[i].Accept(this, arg);

                if (i < node.Count - 1)
                    _currentStmt.Append(", ");
            }

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
            node.Then.Accept(this, arg);
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
            node.Then.Accept(this, arg);
            node.ElseIfStatements.Accept(this, arg);

            return null;
        }

        public RuntimeEntity Visit(LiteralValueNode node, VisitorArgs arg)
        {
            const string posInf = "INFINITY", negInf = "-INFINITY", nan = "NAN";

            PrimitiveTypeNode nodeType = node.Type as PrimitiveTypeNode;

            if (nodeType != null)
            {
                switch (nodeType.Type)
                {
                    case PrimitiveType.Number:
                        double value;
                        var style = NumberStyles.Number;
                        var culture = CultureInfo.CreateSpecificCulture("en-GB");
                        if (!double.TryParse(node.Value, style, culture, out value))
                        {
                            if (node.Value.ToLower() == "inf")
                            {
                                _currentStmt.Append(posInf);
                            }
                            else
                            {
                                _currentStmt.Append(nan);
                            }
                        }
                        else if (double.IsPositiveInfinity(value))
                        {
                            _currentStmt.Append(posInf);
                        }
                        else if (double.IsNegativeInfinity(value))
                        {
                            _currentStmt.Append(negInf);
                        }
                        else
                        {
                            _currentStmt.Append(value.ToString(culture));
                        }
                        break;
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

            var prefix = new List<Stmt>();
            prefix.Add(new Stmt("//LHZ Program Prefix"));
            prefix.Add(new Stmt("srand(time(NULL));"));
            prefix.Add(new Stmt("//End of LHZ Program Prefix"));
            prefix.Add(new Stmt(""));
            var suffix = new List<Stmt>();
            suffix.Add(new Stmt(""));
            suffix.Add(new Stmt("//LHZ Program Suffix"));
            suffix.Add(new Stmt("return 0;"));
            suffix.Add(new Stmt("//End of LHZ Program Suffix"));

            node.Body.Accept(this, new StmtBlockNodeArgs(prefix, suffix, arg));
            CProgram.AddMainModule(_currentModule);

            return null;
        }

        public RuntimeEntity Visit(RepeatStmtNode node, VisitorArgs arg)
        {
            CreateStmtAndAddToCurrentModule("{");
            CreateStmtAndAddToCurrentModule("int _i = 0;");
            _currentStmt = new Stmt("for (; _i<");
            node.Number.Accept(this, arg);
            _currentStmt.Append("; _i++)");
            _currentModule.Append(_currentStmt);
            node.Body.Accept(this, arg);
            CreateStmtAndAddToCurrentModule("}");

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
            ReturnStmtNodeArgs retArg = (arg as StmtBlockNodeArgs).Arg as ReturnStmtNodeArgs;

            if (!isMultipleReturn)
            {
                if (node.Expressions.Count != 0)
                {
                    IdentifierNode retPar = node.Expressions[0] as IdentifierNode;
                    ArrayTypeNode retArgType = retArg.ArrTypeNode[0] as ArrayTypeNode;
                    if (retArg != null && retArgType != null)
                    {
                        WriteArrayReturnCheck(retPar, retArg.ArrTypeNode[0] as ArrayTypeNode, arg);
                    }
                    _currentStmt = new Stmt("return ");

                    node.Expressions[0].Accept(this, arg);
                    _currentStmt.Append(";");
                    _currentModule.Append(_currentStmt);
                }
            }
            else
            {
                _currentModule.Append(new Stmt("//Return"));

                for (int j = 0; j < node.Expressions.Count; j++)
                {
                    IdentifierNode retPar = node.Expressions[j] as IdentifierNode;
                    ArrayTypeNode retArgType = retArg.ArrTypeNode[j] as ArrayTypeNode;
                    if (retArg != null && retArgType != null)
                    {
                        WriteArrayReturnCheck(retPar, retArg.ArrTypeNode[j] as ArrayTypeNode, arg);
                    }
                    _currentStmt = new Stmt("*_ret" + i);
                    _currentStmt.Append(" = ");
                    node.Expressions[j].Accept(this, arg);
                    _currentStmt.Append(";");
                    _currentModule.Append(_currentStmt);
                    i++;
                }

                _currentModule.Append(new Stmt("return ;"));
            }

            return null;
        }

        public RuntimeEntity Visit(RootNode node, VisitorArgs arg)
        {
            _currentModule = new Module();
            node.ConstDecls.Accept(this, arg);
            node.FuncDecls.Accept(this, new FuncVisitorArgs(false, arg));
            CProgram.AddUserFuncDeclModule(_currentModule);
            node.Program.Accept(this, arg);
            node.FuncDecls.Accept(this, new FuncVisitorArgs(true, arg));
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
            _currentStmt.Append(" _" + funcNode.Name);

            //Parameters
            _currentStmt.Append("(");
            funcNode.Parameters.Accept(this, arg);

            if (multiReturn)
            {
                if (funcNode.Parameters.Count > 0)
                    _currentStmt.Append(", ");

                for (int i = 0; i < funcNode.ReturnTypes.Count; i++)
                {
                    funcNode.ReturnTypes[i].Accept(this, arg);
                    _currentStmt.Append("*_ret" + (i + 1));
                    
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

        /// <summary>
        /// Writes the default value for LHZ types
        /// </summary>
        /// <param name="type"></param>
        private void WriteInitialValue(PrimitiveTypeNode type)
        {
            if(type == null)
                throw new ArgumentException($"{type.Type} is not a valid tpye");

            switch (type.Type)
            {
                case PrimitiveType.Number: _currentStmt.Append("0.0"); break;
                case PrimitiveType.Text: _currentStmt.Append("CreateText(\"\")"); break;
                case PrimitiveType.Boolean: _currentStmt.Append("false"); break;
                default: throw new ArgumentException($"{type.Type} is not a valid tpye");
            }
        }

        private void GenerateMultiAssignment(AssignmentStmtNode node, VisitorArgs arg)
        {
            //Function | a,b := MyFunc() | MyFunction(formalParameters, &ret1, &ret2 ...);
            int i = 0;
            var funcExp = node.Value as FuncCallExprNode;
            _currentStmt = new Stmt("");

            if (funcExp == null)
                throw new Exception("Programming error, should be a function");

            if (!funcExp.DeclarationNode.IsBuiltIn)
                _currentStmt.Append("_");

            _currentStmt.Append(funcExp.FuncName + "(");
            funcExp.Parameters.Accept(this, arg);

            if (node.Identifiers.Count > 0 && funcExp.Parameters.Count > 0)
                _currentStmt.Append(", ");

            foreach (var retIdentifier in node.Identifiers)
            {
                _currentStmt.Append("&" + retIdentifier.Name.ToLower());

                if (i < node.Identifiers.Count - 1)
                    _currentStmt.Append(", ");

                i++;
            }
            _currentStmt.Append(");");
            _currentModule.Append(_currentStmt);
        }

        /// <summary>
        /// Writes Type id = expression
        /// </summary>
        private void WriteDecl(IdentifierNode decl, VisitorArgs arg)
        {
            var idType = decl.DeclarationNode.Type;

            _currentStmt = new Stmt("");
            decl.DeclarationNode.Type.Accept(this, arg);
            _currentStmt.Append(" ");
            decl.Accept(this, arg);
            _currentStmt.Append(" = ");
        }

        private void WriteDeclWithExpression(IdentifierNode decl, ExpressionNode expression, VisitorArgs arg)
        {
            var idType = decl.DeclarationNode.Type;
            bool isText = (idType is PrimitiveTypeNode) && ((PrimitiveTypeNode)idType).Type == PrimitiveType.Text;

            WriteDecl(decl, arg);

            if (isText)
            {
                _currentStmt.Append("CreateText((");
                expression.Accept(this, arg);
                _currentStmt.Append(")->Value)");
            }
            else
                expression.Accept(this, arg);

            _currentStmt.Append(";");
            _currentModule.Append(_currentStmt);
        }

        private void WriteDeclWithDefaultValue(IdentifierNode decl, VisitorArgs arg)
        {
            var idType = decl.DeclarationNode.Type;

            WriteDecl(decl, arg);

            if (idType is PrimitiveTypeNode)
                WriteInitialValue((PrimitiveTypeNode)idType);
            else
                throw new Exception("Programming error - unknown type");

            _currentStmt.Append(";");
            _currentModule.Append(_currentStmt);
        }

        private void WriteArrayDecl(ArrayTypeNode type, IdentifierListNode identifiers, VisitorArgs arg)
        {
            _currentStmt = new Stmt("");
            CreateStmtAndAddToCurrentModule("//Declaring an Array");
            WriteArrayIndex(type.Sizes, arg);
            type.Accept(this, arg);
            _currentStmt.Append(" ");
            identifiers.Accept(this, arg);
            _currentStmt.Append($"= CreateArray(_index{ReturnUniqueName(type.Sizes)}, t_{type.Type.Type});");
            AppendCurrentStmtToCurrentModule();
            CreateStmtAndAddToCurrentModule("//End of declaring an Array");
        }

        private void WriteArrayIndex(ExpressionListNode exprList, VisitorArgs arg)
        {
            CreateStmtAndAddToCurrentModule("//Create DeclIndex struct");
            CreateStmtAndAddToCurrentModule($"int _numberOfDims{ReturnUniqueName(exprList)} = {exprList.Count};");
            CreateStmtAndAddToCurrentModule($"int *_indices{ReturnUniqueName(exprList)} = (int*)malloc(sizeof(int)*_numberOfDims{ReturnUniqueName(exprList)});");
            for (int i = 0; i < exprList.Count; i++)
            {
                _currentStmt.Append($"_indices{ReturnUniqueName(exprList)}[{i}] = ");
                exprList[i].Accept(this, arg);
                _currentStmt.Append(";");
                AppendCurrentStmtToCurrentModule();
            }
            CreateStmtAndAddToCurrentModule($"DeclIndex *_index{ReturnUniqueName(exprList)} = CreateDeclIndex(_indices{ReturnUniqueName(exprList)}, _numberOfDims{ReturnUniqueName(exprList)});");
            CreateStmtAndAddToCurrentModule("//End of creating DeclIndex struct");
        }

        private void CreateStmtAndAddToCurrentModule(string input)
        {
            Stmt stmt = new Stmt(input);
            _currentModule.Append(stmt);
        }

        private void AppendCurrentStmtToCurrentModule()
        {
            _currentModule.Append(_currentStmt);
            _currentStmt = new Stmt("");
        }

        private string DetermineCType(PrimitiveType type)
        {
            switch (type)
            {
                case PrimitiveType.Number: return "double";
                case PrimitiveType.Text: return "Text";
                case PrimitiveType.Boolean: return "Boolean";
                default: throw new ArgumentException($"{type} is not a valid primitive type");
            }
        }

        private void WriteArrayDeclAss(ArrayTypeNode type, IdentifierListNode identifiers, ExpressionNode expressions, VisitorArgs arg)
        {
            if (identifiers.Count != 1)
            {
                throw new ArgumentException($"Only one identifier can be used in an array decl, so {identifiers.Count} is invalid");
            }
            WriteArrayDecl(type, identifiers, arg);
            WriteFullArrayAss(expressions, identifiers, arg);
        }

        private void WriteFullArrayAss(ExpressionNode input, IdentifierListNode identifiers, VisitorArgs arg)
        {
            ArrayValueNode expressions = input as ArrayValueNode;
            Stack<int> offset = new Stack<int>();
            offset.Push(0);
            if (expressions != null)
            {
                CastNestedExprAndAssign(expressions.Values, identifiers, 0, offset, new List<int>(), arg);
            }
        }

        private void CastNestedExprAndAssign(NestedExpressionListNode expressions, IdentifierListNode identifiers, int layer, Stack<int> offset, IList<int> index, VisitorArgs arg)
        {
            NestedExpressionListNode exprs = expressions[layer] as NestedExpressionListNode;
            index.Add(0);
            if (exprs != null)
            {
                for (int i = 0; i < exprs.Count; i++)
                {
                    index[layer] = i;
                    CastNestedExprAndAssign(exprs, identifiers, i, offset, index, arg);
                }
            }
            else
            {
                ExpressionListNode exprList = expressions[layer] as ExpressionListNode;
                if (exprList != null)
                {
                    AssignArrayIndexWithOffset(exprList, identifiers, index, offset, arg);
                }
                else
                {
                    throw new InvalidCastException($"It is not possible to cast the NestedExpressionListNode to ExpressionListNode");
                }
            }
        }

        private void AssignArrayIndexWithOffset(ExpressionListNode exprList, IdentifierListNode identifiers, IList<int> index, Stack<int> offset, VisitorArgs arg)
        {
            int newCount = offset.Count;
            PrimitiveType type = exprList[0].InferredType.GetFirstAs<PrimitiveTypeNode>().Type;

            for (int j = 0; j < exprList.Count; j++)
            {
                _currentStmt.Append($"Update{type}ArrayIndexViaOffset(");
                identifiers.Accept(this, arg);
                _currentStmt.Append($", {offset.Peek()}, ");
                exprList[j].Accept(this, arg);
                _currentStmt.Append(");");
                AppendCurrentStmtToCurrentModule();
                offset.Push(newCount++);
            }
        }

        private void WriteArrayIndexWithIntPointer(ArrayIdentifierNode node, string indexName, VisitorArgs arg)
        {
            string tempCurrentStmt = _currentStmt.GetLine;
            _currentStmt = new Stmt("");
            _currentStmt.Append($"int {indexName}[] = {{");
            for (int i = 0; i < node.Indices.Count; i++)
            {
                _currentStmt.Append("(int)");
                node.Indices[i].Accept(this, arg);
                if (i < node.Indices.Count - 1)
                    _currentStmt.Append(", ");
            }
            _currentStmt.Append("};");
            AppendCurrentStmtToCurrentModule();
            _currentStmt = new Stmt(tempCurrentStmt);
        }

        private string ReturnUniqueName(BaseNode node)
        {
            string id = $"_{node.SourcePosition.StartLine}_{node.SourcePosition.StartColumn}";

            return id;
        }

        private void WriteArrayReturnCheck(IdentifierNode retPar, ArrayTypeNode retCheck, VisitorArgs arg)
        {
            _currentStmt = new Stmt("");
            WriteArrayIndex(retCheck.Sizes, arg);
            CreateStmtAndAddToCurrentModule($"CheckReturnArraySize(_index{ ReturnUniqueName(retCheck.Sizes)}, {retPar.Name.ToLower()});");
        }
    }
}
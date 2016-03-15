using System;
using System.Collections.Generic;
using System.Diagnostics;
using dumbo.Compiler.AST;
using GOLD;

namespace dumbo.Compiler.SyntaxAnalysis
{
    public enum StmtProduction
    {
        AssignStmt,
        IfStmt,
        RepeatStmt, 
        Decl, 
        ReturnStmt, 
        FuncCall, 
        BreakStmt, 
        nl  
    }

    public class AbstractSyntaxTreeBuilder
    {
        public RootNode Build(Reduction root)
        {
            Debug.Assert(root.Parent.Head().Name() == "Start");

            ProgramNode programNode = BuildProgram(root[0]);
            IList<FunctionDeclNode> funcDelcs;

            if (root.Count() == 3)
                funcDelcs = BuildFunctionDecls(root[2]);
            else
                funcDelcs = new List<FunctionDeclNode>();

            return new RootNode(programNode, funcDelcs);
        }

        private ProgramNode BuildProgram(Token programSymbol)
        {
            Debug.Assert(programSymbol.Parent.Name() == "Program");
            Reduction lhs = (Reduction)programSymbol.Data;

            Debug.Assert(lhs.Count() == 5);

            Token stmtsToken = lhs[2];
            StmtBlockNode stmts = BuildStmtsBlock(stmtsToken);

            return new ProgramNode(stmts);
        }

        private StmtBlockNode BuildStmtsBlock(Token stmtsToken)
        {
            Debug.Assert(stmtsToken.Parent.Name() == "Stmts");
            Reduction lhs = (Reduction)stmtsToken.Data;
            
            StmtBlockNode stmtsBlockNode = new StmtBlockNode();
            
            AppendStatements(stmtsToken, stmtsBlockNode.Body);

            return stmtsBlockNode;
        }

        private void AppendStatements(Token stmtsToken, IList<StmtNode> statements)
        {
            Debug.Assert(stmtsToken.Parent.Name() == "Stmts");
            Reduction lhs = (Reduction)stmtsToken.Data;

            if (lhs.Count() > 0)
            {
                Token stmtToken = lhs[0];
                StmtNode stmtNode = BuildStmt(stmtToken);
                statements.Add(stmtNode);

                Token stmtsToken2 = lhs[2];
                AppendStatements(stmtsToken2, statements);
            }
        }
        
        private StmtNode BuildStmt(Token stmtToken)
        {
            Debug.Assert(stmtToken.Parent.Name() == "Stmt");
            Reduction lhs = (Reduction)stmtToken.Data;

            string nameOfProduction = lhs[0].Parent.Name();
            StmtProduction production = ConvertToEnum<StmtProduction>(nameOfProduction);
            
            switch (production)
            {
                case StmtProduction.AssignStmt:
                    return BuildAssignStmt(lhs[0]);
                    break;
                case StmtProduction.IfStmt:
                    return BuildIfStmt(lhs[0]);
                    break;
                case StmtProduction.RepeatStmt:
                    break;
                case StmtProduction.Decl:
                    return BuildDeclStmt(lhs[0]);
                case StmtProduction.ReturnStmt:
                    break;
                case StmtProduction.FuncCall:
                    break;
                case StmtProduction.BreakStmt:
                    break;
                case StmtProduction.nl:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return null;
        }

        private AssignmentStmtNode BuildAssignStmt(Token token)
        {
            Debug.Assert(token.Parent.Name() == "AssignStmt");
            Reduction lhs = (Reduction)token.Data;

            string firstLhfName = lhs[0].Parent.Name();

            ExpressionListNode exprListNode = BuildExprList(lhs[2]);

            if ("Id".Equals(firstLhfName, StringComparison.InvariantCultureIgnoreCase))
            {
                return new AssignmentStmtNode(BuildIdentifierList(lhs[0]), exprListNode);
            }
            var declNode = BuildDeclStmt(lhs[0]);
            return new DeclAndAssignmentStmtNode(declNode.Type, declNode.Identifiers, exprListNode);
        }

        
        
        private ExpressionListNode BuildExprList(Token token)
        {
            Debug.Assert(token.Parent.Name() == "ExprList");
            Reduction lhs = (Reduction)token.Data;

            IList<ExpressionNode> list = new List<ExpressionNode>();

            list.Add(BuildExprNode(lhs[0]));

            Token multiToken = lhs[1];
            AppendExpression(multiToken, list);

            return new ExpressionListNode(list);
        }

        private void AppendExpression(Token token, IList<ExpressionNode> list)
        {
            Debug.Assert(token.Parent.Name() == "MultiExpr");
            Reduction lhs = (Reduction)token.Data;

            if (lhs.Count() > 0)
            {
                list.Add(BuildExprNode(lhs[1]));

                Token multiIdToken2 = lhs[2];
                AppendExpression(multiIdToken2, list);
            }
        }

        private IfStmtNode BuildIfStmt(Token ifStmToken)
        {
            Debug.Assert(ifStmToken.Parent.Name() == "IfStmt");
            Reduction lhs = (Reduction)ifStmToken.Data;

            bool longIfStmt = lhs.Count() == 11;

            Token exprToken = longIfStmt ? lhs[2] : lhs[1];
            Token stmtsToken = longIfStmt ? lhs[6] : lhs[4];
            Token elseIfToken = longIfStmt ? lhs[7] : lhs[5];
            Token elseToken = longIfStmt ? lhs[8] : lhs[6];

            ExpressionNode predicate = BuildExprNode(exprToken);
            StmtBlockNode stmtsNode = BuildStmtsBlock(stmtsToken);


            Reduction elseReduction = (Reduction) elseToken.Data;
            
            if (elseReduction.Count() > 0)
            {
                StmtBlockNode elseStmtsNode = BuildStmtsBlock(elseReduction[2]);
            }

            Reduction elseIfReduction = (Reduction)elseIfToken.Data;
            if (elseIfReduction.Count() > 0)
            {
                ElseIfStmtNode elseIfNode = BuildElseIfStmt(elseIfToken);
            }

            //return new IfStmtNode(predicate, elseStmtsNode, null, stmtsNode);
            return new IfStmtNode(predicate, stmtsNode);
        }

        private ElseIfStmtNode BuildElseIfStmt(Token elseIfToken)
        {
            return null;
        }

        private ExpressionNode BuildExprNode(Token exprToken)
        {
            string name = exprToken.Parent.Name();
            Reduction lhs = (Reduction)exprToken.Data;

            if (name == "Unary")
            {
                return BuildUnaryExprNode(exprToken);
            }
            else
            {
                return BuildBinaryExpr(exprToken);
            }
        }

        private ExpressionNode BuildBinaryExpr(Token andToken)
        {
            string name = andToken.Parent.Name();
            Reduction lhs = (Reduction) andToken.Data;

            if (name.Equals("Unary"))
                throw new ArgumentException("Unary not allowed here.");

            if (lhs.Count() == 3)
            {
                ExpressionNode leftOperand = BuildExprNode(lhs[0]);
                BinaryOperatorType operatorType = ParseOperator((string) lhs[1].Data);
                ExpressionNode rightOperand = BuildExprNode(lhs[2]);

                return new BinaryOperationNode(leftOperand, operatorType, rightOperand);
            }

            return BuildExprNode(lhs[0]);
        }

        private ExpressionNode BuildUnaryExprNode(Token unaryToken)
        {
            Debug.Assert(unaryToken.Parent.Name() == "Unary");
            Reduction lhs = (Reduction)unaryToken.Data;

            if (lhs.Count() == 2)
            {
                string operatorCode = ((string) lhs[0].Data).ToLower();
                return new UnaryOperationNode(
                    ParseUnaryOperator(operatorCode),
                    BuildUnaryExprNode(lhs[1]));
            }

            return BuildValueNode(lhs[0]);
        }

        private ExpressionNode BuildValueNode(Token valueToken)
        {
            Debug.Assert(valueToken.Parent.Name() == "Value");
            Reduction lhs = (Reduction)valueToken.Data;

            string lhsName = lhs[0].Parent.Name();

            switch (lhsName)
            {
                case "Literal":
                    Reduction lhs2 = (Reduction) lhs[0].Data;
                    string value = (string)lhs2[0].Data;
                    string literalType = lhs2[0].Parent.Name();
                    HappyType happyType;

                    if (literalType == "NumberLiteral")
                        happyType = HappyType.Number;
                    else if (literalType == "TextLiteral")
                        happyType = HappyType.Text;
                    else if (literalType == "BooleanLiteral")
                        happyType = HappyType.Boolean;
                    else
                        throw new InvalidOperationException("Invalid literal type.");

                    return new LiteralValueNode(value, happyType);
                case "Id":
                    string name = (string)lhs[0].Data;
                    return new IdentifierNode(name);
                case "FuncCall":
                    return null;
                case "(":
                    return BuildExprNode(lhs[1]);
                default:
                    throw new ArgumentException("Unexpected value node.");
            }
        }

        private UnaryOperatorType ParseUnaryOperator(string operatorCode)
        {
            operatorCode = operatorCode.ToLower();

            switch (operatorCode)
            {
                case "not": return UnaryOperatorType.Negation;
                case "-": return UnaryOperatorType.Minus;
                case "+": return UnaryOperatorType.Plus;

                default:
                    throw new InvalidOperationException("Unknown unary operator " + operatorCode);
            }
        }

        private BinaryOperatorType ParseOperator(string operatorCode)
        {
            operatorCode = operatorCode.ToLower();

            switch (operatorCode)
            {
                case "or": return BinaryOperatorType.Or;
                case "and": return BinaryOperatorType.And;
                case ">": return BinaryOperatorType.GreaterThan;
                case ">=": return BinaryOperatorType.GreaterOrEqual;
                case "<": return BinaryOperatorType.LessThan;
                case "<=": return BinaryOperatorType.LessOrEqual;
                case "=": return BinaryOperatorType.Equals;
                case "+": return BinaryOperatorType.Plus;
                case "-": return BinaryOperatorType.Minus;
                case "*": return BinaryOperatorType.Times;
                case "/": return BinaryOperatorType.Division;
                case "%": return BinaryOperatorType.Modulo;
                default:
                    throw new InvalidOperationException("Unknown operator " + operatorCode);
            }
        }

        private DeclStmtNode BuildDeclStmt(Token declStmtToken)
        {
            Debug.Assert(declStmtToken.Parent.Name() == "Decl");
            Reduction lhs = (Reduction)declStmtToken.Data;

            Token underToken = lhs[0];
            string name = underToken.Parent.Name();
            Reduction underLhs = (Reduction) underToken.Data;

            IdentifierListNode idList = BuildIdentifierList(underLhs[1]);
            DeclStmtNode returnNode = null;

            if (name == "NumberDecl")
                return new DeclStmtNode(idList, HappyType.Number);

            if (name == "TextDecl")
                return new DeclStmtNode(idList, HappyType.Text);

            if (name == "BoolDecl")
                return new DeclStmtNode(idList, HappyType.Boolean);

            throw new InvalidOperationException("Invalid happy type!");
        }

        private IdentifierListNode BuildIdentifierList(Token idToken)
        {
            Debug.Assert(idToken.Parent.Name() == "Id");
            Reduction lhs = (Reduction)idToken.Data;

            IList<IdentifierNode> list = new List<IdentifierNode>();

            string name = (string)lhs[0].Data;

            list.Add(new IdentifierNode(name));

            

            Token multiIdentToken = lhs[1];
            AppendIdentifier(multiIdentToken, list);

            return new IdentifierListNode(list);
        }

        private void AppendIdentifier(Token multiIdToken, IList<IdentifierNode> list)
        {
            Debug.Assert(multiIdToken.Parent.Name() == "MultiId");
            Reduction lhs = (Reduction)multiIdToken.Data;

            if (lhs.Count() > 0)
            {
                string name = (string)lhs[1].Data;
                list.Add(new IdentifierNode(name));

                Token multiIdToken2 = lhs[2];
                AppendIdentifier(multiIdToken2, list);
            }
        }

        private IList<FunctionDeclNode> BuildFunctionDecls(Token funcDeclsSymbol)
        {
            Debug.Assert(funcDeclsSymbol.Parent.Name() == "FuncDecls");

            var lhs = (Reduction) funcDeclsSymbol.Data;

            return null;
        }




        public T ConvertToEnum<T>(string input) where T : struct, IComparable, IFormattable, IConvertible
        {
            T output;

            if (!typeof(T).IsEnum)
            {
                throw new ArgumentException("The type T, is invalid in the given context");
            }
            else
            {
                if (Enum.TryParse(input, out output))
                {
                    return output;
                }
                else
                {
                    throw new ArgumentException(input + " is not of the type T");
                }
            }
        }
    }
}
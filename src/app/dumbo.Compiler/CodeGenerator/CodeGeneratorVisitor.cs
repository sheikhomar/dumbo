using dumbo.Compiler.AST;
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
            throw new System.NotImplementedException();
        }

        public RuntimeEntity Visit(BreakStmtNode node, VisitorArgs arg)
        {
            throw new System.NotImplementedException();
        }

        public RuntimeEntity Visit(DeclAndAssignmentStmtNode node, VisitorArgs arg)
        {
            throw new System.NotImplementedException();
        }

        public RuntimeEntity Visit(DeclStmtNode node, VisitorArgs arg)
        {
            throw new System.NotImplementedException();
        }

        public RuntimeEntity Visit(ElseIfStmtListNode node, VisitorArgs arg)
        {
            throw new System.NotImplementedException();
        }

        public RuntimeEntity Visit(ElseIfStmtNode node, VisitorArgs arg)
        {
            throw new System.NotImplementedException();
        }

        public RuntimeEntity Visit(ExpressionListNode node, VisitorArgs arg)
        {
            throw new System.NotImplementedException();
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
            throw new System.NotImplementedException();
        }

        public RuntimeEntity Visit(IdentifierNode node, VisitorArgs arg)
        {
            throw new System.NotImplementedException();
        }

        public RuntimeEntity Visit(IfElseStmtNode node, VisitorArgs arg)
        {
            throw new System.NotImplementedException();
        }

        public RuntimeEntity Visit(IfStmtNode node, VisitorArgs arg)
        {
            _currentModule.Append(new Stmt($"if ({node.Predicate.Accept(this, arg)})"));

            throw new System.NotImplementedException();
        }

        public RuntimeEntity Visit(LiteralValueNode node, VisitorArgs arg)
        {
            throw new System.NotImplementedException();
        }

        public RuntimeEntity Visit(ProgramNode node, VisitorArgs arg)
        {
            throw new System.NotImplementedException();
        }

        public RuntimeEntity Visit(RepeatStmtNode node, VisitorArgs arg)
        {
            throw new System.NotImplementedException();
        }

        public RuntimeEntity Visit(RepeatWhileStmtNode node, VisitorArgs arg)
        {
            throw new System.NotImplementedException();
        }

        public RuntimeEntity Visit(ReturnStmtNode node, VisitorArgs arg)
        {
            throw new System.NotImplementedException();
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
            throw new System.NotImplementedException();
        }

        public RuntimeEntity Visit(UnaryOperationNode node, VisitorArgs arg)
        {
            throw new System.NotImplementedException();
        }
    }
}
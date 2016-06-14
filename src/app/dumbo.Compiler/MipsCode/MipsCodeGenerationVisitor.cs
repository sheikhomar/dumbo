using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using dumbo.Compiler.AST;

namespace dumbo.Compiler.MipsCode
{
    public class MipsCodeGenerationVisitor : IVisitor<object, VisitorArgs>
    {
        private const int IndentationLength = 4;
        private const int InstructionPadding = 30;
        private const int CommandPadding = 8;
        private readonly IList<string> _instructions;
        private readonly IDictionary<string, DataSegmentItem> _data;
        private readonly RegisterManagement _regAllocator;
        private readonly IDictionary<string, VariableLocation> _locations;

        public MipsCodeGenerationVisitor()
        {
            _instructions = new List<string>();
            _data = new Dictionary<string, DataSegmentItem>();
            _regAllocator = new RegisterManagement();
            _locations = new Dictionary<string, VariableLocation>();
        }
        
        public object Visit(ActualParamListNode node, VisitorArgs arg)
        {
            return null;
        }

        public object Visit(ArrayDeclStmtNode node, VisitorArgs arg)
        {
            return null;
        }

        public object Visit(ArrayIdentifierNode node, VisitorArgs arg)
        {
            return null;
        }

        public object Visit(ArrayTypeNode node, VisitorArgs arg)
        {
            return null;
        }

        public object Visit(ArrayValueNode node, VisitorArgs arg)
        {
            return null;
        }

        public object Visit(AssignmentStmtNode node, VisitorArgs arg)
        {
            // The value of the expression is stored in register $f2
            node.Value.Accept(this, arg);

            foreach (var identifier in node.Identifiers)
            {
                var loc = _locations[identifier.Name];
                var register = _regAllocator.Find(loc);

                Emit($"mov.d  {register.Name}, $f2    # Allocate {register.Name} to variable '{identifier.Name}'.");

                EmitPrintString($"'{identifier.Name}' = ");
                EmitPrintDouble(register.Name);
            }

            return null;
        }

        public object Visit(BinaryOperationNode node, VisitorArgs arg)
        {
            ExpressionNode expr1, expr2;

            /* 
            Up to three registers are used to compute arithmetic expressions:
             - $f2: stores the result of an expression
             - $f4: stores the value of left expression before evaluating the right expression
             - $f6: used when both left and right expression have subtrees. Stores the value of larger expression tree.
            */

            string regLeftOp;  // Register used as left operand for the instruction
            string regRightOp; // Register used as right operand for the instruction

            int leftNodeCount = CountNodes(node.LeftOperand);
            int rightNodeCount = CountNodes(node.RightOperand);

            if (rightNodeCount > leftNodeCount)
            {
                // Right subtree has more nodes, so we will evaluate it first
                expr1 = node.RightOperand;
                expr2 = node.LeftOperand;

                // Since we are evaluating left subtree last, the value of 
                // the left expression is stored in $f2. To ensure correct
                // computation $f2 must be the left operand used for the instruction.
                regLeftOp = "$f2";
                regRightOp = "$f4";
            }
            else
            {
                expr1 = node.LeftOperand;
                expr2 = node.RightOperand;
                regLeftOp = "$f4";
                regRightOp = "$f2"; // Right expression is computed last.
            }

            // Generate code to evaluate the expression. 
            // The value of the expression is stored in $f2.
            expr1.Accept(this, arg);

            if (leftNodeCount > 1 && rightNodeCount > 1)
            {
                // Both the left and the right operand of the current expression have subtrees.
                // We need another register to keep result of the first subtree.
                Emit("mov.d   $f6, $f2  # $f6 = $f2");

                if (rightNodeCount > leftNodeCount)
                {
                    regLeftOp = "$f2";
                    regRightOp = "$f6";
                }
                else
                {
                    regLeftOp = "$f6";
                    regRightOp = "$f2";
                }
            }
            else
            {
                // Copy the value of first expression to $f4 before evaluating the right expression
                Emit("mov.d   $f4, $f2  # $f4 = $f2");
            }

            // Generate code to evaluate the second expression. The value of is stored in $f2.
            expr2.Accept(this, arg);

            switch (node.Operator)
            {
                case BinaryOperatorType.Plus:
                    Emit($"add.d   $f2, {regLeftOp}, {regRightOp}   # $f2 = {regLeftOp} + {regRightOp}");
                    break;
                case BinaryOperatorType.Minus:
                    Emit($"sub.d   $f2, {regLeftOp}, {regRightOp}   # $f2 = {regLeftOp} - {regRightOp}");
                    break;
                case BinaryOperatorType.Times:
                    Emit($"mul.d   $f2, {regLeftOp}, {regRightOp}   # $f2 = {regLeftOp} * {regRightOp}");
                    break;
                case BinaryOperatorType.Division:
                    Emit($"div.d   $f2, {regLeftOp}, {regRightOp}   # $f2 = {regLeftOp} / {regRightOp}");
                    break;
                case BinaryOperatorType.Modulo:
                    break;
                case BinaryOperatorType.Equals:
                    break;
                case BinaryOperatorType.GreaterThan:
                    break;
                case BinaryOperatorType.GreaterOrEqual:
                    break;
                case BinaryOperatorType.LessThan:
                    break;
                case BinaryOperatorType.LessOrEqual:
                    break;
                case BinaryOperatorType.Or:
                    break;
                case BinaryOperatorType.And:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            //EmitPrintDouble("$f2");

            return null;
        }

        public object Visit(BreakStmtNode node, VisitorArgs arg)
        {
            return null;
        }

        public object Visit(ContinueStmtNode node, VisitorArgs arg)
        {
            return null;
        }

        public object Visit(ConstDeclListNode node, VisitorArgs arg)
        {
            return null;
        }

        public object Visit(ConstDeclNode node, VisitorArgs arg)
        {
            return null;
        }

        public object Visit(BuiltInFuncDeclNode node, VisitorArgs arg)
        {
            return null;
        }

        public object Visit(DeclAndAssignmentStmtNode node, VisitorArgs arg)
        {
            // The value of the expression is stored in register $f2
            node.Value.Accept(this, arg);

            foreach (var identifier in node.Identifiers)
            {
                if (IsOfType(node.Type, PrimitiveType.Number))
                {
                    var location = NewFrameLocation(identifier.Name);
                    var register = _regAllocator.Assign(location);

                    Emit($"mov.d  {register.Name}, $f2    # Allocate {register.Name} to variable '{identifier.Name}'.");

                    EmitPrintString($"'{identifier.Name}' = ");
                    EmitPrintDouble(register.Name);
                }
            }
            return null;
        }

        public object Visit(ElseIfStmtListNode node, VisitorArgs arg)
        {
            return null;
        }

        public object Visit(ElseIfStmtNode node, VisitorArgs arg)
        {
            return null;
        }

        public object Visit(ExpressionListNode node, VisitorArgs arg)
        {
            return null;
        }

        public object Visit(FormalParamListNode node, VisitorArgs arg)
        {
            return null;
        }

        public object Visit(FormalParamNode node, VisitorArgs arg)
        {
            return null;
        }

        public object Visit(FuncCallExprNode node, VisitorArgs arg)
        {
            return null;
        }

        public object Visit(FuncCallStmtNode node, VisitorArgs arg)
        {
            return null;
        }

        public object Visit(FuncDeclListNode node, VisitorArgs arg)
        {
            return null;
        }

        public object Visit(FuncDeclNode node, VisitorArgs arg)
        {
            return null;
        }

        public object Visit(IdentifierListNode node, VisitorArgs arg)
        {
            return null;
        }

        public object Visit(IdentifierNode node, VisitorArgs arg)
        {
            var loc = _locations[node.Name];
            var register = _regAllocator.Find(loc);

            // Move value to $f2 before evaluating the right operand
            Emit($"mov.d   $f2, {register.Name}    # Value of '{node.Name}'");

            return null;
        }

        public object Visit(IfElseStmtNode node, VisitorArgs arg)
        {
            return null;
        }

        public object Visit(IfStmtNode node, VisitorArgs arg)
        {
            return null;
        }

        public object Visit(LiteralValueNode node, VisitorArgs arg)
        {
            if (IsOfType(node.Type, PrimitiveType.Number))
            {
                string location = "litVal" + _data.Count;
                AddDataItem(location, "double", node.Value);
                Emit($"l.d  $f2, {location}        # $f2 = {node.Value}");
                return null;
            }

            return null;
        }

        public object Visit(PrimitiveDeclStmtNode node, VisitorArgs arg)
        {
            AddDataItem("fz", "double", "0.0", true);

            foreach (var identifier in node.Identifiers)
            {
                if (IsOfType(node.Type, PrimitiveType.Number))
                {
                    var location = NewFrameLocation(identifier.Name);
                    var register = _regAllocator.Assign(location);

                    Emit($"l.d  {register.Name}, fz    # Allocate {register.Name} to variable '{identifier.Name}'.");
                }
            }
            return null;
        }

        public object Visit(PrimitiveTypeNode node, VisitorArgs arg)
        {
            return null;
        }

        public object Visit(ProgramNode node, VisitorArgs arg)
        {
            Emit("main:");

            // Local variables are allocated on the stack. The 4 bytes padding 
            // in the stack is required to maintain the doubleword alignment.
            Emit("sub  $sp, $sp, -4    # Double-word alignment");

            node.Body.Accept(this, arg);

            return null;
        }

        public object Visit(RepeatStmtNode node, VisitorArgs arg)
        {
            return null;
        }

        public object Visit(RepeatWhileStmtNode node, VisitorArgs arg)
        {
            return null;
        }

        public object Visit(ReturnStmtNode node, VisitorArgs arg)
        {
            return null;
        }

        public object Visit(RootNode node, VisitorArgs arg)
        {
            node.Program.Accept(this, arg);

            Emit("li   $v0, 10   # Terminate execution");
            Emit("syscall");

            return null;
        }

        public object Visit(StmtBlockNode node, VisitorArgs arg)
        {
            foreach (var item in node)
                item.Accept(this, arg);
            return null;
        }

        public object Visit(UnaryOperationNode node, VisitorArgs arg)
        {
            return null;
        }

        public string GetFormattedCode()
        {
            var buffer = new StringBuilder();

            AppendDataSegment(buffer);
            AppendCodeSegment(buffer);

            return buffer.ToString();
        }

        private void AppendDataSegment(StringBuilder buffer)
        {
            AppendInstruction(buffer, ".data        # Data section");
            AppendInstruction(buffer, ".align 3     # 3=Double-word alignment");

            foreach (var key in _data.Keys)
            {
                var item = _data[key];
                buffer.Append(' ', IndentationLength*2);
                buffer.Append($"{item.Name}: .{item.Type} {item.Value}");
                buffer.AppendLine();
            }
        }

        private void AppendCodeSegment(StringBuilder buffer)
        {
            AppendInstruction(buffer, ".text        # Code section");
            AppendInstruction(buffer, ".globl main  # Entry point");

            foreach (var instruction in _instructions)
            {
                AppendInstruction(buffer, instruction);
            }
        }

        private static void AppendInstruction(StringBuilder buffer, string instruction)
        {
            if (instruction.Contains(":"))
            {
                // Make new lines before label
                buffer.Append("\n\n");
            }
            else
            {
                // Indent otherwise
                buffer.Append(' ', IndentationLength);
            }

            if (instruction.Contains("#"))
            {
                var strings = instruction.Split('#');


                string instructionPart = strings[0].Trim();
                string commentPart = strings[1].Trim();

                if (instructionPart.Length > InstructionPadding)
                {
                    buffer.AppendLine(instruction);
                }
                else if (string.IsNullOrEmpty(instructionPart))
                {
                    buffer.Append("# ");
                    buffer.AppendLine(commentPart);
                }
                else
                {
                    int instructionPartLength = instructionPart.Length;
                    if (!instructionPart.StartsWith(".") && instructionPart.Contains(" "))
                    {
                        string cmd = instructionPart.Substring(0, instructionPart.IndexOf(" "));
                        string operands = instructionPart.Substring(instructionPart.IndexOf(" ")).Trim();
                        buffer.Append(cmd);
                        buffer.Append(' ', CommandPadding - cmd.Length);
                        buffer.Append(operands);
                        instructionPartLength = CommandPadding + operands.Length;
                    }
                    else
                    {
                        buffer.Append(instructionPart);
                    }
                    
                    buffer.Append(' ', InstructionPadding - instructionPartLength);
                    buffer.Append(" # ");
                    buffer.AppendLine(commentPart);
                }
            }
            else
            {
                buffer.AppendLine(instruction);
            }
        }

        private bool IsOfType(TypeNode typeNode, PrimitiveType primitiveType)
        {
            var primitiveTypeNode = typeNode as PrimitiveTypeNode;
            return primitiveTypeNode?.Type == primitiveType;
        }

        private void Emit(string instruction)
        {
            _instructions.Add(instruction);
        }

        private void AddDataItem(string name, string type, string value, bool dublicatesAllowed = false)
        {
            if (!dublicatesAllowed && _data.ContainsKey(name))
                throw new ArgumentException($"Data item '{name}' is already specified", nameof(name));

            if (!_data.ContainsKey(name))
                _data.Add(name, new DataSegmentItem(name, type, value));
        }

        private void EmitPrintDouble(string register)
        {
            Emit("             # Print double");
            Emit("li  $v0, 3   # Syscall 3 (print_double)");
            if (register != "$f12")
                Emit($"mov.d  $f12, {register}  # Argument register");
            Emit("syscall");
            EmitPrintNewLine();
        }

        private void EmitPrintNewLine()
        {
            AddDataItem("nl", "asciiz", "\"\\n\"", true);

            Emit("             # Print newline");
            Emit("li  $v0, 4   # Syscall 4 (print_string)");
            Emit("la  $a0, nl  # Load newline into register");
            Emit("syscall");
        }

        private void EmitPrintString(string str)
        {
            string memoryLoc = "str" + _data.Count;
            AddDataItem(memoryLoc, "asciiz", $"\"{str}\"");

            Emit($"             # Print \"{str}\"");
            Emit($"li  $v0, 4   # Syscall 4 (print_string)");
            Emit($"la  $a0, {memoryLoc}  # Load argument");
            Emit("syscall");
        }

        private VariableLocation NewFrameLocation(string name)
        {
            var location = new VariableLocation();

            _locations.Add(name, location);

            return location;
        }

        private int CountNodes(ExpressionNode expr)
        {
            var bexpr = expr as BinaryOperationNode;
            if (bexpr != null)
                return CountNodes(bexpr.LeftOperand) + CountNodes(bexpr.RightOperand) + 1;

            return 1;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using dumbo.Compiler.AST;

namespace dumbo.Compiler.MipsCode
{
    public class MipsCodeGenerationVisitor : IVisitor<object, VisitorArgs>
    {
        private const int IndentationLength = 8;
        private const int InstructionPadding = 20;
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
            return null;
        }

        public object Visit(BinaryOperationNode node, VisitorArgs arg)
        {
            // The value of the left operand is stored in $f2
            node.LeftOperand.Accept(this, arg);

            // Copy the value of $f2 to $f4 before evaluating the right operand
            Emit("mov.d   $f4, $f2");

            // The value of the right operand is stored in $f2
            node.RightOperand.Accept(this, arg);

            switch (node.Operator)
            {
                case BinaryOperatorType.Plus:
                    Emit("add.d   $f2, $f4, $f2   # $f2 = $f4 + $f2");
                    break;
                case BinaryOperatorType.Minus:
                    Emit("sub.d   $f2, $f4, $f2   # $f2 = $f4 - $f2");
                    break;
                case BinaryOperatorType.Times:
                    Emit("mul.d   $f2, $f4, $f2   # $f2 = $f4 * $f2");
                    break;
                case BinaryOperatorType.Division:
                    Emit("div.d   $f2, $f4, $f2   # $f2 = $f4 / $f2");
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

            EmitPrintDouble("$f2");

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
                var strings = instruction.Split(new[] {'#'});


                string instructionPart = strings[0].Trim();
                string commentPart = strings[1].Trim();

                if (instructionPart.Length > InstructionPadding)
                {
                    buffer.AppendLine(instruction);
                }
                else
                {
                    buffer.Append(instructionPart);
                    buffer.Append(' ', InstructionPadding - instructionPart.Length);
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

        private VariableLocation NewFrameLocation(string name)
        {
            var location = new VariableLocation();

            _locations.Add(name, location);

            return location;
        }
    }
}
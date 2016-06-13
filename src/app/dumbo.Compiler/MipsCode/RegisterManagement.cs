using System;
using System.Collections.Generic;
using System.Linq;

namespace dumbo.Compiler.MipsCode
{
    public class RegisterManagement
    {
        private readonly IList<Register> _registers;

        public RegisterManagement()
        {
            _registers = new List<Register>
            {
                //new Register("$f0"),  // Used when reading input from user
                //new Register("$f2"),  // Used for result when evaluation eded
                //new Register("$f4"),  // Used as temporary register for working with two operands
                new Register("$f6"),
                new Register("$f8"),
                new Register("$f10"),
                //new Register("$f12"), // Used when printing to the console
                new Register("$f14"),
                new Register("$f16"),
                new Register("$f18"),
                new Register("$f22"),
                new Register("$f24"),
                new Register("$f26"),
                new Register("$f28"),
                new Register("$f30")
            };
        }

        public Register Assign(VariableLocation variable)
        {
            foreach (var r2 in _registers)
            {
                if (r2.AssignedVariable == null)
                {
                    r2.AssignedVariable = variable;
                    return r2;
                }
            }

            // All available registers are assigned to variables.
            // We need to find a register that we can spill in order
            // to assign the new variable.
            throw new NotImplementedException("Register spilling is not implemented.");
        }

        public Register Find(VariableLocation variable)
        {
            var register = _registers.FirstOrDefault(r => r.AssignedVariable == variable);
            if (register == null)
                throw new ArgumentOutOfRangeException("Variable has not been assigned to any register.");
            return register;
        }
    }
}
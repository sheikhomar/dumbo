using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dumbo.Compiler.AST
{
    public class TypeDescriptor
    {
        private IList<HappyType> _types;

        public TypeDescriptor(IList<HappyType> happyTypes)
        {
            _types = happyTypes;
        }

        IList<HappyType> Types { get; }
    }
}

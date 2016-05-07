using dumbo.Compiler.AST;
using NUnit.Framework;

namespace dumbo.Tests.SyntaxAnalysis
{
    [TestFixture]
    public class ParserDeclarationTests : BaseTest
    {
        [TestCase(PrimitiveType.Number, "num", "Program\r\n    Number num\r\nEnd Program")]
        [TestCase(PrimitiveType.Number, "a", "Program\r\n    NUMBER a\r\nEnd Program")]
        [TestCase(PrimitiveType.Number, "b", "Program\r\n    number b\r\nEnd Program")]
        [TestCase(PrimitiveType.Boolean, "a", "Program\r\n    Boolean a\r\nEnd Program")]
        [TestCase(PrimitiveType.Boolean, "b", "Program\r\n    BOOLEAN b\r\nEnd Program")]
        [TestCase(PrimitiveType.Boolean, "c", "Program\r\n    booleAn c\r\nEnd Program")]
        [TestCase(PrimitiveType.Text, "d", "Program\r\n    Text d\r\nEnd Program")]
        [TestCase(PrimitiveType.Text, "e", "Program\r\n    TEXT e\r\nEnd Program")]
        [TestCase(PrimitiveType.Text, "f", "Program\r\n    texT f\r\nEnd Program")]
        public void ShouldParseDeclerationStatements(PrimitiveType expectedType, string expectedName, string programText)
        {
            var rootNode = ParseAndGetRootNode(programText);
            var node = rootNode.Program.Body.GetAs<PrimitiveDeclStmtNode>(0);
            Assert.IsNotNull(node);
            Assert.AreEqual(1, node.Identifiers.Count);
            Assert.AreEqual(expectedType, (node.Type as PrimitiveTypeNode).Type);
            Assert.AreEqual(expectedName, node.Identifiers[0].Name);
        }

        [TestCase(PrimitiveType.Number, "a", "b", "Program\r\n    Number a, b\r\nEnd Program")]
        [TestCase(PrimitiveType.Number, "a", "b", "Program\r\n    Number a , b\r\nEnd Program")]
        [TestCase(PrimitiveType.Boolean, "c", "b", "Program\r\n    Boolean c, b\r\nEnd Program")]
        [TestCase(PrimitiveType.Boolean, "a", "b", "Program\r\n    BooleAn a , b\r\nEnd Program")]
        [TestCase(PrimitiveType.Text, "a", "b", "Program\r\n    Text a, b\r\nEnd Program")]
        [TestCase(PrimitiveType.Text, "a", "b", "Program\r\n    Text a , b\r\nEnd Program")]
        public void ShouldParseDeclStmtWithMultiIdentifiers(PrimitiveType expectedType, string var1, string var2, string programText)
        {
            var rootNode = ParseAndGetRootNode(programText);
            var node = rootNode.Program.Body.GetAs<PrimitiveDeclStmtNode>(0);
            Assert.IsNotNull(node);
            Assert.AreEqual(2, node.Identifiers.Count);
            Assert.AreEqual(expectedType, (node.Type as PrimitiveTypeNode).Type);
            Assert.AreEqual(var1, node.Identifiers[0].Name);
            Assert.AreEqual(var2, node.Identifiers[1].Name);
        }
    }
}

﻿using System.IO;
using dumbo.Compiler;
using dumbo.Compiler.AST;
using dumbo.Compiler.SyntaxAnalysis;
using NUnit.Framework;

namespace dumbo.Tests.SyntaxAnalysis
{
    [TestFixture]
    public class ParserTests
    {
        private readonly PrimitiveTypeNode BooleanType = new PrimitiveTypeNode(PrimitiveType.Boolean);
        private readonly PrimitiveTypeNode TextType = new PrimitiveTypeNode(PrimitiveType.Text);
        private readonly PrimitiveTypeNode NumberType = new PrimitiveTypeNode(PrimitiveType.Number);

        private RootNode ParseAndGetRootNode(string programText)
        {
            var parser = new Parser(Utils.GetGrammarTablePath());
            var result = parser.Parse(new StringReader(programText));
            return result.Root;
        }

        [Test]
        public void ShouldParseEmptyProgram()
        {
            const string programText =
@"Program
End Program";
            var rootNode = ParseAndGetRootNode(programText);
            Assert.IsNotNull(rootNode.Program);
            Assert.IsNotNull(rootNode.Program.Body);
            Assert.AreEqual(0, rootNode.Program.Body.Count);
        }

        [Test]
        public void ShouldParseProgramWithWhitespaces()
        {
            const string programText =
@"
   Program
    
            
    End Program

";
            var rootNode = ParseAndGetRootNode(programText);
            Assert.IsNotNull(rootNode.Program);
            Assert.IsNotNull(rootNode.Program.Body);
            Assert.AreEqual(0, rootNode.Program.Body.Count);
        }

        [Test]
        public void ShouldParseNumberDelcStmt()
        {
            const string programText =
@"Program
  Number age
End Program";
            var rootNode = ParseAndGetRootNode(programText);
            var node = rootNode.Program.Body.GetAs<DeclStmtNode>(0);
            Assert.IsNotNull(node);
            Assert.AreEqual(NumberType, node.Type);
            Assert.AreEqual(1, node.Identifiers.Count);
            Assert.AreEqual("age", node.Identifiers[0].Name);
        }

        [Test]
        public void ShouldParseTextDelcStmt()
        {
            const string programText =
@"Program
  Text name
End Program";
            var rootNode = ParseAndGetRootNode(programText);
            var node = rootNode.Program.Body.GetAs<DeclStmtNode>(0);
            Assert.IsNotNull(node);
            Assert.AreEqual(TextType, node.Type);
            Assert.AreEqual(1, node.Identifiers.Count);
            Assert.AreEqual("name", node.Identifiers[0].Name);
        }

        [Test]
        public void ShouldParseBooleanDelcStmt()
        {
            const string programText =
@"Program
  Boolean isStudent
End Program";
            var rootNode = ParseAndGetRootNode(programText);
            var node = rootNode.Program.Body.GetAs<DeclStmtNode>(0);
            Assert.IsNotNull(node);
            Assert.AreEqual(BooleanType, node.Type);
            Assert.AreEqual(1, node.Identifiers.Count);
            Assert.AreEqual("isStudent", node.Identifiers[0].Name);
        }

        [Test]
        public void ShouldParseDelcAndAssignStmt()
        {
            const string programText =
@"Program
  Text name := ""Karen Blixen""
End Program";
            var rootNode = ParseAndGetRootNode(programText);
            var node = rootNode.Program.Body.GetAs<DeclAndAssignmentStmtNode>(0);
            Assert.IsNotNull(node);
            Assert.AreEqual(TextType, node.Type);
            Assert.AreEqual(1, node.Identifiers.Count);
            Assert.AreEqual("name", node.Identifiers[0].Name);

            Assert.AreEqual(1, node.Expressions.Count);
            var expr = node.Expressions.GetAs<LiteralValueNode>(0);
            Assert.AreEqual(TextType, expr.Type);
            Assert.AreEqual("Karen Blixen", expr.Value);
        }
    }
}

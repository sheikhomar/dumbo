using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dumbo.Compiler.AST;
using dumbo.Compiler.SymbolTable;
using NUnit.Framework;

namespace dumbo.Tests.SymbolTable
{
    [TestFixture]
    public class SymbolTableTests
    {
        dumbo.Compiler.SymbolTable.SymbolTable table = new dumbo.Compiler.SymbolTable.SymbolTable();
        private SymbolTablePrimitiveType booleanType = new SymbolTablePrimitiveType(new PrimitiveTypeNode(PrimitiveType.Boolean));
        private SymbolTablePrimitiveType textType = new SymbolTablePrimitiveType(new PrimitiveTypeNode(PrimitiveType.Text));
        private SymbolTablePrimitiveType numberType = new SymbolTablePrimitiveType(new PrimitiveTypeNode(PrimitiveType.Text));

        [SetUp]
        public void TableSetup()
        {
            table = new dumbo.Compiler.SymbolTable.SymbolTable();
        }

        [TestCase(0, 0, 0)]
        [TestCase(2, 0, 2)]
        [TestCase(2, 1, 1)]
        public void EnsureDepthIsIncrementedAndDecremented(int opens, int closes, int depth)
        {
            // act
            for (int i = 0; i < opens; i++)
            {
                table.OpenScope();
            }

            for (int i = 0; i < closes; i++)
            {
                table.CloseScope();
            }

            table.EnterSymbol("test", booleanType);

            // assert
            Assert.AreEqual(depth, table.RetrieveSymbol("test").Depth);
        }

        [Test]
        public void AddsAndRemovesNamesProperly()
        {
            //Arrange
            table.EnterSymbol("fisk", textType);
            table.EnterSymbol("Omar", numberType);
            table.OpenScope();
            table.EnterSymbol("Marc", booleanType);

            //Act
            table.CloseScope();

            //Assert
            Assert.AreEqual(null, table.RetrieveSymbol("Marc"));
        }

        [Test]
        public void DeclaredLocallyReturnsTrue()
        {
            // Arrange
            table.OpenScope();

            // Act
            table.EnterSymbol("test", booleanType);

            // Assert
            Assert.True(table.DeclaredLocally("test"));
        }

        [Test]
        public void DeclaredLocallyReturnsFalse()
        {
            // Arrange
            table.EnterSymbol("test", booleanType);

            // Act
            table.OpenScope();

            // Assert
            Assert.False(table.DeclaredLocally("test"));
        }

        [Test]
        public void ThrowsDuplicateDeclaration()
        {
            // Arrange
            table.OpenScope();
            table.EnterSymbol("tst", booleanType);

            // Act & Assert
            Assert.Throws(typeof(Compiler.SymbolTable.DuplicateDeclarationException), () => table.EnterSymbol("tst", booleanType));
        }

        [Test]
        public void ThrowsOutermostScope()
        {
            // Act & Assert
            Assert.Throws(typeof(Compiler.SymbolTable.OutermostScopeException), () => table.CloseScope());
        }

        [Test]
        public void EnsureOuterDeclarationIsProperlyRestored()
        {
            // Arrange
            table.OpenScope();
            table.EnterSymbol("Declaration", booleanType);
            table.OpenScope();
            table.EnterSymbol("Declaration", booleanType);

            // Act
            table.CloseScope();

            // Assert
            Assert.AreEqual(1, table.RetrieveSymbol("Declaration").Depth);
        }

        [Test]
        public void EnsureFunctionParametersAreStored()
        {

            // Arrange
            var funcDecl = new FuncDeclNode("MyFunc", new StmtBlockNode(), new SourcePosition(0, 0, 0, 0));
            var entry = new SymbolTableFunctionType(funcDecl);

            // Act
            table.EnterSymbol("test", entry);

            // Assert
            var entryType = table.RetrieveSymbol("test").Type as SymbolTableFunctionType;
            Assert.AreSame(funcDecl, entryType.DeclarationNode);
        }

        [Test]
        public void ThrowsUnhideableError()
        {
            // Arrange
            table.EnterSymbol("i", booleanType, true);
            table.OpenScope();

            // Act & Assert
            Assert.Throws(typeof(Compiler.SymbolTable.IllegalHideException), () => table.EnterSymbol("i", booleanType));
        }
    }
}

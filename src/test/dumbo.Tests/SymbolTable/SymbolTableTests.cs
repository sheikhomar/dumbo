using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace dumbo.Tests.SymbolTable
{
    [TestFixture]
    public class SymbolTableTests
    {
        dumbo.Compiler.SymbolTable.SymbolTable table = new dumbo.Compiler.SymbolTable.SymbolTable();

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

            table.EnterSymbol("test", new Compiler.SymbolTable.SymbolTablePrimitiveType(Compiler.AST.HappyType.Boolean));

            // assert
            Assert.AreEqual(depth, table.RetrieveSymbol("test").Depth);
        }

        [Test]
        public void AddsAndRemovesNamesProperly()
        {
            //Arrange
            table.EnterSymbol("fisk", new Compiler.SymbolTable.SymbolTablePrimitiveType(Compiler.AST.HappyType.Text));
            table.EnterSymbol("Omar", new Compiler.SymbolTable.SymbolTablePrimitiveType(Compiler.AST.HappyType.Number));
            table.OpenScope();
            table.EnterSymbol("Marc", new Compiler.SymbolTable.SymbolTablePrimitiveType(Compiler.AST.HappyType.Boolean));

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
            table.EnterSymbol("test", new Compiler.SymbolTable.SymbolTablePrimitiveType(Compiler.AST.HappyType.Boolean));

            // Assert
            Assert.True(table.DeclaredLocally("test"));
        }

        [Test]
        public void DeclaredLocallyReturnsFalse()
        {
            // Arrange
            table.EnterSymbol("test", new Compiler.SymbolTable.SymbolTablePrimitiveType(Compiler.AST.HappyType.Boolean));

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
            table.EnterSymbol("tst", new Compiler.SymbolTable.SymbolTablePrimitiveType(Compiler.AST.HappyType.Boolean));

            // Act & Assert
            Assert.Throws(typeof(Compiler.SymbolTable.DuplicateDeclarationException), () => table.EnterSymbol("tst", new Compiler.SymbolTable.SymbolTablePrimitiveType(Compiler.AST.HappyType.Boolean)));
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
            table.EnterSymbol("Declaration", new Compiler.SymbolTable.SymbolTablePrimitiveType(Compiler.AST.HappyType.Boolean));
            table.OpenScope();
            table.EnterSymbol("Declaration", new Compiler.SymbolTable.SymbolTablePrimitiveType(Compiler.AST.HappyType.Boolean));

            // Act
            table.CloseScope();

            // Assert
            Assert.AreEqual(1, table.RetrieveSymbol("Declaration").Depth);
        }

        [Test]
        public void EnsureFunctionParametersAreStored()
        {

            // Arrange
            IList<Compiler.AST.HappyType> list = new List<Compiler.AST.HappyType>();
            list.Add(Compiler.AST.HappyType.Boolean);
            list.Add(Compiler.AST.HappyType.Text);
            Compiler.SymbolTable.SymbolTableFunctionType entry = new Compiler.SymbolTable.SymbolTableFunctionType(list, list);

            // Act
            table.EnterSymbol("test", entry);

            // Assert
            bool first = (Compiler.AST.HappyType.Boolean == ((Compiler.SymbolTable.SymbolTableFunctionType)(table.RetrieveSymbol("test").Type)).parametertypes[0]);
            bool second = (Compiler.AST.HappyType.Text == ((Compiler.SymbolTable.SymbolTableFunctionType)(table.RetrieveSymbol("test").Type)).parametertypes[1]);

            Assert.True(first && second);
        }

        [Test]
        public void ThrowsUnhideableError()
        {
            // Arrange
            table.EnterSymbol("i", new Compiler.SymbolTable.SymbolTablePrimitiveType(Compiler.AST.HappyType.Boolean), true);
            table.OpenScope();

            // Act & Assert
            Assert.Throws(typeof(Compiler.SymbolTable.IllegalHideException), () => table.EnterSymbol("i", new Compiler.SymbolTable.SymbolTablePrimitiveType(Compiler.AST.HappyType.Boolean)));

            

        }
    }
}

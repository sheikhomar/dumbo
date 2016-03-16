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
        public void tableSetup()
        {
            table = new dumbo.Compiler.SymbolTable.SymbolTable();
        }

        [TestCase(0,0,0)]
        [TestCase(2,0,2)]
        [TestCase(2,1,1)]
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

            table.EnterSymbol("test", new Compiler.SymbolTable.SymbolTablePrimitiveType(Compiler.SymbolTable.SymbolTableType.Boolean));

            // assert
            Assert.AreEqual(depth, table.RetrieveSymbol("test").Depth);
        }

        [Test]
        public void AddsAndRemovesNamesProperly()
        {
            //Arrange
            table.EnterSymbol("fisk", new Compiler.SymbolTable.SymbolTablePrimitiveType(Compiler.SymbolTable.SymbolTableType.Text));
            table.EnterSymbol("Omar", new Compiler.SymbolTable.SymbolTablePrimitiveType(Compiler.SymbolTable.SymbolTableType.Number));
            table.OpenScope();
            table.EnterSymbol("Marc", new Compiler.SymbolTable.SymbolTablePrimitiveType(Compiler.SymbolTable.SymbolTableType.Boolean));

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
            table.EnterSymbol("test", new Compiler.SymbolTable.SymbolTablePrimitiveType(Compiler.SymbolTable.SymbolTableType.Boolean));

            // Assert
            Assert.True(table.DeclaredLocally("test"));
        }

        [Test]
        public void DeclaredLocallyReturnsFalse()
        {
            // Arrange
            table.EnterSymbol("test", new Compiler.SymbolTable.SymbolTablePrimitiveType(Compiler.SymbolTable.SymbolTableType.Boolean));

            // Act
            table.OpenScope();

            // Assert
            Assert.False(table.DeclaredLocally("test"));
        }

        [Test]
        public void EnsureOuterDeclarationIsProperlyRestored()
        {
            // Arrange
            table.OpenScope();
            table.EnterSymbol("Declaration", new Compiler.SymbolTable.SymbolTablePrimitiveType(Compiler.SymbolTable.SymbolTableType.Boolean));
            table.OpenScope();
            table.EnterSymbol("Declaration", new Compiler.SymbolTable.SymbolTablePrimitiveType(Compiler.SymbolTable.SymbolTableType.Boolean));

            // Act
            table.CloseScope();

            // Assert
            Assert.AreEqual(1, table.RetrieveSymbol("Declaration").Depth);
        }

        [Test]
        public void EnsureFunctionParametersAreStored()
        {
            // Arrange
            Compiler.SymbolTable.SymbolTableFunctionType entry = new Compiler.SymbolTable.SymbolTableFunctionType();
            entry.parameters.Add(Compiler.SymbolTable.SymbolTableType.Boolean);
            entry.parameters.Add(Compiler.SymbolTable.SymbolTableType.Text);

            // Act
            table.EnterSymbol("test", entry);

            // Assert
            bool first = (Compiler.SymbolTable.SymbolTableType.Boolean == ((Compiler.SymbolTable.SymbolTableFunctionType)(table.RetrieveSymbol("test").Type)).parameters[0]);
            bool second = (Compiler.SymbolTable.SymbolTableType.Text == ((Compiler.SymbolTable.SymbolTableFunctionType)(table.RetrieveSymbol("test").Type)).parameters[1]);

            Assert.True(first && second);
        }
    }
}

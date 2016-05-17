using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using dumbo.Compiler;
using dumbo.Compiler.AST;
using dumbo.Compiler.CodeGenerator;
using dumbo.Compiler.TypeChecking;
using NUnit.Framework;

namespace dumbo.Tests.CodeGeneration
{
    [TestFixture]
    public class CodeGenTests : BaseTest
    {
        [Test, TestCaseSource(typeof(CodeGenTestProgramFactory), nameof(CodeGenTestProgramFactory.TestPrograms))]
        public void TestCodeGen(string fileName, int startLine, string programText, bool shouldFail)
        {
            TestProgram(fileName, startLine, programText, shouldFail);
        }

        private void TestProgram(string fileName, int startLine, string programText, bool shouldFail)
        {
            var rootNode = GetRootNode(startLine, programText);
            var generatedCode = GenerateCode(rootNode);
            var targetPath = Path.Combine(Path.GetTempPath(), "final.exe");
            var gccCompilerRes = GccCompileAndRun(generatedCode, targetPath);

            if (gccCompilerRes.StatusCode == 0)
            {
                var runRes = new CProgramRunner().Run(targetPath);
                if (runRes.StatusCode != 0)
                {
                    var msg = FormatRuntimeError(fileName, startLine, programText, generatedCode, runRes);
                    Assert.Fail(msg);
                }
                else if (shouldFail)
                {
                    var msg = FormatShouldFailMessage(fileName, startLine, programText, generatedCode, runRes);
                    Assert.Fail(msg);
                }
            }
            else
            {
                var msg = FormatCompilationError(fileName, startLine, programText, generatedCode, gccCompilerRes);
                Assert.Fail(msg);

            }
        }

        private string FormatCompilationError(string fileName, int programLine, string sourceProgram, string targetProgram, ProcessResult result)
        {
            StringBuilder buffer = new StringBuilder();

            buffer.AppendLine($"Compilation failed with status code {result.StatusCode} \n");

            AppendMessages(fileName, programLine, sourceProgram, targetProgram, result, buffer);
            
            return buffer.ToString();
        }

        private string FormatRuntimeError(string fileName, int programLine, string sourceProgram, string targetProgram, ProcessResult result)
        {
            StringBuilder buffer = new StringBuilder();

            buffer.AppendLine($"Running the program failed with status code {result.StatusCode} \n");
            AppendMessages(fileName, programLine, sourceProgram, targetProgram, result, buffer);

            return buffer.ToString();
        }

        private string FormatShouldFailMessage(string fileName, int programLine, string sourceProgram, string targetProgram, ProcessResult result)
        {
            StringBuilder buffer = new StringBuilder();

            buffer.AppendLine($"Running the program should have failed. \n");
            AppendMessages(fileName, programLine, sourceProgram, targetProgram, result, buffer);

            return buffer.ToString();
        }

        private void AppendMessages(string fileName, int programLine, string sourceProgram, string targetProgram, ProcessResult result, StringBuilder buffer)
        {
            if (result.OutputData.Any())
            {
                buffer.AppendLine("Output messages: \n");
                foreach (var msg in result.OutputData)
                    buffer.AppendLine(msg);
            }

            if (result.ErrorData.Any())
            {
                buffer.AppendLine("Error messages: \n");
                foreach (var error in result.ErrorData)
                    buffer.AppendLine(error);
            }

            buffer.AppendLine("Source Program: \n");
            AppendProgram(fileName, programLine, sourceProgram, buffer);

            buffer.AppendLine("Target Program: \n");
            buffer.AppendLine(targetProgram);
        }

        private ProcessResult GccCompileAndRun(string generatedCode, string targetPath)
        {
            var finalProgramPath = Path.Combine(Path.GetDirectoryName(targetPath), "final.c");
            File.WriteAllText(finalProgramPath, generatedCode);
            var gccCompiler = new GccCompiler();
            return gccCompiler.Compile(finalProgramPath, targetPath);
        }

        private string GenerateCode(RootNode root)
        {
            foreach (var func in BuiltInFuncDeclNode.GetBuiltInFunctions())
                root.FuncDecls.Add(func);

            var reporter = new EventReporter();

            var codeGen = new CodeGeneratorVisitor();
            var scopeChecker = new ScopeCheckVisitor(reporter);
            var typeChecker = new TypeCheckVisitor(reporter);
            root.Accept(scopeChecker, new VisitorArgs());
            root.Accept(typeChecker, new VisitorArgs());
            root.Accept(codeGen, new VisitorArgs());

            return codeGen.CProgram.Print(true, true, true);
        }

        private RootNode GetRootNode(int startLine, string programText)
        {
            var result = Parse(programText);
            if (result.Errors.Any())
            {
                var errorMessages = result.Errors.Select(e => e.GetErrorMessage()).ToArray();

                Assert.Fail(
                    $"Parser error for program at line {startLine}: \n\r \n\r{string.Join(", ", errorMessages)} \n\r{programText}");
            }

            return result.Root;
        }
    }
}

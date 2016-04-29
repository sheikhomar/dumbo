using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace dumbo.Compiler.CodeGenerator
{
    public class Program
    {
        private IList<Module> _moduleList;
        private Module _mainModule;
        private Module _LibModule;
        private Module _userFuncDeclModule;

        public Program(Module libary)
        {
            _moduleList = new List<Module>();
            _LibModule = libary;
        }

        public void AddModule(Module module)
        {
            _moduleList.Add(module);
        }

        public void AddMainModule(Module module)
        {
            if (_mainModule != null)
                throw new Exception("A program can only have one main Module");
            _mainModule = module;
        }

        public void AddUserFuncDeclModule(Module module)
        {
            if (_userFuncDeclModule != null)
                throw new Exception("A program can only have one UserFuncDecl Module");
            _userFuncDeclModule = module;
        }

        /// <summary>
        /// Prints the C version of the program with the specified options
        /// </summary>
        /// <returns></returns>
        public string Print(bool prettyPrint, bool includeLibary, bool includeUserFuncDecl, int tabSize = 2, string indentation = "{", string outdentation = "}", string separator = "\r\n")
        {
            var builder = new StringBuilder();

            if (includeLibary)
                builder.Append(_LibModule.Print());
            if (includeUserFuncDecl)
                builder.Append(_userFuncDeclModule.Print());
            if (_mainModule != null)
                builder.Append(_mainModule.Print());
            else
                throw new Exception("No mainModule was set");

            foreach (var module in _moduleList)
            {
                builder.Append(module.Print());
            }

            if (prettyPrint)
                return PrettyPrint(builder.ToString(), indentation, outdentation, separator, tabSize);
            else
                return builder.ToString();
        }

        private string PrettyPrint(string input, string indentation, string outdentation, string separator, int tabSize)
        {
            var builder = new StringBuilder();
            int currentIndentation = 0;
            var lines = Regex.Split(input, separator);

            foreach (var line in lines)
            {
                bool hasIndent = Regex.IsMatch(line, indentation);
                bool hasOutdent = Regex.IsMatch(line, outdentation);

                if (hasOutdent && !hasIndent)
                    currentIndentation -= tabSize;

                builder.Append(new string(' ', currentIndentation * tabSize));
                builder.AppendLine(line);

                if (hasIndent && !hasOutdent)
                    currentIndentation += tabSize;
            }

            return builder.ToString();
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml;
using dumbo.Compiler;
using dumbo.Compiler.AST;
using dumbo.Compiler.CodeGenerator;
using dumbo.Compiler.Interpreter;
using ICSharpCode.AvalonEdit.Highlighting;
using Microsoft.Win32;
using Path = System.IO.Path;
using dumbo.Compiler.SyntaxAnalysis;
using dumbo.WpfApp.Editor;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;

namespace dumbo.WpfApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IInteractiveShell
    {
        private string currentSourcePath;
        private string currentGrammarTablePath;
        private DateTime latestCompiledAt;
        private DateTime latestSaveAt;
        private DateTime grammarTableLoadedAt;
        private Parser _myParser;
        private readonly FileSystemWatcher _grammarTableWatcher;
        private ITextMarkerService _textMarkerService;
        private HappyCProcessor _processor;

        public MainWindow()
        {
            InitializeComponent();
            _processor = new HappyCProcessor();
            _processor.Success += ProcessorOnSuccess;
            _processor.Failure += ProcessorOnFailure;

            _grammarTableWatcher = new FileSystemWatcher();
            _grammarTableWatcher.NotifyFilter = NotifyFilters.LastWrite;
            _grammarTableWatcher.EnableRaisingEvents = false;
            _grammarTableWatcher.Changed += GrammarHasChanged;

            textEditor.TextArea.Caret.PositionChanged += CaretOnPositionChanged;


            using (var s = GetType().Assembly.GetManifestResourceStream("dumbo.WpfApp.HappyZSyntaxHighlighting.xshd"))
            {
                using (var reader = new XmlTextReader(s))
                {
                    textEditor.SyntaxHighlighting = HighlightingLoader.Load(reader, HighlightingManager.Instance);
                }
            }

            textEditor.TextArea.TextView.BackgroundRenderers.Add(
                new HighlightCurrentLineBackgroundRenderer(textEditor));

            InitializeTextMarkerService();

            textEditor.Focus();

            UpdateCaretPosition();
            LoadDefaultProgram();
            LoadGrammar(GetGrammarFile());
        }

        void InitializeTextMarkerService()
        {
            var textMarkerService = new TextMarkerService(textEditor.Document);
            textEditor.TextArea.TextView.BackgroundRenderers.Add(textMarkerService);
            textEditor.TextArea.TextView.LineTransformers.Add(textMarkerService);
            IServiceContainer services = (IServiceContainer)textEditor.Document.ServiceProvider.GetService(typeof(IServiceContainer));
            if (services != null)
                services.AddService(typeof(ITextMarkerService), textMarkerService);
            _textMarkerService = textMarkerService;
        }

        private void LoadGrammar(string path)
        {
            _grammarTableWatcher.EnableRaisingEvents = false;
            string message = string.Empty;
            try
            {
                _myParser = new Parser(path);

                lblGrammarTable.ToolTip = path;
                lblGrammarTable.Text = Path.GetFileName(path);

                currentGrammarTablePath = path;
                grammarTableLoadedAt = DateTime.Now;
                UpdateInformationBox();

                _grammarTableWatcher.Path = Path.GetDirectoryName(currentGrammarTablePath);
                _grammarTableWatcher.EnableRaisingEvents = true;
            }
            catch (Exception ex)
            {
                message = $"Grammar {path} could not be loaded.\nException {ex.Message}";
            }

            ResultTextBox.Text = message;
        }

        private void GrammarHasChanged(object sender, FileSystemEventArgs e)
        {
            if (e.FullPath == currentGrammarTablePath && !IsFileReady(e.FullPath)) return;

            Dispatcher.BeginInvoke(new Action(() => LoadGrammar(currentGrammarTablePath)));
        }

        private void CaretOnPositionChanged(object sender, EventArgs eventArgs)
        {
            UpdateCaretPosition();
        }

        private void SaveFileClick(object sender, RoutedEventArgs e)
        {
            if (currentSourcePath == null)
            {
                SaveFileDialog dlg = new SaveFileDialog();
                dlg.DefaultExt = ".txt";
                if (dlg.ShowDialog() ?? false)
                {
                    currentSourcePath = dlg.FileName;
                }
                else
                {
                    return;
                }
            }
            textEditor.Save(currentSourcePath);
            latestSaveAt = DateTime.Now;
            UpdateInformationBox();
        }

        private void OpenFileClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.CheckFileExists = true;
            if (dlg.ShowDialog() ?? false)
            {
                currentSourcePath = dlg.FileName;
                textEditor.Load(currentSourcePath);
            }
        }

        private void UpdateCaretPosition()
        {
            var pos = textEditor.TextArea.Caret;
            lblCursorPosition.Text = $"Line {pos.Line}, Char {pos.Column}";
        }

        private void UpdateInformationBox()
        {
            StringBuilder builder = new StringBuilder();

            if (latestCompiledAt != DateTime.MinValue)
            {
                builder.Append("Compiled at ").Append(latestCompiledAt.ToString("T"));
            }
            builder.Append("\n");

            if (latestSaveAt != DateTime.MinValue)
            {
                builder.Append("Saved at ").Append(latestSaveAt.ToString("T"));
            }
            builder.Append("\n");

            if (grammarTableLoadedAt != DateTime.MinValue)
            {
                builder.Append("Grammar loaded at ").Append(grammarTableLoadedAt.ToString("T"));
            }

            InformationBox.Text = builder.ToString();
        }

        private string GetGrammarFile()
        {
            string appDir = System.AppDomain.CurrentDomain.BaseDirectory;
            string baseDir = Path.GetFullPath(Path.Combine(appDir, "..\\..\\..\\..\\..\\"));
            string grammarPath = Path.Combine(baseDir, "Grammar");
            string path = Path.Combine(grammarPath, "HappyZ-Grammar.egt");
            return path;
        }

        private void LoadDefaultProgram()
        {
            var path = GetDefaultProgramPath();
            if (File.Exists(path))
            {
                currentSourcePath = path;
                textEditor.Load(currentSourcePath);
            }
        }

        private string GetDefaultProgramPath()
        {
            string appDir = System.AppDomain.CurrentDomain.BaseDirectory;
            return Path.Combine(appDir, "DefaultProgram.hz");
        }

        private void Print(object sender, ExecutedRoutedEventArgs e)
        {
            Compile(sender, e);

        }

        private void Compile(object sender, ExecutedRoutedEventArgs e)
        {
            if (!File.Exists(currentSourcePath))
            {
                ResultTextBox.Text = $"File {currentSourcePath} does not exist!";
                return;
            }

            SaveFile(sender, e);

            var parserResult = Parse(textEditor.Text);

            if (parserResult.IsSuccess)
            {
                var root = parserResult.Root;
                PrettyPrint(root);
                EventReporter reporter = new EventReporter();
                _processor.Start(reporter, this, root);
                MarkErrors(reporter);
            }

            latestCompiledAt = DateTime.Now;
            UpdateInformationBox();
        }

        private ParserResult Parse(string text)
        {
            var data = new StringReader(text);
            var parserResult = _myParser.Parse(data);

            if (parserResult.IsSuccess)
            {
                foreach (var node in BuiltInFuncDeclNode.GetBuiltInFunctions())
                    parserResult.Root.FuncDecls.InsertAt(0, node);
            }
            else
            {
                StringBuilder errorMessage = new StringBuilder();

                foreach (var parserError in parserResult.Errors)
                {
                    textEditor.TextArea.Caret.Column = parserError.Column;
                    textEditor.TextArea.Caret.Line = parserError.LineNumber;
                    errorMessage.Append(parserError.GetErrorMessage());
                }

                ResultTextBox.Text = errorMessage.ToString();
            }

            return parserResult;
        }

        private void SaveFile(object sender, ExecutedRoutedEventArgs e)
        {
            SaveFileClick(sender, e);
        }

        private void DrawReductionTree(GOLD.Reduction Root)
        {
            //This procedure starts the recursion that draws the parse tree.
            StringBuilder tree = new StringBuilder();

            tree.AppendLine("+ " + Root.Parent.Text(false));
            DrawReduction(tree, Root, 1);

            ResultTextBox.Text = tree.ToString();
        }

        private void DrawReduction(StringBuilder tree, GOLD.Reduction reduction, int indent)
        {
            int n;
            string indentText = "";

            for (n = 1; n <= indent; n++)
            {
                indentText += "| ";
            }

            //=== Display the children of the reduction
            for (n = 0; n < reduction.Count(); n++)
            {
                switch (reduction[n].Type())
                {
                    case GOLD.SymbolType.Nonterminal:
                        GOLD.Reduction branch = (GOLD.Reduction)reduction[n].Data;

                        tree.AppendLine(indentText + "+ " + branch.Parent.Text(false));
                        DrawReduction(tree, branch, indent + 1);
                        break;

                    default:
                        string leaf = (string)reduction[n].Data;

                        tree.AppendLine(indentText + "+ " + leaf);
                        break;
                }
            }
        }

        private void PickGrammar(object sender, ExecutedRoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.CheckFileExists = true;
            if (dlg.ShowDialog() ?? false)
            {
                string path = dlg.FileName;
                LoadGrammar(path);
            }
        }

        private void ReloadGrammar(object sender, ExecutedRoutedEventArgs e)
        {
            LoadGrammar(currentGrammarTablePath);
        }

        private bool IsFileReady(string path)
        {
            try
            {
                using (File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read))
                    return true;
            }
            catch (IOException)
            {
                return false;
            }
        }

        private void PrettyPrint(RootNode root)
        {
            var ppv = new PrettyPrintVisitor();
            root.Accept(ppv, new VisitorArgs());
            ResultTextBox.Text = ppv.GetResult();
        }

        private void ScopeAndTypeCheck(RootNode root, EventReporter reporter)
        {
            var scopeChecker = new ScopeCheckVisitor(reporter);
            var typeChecker = new TypeCheckVisitor(reporter);
            root.Accept(scopeChecker, new VisitorArgs());
            root.Accept(typeChecker, new VisitorArgs());
        }

        private void MarkErrors(EventReporter reporter)
        {
            var events = reporter.GetEvents().ToArray();
            ErrorList.ItemsSource = events;

            _textMarkerService.RemoveAll(m => true);

            foreach (var item in events)
            {
                if (item.SourcePosition.StartLine == 0)
                    continue;
                var startLine = textEditor.Document.GetLineByNumber(item.SourcePosition.StartLine);

                int selectionStart = startLine.Offset + item.SourcePosition.StartColumn;
                int selectionEnd = item.SourcePosition.EndColumn - item.SourcePosition.StartColumn;
                ITextMarker marker = _textMarkerService.Create(selectionStart, selectionEnd);
                marker.MarkerTypes = TextMarkerTypes.SquigglyUnderline;
                marker.MarkerColor = Colors.Red;
            }
        }

        public void Write(string writeParameter)
        {
            Dispatcher.Invoke(() =>
            {
                ShellTab.Focus();
                ShellTextBox.Text += writeParameter + "\n";
                textEditor.Focus();
            });
        }

        public NumberValue ReadNumber()
        {
            return Dispatcher.Invoke(() =>
            {
                var rw = new ReaderWindow(true);
                rw.ShowDialog();
                double val;
                if (double.TryParse(rw.ReturnValue, out val))
                    return new NumberValue(val);
                return new NumberValue(0);
            });
        }

        public TextValue ReadText()
        {
            return Dispatcher.Invoke(() =>
            {
                var rw = new ReaderWindow(false);
                rw.ShowDialog();
                return new TextValue(rw.ReturnValue);
            });
        }

        private void GenerateCode(object sender, ExecutedRoutedEventArgs e)
        {
            if (!File.Exists(currentSourcePath))
            {
                ResultTextBox.Text = $"File {currentSourcePath} does not exist!";
                return;
            }

            SaveFile(sender, e);

            var parserResult = Parse(textEditor.Text);

            if (parserResult.IsSuccess)
            {
                var root = parserResult.Root;
                EventReporter reporter = new EventReporter();
                ScopeAndTypeCheck(parserResult.Root, reporter);
                if (!reporter.HasErrors)
                {
                    var codeGen = new CodeGeneratorVisitor();
                    root.Accept(codeGen, new VisitorArgs());
                    ResultTextBox.Text = codeGen.CProgram.Print(true, false);
                }

                MarkErrors(reporter);
            }

            latestCompiledAt = DateTime.Now;
            UpdateInformationBox();
        }

        private void BreakExecution(object sender, ExecutedRoutedEventArgs e)
        {
            Environment.Exit(0);
        }
        
        private void ProcessorOnFailure(object sender, IEnumerable<Exception> exceptions)
        {
            ErrorList.ItemsSource = exceptions.Select(e => new Event(EventKind.Error, e.Message, null));
        }

        private void ProcessorOnSuccess(object sender, ProcessorResult processorResult)
        {
            MarkErrors(processorResult.Reporter);
        }
    }
}

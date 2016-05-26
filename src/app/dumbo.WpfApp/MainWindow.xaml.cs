using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;
using System.Text;
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
using dumbo.Compiler.TypeChecking;
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
        private ITextMarkerService _textMarkerService;
        private HappyProcessor _processor;
        private GccCompiler _gccCompiler;

        public MainWindow()
        {
            InitializeComponent();
            _processor = new HappyProcessor();
            _processor.Success += ProcessorOnSuccess;
            _processor.Failure += ProcessorOnFailure;

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

            _gccCompiler = new GccCompiler();
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
            string message = string.Empty;
            try
            {
                _myParser = new Parser(path);

                lblGrammarTable.ToolTip = path;
                lblGrammarTable.Text = Path.GetFileName(path);

                currentGrammarTablePath = path;
                grammarTableLoadedAt = DateTime.Now;
                UpdateInformationBox();
            }
            catch (Exception ex)
            {
                message = $"Grammar {path} could not be loaded.\nException {ex.Message}";
            }

            ResultTextBox.Text = message;
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
            var data = new StringReader(text + "\r\n");
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
            //GccCompiler.Compile("");

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
                    string generatedCode = codeGen.CProgram.Print(true, true, true);

                    GccCompileAndRun(generatedCode);

                    
                }

                MarkErrors(reporter);
            }

            latestCompiledAt = DateTime.Now;
            UpdateInformationBox();
        }

        private void GccCompileAndRun(string generatedCode)
        {
            var finalProgramPath = Path.Combine(Environment.CurrentDirectory, "final.c");
            var targetPath = Path.Combine(Environment.CurrentDirectory, "final.exe");
            File.WriteAllText(finalProgramPath, generatedCode);

            var result = _gccCompiler.Compile(finalProgramPath, targetPath);
            ResultTextBox.Text = $"Binary file path:  {targetPath} \n";

            if (result.StatusCode == 0)
            {
                RunCompiledProgram(targetPath);
            }
            else
            {
                ResultTextBox.Text = $"Compilation failed with status code {result.StatusCode} \n";

                foreach (var error in result.OutputData)
                    ResultTextBox.Text += $"{error} \n";

                if (result.ErrorData.Any())
                {
                    ResultTextBox.Text += "Errors:\n";
                    foreach (var error in result.ErrorData)
                        ResultTextBox.Text += $"{error} \n";
                }
            }
        }

        private void RunCompiledProgram(string path)
        {
            var runner = new CProgramRunner();
            var result = runner.Run(path);
            if (result.StatusCode == 0)
            {
                ResultTextBox.Text = $"Compiled program has been run successfully. Output:\n";

                foreach (var output in result.OutputData)
                    ResultTextBox.Text += $"{output} \n";
            }
            else
            {
                ResultTextBox.Text = $"Running program failed with status code {result.StatusCode} \n";

                foreach (var error in result.OutputData)
                    ResultTextBox.Text += $"{error} \n";

                if (result.ErrorData.Any())
                {
                    ResultTextBox.Text += "Errors:\n";
                    foreach (var error in result.ErrorData)
                        ResultTextBox.Text += $"{error} \n";
                }
            }
        }

        private void BreakExecution(object sender, ExecutedRoutedEventArgs e)
        {
            Environment.Exit(0);
        }

        private void CopyCCodeToClipboard(object sender, ExecutedRoutedEventArgs e)
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
                    string compiledText = codeGen.CProgram.Print(true, true, true);
                    ResultTextBox.Text = compiledText;
                    Clipboard.SetText(compiledText);
                }

                MarkErrors(reporter);
            }

            latestCompiledAt = DateTime.Now;
            UpdateInformationBox();
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


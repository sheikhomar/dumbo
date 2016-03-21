using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Xml;
using dumbo.Compiler.AST;
using dumbo.Compiler.CCAnalysis;
using ICSharpCode.AvalonEdit.Highlighting;
using Microsoft.Win32;
using Path = System.IO.Path;
using dumbo.Compiler.SyntaxAnalysis;
using dumbo.Compiler.PrettyPrint;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;

namespace dumbo.WpfApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string currentSourcePath;
        private string currentGrammarTablePath;
        private DateTime latestCompiledAt;
        private DateTime latestSaveAt;
        private DateTime grammarTableLoadedAt;
        private Parser _myParser;
        private readonly FileSystemWatcher _grammarTableWatcher;

        public MainWindow()
        {
            InitializeComponent();
            
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

            textEditor.Focus();

            UpdateCaretPosition();
            LoadDefaultProgram();
            LoadGrammar(GetGrammarFile());
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
            
            var data = new StringReader(textEditor.Text);
            var parserResult = _myParser.Parse(data);

            if (parserResult.Errors.Any())
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
            else
            {
                PrettyPrint(parserResult.Root);
                CheckContextualContraints(parserResult.Root);
            }

            latestCompiledAt = DateTime.Now;
            UpdateInformationBox();
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
            var prettyPrinter = new PrettyPrinter();
            var output = prettyPrinter.print(root);
            ResultTextBox.Text = output.ToString();
        }

        private void CheckContextualContraints(RootNode root)
        {
            CCAnalyser analyser = new CCAnalyser();
            root.CCAnalyse(analyser);

            IList<ErrorListItem> list = new List<ErrorListItem>();

            var errors = analyser.ErrorReporter.Errors;
            if (errors.Any())
            {
                

                foreach (var error in errors)
                {
                    ResultTextBox.Text += $"\nLine {error.Line}, Col {error.Column}: {error.Message}";
                    list.Add(new ErrorListItem() { Line = error.Line, Status = "Error", Description = error.Message});
                }
            }

            
            ErrorList.ItemsSource = list;
        }
    }

    public class ErrorListItem
    {
        public string Status { get; set; }
        public string Description { get; set; }
        public int Line { get; set; }
        public int Column { get; set; }
    }
}

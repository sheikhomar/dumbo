using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ICSharpCode.AvalonEdit.Highlighting;
using Microsoft.Win32;
using Path = System.IO.Path;

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
        private readonly MyParser _myParser;
        private readonly FileSystemWatcher _grammarTableWatcher;

        public MainWindow()
        {
            InitializeComponent();

            _myParser = new MyParser();
            _grammarTableWatcher = new FileSystemWatcher();
            _grammarTableWatcher.NotifyFilter = NotifyFilters.LastWrite;
            _grammarTableWatcher.EnableRaisingEvents = false;
            _grammarTableWatcher.Changed += GrammarHasChanged;

            textEditor.TextArea.Caret.PositionChanged += CaretOnPositionChanged;

            textEditor.SyntaxHighlighting = HighlightingManager.Instance.GetDefinition("VB");

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
                if (!_myParser.Setup(path))
                    message = $"Grammar {path} could not be loaded.";
                else
                {
                    lblGrammarTable.ToolTip = path;
                    lblGrammarTable.Text = Path.GetFileName(path);

                    currentGrammarTablePath = path;
                    grammarTableLoadedAt = DateTime.Now;
                    UpdateInformationBox();

                    _grammarTableWatcher.Path = Path.GetDirectoryName(currentGrammarTablePath);
                    _grammarTableWatcher.EnableRaisingEvents = true;
                }
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

        private void Compile(object sender, ExecutedRoutedEventArgs e)
        {
            if (!File.Exists(currentSourcePath))
            {
                ResultTextBox.Text = $"File {currentSourcePath} does not exist!";
                return;
            }

            var data = new StringReader(currentSourcePath);


            data = new StringReader(textEditor.Text);
            if (_myParser.Parse(data))
            {
                DrawReductionTree(_myParser.Root);
            }
            else
            {
                ResultTextBox.Text = _myParser.FailMessage;
                textEditor.TextArea.Caret.Column = _myParser.Column+1;
                textEditor.TextArea.Caret.Line = _myParser.Line+1;
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
    }
}

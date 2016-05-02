using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using dumbo.Compiler.AST;

namespace dumbo.WpfApp
{
    /// <summary>
    /// Interaction logic for ReaderWindow.xaml
    /// </summary>
    public partial class ReaderWindow : Window
    {
        private readonly Regex _regex;
        private readonly bool _isRequestedTypeNumber;

        public string ReturnValue { get; private set; }

        public ReaderWindow(bool isNumber)
        {
            InitializeComponent();
            InputTextBox.Focusable = true;
            Keyboard.Focus(InputTextBox);
            _isRequestedTypeNumber = isNumber;
            _regex = new Regex("[^0-9.-]+");
            Title = "Expecting " + (_isRequestedTypeNumber ? "Number" : "Text");
        }

        protected override void OnActivated(EventArgs e)
        {
            InputTextBox.Focusable = true;
            Keyboard.Focus(InputTextBox);
            base.OnActivated(e);
        }

        private void InputTextBox_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                ReturnValue = InputTextBox.Text;
                Close();
            }
            else if (e.Key == Key.Escape)
            {
                ReturnValue = string.Empty;
                Close();
            }
        }

        private void InputTextBox_OnPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (_isRequestedTypeNumber)
                e.Handled = _regex.IsMatch(e.Text);
        }
    }
}

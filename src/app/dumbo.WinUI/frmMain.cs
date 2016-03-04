using System;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace dumbo.WinUI
{
    public partial class frmMain : Form
    {
        MyParserClass MyParser = new MyParserClass();

        public frmMain()
        {
            InitializeComponent();
        }

        private void LoadGrammar()
        {
            try
            {
                if (MyParser.Setup(txtTableFile.Text))
                {
                    //Change button enable/disable for the user
                    btnLoad.Enabled = true;
                    btnParse.Enabled = true;
                }
                else
                {
                    MessageBox.Show("CGT failed to load");
                }
            }
            catch (GOLD.ParserException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            LoadGrammar();
        }

        private void btnParse_Click(object sender, EventArgs e)
        {
            btnParse.Enabled = false;

            if (MyParser.Parse(new StringReader(txtSource.Text))) 
            {
                DrawReductionTree(MyParser.Root);
            } else {
                txtParseTree.Text = MyParser.FailMessage;
            } 

            btnParse.Enabled = true;
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            btnLoad.Enabled = true;
            btnParse.Enabled = false;

            string baseDir = Path.GetFullPath(Path.Combine(Application.StartupPath, "..\\..\\..\\..\\..\\"));
            string grammarPath = Path.Combine(baseDir, "Grammar");
            string path = Path.Combine(grammarPath, "HappyZ-Grammar.egt");
            
            txtTableFile.Text = path;
        }

       private void DrawReductionTree(GOLD.Reduction Root)
       {
            //This procedure starts the recursion that draws the parse tree.
            StringBuilder tree = new StringBuilder();

            tree.AppendLine("+ " + Root.Parent.Text(false));
            DrawReduction(tree, Root, 1);

            txtParseTree.Text = tree.ToString();
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


    }
} 

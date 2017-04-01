using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using StoredProcedureCaller;
using System.Text.RegularExpressions;

namespace SPCodeGenWpfApp
{
    /// <summary>
    /// Interaction logic for GetTableScriptWin.xaml
    /// </summary>
    public partial class GetTableScriptWin : Window
    {
        public GetTableScriptWin()
        {
            InitializeComponent();
            SProcSignature = null;
            TableTypeSignatureList = new List<TableTypeSignature>();
        }
        public SPSignature SProcSignature { get; set; }
        public List<TableTypeSignature> TableTypeSignatureList { get; set; }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (SProcSignature != null)
            {
                lblInstructions.Content = string.Format(lblInstructions.Content.ToString(), SProcSignature.TableParamNum);
                if (SProcSignature.TableParamNum > 1)
                    lblInstructions.Content = lblInstructions.Content.ToString() + " separated by 'GO'";
            }
        }

        private void btnParse_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtTableScript.Text))
            {
                return;
            }
            TableTypeSignatureList.Clear();
            if (SProcSignature.TableParamNum > 1)
            {
                string[] sqlLine;
                Regex regex = new Regex("^GO", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                sqlLine = regex.Split(txtTableScript.Text);

                int correct = 0;
                foreach (string str in sqlLine)
                {
                    TableTypeSignature sign = new TableTypeSignature();
                    if (sign.Parse(str, chkNoNullableTypes.IsChecked==true) == false) // successful;
                    {
                        ++correct;
                        TableTypeSignatureList.Add(sign);
                    }
                }
                if (correct != SProcSignature.TableParamNum)
                {
                    MessageBox.Show("Error with the table type creation script!",
                        "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                    this.Close();
            }
            else
            {
                TableTypeSignature sign = new TableTypeSignature();
                if (sign.Parse(txtTableScript.Text, chkNoNullableTypes.IsChecked == true) == false) // successful;
                {
                    TableTypeSignatureList.Add(sign);
                    Close();
                }
                else
                    MessageBox.Show("Error with the table type creation script!",
                        "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

    }
}

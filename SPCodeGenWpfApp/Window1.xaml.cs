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
using System.Windows.Navigation;
using System.Windows.Shapes;
using StoredProcedureCaller;

namespace SPCodeGenWpfApp
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        public Window1()
        {
            InitializeComponent();

            txtSignature.AddHandler(CommandManager.ExecutedEvent, new RoutedEventHandler(CommandExecuted), true);
        }

        private void CommandExecuted(object sender, RoutedEventArgs e)
        {
            if ((e as ExecutedRoutedEventArgs).Command == ApplicationCommands.Paste)
            {
                // verify that the textbox handled the paste command

                if (e.Handled)
                {
                    string signLower = txtSignature.Text.ToLower();

                    int pos = signLower.IndexOf("proc");

                    if (pos == -1)
                        return;

                    bool PosInSpaceBefProcName = false;
                    bool PosProcName = false;
                    string spname = string.Empty;
                    for (int i = pos; i < txtSignature.Text.Length; ++i)
                    {
                        char c = txtSignature.Text[i];

                        if (PosInSpaceBefProcName == false)
                        {
                            if (SPSignature.IsWhitespace(c) == false)
                                continue;
                            else
                            {
                                PosInSpaceBefProcName = true;
                            }
                        }
                        else if (PosInSpaceBefProcName && PosProcName == false)
                        {
                            if (SPSignature.IsWhitespace(c))
                                continue;
                            else
                            {
                                spname += c;
                                PosProcName = true;
                            }
                        }
                        else if (PosProcName)
                        {
                            if (SPSignature.IsWhitespace(c) == false && c != '(')
                                spname += c;
                            else
                            {

                                break;
                            }
                        }
                    }

                    txtMethodName.Text = spname.Trim();
                }
            }
        }

        private void btnGenCode_Click(object sender, RoutedEventArgs e)
        {
            string msg = null;
            if (string.IsNullOrEmpty(txtSignature.Text))
                msg = "Signature is not specified.";
            if (string.IsNullOrEmpty(txtMethodName.Text))
                msg = "Method Name is not specified.";

            if (string.IsNullOrEmpty(msg) == false)
            {
                MessageBox.Show(msg);
                return;
            }

            string code = string.Empty;

            if (chkMySql.IsChecked == false)
            {
                SPCallerGen.ReturnType retType = SPCallerGen.ReturnType.Tables;
                if (radioTables.IsChecked == true)
                    retType = SPCallerGen.ReturnType.Tables;
                else if (radioInteger.IsChecked == true)
                    retType = SPCallerGen.ReturnType.Integer;
                else if (radioNone.IsChecked == true)
                    retType = SPCallerGen.ReturnType.None;
                else
                {
                    MessageBox.Show("No return type is selected.");
                    return;
                }

                SPSignature signature = new SPSignature();
                try
                {
                    signature.Parse(txtSignature.Text+" ", chkNoNullableTypes.IsChecked == true);
                    if (signature.HasTableParam)
                    {
                        GetTableScriptWin form = new GetTableScriptWin();
                        form.SProcSignature = signature;
                        form.ShowDialog();
                        if (form.TableTypeSignatureList.Count != signature.TableParamNum)
                        {
                            MessageBox.Show("Error with getting table script", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }
                        foreach (TableTypeSignature ttSign in form.TableTypeSignatureList)
                        {
                            code += TableTypeGen.GenCode(ttSign);
                        }
                    }
                    code += SPCallerGen.GenCode(signature, txtMethodName.Text, retType, null, null, false);
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show("Error:" + ex.Message);
                    return;
                }
            }
            else
            {
                SPMySQLCallerGen.ReturnType retType = SPMySQLCallerGen.ReturnType.Tables;
                if (radioTables.IsChecked == true)
                    retType = SPMySQLCallerGen.ReturnType.Tables;
                else if (radioInteger.IsChecked == true)
                    retType = SPMySQLCallerGen.ReturnType.Integer;
                else if (radioNone.IsChecked == true)
                    retType = SPMySQLCallerGen.ReturnType.None;
                else
                {
                    MessageBox.Show("No return type is selected.");
                    return;
                }

                SPMySQLSignature signature = new SPMySQLSignature();
                try
                {
                    signature.Parse(txtSignature.Text, chkNoNullableTypes.IsChecked == true);
                    code += SPMySQLCallerGen.GenCode(signature, txtMethodName.Text, retType, null, null, false);
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show("Error:" + ex.Message);
                    return;
                }
            }

            if (string.IsNullOrEmpty(code) == false)
            {
                Clipboard.SetData(DataFormats.Text, code);
                MessageBox.Show("The auto-generated code has been copied to clipboard!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
                MessageBox.Show("No code is generated", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}

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

namespace CRUDGenerator
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        private SPSignature _Signature = null;
        private List<Parameter> _ListParam = null;

        public Window1()
        {
            InitializeComponent();

            //List<Parameter> list = new List<Parameter>();
            //list.Add(new Parameter {Name="Steve Joo", SQLType="Integer", TextLength="50"});
            //list.Add(new Parameter { Name = "Jenny Poh", SQLType = "NVarChar", TextLength = "100" });

            //listBoxSQPParameterType.ItemsSource = list;
        }

        private void btnParse_Click(object sender, RoutedEventArgs e)
        {
            if(string.IsNullOrEmpty(txtFunctionName.Text))
            {
                MessageBox.Show("Please fill in the function name");
                return;
            }
            if(string.IsNullOrEmpty(txtSQL.Text))
            {
                MessageBox.Show("Please fill in the SQL statement");
                return;
            }

            _Signature = new SPSignature();
            _Signature.Name = txtFunctionName.Text;

            List<string> listParamStr = _Signature.GetAllParam(txtSQL.Text);

            if (listParamStr.Count == 0)
            {
                MessageBox.Show("No @ parameters found");
                return;
            }

            _ListParam = new List<Parameter>();

            foreach (string str in listParamStr)
            {
                _ListParam.Add(new Parameter { Name = str, SQLType = "NVarChar", TextLength = "50" });
            }

            SQLType type = _Signature.GetType(txtSQL.Text);

            if (type == SQLType.INSERT)
            {
                chkReturnPrimaryKey.IsEnabled = true;
            }
            else
            {
                chkReturnPrimaryKey.IsEnabled = false;
            }
            if (type == SQLType.SELECT)
            {
                chkUseSQLReader.IsEnabled = true;
            }
            else
            {
                chkUseSQLReader.IsEnabled = false;
            }

            listBoxSQPParameterType.ItemsSource = _ListParam;

            btnGenerateCode.IsEnabled = true;
        }

        private void btnGenerateCode_Click(object sender, RoutedEventArgs e)
        {
            if(_Signature==null)
            {
                MessageBox.Show("Please click the Parse button first");
                return;
            }

            _Signature.ClearAllColumns();
            foreach (var par in _ListParam)
            {
                _Signature.AddColumn(par.Name, par.SQLType, par.TextLength);
            }

            _Signature.Sql = txtSQL.Text;
            SQLType type = _Signature.GetType(txtSQL.Text);

            string GeneratedCode = string.Empty;

            if (type == SQLType.INSERT)
            {
                if (chkReturnPrimaryKey.IsChecked==true)
                {
                    GeneratedCode = SPCallerGen.GenInsertRetPriKeyCode(_Signature, _Signature.Name, txtAddParameterCode.Text,
                        txtAddExceptionCode.Text, chkPassConnection.IsChecked ?? false, chkPassTransaction.IsChecked ?? false,
                        chkUseTransaction.IsChecked ?? false);
                }
                else
                {
                    GeneratedCode = SPCallerGen.GenInsertCode(_Signature, _Signature.Name, txtAddParameterCode.Text,
                        txtAddExceptionCode.Text, chkPassConnection.IsChecked ?? false, chkPassTransaction.IsChecked ?? false,
                        chkUseTransaction.IsChecked ?? false);
                }
            }
            else if (type == SQLType.SELECT)
            {
                if (chkUseSQLReader.IsChecked==true)
                {
                    GeneratedCode = SPCallerGen.GenSelectReaderCode(_Signature, _Signature.Name, txtAddParameterCode.Text,
                        txtAddExceptionCode.Text, chkPassConnection.IsChecked ?? false, chkPassTransaction.IsChecked ?? false,
                        chkUseTransaction.IsChecked ?? false);
                }
                else
                {
                    GeneratedCode = SPCallerGen.GenSelectCode(_Signature, _Signature.Name, txtAddParameterCode.Text,
                        txtAddExceptionCode.Text, chkPassConnection.IsChecked ?? false, chkPassTransaction.IsChecked ?? false,
                        chkUseTransaction.IsChecked ?? false);
                }
            }
            else if (type == SQLType.UPDATE)
            {
                GeneratedCode = SPCallerGen.GenUpdateCode(_Signature, _Signature.Name, txtAddParameterCode.Text,
                    txtAddExceptionCode.Text, chkPassConnection.IsChecked ?? false, chkPassTransaction.IsChecked ?? false,
                    chkUseTransaction.IsChecked ?? false);
            }
            else if (type == SQLType.DELETE)
            {
                GeneratedCode = SPCallerGen.GenDeleteCode(_Signature, _Signature.Name, txtAddParameterCode.Text,
                    txtAddExceptionCode.Text, chkPassConnection.IsChecked ?? false, chkPassTransaction.IsChecked ?? false,
                    chkUseTransaction.IsChecked ?? false);
            }
			
			
			if (string.IsNullOrEmpty(GeneratedCode) == false)
            {
				txtGeneratedCode.Text = GeneratedCode;
				if(chkCopyClipboard.IsChecked==true)
				{
					Clipboard.SetData(DataFormats.Text, GeneratedCode);
                    MessageBox.Show("The auto-generated code has been copied to clipboard!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
					return;
				}
                MessageBox.Show("The auto-generated code has been generated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
			{
				txtGeneratedCode.Text = string.Empty;
                MessageBox.Show("No code is generated", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}
        }

        private void chkUseTransaction_Checked(object sender, RoutedEventArgs e)
        {
            if (chkUseTransaction.IsChecked == true)
            {
                chkPassTransaction.IsEnabled = true;
            }
            else
            {
                chkPassTransaction.IsEnabled = false;
            }
        }
    }
}

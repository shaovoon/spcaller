using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using StoredProcedureCaller;
using System.Text.RegularExpressions;

namespace SPCodeGenForm
{
    public partial class GetTableScriptForm : Form
    {
        public GetTableScriptForm()
        {
            InitializeComponent();
            SProcSignature = null;
            TableTypeSignatureList = new List<TableTypeSignature>();

        }
        public SPSignature SProcSignature { get; set; }
        public List<TableTypeSignature> TableTypeSignatureList { get; set; }

        private void GetTableScriptForm_Load(object sender, EventArgs e)
        {
            if (SProcSignature != null)
            {
                lblInstructions.Text = string.Format(lblInstructions.Text, SProcSignature.TableParamNum);
                if (SProcSignature.TableParamNum > 1)
                    lblInstructions.Text += " separated by 'GO'";
            }
        }

        private void btnParse_Click(object sender, EventArgs e)
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
                    if (sign.Parse(str, chkNoNullableTypes.Checked) == false) // successful;
                    {
                        ++correct;
                        TableTypeSignatureList.Add(sign);
                    }
                }
                if (correct != SProcSignature.TableParamNum)
                {
                    MessageBox.Show("Error with the table type creation script!",
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                    this.Close();
            }
            else
            {
                TableTypeSignature sign = new TableTypeSignature();
                if (sign.Parse(txtTableScript.Text, chkNoNullableTypes.Checked) == false) // successful;
                {
                    TableTypeSignatureList.Add(sign);
                    Close();
                }
                else
                    MessageBox.Show("Error with the table type creation script!",
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using StoredProcedureCaller;

namespace SPCodeGenForm
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            txtSignature.PastedEvent += PastedEvent;
        }

        public void PastedEvent(string spname)
        {
            txtMethodName.Text = spname;
        }

        private void btnGenCode_Click(object sender, EventArgs e)
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

            if (chkMySql.Checked == false)
            {
                SPCallerGen.ReturnType retType = SPCallerGen.ReturnType.Tables;
                if (radioTables.Checked)
                    retType = SPCallerGen.ReturnType.Tables;
                else if (radioInteger.Checked)
                    retType = SPCallerGen.ReturnType.Integer;
                else if (radioNone.Checked)
                    retType = SPCallerGen.ReturnType.None;
                else
                {
                    MessageBox.Show("No return type is selected.");
                    return;
                }

                SPSignature signature = new SPSignature();
                try
                {
                    signature.Parse(txtSignature.Text+" ", chkNoNullableTypes.Checked);
                    if (signature.HasTableParam)
                    {
                        GetTableScriptForm form = new GetTableScriptForm();
                        form.SProcSignature = signature;
                        form.ShowDialog();
                        if (form.TableTypeSignatureList.Count != signature.TableParamNum)
                        {
                            MessageBox.Show("Error with getting table script", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                if (radioTables.Checked)
                    retType = SPMySQLCallerGen.ReturnType.Tables;
                else if (radioInteger.Checked)
                    retType = SPMySQLCallerGen.ReturnType.Integer;
                else if (radioNone.Checked)
                    retType = SPMySQLCallerGen.ReturnType.None;
                else
                {
                    MessageBox.Show("No return type is selected.");
                    return;
                }

                SPMySQLSignature signature = new SPMySQLSignature();
                try
                {
                    signature.Parse(txtSignature.Text, chkNoNullableTypes.Checked);
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
                System.Windows.Forms.Clipboard.SetText(code);
                MessageBox.Show("The auto-generated code has been copied to clipboard!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
                MessageBox.Show("No code is generated", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);


        }
    }
}

namespace SPCodeGenForm
{
    partial class GetTableScriptForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.txtTableScript = new System.Windows.Forms.TextBox();
            this.lblInstructions = new System.Windows.Forms.Label();
            this.btnParse = new System.Windows.Forms.Button();
            this.chkNoNullableTypes = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // txtTableScript
            // 
            this.txtTableScript.Location = new System.Drawing.Point(13, 25);
            this.txtTableScript.Multiline = true;
            this.txtTableScript.Name = "txtTableScript";
            this.txtTableScript.Size = new System.Drawing.Size(292, 299);
            this.txtTableScript.TabIndex = 0;
            // 
            // lblInstructions
            // 
            this.lblInstructions.AutoSize = true;
            this.lblInstructions.Location = new System.Drawing.Point(12, 9);
            this.lblInstructions.Name = "lblInstructions";
            this.lblInstructions.Size = new System.Drawing.Size(126, 13);
            this.lblInstructions.TabIndex = 1;
            this.lblInstructions.Text = "Enter {0} table type script";
            // 
            // btnParse
            // 
            this.btnParse.Location = new System.Drawing.Point(230, 330);
            this.btnParse.Name = "btnParse";
            this.btnParse.Size = new System.Drawing.Size(75, 23);
            this.btnParse.TabIndex = 2;
            this.btnParse.Text = "Parse";
            this.btnParse.UseVisualStyleBackColor = true;
            this.btnParse.Click += new System.EventHandler(this.btnParse_Click);
            // 
            // chkNoNullableTypes
            // 
            this.chkNoNullableTypes.AutoSize = true;
            this.chkNoNullableTypes.Location = new System.Drawing.Point(15, 334);
            this.chkNoNullableTypes.Name = "chkNoNullableTypes";
            this.chkNoNullableTypes.Size = new System.Drawing.Size(113, 17);
            this.chkNoNullableTypes.TabIndex = 7;
            this.chkNoNullableTypes.Text = "No Nullable Types";
            this.chkNoNullableTypes.UseVisualStyleBackColor = true;
            // 
            // GetTableScriptForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(318, 364);
            this.Controls.Add(this.chkNoNullableTypes);
            this.Controls.Add(this.btnParse);
            this.Controls.Add(this.lblInstructions);
            this.Controls.Add(this.txtTableScript);
            this.Name = "GetTableScriptForm";
            this.Text = "GetTableScriptForm";
            this.Load += new System.EventHandler(this.GetTableScriptForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtTableScript;
        private System.Windows.Forms.Label lblInstructions;
        private System.Windows.Forms.Button btnParse;
        private System.Windows.Forms.CheckBox chkNoNullableTypes;
    }
}
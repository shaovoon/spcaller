namespace SPCodeGenForm
{
    partial class Form1
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
            this.lblSignature = new System.Windows.Forms.Label();
            this.txtSignature = new SPCodeGenForm.NewTextBox();
            this.lblMethodName = new System.Windows.Forms.Label();
            this.txtMethodName = new System.Windows.Forms.TextBox();
            this.grpRetType = new System.Windows.Forms.GroupBox();
            this.radioNone = new System.Windows.Forms.RadioButton();
            this.radioInteger = new System.Windows.Forms.RadioButton();
            this.radioTables = new System.Windows.Forms.RadioButton();
            this.btnGenCode = new System.Windows.Forms.Button();
            this.chkNoNullableTypes = new System.Windows.Forms.CheckBox();
            this.chkMySql = new System.Windows.Forms.CheckBox();
            this.grpRetType.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblSignature
            // 
            this.lblSignature.AutoSize = true;
            this.lblSignature.Location = new System.Drawing.Point(10, 9);
            this.lblSignature.Name = "lblSignature";
            this.lblSignature.Size = new System.Drawing.Size(138, 13);
            this.lblSignature.TabIndex = 0;
            this.lblSignature.Text = "Stored Procedure Signature";
            // 
            // txtSignature
            // 
            this.txtSignature.Location = new System.Drawing.Point(12, 25);
            this.txtSignature.Multiline = true;
            this.txtSignature.Name = "txtSignature";
            this.txtSignature.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtSignature.Size = new System.Drawing.Size(481, 136);
            this.txtSignature.TabIndex = 1;
            // 
            // lblMethodName
            // 
            this.lblMethodName.AutoSize = true;
            this.lblMethodName.Location = new System.Drawing.Point(10, 183);
            this.lblMethodName.Name = "lblMethodName";
            this.lblMethodName.Size = new System.Drawing.Size(77, 13);
            this.lblMethodName.TabIndex = 2;
            this.lblMethodName.Text = "Method Name:";
            // 
            // txtMethodName
            // 
            this.txtMethodName.Location = new System.Drawing.Point(96, 180);
            this.txtMethodName.Name = "txtMethodName";
            this.txtMethodName.Size = new System.Drawing.Size(397, 20);
            this.txtMethodName.TabIndex = 3;
            // 
            // grpRetType
            // 
            this.grpRetType.Controls.Add(this.radioNone);
            this.grpRetType.Controls.Add(this.radioInteger);
            this.grpRetType.Controls.Add(this.radioTables);
            this.grpRetType.Location = new System.Drawing.Point(13, 223);
            this.grpRetType.Name = "grpRetType";
            this.grpRetType.Size = new System.Drawing.Size(114, 119);
            this.grpRetType.TabIndex = 4;
            this.grpRetType.TabStop = false;
            this.grpRetType.Text = "Return Type";
            // 
            // radioNone
            // 
            this.radioNone.AutoSize = true;
            this.radioNone.Location = new System.Drawing.Point(26, 79);
            this.radioNone.Name = "radioNone";
            this.radioNone.Size = new System.Drawing.Size(51, 17);
            this.radioNone.TabIndex = 2;
            this.radioNone.Text = "None";
            this.radioNone.UseVisualStyleBackColor = true;
            // 
            // radioInteger
            // 
            this.radioInteger.AutoSize = true;
            this.radioInteger.Location = new System.Drawing.Point(26, 56);
            this.radioInteger.Name = "radioInteger";
            this.radioInteger.Size = new System.Drawing.Size(58, 17);
            this.radioInteger.TabIndex = 1;
            this.radioInteger.Text = "Integer";
            this.radioInteger.UseVisualStyleBackColor = true;
            // 
            // radioTables
            // 
            this.radioTables.AutoSize = true;
            this.radioTables.Checked = true;
            this.radioTables.Location = new System.Drawing.Point(26, 33);
            this.radioTables.Name = "radioTables";
            this.radioTables.Size = new System.Drawing.Size(57, 17);
            this.radioTables.TabIndex = 0;
            this.radioTables.TabStop = true;
            this.radioTables.Text = "Tables";
            this.radioTables.UseVisualStyleBackColor = true;
            // 
            // btnGenCode
            // 
            this.btnGenCode.Location = new System.Drawing.Point(360, 319);
            this.btnGenCode.Name = "btnGenCode";
            this.btnGenCode.Size = new System.Drawing.Size(133, 23);
            this.btnGenCode.TabIndex = 5;
            this.btnGenCode.Text = "Generate Code";
            this.btnGenCode.UseVisualStyleBackColor = true;
            this.btnGenCode.Click += new System.EventHandler(this.btnGenCode_Click);
            // 
            // chkNoNullableTypes
            // 
            this.chkNoNullableTypes.AutoSize = true;
            this.chkNoNullableTypes.Checked = true;
            this.chkNoNullableTypes.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkNoNullableTypes.Location = new System.Drawing.Point(360, 235);
            this.chkNoNullableTypes.Name = "chkNoNullableTypes";
            this.chkNoNullableTypes.Size = new System.Drawing.Size(113, 17);
            this.chkNoNullableTypes.TabIndex = 6;
            this.chkNoNullableTypes.Text = "No Nullable Types";
            this.chkNoNullableTypes.UseVisualStyleBackColor = true;
            // 
            // chkMySql
            // 
            this.chkMySql.AutoSize = true;
            this.chkMySql.Location = new System.Drawing.Point(360, 259);
            this.chkMySql.Name = "chkMySql";
            this.chkMySql.Size = new System.Drawing.Size(89, 17);
            this.chkMySql.TabIndex = 7;
            this.chkMySql.Text = "MySQL Code";
            this.chkMySql.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(506, 353);
            this.Controls.Add(this.chkMySql);
            this.Controls.Add(this.chkNoNullableTypes);
            this.Controls.Add(this.btnGenCode);
            this.Controls.Add(this.grpRetType);
            this.Controls.Add(this.txtMethodName);
            this.Controls.Add(this.lblMethodName);
            this.Controls.Add(this.txtSignature);
            this.Controls.Add(this.lblSignature);
            this.Name = "Form1";
            this.Text = "Stored Procedure Caller Generator";
            this.grpRetType.ResumeLayout(false);
            this.grpRetType.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblSignature;
        private SPCodeGenForm.NewTextBox txtSignature;
        private System.Windows.Forms.Label lblMethodName;
        private System.Windows.Forms.TextBox txtMethodName;
        private System.Windows.Forms.GroupBox grpRetType;
        private System.Windows.Forms.RadioButton radioNone;
        private System.Windows.Forms.RadioButton radioInteger;
        private System.Windows.Forms.RadioButton radioTables;
        private System.Windows.Forms.Button btnGenCode;
        private System.Windows.Forms.CheckBox chkNoNullableTypes;
        private System.Windows.Forms.CheckBox chkMySql;
    }
}


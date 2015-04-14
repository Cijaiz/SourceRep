namespace AutoScriptExecuter
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
            this.btn_start = new System.Windows.Forms.Button();
            this.btn_exit = new System.Windows.Forms.Button();
            this.lstboxMessage = new System.Windows.Forms.ListBox();
            this.lblRootPath = new System.Windows.Forms.Label();
            this.lblEntryLog = new System.Windows.Forms.Label();
            this.lblConnectionString = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.lblRootpathSelected = new System.Windows.Forms.Label();
            this.lblEntryLogPathSelected = new System.Windows.Forms.Label();
            this.btnSetEntryLogpath = new System.Windows.Forms.Button();
            this.btnSetRootpath = new System.Windows.Forms.Button();
            this.txtConnectionString = new System.Windows.Forms.TextBox();
            this.btnsetConnectionString = new System.Windows.Forms.Button();
            this.btnTestConnection = new System.Windows.Forms.Button();
            this.lblExecutionType = new System.Windows.Forms.Label();
            this.cbxExecutionType = new System.Windows.Forms.ComboBox();
            this.btnLoad = new System.Windows.Forms.Button();
            this.cbxConnectionString = new System.Windows.Forms.ComboBox();
            this.btnLoadCStr = new System.Windows.Forms.Button();
            this.btnDisplay = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btn_start
            // 
            this.btn_start.BackColor = System.Drawing.Color.Silver;
            this.btn_start.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_start.Location = new System.Drawing.Point(376, 193);
            this.btn_start.Name = "btn_start";
            this.btn_start.Size = new System.Drawing.Size(149, 25);
            this.btn_start.TabIndex = 0;
            this.btn_start.Text = "Start Script Executing";
            this.btn_start.UseVisualStyleBackColor = false;
            this.btn_start.Visible = false;
            this.btn_start.Click += new System.EventHandler(this.btn_start_Click);
            // 
            // btn_exit
            // 
            this.btn_exit.BackColor = System.Drawing.Color.Silver;
            this.btn_exit.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_exit.Location = new System.Drawing.Point(650, 193);
            this.btn_exit.Name = "btn_exit";
            this.btn_exit.Size = new System.Drawing.Size(75, 25);
            this.btn_exit.TabIndex = 1;
            this.btn_exit.Text = "Exit";
            this.btn_exit.UseVisualStyleBackColor = false;
            this.btn_exit.Visible = false;
            this.btn_exit.Click += new System.EventHandler(this.btn_exit_Click);
            // 
            // lstboxMessage
            // 
            this.lstboxMessage.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.lstboxMessage.CausesValidation = false;
            this.lstboxMessage.Cursor = System.Windows.Forms.Cursors.No;
            this.lstboxMessage.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.lstboxMessage.ForeColor = System.Drawing.Color.DarkRed;
            this.lstboxMessage.FormattingEnabled = true;
            this.lstboxMessage.HorizontalScrollbar = true;
            this.lstboxMessage.Location = new System.Drawing.Point(8, 222);
            this.lstboxMessage.Name = "lstboxMessage";
            this.lstboxMessage.Size = new System.Drawing.Size(1029, 193);
            this.lstboxMessage.TabIndex = 2;
            this.lstboxMessage.Visible = false;
            this.lstboxMessage.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.lstboxMessage_DrawItem);
            // 
            // lblRootPath
            // 
            this.lblRootPath.AutoSize = true;
            this.lblRootPath.BackColor = System.Drawing.Color.LightSteelBlue;
            this.lblRootPath.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblRootPath.Location = new System.Drawing.Point(13, 51);
            this.lblRootPath.Name = "lblRootPath";
            this.lblRootPath.Size = new System.Drawing.Size(101, 13);
            this.lblRootPath.TabIndex = 3;
            this.lblRootPath.Text = "Script Root Path";
            // 
            // lblEntryLog
            // 
            this.lblEntryLog.AutoSize = true;
            this.lblEntryLog.BackColor = System.Drawing.Color.LightSteelBlue;
            this.lblEntryLog.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblEntryLog.Location = new System.Drawing.Point(13, 89);
            this.lblEntryLog.Name = "lblEntryLog";
            this.lblEntryLog.Size = new System.Drawing.Size(91, 13);
            this.lblEntryLog.TabIndex = 4;
            this.lblEntryLog.Text = "Audit Log Path";
            // 
            // lblConnectionString
            // 
            this.lblConnectionString.AutoSize = true;
            this.lblConnectionString.BackColor = System.Drawing.Color.LightSteelBlue;
            this.lblConnectionString.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblConnectionString.Location = new System.Drawing.Point(13, 134);
            this.lblConnectionString.Name = "lblConnectionString";
            this.lblConnectionString.Size = new System.Drawing.Size(61, 13);
            this.lblConnectionString.TabIndex = 5;
            this.lblConnectionString.Text = "Database";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(-161, -96);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(269, 20);
            this.textBox1.TabIndex = 6;
            // 
            // lblRootpathSelected
            // 
            this.lblRootpathSelected.AutoSize = true;
            this.lblRootpathSelected.BackColor = System.Drawing.Color.LightSteelBlue;
            this.lblRootpathSelected.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblRootpathSelected.Location = new System.Drawing.Point(245, 54);
            this.lblRootpathSelected.Name = "lblRootpathSelected";
            this.lblRootpathSelected.Size = new System.Drawing.Size(0, 13);
            this.lblRootpathSelected.TabIndex = 8;
            // 
            // lblEntryLogPathSelected
            // 
            this.lblEntryLogPathSelected.AutoSize = true;
            this.lblEntryLogPathSelected.BackColor = System.Drawing.Color.LightSteelBlue;
            this.lblEntryLogPathSelected.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblEntryLogPathSelected.Location = new System.Drawing.Point(245, 94);
            this.lblEntryLogPathSelected.Name = "lblEntryLogPathSelected";
            this.lblEntryLogPathSelected.Size = new System.Drawing.Size(0, 13);
            this.lblEntryLogPathSelected.TabIndex = 9;
            // 
            // btnSetEntryLogpath
            // 
            this.btnSetEntryLogpath.BackColor = System.Drawing.Color.Silver;
            this.btnSetEntryLogpath.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSetEntryLogpath.Location = new System.Drawing.Point(144, 89);
            this.btnSetEntryLogpath.Name = "btnSetEntryLogpath";
            this.btnSetEntryLogpath.Size = new System.Drawing.Size(75, 25);
            this.btnSetEntryLogpath.TabIndex = 10;
            this.btnSetEntryLogpath.Text = "Set";
            this.btnSetEntryLogpath.UseVisualStyleBackColor = false;
            this.btnSetEntryLogpath.Click += new System.EventHandler(this.btnSetEntryLogpath_Click);
            // 
            // btnSetRootpath
            // 
            this.btnSetRootpath.BackColor = System.Drawing.Color.Silver;
            this.btnSetRootpath.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSetRootpath.Location = new System.Drawing.Point(144, 51);
            this.btnSetRootpath.Name = "btnSetRootpath";
            this.btnSetRootpath.Size = new System.Drawing.Size(75, 25);
            this.btnSetRootpath.TabIndex = 11;
            this.btnSetRootpath.Text = "Set";
            this.btnSetRootpath.UseVisualStyleBackColor = false;
            this.btnSetRootpath.Click += new System.EventHandler(this.btnSetRootpath_Click);
            // 
            // txtConnectionString
            // 
            this.txtConnectionString.BackColor = System.Drawing.Color.Azure;
            this.txtConnectionString.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtConnectionString.Location = new System.Drawing.Point(237, 131);
            this.txtConnectionString.Name = "txtConnectionString";
            this.txtConnectionString.Size = new System.Drawing.Size(800, 20);
            this.txtConnectionString.TabIndex = 13;
            this.txtConnectionString.TextChanged += new System.EventHandler(this.txtConnectionString_TextChanged);
            // 
            // btnsetConnectionString
            // 
            this.btnsetConnectionString.BackColor = System.Drawing.Color.Silver;
            this.btnsetConnectionString.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnsetConnectionString.Location = new System.Drawing.Point(143, 130);
            this.btnsetConnectionString.Name = "btnsetConnectionString";
            this.btnsetConnectionString.Size = new System.Drawing.Size(75, 25);
            this.btnsetConnectionString.TabIndex = 14;
            this.btnsetConnectionString.Text = "Set";
            this.btnsetConnectionString.UseVisualStyleBackColor = false;
            this.btnsetConnectionString.Click += new System.EventHandler(this.btnsetConnectionString_Click);
            // 
            // btnTestConnection
            // 
            this.btnTestConnection.BackColor = System.Drawing.Color.Silver;
            this.btnTestConnection.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnTestConnection.Location = new System.Drawing.Point(143, 193);
            this.btnTestConnection.Name = "btnTestConnection";
            this.btnTestConnection.Size = new System.Drawing.Size(162, 25);
            this.btnTestConnection.TabIndex = 15;
            this.btnTestConnection.Text = "Test Connection";
            this.btnTestConnection.UseVisualStyleBackColor = false;
            this.btnTestConnection.Click += new System.EventHandler(this.btnTestConnection_Click);
            // 
            // lblExecutionType
            // 
            this.lblExecutionType.AutoSize = true;
            this.lblExecutionType.BackColor = System.Drawing.Color.LightSteelBlue;
            this.lblExecutionType.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblExecutionType.Location = new System.Drawing.Point(13, 15);
            this.lblExecutionType.Name = "lblExecutionType";
            this.lblExecutionType.Size = new System.Drawing.Size(99, 13);
            this.lblExecutionType.TabIndex = 16;
            this.lblExecutionType.Text = "Execution Type ";
            // 
            // cbxExecutionType
            // 
            this.cbxExecutionType.BackColor = System.Drawing.Color.Azure;
            this.cbxExecutionType.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbxExecutionType.FormattingEnabled = true;
            this.cbxExecutionType.Location = new System.Drawing.Point(143, 12);
            this.cbxExecutionType.Name = "cbxExecutionType";
            this.cbxExecutionType.Size = new System.Drawing.Size(121, 21);
            this.cbxExecutionType.TabIndex = 17;
            // 
            // btnLoad
            // 
            this.btnLoad.BackColor = System.Drawing.Color.Silver;
            this.btnLoad.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnLoad.Location = new System.Drawing.Point(282, 8);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(75, 25);
            this.btnLoad.TabIndex = 18;
            this.btnLoad.Text = "Load";
            this.btnLoad.UseVisualStyleBackColor = false;
            this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
            // 
            // cbxConnectionString
            // 
            this.cbxConnectionString.BackColor = System.Drawing.Color.Azure;
            this.cbxConnectionString.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbxConnectionString.FormattingEnabled = true;
            this.cbxConnectionString.Location = new System.Drawing.Point(237, 157);
            this.cbxConnectionString.Name = "cbxConnectionString";
            this.cbxConnectionString.Size = new System.Drawing.Size(800, 21);
            this.cbxConnectionString.TabIndex = 19;
            this.cbxConnectionString.SelectedIndexChanged += new System.EventHandler(this.cbxConnectionString_SelectedIndexChanged);
            // 
            // btnLoadCStr
            // 
            this.btnLoadCStr.BackColor = System.Drawing.Color.Silver;
            this.btnLoadCStr.Enabled = false;
            this.btnLoadCStr.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnLoadCStr.Location = new System.Drawing.Point(144, 157);
            this.btnLoadCStr.Name = "btnLoadCStr";
            this.btnLoadCStr.Size = new System.Drawing.Size(75, 25);
            this.btnLoadCStr.TabIndex = 20;
            this.btnLoadCStr.Text = "Set";
            this.btnLoadCStr.UseVisualStyleBackColor = false;
            this.btnLoadCStr.Click += new System.EventHandler(this.btnLoadCStr_Click);
            // 
            // btnDisplay
            // 
            this.btnDisplay.BackColor = System.Drawing.Color.Silver;
            this.btnDisplay.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDisplay.Location = new System.Drawing.Point(409, 8);
            this.btnDisplay.Name = "btnDisplay";
            this.btnDisplay.Size = new System.Drawing.Size(194, 25);
            this.btnDisplay.TabIndex = 21;
            this.btnDisplay.Text = "Cogfigure Script Details";
            this.btnDisplay.UseVisualStyleBackColor = false;
            this.btnDisplay.Click += new System.EventHandler(this.btnDisplay_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.LightSteelBlue;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(13, 149);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(108, 13);
            this.label1.TabIndex = 22;
            this.label1.Text = "Connection String";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.LightSteelBlue;
            this.ClientSize = new System.Drawing.Size(1049, 419);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnDisplay);
            this.Controls.Add(this.btnLoadCStr);
            this.Controls.Add(this.cbxConnectionString);
            this.Controls.Add(this.btnLoad);
            this.Controls.Add(this.cbxExecutionType);
            this.Controls.Add(this.lblExecutionType);
            this.Controls.Add(this.btnTestConnection);
            this.Controls.Add(this.btnsetConnectionString);
            this.Controls.Add(this.txtConnectionString);
            this.Controls.Add(this.btnSetRootpath);
            this.Controls.Add(this.btnSetEntryLogpath);
            this.Controls.Add(this.lblEntryLogPathSelected);
            this.Controls.Add(this.lblRootpathSelected);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.lblConnectionString);
            this.Controls.Add(this.lblEntryLog);
            this.Controls.Add(this.lblRootPath);
            this.Controls.Add(this.lstboxMessage);
            this.Controls.Add(this.btn_exit);
            this.Controls.Add(this.btn_start);
            this.Name = "Form1";
            this.Text = "Auto Script Executer";
            this.TransparencyKey = System.Drawing.Color.White;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btn_start;
        private System.Windows.Forms.Button btn_exit;
        private System.Windows.Forms.ListBox lstboxMessage;
        private System.Windows.Forms.Label lblRootPath;
        private System.Windows.Forms.Label lblEntryLog;
        private System.Windows.Forms.Label lblConnectionString;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label lblRootpathSelected;
        private System.Windows.Forms.Label lblEntryLogPathSelected;
        private System.Windows.Forms.Button btnSetEntryLogpath;
        private System.Windows.Forms.Button btnSetRootpath;
        private System.Windows.Forms.TextBox txtConnectionString;
        private System.Windows.Forms.Button btnsetConnectionString;
        private System.Windows.Forms.Button btnTestConnection;
        private System.Windows.Forms.Label lblExecutionType;
        private System.Windows.Forms.ComboBox cbxExecutionType;
        private System.Windows.Forms.Button btnLoad;
        private System.Windows.Forms.ComboBox cbxConnectionString;
        private System.Windows.Forms.Button btnLoadCStr;
        private System.Windows.Forms.Button btnDisplay;
        private System.Windows.Forms.Label label1;
    }
}


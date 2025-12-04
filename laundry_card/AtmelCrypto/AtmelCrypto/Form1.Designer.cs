namespace AtmelCrypto
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.readerComboBox = new System.Windows.Forms.ComboBox();
            this.readerSelectBtn = new System.Windows.Forms.Button();
            this.consoleTB = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.cardTB = new System.Windows.Forms.TextBox();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.groupBox14 = new System.Windows.Forms.GroupBox();
            this.pr3TB = new System.Windows.Forms.TextBox();
            this.groupBox15 = new System.Windows.Forms.GroupBox();
            this.ar3TB = new System.Windows.Forms.TextBox();
            this.groupBox16 = new System.Windows.Forms.GroupBox();
            this.pr2TB = new System.Windows.Forms.TextBox();
            this.groupBox17 = new System.Windows.Forms.GroupBox();
            this.ar2TB = new System.Windows.Forms.TextBox();
            this.groupBox12 = new System.Windows.Forms.GroupBox();
            this.pr1TB = new System.Windows.Forms.TextBox();
            this.groupBox13 = new System.Windows.Forms.GroupBox();
            this.ar1TB = new System.Windows.Forms.TextBox();
            this.groupBox11 = new System.Windows.Forms.GroupBox();
            this.pr0TB = new System.Windows.Forms.TextBox();
            this.groupBox10 = new System.Windows.Forms.GroupBox();
            this.ar0TB = new System.Windows.Forms.TextBox();
            this.writeConfigBtn = new System.Windows.Forms.Button();
            this.groupBox9 = new System.Windows.Forms.GroupBox();
            this.fuseTB = new System.Windows.Forms.TextBox();
            this.groupBox8 = new System.Windows.Forms.GroupBox();
            this.dcrTB = new System.Windows.Forms.TextBox();
            this.readConfigBtn = new System.Windows.Forms.Button();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.write7AttemptTB = new System.Windows.Forms.TextBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.write7passTB = new System.Windows.Forms.TextBox();
            this.groupBox26 = new System.Windows.Forms.GroupBox();
            this.writeUserPage = new System.Windows.Forms.Button();
            this.readUserPageBtn = new System.Windows.Forms.Button();
            this.macroDumpDevBtn = new System.Windows.Forms.Button();
            this.macroRestoreBtn = new System.Windows.Forms.Button();
            this.macroBackupBtn = new System.Windows.Forms.Button();
            this.folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox7.SuspendLayout();
            this.groupBox14.SuspendLayout();
            this.groupBox15.SuspendLayout();
            this.groupBox16.SuspendLayout();
            this.groupBox17.SuspendLayout();
            this.groupBox12.SuspendLayout();
            this.groupBox13.SuspendLayout();
            this.groupBox11.SuspendLayout();
            this.groupBox10.SuspendLayout();
            this.groupBox9.SuspendLayout();
            this.groupBox8.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox26.SuspendLayout();
            this.SuspendLayout();
            //
            // readerComboBox
            //
            this.readerComboBox.FormattingEnabled = true;
            this.readerComboBox.Location = new System.Drawing.Point(6, 19);
            this.readerComboBox.Name = "readerComboBox";
            this.readerComboBox.Size = new System.Drawing.Size(222, 21);
            this.readerComboBox.TabIndex = 1;
            //
            // readerSelectBtn
            //
            this.readerSelectBtn.Location = new System.Drawing.Point(234, 19);
            this.readerSelectBtn.Name = "readerSelectBtn";
            this.readerSelectBtn.Size = new System.Drawing.Size(57, 22);
            this.readerSelectBtn.TabIndex = 2;
            this.readerSelectBtn.Text = "Select";
            this.readerSelectBtn.UseVisualStyleBackColor = true;
            this.readerSelectBtn.Click += new System.EventHandler(this.readerSelectBtn_Click);
            //
            // consoleTB
            //
            this.consoleTB.Location = new System.Drawing.Point(12, 264);
            this.consoleTB.Multiline = true;
            this.consoleTB.Name = "consoleTB";
            this.consoleTB.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.consoleTB.Size = new System.Drawing.Size(547, 271);
            this.consoleTB.TabIndex = 3;
            //
            // groupBox1
            //
            this.groupBox1.Controls.Add(this.readerComboBox);
            this.groupBox1.Controls.Add(this.readerSelectBtn);
            this.groupBox1.Location = new System.Drawing.Point(13, 10);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(297, 56);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Smart Card Reader";
            //
            // groupBox2
            //
            this.groupBox2.Controls.Add(this.cardTB);
            this.groupBox2.Location = new System.Drawing.Point(316, 10);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(243, 56);
            this.groupBox2.TabIndex = 5;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Card";
            //
            // cardTB
            //
            this.cardTB.Location = new System.Drawing.Point(6, 19);
            this.cardTB.Name = "cardTB";
            this.cardTB.ReadOnly = true;
            this.cardTB.Size = new System.Drawing.Size(221, 20);
            this.cardTB.TabIndex = 0;
            //
            // groupBox7
            //
            this.groupBox7.Controls.Add(this.groupBox14);
            this.groupBox7.Controls.Add(this.groupBox15);
            this.groupBox7.Controls.Add(this.groupBox16);
            this.groupBox7.Controls.Add(this.groupBox17);
            this.groupBox7.Controls.Add(this.groupBox12);
            this.groupBox7.Controls.Add(this.groupBox13);
            this.groupBox7.Controls.Add(this.groupBox11);
            this.groupBox7.Controls.Add(this.groupBox10);
            this.groupBox7.Controls.Add(this.writeConfigBtn);
            this.groupBox7.Controls.Add(this.groupBox9);
            this.groupBox7.Controls.Add(this.groupBox8);
            this.groupBox7.Controls.Add(this.readConfigBtn);
            this.groupBox7.Controls.Add(this.groupBox6);
            this.groupBox7.Controls.Add(this.groupBox4);
            this.groupBox7.Location = new System.Drawing.Point(13, 72);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.Size = new System.Drawing.Size(360, 186);
            this.groupBox7.TabIndex = 12;
            this.groupBox7.TabStop = false;
            this.groupBox7.Text = "Config";
            //
            // groupBox14
            //
            this.groupBox14.Controls.Add(this.pr3TB);
            this.groupBox14.Location = new System.Drawing.Point(177, 116);
            this.groupBox14.Name = "groupBox14";
            this.groupBox14.Size = new System.Drawing.Size(51, 42);
            this.groupBox14.TabIndex = 17;
            this.groupBox14.TabStop = false;
            this.groupBox14.Text = "PR3";
            //
            // pr3TB
            //
            this.pr3TB.Location = new System.Drawing.Point(6, 16);
            this.pr3TB.Name = "pr3TB";
            this.pr3TB.ReadOnly = true;
            this.pr3TB.Size = new System.Drawing.Size(37, 20);
            this.pr3TB.TabIndex = 0;
            //
            // groupBox15
            //
            this.groupBox15.Controls.Add(this.ar3TB);
            this.groupBox15.Location = new System.Drawing.Point(177, 68);
            this.groupBox15.Name = "groupBox15";
            this.groupBox15.Size = new System.Drawing.Size(51, 42);
            this.groupBox15.TabIndex = 16;
            this.groupBox15.TabStop = false;
            this.groupBox15.Text = "AR3";
            //
            // ar3TB
            //
            this.ar3TB.Location = new System.Drawing.Point(6, 16);
            this.ar3TB.Name = "ar3TB";
            this.ar3TB.ReadOnly = true;
            this.ar3TB.Size = new System.Drawing.Size(37, 20);
            this.ar3TB.TabIndex = 0;
            //
            // groupBox16
            //
            this.groupBox16.Controls.Add(this.pr2TB);
            this.groupBox16.Location = new System.Drawing.Point(120, 116);
            this.groupBox16.Name = "groupBox16";
            this.groupBox16.Size = new System.Drawing.Size(51, 42);
            this.groupBox16.TabIndex = 15;
            this.groupBox16.TabStop = false;
            this.groupBox16.Text = "PR2";
            //
            // pr2TB
            //
            this.pr2TB.Location = new System.Drawing.Point(6, 16);
            this.pr2TB.Name = "pr2TB";
            this.pr2TB.ReadOnly = true;
            this.pr2TB.Size = new System.Drawing.Size(37, 20);
            this.pr2TB.TabIndex = 0;
            //
            // groupBox17
            //
            this.groupBox17.Controls.Add(this.ar2TB);
            this.groupBox17.Location = new System.Drawing.Point(120, 68);
            this.groupBox17.Name = "groupBox17";
            this.groupBox17.Size = new System.Drawing.Size(51, 42);
            this.groupBox17.TabIndex = 14;
            this.groupBox17.TabStop = false;
            this.groupBox17.Text = "AR2";
            //
            // ar2TB
            //
            this.ar2TB.Location = new System.Drawing.Point(6, 16);
            this.ar2TB.Name = "ar2TB";
            this.ar2TB.ReadOnly = true;
            this.ar2TB.Size = new System.Drawing.Size(37, 20);
            this.ar2TB.TabIndex = 0;
            //
            // groupBox12
            //
            this.groupBox12.Controls.Add(this.pr1TB);
            this.groupBox12.Location = new System.Drawing.Point(63, 116);
            this.groupBox12.Name = "groupBox12";
            this.groupBox12.Size = new System.Drawing.Size(51, 42);
            this.groupBox12.TabIndex = 13;
            this.groupBox12.TabStop = false;
            this.groupBox12.Text = "PR1";
            //
            // pr1TB
            //
            this.pr1TB.Location = new System.Drawing.Point(6, 16);
            this.pr1TB.Name = "pr1TB";
            this.pr1TB.ReadOnly = true;
            this.pr1TB.Size = new System.Drawing.Size(37, 20);
            this.pr1TB.TabIndex = 0;
            //
            // groupBox13
            //
            this.groupBox13.Controls.Add(this.ar1TB);
            this.groupBox13.Location = new System.Drawing.Point(63, 68);
            this.groupBox13.Name = "groupBox13";
            this.groupBox13.Size = new System.Drawing.Size(51, 42);
            this.groupBox13.TabIndex = 12;
            this.groupBox13.TabStop = false;
            this.groupBox13.Text = "AR1";
            //
            // ar1TB
            //
            this.ar1TB.Location = new System.Drawing.Point(6, 16);
            this.ar1TB.Name = "ar1TB";
            this.ar1TB.ReadOnly = true;
            this.ar1TB.Size = new System.Drawing.Size(37, 20);
            this.ar1TB.TabIndex = 0;
            //
            // groupBox11
            //
            this.groupBox11.Controls.Add(this.pr0TB);
            this.groupBox11.Location = new System.Drawing.Point(6, 116);
            this.groupBox11.Name = "groupBox11";
            this.groupBox11.Size = new System.Drawing.Size(51, 42);
            this.groupBox11.TabIndex = 10;
            this.groupBox11.TabStop = false;
            this.groupBox11.Text = "PR0";
            //
            // pr0TB
            //
            this.pr0TB.Location = new System.Drawing.Point(6, 16);
            this.pr0TB.Name = "pr0TB";
            this.pr0TB.ReadOnly = true;
            this.pr0TB.Size = new System.Drawing.Size(37, 20);
            this.pr0TB.TabIndex = 0;
            //
            // groupBox10
            //
            this.groupBox10.Controls.Add(this.ar0TB);
            this.groupBox10.Location = new System.Drawing.Point(6, 68);
            this.groupBox10.Name = "groupBox10";
            this.groupBox10.Size = new System.Drawing.Size(51, 42);
            this.groupBox10.TabIndex = 9;
            this.groupBox10.TabStop = false;
            this.groupBox10.Text = "AR0";
            //
            // ar0TB
            //
            this.ar0TB.Location = new System.Drawing.Point(6, 16);
            this.ar0TB.Name = "ar0TB";
            this.ar0TB.ReadOnly = true;
            this.ar0TB.Size = new System.Drawing.Size(37, 20);
            this.ar0TB.TabIndex = 0;
            //
            // writeConfigBtn
            //
            this.writeConfigBtn.Location = new System.Drawing.Point(245, 128);
            this.writeConfigBtn.Name = "writeConfigBtn";
            this.writeConfigBtn.Size = new System.Drawing.Size(89, 27);
            this.writeConfigBtn.TabIndex = 8;
            this.writeConfigBtn.Text = "Write Config";
            this.writeConfigBtn.UseVisualStyleBackColor = true;
            this.writeConfigBtn.Click += new System.EventHandler(this.writeConfigBtn_Click);
            //
            // groupBox9
            //
            this.groupBox9.Controls.Add(this.fuseTB);
            this.groupBox9.Location = new System.Drawing.Point(291, 20);
            this.groupBox9.Name = "groupBox9";
            this.groupBox9.Size = new System.Drawing.Size(51, 42);
            this.groupBox9.TabIndex = 5;
            this.groupBox9.TabStop = false;
            this.groupBox9.Text = "FUSE";
            //
            // fuseTB
            //
            this.fuseTB.Location = new System.Drawing.Point(6, 16);
            this.fuseTB.Name = "fuseTB";
            this.fuseTB.ReadOnly = true;
            this.fuseTB.Size = new System.Drawing.Size(37, 20);
            this.fuseTB.TabIndex = 0;
            //
            // groupBox8
            //
            this.groupBox8.Controls.Add(this.dcrTB);
            this.groupBox8.Location = new System.Drawing.Point(234, 20);
            this.groupBox8.Name = "groupBox8";
            this.groupBox8.Size = new System.Drawing.Size(51, 42);
            this.groupBox8.TabIndex = 5;
            this.groupBox8.TabStop = false;
            this.groupBox8.Text = "DCR";
            //
            // dcrTB
            //
            this.dcrTB.Location = new System.Drawing.Point(6, 16);
            this.dcrTB.Name = "dcrTB";
            this.dcrTB.ReadOnly = true;
            this.dcrTB.Size = new System.Drawing.Size(37, 20);
            this.dcrTB.TabIndex = 0;
            //
            // readConfigBtn
            //
            this.readConfigBtn.Location = new System.Drawing.Point(245, 80);
            this.readConfigBtn.Name = "readConfigBtn";
            this.readConfigBtn.Size = new System.Drawing.Size(89, 26);
            this.readConfigBtn.TabIndex = 7;
            this.readConfigBtn.Text = "Read Config";
            this.readConfigBtn.UseVisualStyleBackColor = true;
            this.readConfigBtn.Click += new System.EventHandler(this.readConfigBtn_Click);
            //
            // groupBox6
            //
            this.groupBox6.Controls.Add(this.write7AttemptTB);
            this.groupBox6.Location = new System.Drawing.Point(120, 20);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(96, 42);
            this.groupBox6.TabIndex = 4;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "Write7 Attempts";
            //
            // write7AttemptTB
            //
            this.write7AttemptTB.Location = new System.Drawing.Point(10, 16);
            this.write7AttemptTB.Name = "write7AttemptTB";
            this.write7AttemptTB.ReadOnly = true;
            this.write7AttemptTB.Size = new System.Drawing.Size(80, 20);
            this.write7AttemptTB.TabIndex = 0;
            //
            // groupBox4
            //
            this.groupBox4.Controls.Add(this.write7passTB);
            this.groupBox4.Location = new System.Drawing.Point(6, 19);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(108, 43);
            this.groupBox4.TabIndex = 2;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Write7 Password";
            //
            // write7passTB
            //
            this.write7passTB.Location = new System.Drawing.Point(6, 17);
            this.write7passTB.MaxLength = 6;
            this.write7passTB.Name = "write7passTB";
            this.write7passTB.Size = new System.Drawing.Size(94, 20);
            this.write7passTB.TabIndex = 0;
            //
            // groupBox26
            //
            this.groupBox26.Controls.Add(this.writeUserPage);
            this.groupBox26.Controls.Add(this.readUserPageBtn);
            this.groupBox26.Controls.Add(this.macroDumpDevBtn);
            this.groupBox26.Controls.Add(this.macroRestoreBtn);
            this.groupBox26.Controls.Add(this.macroBackupBtn);
            this.groupBox26.Location = new System.Drawing.Point(388, 72);
            this.groupBox26.Name = "groupBox26";
            this.groupBox26.Size = new System.Drawing.Size(171, 186);
            this.groupBox26.TabIndex = 27;
            this.groupBox26.TabStop = false;
            this.groupBox26.Text = "Macros";
            //
            // writeUserPage
            //
            this.writeUserPage.Location = new System.Drawing.Point(16, 150);
            this.writeUserPage.Name = "writeUserPage";
            this.writeUserPage.Size = new System.Drawing.Size(137, 25);
            this.writeUserPage.TabIndex = 4;
            this.writeUserPage.Text = "Write User Page";
            this.writeUserPage.UseVisualStyleBackColor = true;
            this.writeUserPage.Click += new System.EventHandler(this.writeUserPage_Click);
            //
            // readUserPageBtn
            //
            this.readUserPageBtn.Location = new System.Drawing.Point(16, 116);
            this.readUserPageBtn.Name = "readUserPageBtn";
            this.readUserPageBtn.Size = new System.Drawing.Size(137, 25);
            this.readUserPageBtn.TabIndex = 3;
            this.readUserPageBtn.Text = "Read User Page";
            this.readUserPageBtn.UseVisualStyleBackColor = true;
            this.readUserPageBtn.Click += new System.EventHandler(this.readUserPageBtn_Click);
            //
            // macroDumpDevBtn
            //
            this.macroDumpDevBtn.Location = new System.Drawing.Point(16, 85);
            this.macroDumpDevBtn.Name = "macroDumpDevBtn";
            this.macroDumpDevBtn.Size = new System.Drawing.Size(137, 25);
            this.macroDumpDevBtn.TabIndex = 2;
            this.macroDumpDevBtn.Text = "Dump Config";
            this.macroDumpDevBtn.UseVisualStyleBackColor = true;
            this.macroDumpDevBtn.Click += new System.EventHandler(this.macroDumpDevBtn_Click);
            //
            // macroRestoreBtn
            //
            this.macroRestoreBtn.Location = new System.Drawing.Point(16, 52);
            this.macroRestoreBtn.Name = "macroRestoreBtn";
            this.macroRestoreBtn.Size = new System.Drawing.Size(137, 25);
            this.macroRestoreBtn.TabIndex = 1;
            this.macroRestoreBtn.Text = "Restore Device";
            this.macroRestoreBtn.UseVisualStyleBackColor = true;
            this.macroRestoreBtn.Click += new System.EventHandler(this.macroRestoreBtn_Click);
            //
            // macroBackupBtn
            //
            this.macroBackupBtn.Location = new System.Drawing.Point(18, 21);
            this.macroBackupBtn.Name = "macroBackupBtn";
            this.macroBackupBtn.Size = new System.Drawing.Size(137, 25);
            this.macroBackupBtn.TabIndex = 0;
            this.macroBackupBtn.Text = "Backup Device";
            this.macroBackupBtn.UseVisualStyleBackColor = true;
            this.macroBackupBtn.Click += new System.EventHandler(this.macroBackupBtn_Click);
            //
            // Form1
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(572, 547);
            this.Controls.Add(this.groupBox26);
            this.Controls.Add(this.groupBox7);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.consoleTB);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.Text = "Atmel CryptoMemory Utility";
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox7.ResumeLayout(false);
            this.groupBox14.ResumeLayout(false);
            this.groupBox14.PerformLayout();
            this.groupBox15.ResumeLayout(false);
            this.groupBox15.PerformLayout();
            this.groupBox16.ResumeLayout(false);
            this.groupBox16.PerformLayout();
            this.groupBox17.ResumeLayout(false);
            this.groupBox17.PerformLayout();
            this.groupBox12.ResumeLayout(false);
            this.groupBox12.PerformLayout();
            this.groupBox13.ResumeLayout(false);
            this.groupBox13.PerformLayout();
            this.groupBox11.ResumeLayout(false);
            this.groupBox11.PerformLayout();
            this.groupBox10.ResumeLayout(false);
            this.groupBox10.PerformLayout();
            this.groupBox9.ResumeLayout(false);
            this.groupBox9.PerformLayout();
            this.groupBox8.ResumeLayout(false);
            this.groupBox8.PerformLayout();
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox26.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox readerComboBox;
        private System.Windows.Forms.Button readerSelectBtn;
        private System.Windows.Forms.TextBox consoleTB;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox cardTB;
        private System.Windows.Forms.GroupBox groupBox7;
        private System.Windows.Forms.Button readConfigBtn;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.TextBox write7AttemptTB;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.TextBox write7passTB;
        private System.Windows.Forms.Button writeConfigBtn;
        private System.Windows.Forms.GroupBox groupBox9;
        private System.Windows.Forms.TextBox fuseTB;
        private System.Windows.Forms.GroupBox groupBox8;
        private System.Windows.Forms.TextBox dcrTB;
        private System.Windows.Forms.GroupBox groupBox10;
        private System.Windows.Forms.TextBox ar0TB;
        private System.Windows.Forms.GroupBox groupBox11;
        private System.Windows.Forms.TextBox pr0TB;
        private System.Windows.Forms.GroupBox groupBox14;
        private System.Windows.Forms.TextBox pr3TB;
        private System.Windows.Forms.GroupBox groupBox15;
        private System.Windows.Forms.TextBox ar3TB;
        private System.Windows.Forms.GroupBox groupBox16;
        private System.Windows.Forms.TextBox pr2TB;
        private System.Windows.Forms.GroupBox groupBox17;
        private System.Windows.Forms.TextBox ar2TB;
        private System.Windows.Forms.GroupBox groupBox12;
        private System.Windows.Forms.TextBox pr1TB;
        private System.Windows.Forms.GroupBox groupBox13;
        private System.Windows.Forms.TextBox ar1TB;
        private System.Windows.Forms.GroupBox groupBox26;
        private System.Windows.Forms.Button macroDumpDevBtn;
        private System.Windows.Forms.Button macroRestoreBtn;
        private System.Windows.Forms.Button macroBackupBtn;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog;
        private System.Windows.Forms.Button writeUserPage;
        private System.Windows.Forms.Button readUserPageBtn;
    }
}


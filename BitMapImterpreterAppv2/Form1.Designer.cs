namespace BitMapImterpreterAppv2
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            OpenFile = new Button();
            MainTextBox = new TextBox();
            Run = new Button();
            ParseInstructions = new Button();
            SaveFile = new Button();
            CurrCBName = new Label();
            openFileDialog1 = new OpenFileDialog();
            saveFileDialog1 = new SaveFileDialog();
            Status = new Label();
            SuspendLayout();
            // 
            // OpenFile
            // 
            OpenFile.Location = new Point(713, 12);
            OpenFile.Name = "OpenFile";
            OpenFile.Size = new Size(75, 23);
            OpenFile.TabIndex = 0;
            OpenFile.Text = "Open File";
            OpenFile.UseVisualStyleBackColor = true;
            OpenFile.Click += OpenFile_Click;
            // 
            // MainTextBox
            // 
            MainTextBox.AcceptsReturn = true;
            MainTextBox.AcceptsTab = true;
            MainTextBox.AllowDrop = true;
            MainTextBox.Location = new Point(12, 41);
            MainTextBox.Multiline = true;
            MainTextBox.Name = "MainTextBox";
            MainTextBox.Size = new Size(695, 397);
            MainTextBox.TabIndex = 1;
            // 
            // Run
            // 
            Run.Location = new Point(713, 70);
            Run.Name = "Run";
            Run.Size = new Size(75, 23);
            Run.TabIndex = 2;
            Run.Text = "Run";
            Run.UseVisualStyleBackColor = true;
            Run.Click += Run_Click;
            // 
            // ParseInstructions
            // 
            ParseInstructions.Location = new Point(713, 99);
            ParseInstructions.Name = "ParseInstructions";
            ParseInstructions.Size = new Size(75, 40);
            ParseInstructions.TabIndex = 3;
            ParseInstructions.Text = "Parse Instructions";
            ParseInstructions.UseVisualStyleBackColor = true;
            ParseInstructions.Click += ParseInstructions_Click;
            // 
            // SaveFile
            // 
            SaveFile.Location = new Point(713, 41);
            SaveFile.Name = "SaveFile";
            SaveFile.Size = new Size(75, 23);
            SaveFile.TabIndex = 4;
            SaveFile.Text = "Save File";
            SaveFile.UseVisualStyleBackColor = true;
            SaveFile.Click += SaveFile_Click;
            // 
            // CurrCBName
            // 
            CurrCBName.AutoSize = true;
            CurrCBName.Location = new Point(12, 9);
            CurrCBName.Name = "CurrCBName";
            CurrCBName.Size = new Size(0, 15);
            CurrCBName.TabIndex = 5;
            // 
            // openFileDialog1
            // 
            openFileDialog1.DefaultExt = "json";
            openFileDialog1.FileName = "openFileDialog1";
            openFileDialog1.Filter = "Json files (*.json)|*.json";
            // 
            // saveFileDialog1
            // 
            saveFileDialog1.Filter = "Json files (*.json)|*.json";
            // 
            // Status
            // 
            Status.AutoSize = true;
            Status.Location = new Point(713, 142);
            Status.Name = "Status";
            Status.Size = new Size(68, 15);
            Status.TabIndex = 6;
            Status.Text = "NoCompile";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(Status);
            Controls.Add(CurrCBName);
            Controls.Add(SaveFile);
            Controls.Add(ParseInstructions);
            Controls.Add(Run);
            Controls.Add(MainTextBox);
            Controls.Add(OpenFile);
            Name = "Form1";
            Text = "Form1";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button OpenFile;
        private TextBox MainTextBox;
        private Button Run;
        private Button ParseInstructions;
        private Button SaveFile;
        private Label CurrCBName;
        private OpenFileDialog openFileDialog1;
        private SaveFileDialog saveFileDialog1;
        private Label Status;
    }
}
namespace CG3JTluczek
{
    partial class Form1
    {
        /// <summary>
        /// Wymagana zmienna projektanta.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Wyczyść wszystkie używane zasoby.
        /// </summary>
        /// <param name="disposing">prawda, jeżeli zarządzane zasoby powinny zostać zlikwidowane; Fałsz w przeciwnym wypadku.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Kod generowany przez Projektanta formularzy systemu Windows

        /// <summary>
        /// Metoda wymagana do obsługi projektanta — nie należy modyfikować
        /// jej zawartości w edytorze kodu.
        /// </summary>
        private void InitializeComponent()
        {
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.buttonLine = new System.Windows.Forms.Button();
            this.buttonClear = new System.Windows.Forms.Button();
            this.comboLine = new System.Windows.Forms.ComboBox();
            this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
            this.labelThicc = new System.Windows.Forms.Label();
            this.buttonCircle = new System.Windows.Forms.Button();
            this.comboCircle = new System.Windows.Forms.ComboBox();
            this.buttonPoly = new System.Windows.Forms.Button();
            this.comboPoly = new System.Windows.Forms.ComboBox();
            this.labelInfo = new System.Windows.Forms.Label();
            this.labelAliasing = new System.Windows.Forms.Label();
            this.buttonAliasing = new System.Windows.Forms.Button();
            this.buttonCol = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.buttonSave = new System.Windows.Forms.Button();
            this.buttonLoad = new System.Windows.Forms.Button();
            this.buttonCapsule = new System.Windows.Forms.Button();
            this.comboCapsule = new System.Windows.Forms.ComboBox();
            this.buttonRectangle = new System.Windows.Forms.Button();
            this.comboRectangle = new System.Windows.Forms.ComboBox();
            this.buttonFill = new System.Windows.Forms.Button();
            this.buttonFillImg = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.White;
            this.pictureBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBox1.Location = new System.Drawing.Point(12, 12);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(955, 538);
            this.pictureBox1.TabIndex = 2;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseDown);
            // 
            // buttonLine
            // 
            this.buttonLine.Location = new System.Drawing.Point(983, 38);
            this.buttonLine.Name = "buttonLine";
            this.buttonLine.Size = new System.Drawing.Size(75, 23);
            this.buttonLine.TabIndex = 3;
            this.buttonLine.Text = "Line";
            this.buttonLine.UseVisualStyleBackColor = true;
            this.buttonLine.Click += new System.EventHandler(this.buttonLine_Click);
            // 
            // buttonClear
            // 
            this.buttonClear.Location = new System.Drawing.Point(983, 527);
            this.buttonClear.Name = "buttonClear";
            this.buttonClear.Size = new System.Drawing.Size(181, 23);
            this.buttonClear.TabIndex = 4;
            this.buttonClear.Text = "Clear";
            this.buttonClear.UseVisualStyleBackColor = true;
            this.buttonClear.Click += new System.EventHandler(this.buttonClear_Click);
            // 
            // comboLine
            // 
            this.comboLine.AutoCompleteCustomSource.AddRange(new string[] {
            "Draw",
            "Move",
            "Delete"});
            this.comboLine.FormattingEnabled = true;
            this.comboLine.Location = new System.Drawing.Point(1064, 40);
            this.comboLine.Name = "comboLine";
            this.comboLine.Size = new System.Drawing.Size(100, 21);
            this.comboLine.TabIndex = 5;
            // 
            // numericUpDown1
            // 
            this.numericUpDown1.Location = new System.Drawing.Point(1064, 195);
            this.numericUpDown1.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDown1.Name = "numericUpDown1";
            this.numericUpDown1.Size = new System.Drawing.Size(100, 20);
            this.numericUpDown1.TabIndex = 7;
            this.numericUpDown1.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDown1.ValueChanged += new System.EventHandler(this.numericUpDown1_ValueChanged);
            // 
            // labelThicc
            // 
            this.labelThicc.AutoSize = true;
            this.labelThicc.Location = new System.Drawing.Point(998, 197);
            this.labelThicc.Name = "labelThicc";
            this.labelThicc.Size = new System.Drawing.Size(34, 13);
            this.labelThicc.TabIndex = 8;
            this.labelThicc.Text = "Thicc";
            // 
            // buttonCircle
            // 
            this.buttonCircle.Location = new System.Drawing.Point(983, 67);
            this.buttonCircle.Name = "buttonCircle";
            this.buttonCircle.Size = new System.Drawing.Size(75, 23);
            this.buttonCircle.TabIndex = 9;
            this.buttonCircle.Text = "Circle";
            this.buttonCircle.UseVisualStyleBackColor = true;
            this.buttonCircle.Click += new System.EventHandler(this.buttonCircle_Click);
            // 
            // comboCircle
            // 
            this.comboCircle.FormattingEnabled = true;
            this.comboCircle.Location = new System.Drawing.Point(1064, 69);
            this.comboCircle.Name = "comboCircle";
            this.comboCircle.Size = new System.Drawing.Size(100, 21);
            this.comboCircle.TabIndex = 10;
            // 
            // buttonPoly
            // 
            this.buttonPoly.Location = new System.Drawing.Point(983, 96);
            this.buttonPoly.Name = "buttonPoly";
            this.buttonPoly.Size = new System.Drawing.Size(75, 23);
            this.buttonPoly.TabIndex = 11;
            this.buttonPoly.Text = "Poly";
            this.buttonPoly.UseVisualStyleBackColor = true;
            this.buttonPoly.Click += new System.EventHandler(this.buttonPoly_Click);
            // 
            // comboPoly
            // 
            this.comboPoly.FormattingEnabled = true;
            this.comboPoly.Location = new System.Drawing.Point(1064, 98);
            this.comboPoly.Name = "comboPoly";
            this.comboPoly.Size = new System.Drawing.Size(100, 21);
            this.comboPoly.TabIndex = 12;
            // 
            // labelInfo
            // 
            this.labelInfo.AutoSize = true;
            this.labelInfo.Location = new System.Drawing.Point(1032, 12);
            this.labelInfo.Name = "labelInfo";
            this.labelInfo.Size = new System.Drawing.Size(0, 13);
            this.labelInfo.TabIndex = 13;
            // 
            // labelAliasing
            // 
            this.labelAliasing.AutoSize = true;
            this.labelAliasing.Location = new System.Drawing.Point(1020, 246);
            this.labelAliasing.Name = "labelAliasing";
            this.labelAliasing.Size = new System.Drawing.Size(104, 13);
            this.labelAliasing.TabIndex = 14;
            this.labelAliasing.Text = "ANTIALIASING OFF";
            // 
            // buttonAliasing
            // 
            this.buttonAliasing.Location = new System.Drawing.Point(1023, 262);
            this.buttonAliasing.Name = "buttonAliasing";
            this.buttonAliasing.Size = new System.Drawing.Size(101, 23);
            this.buttonAliasing.TabIndex = 15;
            this.buttonAliasing.Text = "Antialiasing";
            this.buttonAliasing.UseVisualStyleBackColor = true;
            this.buttonAliasing.Click += new System.EventHandler(this.buttonAliasing_Click);
            // 
            // buttonCol
            // 
            this.buttonCol.Location = new System.Drawing.Point(1058, 291);
            this.buttonCol.Name = "buttonCol";
            this.buttonCol.Size = new System.Drawing.Size(66, 23);
            this.buttonCol.TabIndex = 16;
            this.buttonCol.Text = "Color";
            this.buttonCol.UseVisualStyleBackColor = true;
            this.buttonCol.Click += new System.EventHandler(this.buttonCol_Click);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.Control;
            this.panel1.Location = new System.Drawing.Point(1023, 291);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(29, 23);
            this.panel1.TabIndex = 17;
            // 
            // buttonSave
            // 
            this.buttonSave.Location = new System.Drawing.Point(983, 469);
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.Size = new System.Drawing.Size(181, 23);
            this.buttonSave.TabIndex = 18;
            this.buttonSave.Text = "Save";
            this.buttonSave.UseVisualStyleBackColor = true;
            this.buttonSave.Click += new System.EventHandler(this.buttonSave_Click);
            // 
            // buttonLoad
            // 
            this.buttonLoad.Location = new System.Drawing.Point(983, 498);
            this.buttonLoad.Name = "buttonLoad";
            this.buttonLoad.Size = new System.Drawing.Size(181, 23);
            this.buttonLoad.TabIndex = 19;
            this.buttonLoad.Text = "Load";
            this.buttonLoad.UseVisualStyleBackColor = true;
            this.buttonLoad.Click += new System.EventHandler(this.buttonLoad_Click);
            // 
            // buttonCapsule
            // 
            this.buttonCapsule.Location = new System.Drawing.Point(983, 125);
            this.buttonCapsule.Name = "buttonCapsule";
            this.buttonCapsule.Size = new System.Drawing.Size(75, 23);
            this.buttonCapsule.TabIndex = 20;
            this.buttonCapsule.Text = "Capsule";
            this.buttonCapsule.UseVisualStyleBackColor = true;
            this.buttonCapsule.Click += new System.EventHandler(this.buttonCapsule_Click);
            // 
            // comboCapsule
            // 
            this.comboCapsule.FormattingEnabled = true;
            this.comboCapsule.Location = new System.Drawing.Point(1064, 127);
            this.comboCapsule.Name = "comboCapsule";
            this.comboCapsule.Size = new System.Drawing.Size(100, 21);
            this.comboCapsule.TabIndex = 21;
            // 
            // buttonRectangle
            // 
            this.buttonRectangle.Location = new System.Drawing.Point(983, 154);
            this.buttonRectangle.Name = "buttonRectangle";
            this.buttonRectangle.Size = new System.Drawing.Size(75, 23);
            this.buttonRectangle.TabIndex = 22;
            this.buttonRectangle.Text = "Rectangle";
            this.buttonRectangle.UseVisualStyleBackColor = true;
            this.buttonRectangle.Click += new System.EventHandler(this.buttonRectangle_Click);
            // 
            // comboRectangle
            // 
            this.comboRectangle.FormattingEnabled = true;
            this.comboRectangle.Location = new System.Drawing.Point(1064, 156);
            this.comboRectangle.Name = "comboRectangle";
            this.comboRectangle.Size = new System.Drawing.Size(100, 21);
            this.comboRectangle.TabIndex = 23;
            // 
            // buttonFill
            // 
            this.buttonFill.Location = new System.Drawing.Point(1023, 320);
            this.buttonFill.Name = "buttonFill";
            this.buttonFill.Size = new System.Drawing.Size(101, 23);
            this.buttonFill.TabIndex = 25;
            this.buttonFill.Text = "Fill poly";
            this.buttonFill.UseVisualStyleBackColor = true;
            this.buttonFill.Click += new System.EventHandler(this.buttonFill_Click);
            // 
            // buttonFillImg
            // 
            this.buttonFillImg.Location = new System.Drawing.Point(983, 437);
            this.buttonFillImg.Name = "buttonFillImg";
            this.buttonFillImg.Size = new System.Drawing.Size(181, 26);
            this.buttonFillImg.TabIndex = 26;
            this.buttonFillImg.Text = "Fill with Image";
            this.buttonFillImg.UseVisualStyleBackColor = true;
            this.buttonFillImg.Click += new System.EventHandler(this.buttonFillImg_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1184, 562);
            this.Controls.Add(this.buttonFillImg);
            this.Controls.Add(this.buttonFill);
            this.Controls.Add(this.comboRectangle);
            this.Controls.Add(this.buttonRectangle);
            this.Controls.Add(this.comboCapsule);
            this.Controls.Add(this.buttonCapsule);
            this.Controls.Add(this.buttonLoad);
            this.Controls.Add(this.buttonSave);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.buttonCol);
            this.Controls.Add(this.buttonAliasing);
            this.Controls.Add(this.labelAliasing);
            this.Controls.Add(this.labelInfo);
            this.Controls.Add(this.comboPoly);
            this.Controls.Add(this.buttonPoly);
            this.Controls.Add(this.comboCircle);
            this.Controls.Add(this.buttonCircle);
            this.Controls.Add(this.labelThicc);
            this.Controls.Add(this.numericUpDown1);
            this.Controls.Add(this.comboLine);
            this.Controls.Add(this.buttonClear);
            this.Controls.Add(this.buttonLine);
            this.Controls.Add(this.pictureBox1);
            this.MaximumSize = new System.Drawing.Size(1200, 600);
            this.MinimumSize = new System.Drawing.Size(1200, 600);
            this.Name = "Form1";
            this.Text = "Rasterizy";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button buttonLine;
        private System.Windows.Forms.Button buttonClear;
        private System.Windows.Forms.ComboBox comboLine;
        private System.Windows.Forms.NumericUpDown numericUpDown1;
        private System.Windows.Forms.Label labelThicc;
        private System.Windows.Forms.Button buttonCircle;
        private System.Windows.Forms.ComboBox comboCircle;
        private System.Windows.Forms.Button buttonPoly;
        private System.Windows.Forms.ComboBox comboPoly;
        private System.Windows.Forms.Label labelInfo;
        private System.Windows.Forms.Label labelAliasing;
        private System.Windows.Forms.Button buttonAliasing;
        private System.Windows.Forms.Button buttonCol;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button buttonSave;
        private System.Windows.Forms.Button buttonLoad;
        private System.Windows.Forms.Button buttonCapsule;
        private System.Windows.Forms.ComboBox comboCapsule;
        private System.Windows.Forms.Button buttonRectangle;
        private System.Windows.Forms.ComboBox comboRectangle;
        private System.Windows.Forms.Button buttonFill;
        private System.Windows.Forms.Button buttonFillImg;
    }
}


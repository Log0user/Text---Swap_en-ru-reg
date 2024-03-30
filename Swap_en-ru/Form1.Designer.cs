namespace Swap_en_ru
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
            button1 = new Button();
            label1 = new Label();
            label2 = new Label();
            radioButton1 = new RadioButton();
            radioButton2 = new RadioButton();
            radioButton3 = new RadioButton();
            radioButton4 = new RadioButton();
            groupBox1 = new GroupBox();
            richTextBox1 = new RichTextBox();
            radioButton5 = new RadioButton();
            label3 = new Label();
            button2 = new Button();
            groupBox2 = new GroupBox();
            richTextBox2 = new RichTextBox();
            radioButton6 = new RadioButton();
            button3 = new Button();
            button4 = new Button();
            button5 = new Button();
            checkBox1 = new CheckBox();
            checkBox2 = new CheckBox();
            groupBox1.SuspendLayout();
            groupBox2.SuspendLayout();
            SuspendLayout();
            // 
            // button1
            // 
            button1.Location = new Point(137, 209);
            button1.Name = "button1";
            button1.Size = new Size(130, 23);
            button1.TabIndex = 0;
            button1.Text = "Сменить раскладку";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(16, 40);
            label1.Name = "label1";
            label1.Size = new Size(160, 15);
            label1.TabIndex = 1;
            label1.Text = "Текст в исходной раскладке";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(46, 40);
            label2.Name = "label2";
            label2.Size = new Size(110, 15);
            label2.TabIndex = 4;
            label2.Text = "Измененный текст";
            // 
            // radioButton1
            // 
            radioButton1.AutoSize = true;
            radioButton1.Checked = true;
            radioButton1.Location = new Point(77, 18);
            radioButton1.Name = "radioButton1";
            radioButton1.Size = new Size(40, 19);
            radioButton1.TabIndex = 5;
            radioButton1.TabStop = true;
            radioButton1.Text = "RU";
            radioButton1.UseVisualStyleBackColor = true;
            radioButton1.Click += radioButton1_Click;
            // 
            // radioButton2
            // 
            radioButton2.AutoSize = true;
            radioButton2.Location = new Point(123, 18);
            radioButton2.Name = "radioButton2";
            radioButton2.Size = new Size(40, 19);
            radioButton2.TabIndex = 6;
            radioButton2.Text = "EN";
            radioButton2.UseVisualStyleBackColor = true;
            radioButton2.Click += radioButton2_Click;
            // 
            // radioButton3
            // 
            radioButton3.AutoSize = true;
            radioButton3.Location = new Point(132, 18);
            radioButton3.Name = "radioButton3";
            radioButton3.Size = new Size(40, 19);
            radioButton3.TabIndex = 7;
            radioButton3.Text = "RU";
            radioButton3.UseVisualStyleBackColor = true;
            radioButton3.Click += radioButton3_Click;
            // 
            // radioButton4
            // 
            radioButton4.AutoSize = true;
            radioButton4.Checked = true;
            radioButton4.Location = new Point(86, 18);
            radioButton4.Name = "radioButton4";
            radioButton4.Size = new Size(40, 19);
            radioButton4.TabIndex = 8;
            radioButton4.TabStop = true;
            radioButton4.Text = "EN";
            radioButton4.UseVisualStyleBackColor = true;
            radioButton4.Click += radioButton4_Click;
            // 
            // groupBox1
            // 
            groupBox1.AutoSize = true;
            groupBox1.Controls.Add(richTextBox1);
            groupBox1.Controls.Add(radioButton5);
            groupBox1.Controls.Add(radioButton2);
            groupBox1.Controls.Add(radioButton1);
            groupBox1.Controls.Add(label1);
            groupBox1.Location = new Point(2, 27);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(199, 175);
            groupBox1.TabIndex = 9;
            groupBox1.TabStop = false;
            // 
            // richTextBox1
            // 
            richTextBox1.Location = new Point(39, 57);
            richTextBox1.Name = "richTextBox1";
            richTextBox1.Size = new Size(100, 96);
            richTextBox1.TabIndex = 16;
            richTextBox1.Text = "";
            // 
            // radioButton5
            // 
            radioButton5.AutoSize = true;
            radioButton5.Location = new Point(23, 18);
            radioButton5.Name = "radioButton5";
            radioButton5.Size = new Size(51, 19);
            radioButton5.TabIndex = 9;
            radioButton5.Text = "Auto";
            radioButton5.UseVisualStyleBackColor = true;
            radioButton5.Click += radioButton5_Click;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(140, 191);
            label3.Name = "label3";
            label3.Size = new Size(38, 15);
            label3.TabIndex = 17;
            label3.Text = "label3";
            label3.Visible = false;
            // 
            // button2
            // 
            button2.Location = new Point(154, 12);
            button2.Name = "button2";
            button2.Size = new Size(47, 23);
            button2.TabIndex = 11;
            button2.Text = "a->A";
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click;
            // 
            // groupBox2
            // 
            groupBox2.AutoSize = true;
            groupBox2.Controls.Add(richTextBox2);
            groupBox2.Controls.Add(radioButton6);
            groupBox2.Controls.Add(label2);
            groupBox2.Controls.Add(radioButton4);
            groupBox2.Controls.Add(radioButton3);
            groupBox2.Location = new Point(207, 27);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new Size(199, 176);
            groupBox2.TabIndex = 10;
            groupBox2.TabStop = false;
            groupBox2.Enter += groupBox2_Enter;
            // 
            // richTextBox2
            // 
            richTextBox2.Location = new Point(56, 58);
            richTextBox2.Name = "richTextBox2";
            richTextBox2.Size = new Size(100, 96);
            richTextBox2.TabIndex = 17;
            richTextBox2.Text = "";
            // 
            // radioButton6
            // 
            radioButton6.AutoSize = true;
            radioButton6.Location = new Point(29, 18);
            radioButton6.Name = "radioButton6";
            radioButton6.Size = new Size(51, 19);
            radioButton6.TabIndex = 10;
            radioButton6.Text = "Auto";
            radioButton6.UseVisualStyleBackColor = true;
            radioButton6.CheckedChanged += radioButton6_CheckedChanged;
            radioButton6.Click += radioButton6_Click;
            // 
            // button3
            // 
            button3.Location = new Point(207, 12);
            button3.Name = "button3";
            button3.Size = new Size(47, 23);
            button3.TabIndex = 12;
            button3.Text = "A->a";
            button3.UseVisualStyleBackColor = true;
            button3.Click += button3_Click;
            // 
            // button4
            // 
            button4.Location = new Point(357, 209);
            button4.Name = "button4";
            button4.Size = new Size(47, 23);
            button4.TabIndex = 13;
            button4.Text = "<---";
            button4.UseVisualStyleBackColor = true;
            button4.Click += button4_Click;
            // 
            // button5
            // 
            button5.Location = new Point(0, 209);
            button5.Name = "button5";
            button5.Size = new Size(47, 23);
            button5.TabIndex = 14;
            button5.Text = "--->";
            button5.UseVisualStyleBackColor = true;
            button5.Click += button5_Click;
            // 
            // checkBox1
            // 
            checkBox1.AutoSize = true;
            checkBox1.Checked = true;
            checkBox1.CheckState = CheckState.Checked;
            checkBox1.Location = new Point(2, 7);
            checkBox1.Name = "checkBox1";
            checkBox1.Size = new Size(139, 19);
            checkBox1.TabIndex = 15;
            checkBox1.Text = "Учитывать все знаки";
            checkBox1.UseVisualStyleBackColor = true;
            // 
            // checkBox2
            // 
            checkBox2.AutoSize = true;
            checkBox2.Checked = true;
            checkBox2.CheckState = CheckState.Checked;
            checkBox2.Location = new Point(254, 7);
            checkBox2.Name = "checkBox2";
            checkBox2.Size = new Size(141, 19);
            checkBox2.TabIndex = 16;
            checkBox2.Text = "Использовать буфер";
            checkBox2.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoSize = true;
            ClientSize = new Size(412, 238);
            Controls.Add(label3);
            Controls.Add(checkBox2);
            Controls.Add(checkBox1);
            Controls.Add(button5);
            Controls.Add(button4);
            Controls.Add(button2);
            Controls.Add(button3);
            Controls.Add(groupBox2);
            Controls.Add(groupBox1);
            Controls.Add(button1);
            Name = "Form1";
            Text = "Перевод раскладки";
            FormClosed += Form1_FormClosed;
            Load += Form1_Load;
            Move += Form1_Move;
            Resize += Form1_Resize;
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            groupBox2.ResumeLayout(false);
            groupBox2.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button button1;
        private Label label1;
        private Label label2;
        private RadioButton radioButton1;
        private RadioButton radioButton2;
        private RadioButton radioButton3;
        private RadioButton radioButton4;
        private GroupBox groupBox1;
        private GroupBox groupBox2;
        private RadioButton radioButton5;
        private RadioButton radioButton6;
        private Button button2;
        private Button button3;
        private Button button4;
        private Button button5;
        private CheckBox checkBox1;
        private RichTextBox richTextBox1;
        private RichTextBox richTextBox2;
        private CheckBox checkBox2;
        private Label label3;
    }
}
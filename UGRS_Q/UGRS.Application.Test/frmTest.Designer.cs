namespace UGRS.Application.Test
{
    partial class frmTest
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
            this.components = new System.ComponentModel.Container();
            this.tmrWorkerOne = new System.Windows.Forms.Timer(this.components);
            this.tmrWorkerTwo = new System.Windows.Forms.Timer(this.components);
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.lstThreadOne = new System.Windows.Forms.ListBox();
            this.lstThreadTwo = new System.Windows.Forms.ListBox();
            this.lstThreadThree = new System.Windows.Forms.ListBox();
            this.label3 = new System.Windows.Forms.Label();
            this.lstThreadFour = new System.Windows.Forms.ListBox();
            this.label4 = new System.Windows.Forms.Label();
            this.tmrWorkerThree = new System.Windows.Forms.Timer(this.components);
            this.tmrWorkerFour = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // tmrWorkerOne
            // 
            this.tmrWorkerOne.Interval = 1000;
            this.tmrWorkerOne.Tick += new System.EventHandler(this.tmrWorkerOne_Tick);
            // 
            // tmrWorkerTwo
            // 
            this.tmrWorkerTwo.Interval = 1000;
            this.tmrWorkerTwo.Tick += new System.EventHandler(this.tmrWorkerTwo_Tick);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(25, 174);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(34, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Hilo 2";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(25, 37);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(34, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Hilo 1";
            // 
            // lstThreadOne
            // 
            this.lstThreadOne.FormattingEnabled = true;
            this.lstThreadOne.Location = new System.Drawing.Point(28, 53);
            this.lstThreadOne.Name = "lstThreadOne";
            this.lstThreadOne.Size = new System.Drawing.Size(344, 95);
            this.lstThreadOne.TabIndex = 4;
            // 
            // lstThreadTwo
            // 
            this.lstThreadTwo.FormattingEnabled = true;
            this.lstThreadTwo.Location = new System.Drawing.Point(28, 190);
            this.lstThreadTwo.Name = "lstThreadTwo";
            this.lstThreadTwo.Size = new System.Drawing.Size(344, 95);
            this.lstThreadTwo.TabIndex = 5;
            // 
            // lstThreadThree
            // 
            this.lstThreadThree.FormattingEnabled = true;
            this.lstThreadThree.Location = new System.Drawing.Point(393, 53);
            this.lstThreadThree.Name = "lstThreadThree";
            this.lstThreadThree.Size = new System.Drawing.Size(344, 95);
            this.lstThreadThree.TabIndex = 7;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(390, 37);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(34, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Hilo 3";
            // 
            // lstThreadFour
            // 
            this.lstThreadFour.FormattingEnabled = true;
            this.lstThreadFour.Location = new System.Drawing.Point(393, 192);
            this.lstThreadFour.Name = "lstThreadFour";
            this.lstThreadFour.Size = new System.Drawing.Size(344, 95);
            this.lstThreadFour.TabIndex = 9;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(390, 176);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(34, 13);
            this.label4.TabIndex = 8;
            this.label4.Text = "Hilo 4";
            // 
            // tmrWorkerThree
            // 
            this.tmrWorkerThree.Interval = 1000;
            this.tmrWorkerThree.Tick += new System.EventHandler(this.tmrWorkerThree_Tick);
            // 
            // tmrWorkerFour
            // 
            this.tmrWorkerFour.Interval = 1000;
            this.tmrWorkerFour.Tick += new System.EventHandler(this.tmrWorkerFour_Tick);
            // 
            // frmTest
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(753, 299);
            this.Controls.Add(this.lstThreadFour);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.lstThreadThree);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.lstThreadTwo);
            this.Controls.Add(this.lstThreadOne);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "frmTest";
            this.Text = "Prueba";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmTest_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Timer tmrWorkerOne;
        private System.Windows.Forms.Timer tmrWorkerTwo;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ListBox lstThreadOne;
        private System.Windows.Forms.ListBox lstThreadTwo;
        private System.Windows.Forms.ListBox lstThreadThree;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ListBox lstThreadFour;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Timer tmrWorkerThree;
        private System.Windows.Forms.Timer tmrWorkerFour;
    }
}


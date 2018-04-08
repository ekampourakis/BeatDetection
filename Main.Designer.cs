namespace BeatDetection {
    partial class Main {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.ComboBox_InputDevices = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.ComboBox_BassSensitivity = new System.Windows.Forms.ComboBox();
            this.ComboBox_MidsSensitivity = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.Button_StartStop = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // ComboBox_InputDevices
            // 
            this.ComboBox_InputDevices.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ComboBox_InputDevices.FormattingEnabled = true;
            this.ComboBox_InputDevices.Location = new System.Drawing.Point(101, 12);
            this.ComboBox_InputDevices.Name = "ComboBox_InputDevices";
            this.ComboBox_InputDevices.Size = new System.Drawing.Size(346, 21);
            this.ComboBox_InputDevices.TabIndex = 0;
            this.ComboBox_InputDevices.SelectedIndexChanged += new System.EventHandler(this.ComboBox_InputDevices_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(74, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Audio Source:";
            // 
            // ComboBox_BassSensitivity
            // 
            this.ComboBox_BassSensitivity.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.ComboBox_BassSensitivity.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.ComboBox_BassSensitivity.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ComboBox_BassSensitivity.FormattingEnabled = true;
            this.ComboBox_BassSensitivity.Items.AddRange(new object[] {
            "Very Low",
            "Low",
            "Normal",
            "High",
            "Very High"});
            this.ComboBox_BassSensitivity.Location = new System.Drawing.Point(101, 39);
            this.ComboBox_BassSensitivity.Name = "ComboBox_BassSensitivity";
            this.ComboBox_BassSensitivity.Size = new System.Drawing.Size(125, 21);
            this.ComboBox_BassSensitivity.TabIndex = 3;
            // 
            // ComboBox_MidsSensitivity
            // 
            this.ComboBox_MidsSensitivity.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.ComboBox_MidsSensitivity.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.ComboBox_MidsSensitivity.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ComboBox_MidsSensitivity.FormattingEnabled = true;
            this.ComboBox_MidsSensitivity.Items.AddRange(new object[] {
            "Very Low",
            "Low",
            "Normal",
            "High",
            "Very High"});
            this.ComboBox_MidsSensitivity.Location = new System.Drawing.Point(322, 39);
            this.ComboBox_MidsSensitivity.Name = "ComboBox_MidsSensitivity";
            this.ComboBox_MidsSensitivity.Size = new System.Drawing.Size(125, 21);
            this.ComboBox_MidsSensitivity.TabIndex = 4;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 42);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(83, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Bass Sensitivity:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(232, 42);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(82, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Mids Sensitivity:";
            // 
            // Button_StartStop
            // 
            this.Button_StartStop.Location = new System.Drawing.Point(372, 66);
            this.Button_StartStop.Name = "Button_StartStop";
            this.Button_StartStop.Size = new System.Drawing.Size(75, 23);
            this.Button_StartStop.TabIndex = 7;
            this.Button_StartStop.Text = "Start";
            this.Button_StartStop.UseVisualStyleBackColor = true;
            this.Button_StartStop.Click += new System.EventHandler(this.Button_StartStop_Click);
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(854, 603);
            this.Controls.Add(this.Button_StartStop);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.ComboBox_MidsSensitivity);
            this.Controls.Add(this.ComboBox_BassSensitivity);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.ComboBox_InputDevices);
            this.Name = "Main";
            this.Text = "Main";
            this.Load += new System.EventHandler(this.TestGUI_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox ComboBox_InputDevices;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox ComboBox_BassSensitivity;
        private System.Windows.Forms.ComboBox ComboBox_MidsSensitivity;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button Button_StartStop;
    }
}
namespace BackBag.App
{
    partial class Main
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
            this.MainTab = new System.Windows.Forms.TabControl();
            this.InstalledTab = new System.Windows.Forms.TabPage();
            this.UninstalledTab = new System.Windows.Forms.TabPage();
            this.InstalledListView = new System.Windows.Forms.ListView();
            this.UninstalledListView = new System.Windows.Forms.ListView();
            this.MainTab.SuspendLayout();
            this.InstalledTab.SuspendLayout();
            this.UninstalledTab.SuspendLayout();
            this.SuspendLayout();
            // 
            // MainTab
            // 
            this.MainTab.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.MainTab.Controls.Add(this.InstalledTab);
            this.MainTab.Controls.Add(this.UninstalledTab);
            this.MainTab.Location = new System.Drawing.Point(14, 17);
            this.MainTab.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MainTab.Name = "MainTab";
            this.MainTab.SelectedIndex = 0;
            this.MainTab.Size = new System.Drawing.Size(630, 345);
            this.MainTab.TabIndex = 0;
            // 
            // InstalledTab
            // 
            this.InstalledTab.Controls.Add(this.InstalledListView);
            this.InstalledTab.Location = new System.Drawing.Point(4, 26);
            this.InstalledTab.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.InstalledTab.Name = "InstalledTab";
            this.InstalledTab.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.InstalledTab.Size = new System.Drawing.Size(896, 547);
            this.InstalledTab.TabIndex = 0;
            this.InstalledTab.Text = "My Apps";
            this.InstalledTab.UseVisualStyleBackColor = true;
            // 
            // UninstalledTab
            // 
            this.UninstalledTab.Controls.Add(this.UninstalledListView);
            this.UninstalledTab.Location = new System.Drawing.Point(4, 26);
            this.UninstalledTab.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.UninstalledTab.Name = "UninstalledTab";
            this.UninstalledTab.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.UninstalledTab.Size = new System.Drawing.Size(622, 315);
            this.UninstalledTab.TabIndex = 1;
            this.UninstalledTab.Text = "App Store";
            this.UninstalledTab.UseVisualStyleBackColor = true;
            // 
            // InstalledListView
            // 
            this.InstalledListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.InstalledListView.Location = new System.Drawing.Point(7, 8);
            this.InstalledListView.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.InstalledListView.Name = "InstalledListView";
            this.InstalledListView.Size = new System.Drawing.Size(880, 531);
            this.InstalledListView.TabIndex = 0;
            this.InstalledListView.UseCompatibleStateImageBehavior = false;
            // 
            // UninstalledListView
            // 
            this.UninstalledListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.UninstalledListView.Location = new System.Drawing.Point(7, 8);
            this.UninstalledListView.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.UninstalledListView.Name = "UninstalledListView";
            this.UninstalledListView.Size = new System.Drawing.Size(606, 299);
            this.UninstalledListView.TabIndex = 1;
            this.UninstalledListView.UseCompatibleStateImageBehavior = false;
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(658, 379);
            this.Controls.Add(this.MainTab);
            this.Font = new System.Drawing.Font("Microsoft YaHei", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "Main";
            this.Text = "Main";
            this.MainTab.ResumeLayout(false);
            this.InstalledTab.ResumeLayout(false);
            this.UninstalledTab.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl MainTab;
        private System.Windows.Forms.TabPage InstalledTab;
        private System.Windows.Forms.TabPage UninstalledTab;
        private System.Windows.Forms.ListView InstalledListView;
        private System.Windows.Forms.ListView UninstalledListView;

    }
}
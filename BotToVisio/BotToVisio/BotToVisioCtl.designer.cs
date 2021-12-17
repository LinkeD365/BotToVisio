
namespace LinkeD365.BotToVisio
{
    partial class BotToVisioCtl
    {
        /// <summary> 
        /// Variable nécessaire au concepteur.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Nettoyage des ressources utilisées.
        /// </summary>
        /// <param name="disposing">true si les ressources managées doivent être supprimées ; sinon, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Code généré par le Concepteur de composants

        /// <summary> 
        /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas 
        /// le contenu de cette méthode avec l'éditeur de code.
        /// </summary>
        private void InitializeComponent()
        {
            this.toolStripMenu = new System.Windows.Forms.ToolStrip();
            this.tsbClose = new System.Windows.Forms.ToolStripButton();
            this.tssSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.splitMain = new System.Windows.Forms.SplitContainer();
            this.lblBot = new System.Windows.Forms.Label();
            this.cboBot = new System.Windows.Forms.ComboBox();
            this.grpTopics = new System.Windows.Forms.GroupBox();
            this.gvTopics = new System.Windows.Forms.DataGridView();
            this.btnCreateVisio = new System.Windows.Forms.ToolStripButton();
            this.toolStripMenu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitMain)).BeginInit();
            this.splitMain.Panel1.SuspendLayout();
            this.splitMain.Panel2.SuspendLayout();
            this.splitMain.SuspendLayout();
            this.grpTopics.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gvTopics)).BeginInit();
            this.SuspendLayout();
            // 
            // toolStripMenu
            // 
            this.toolStripMenu.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.toolStripMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsbClose,
            this.tssSeparator1,
            this.btnCreateVisio});
            this.toolStripMenu.Location = new System.Drawing.Point(0, 0);
            this.toolStripMenu.Name = "toolStripMenu";
            this.toolStripMenu.Size = new System.Drawing.Size(559, 31);
            this.toolStripMenu.TabIndex = 4;
            this.toolStripMenu.Text = "toolStrip1";
            // 
            // tsbClose
            // 
            this.tsbClose.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsbClose.Name = "tsbClose";
            this.tsbClose.Size = new System.Drawing.Size(86, 28);
            this.tsbClose.Text = "Close this tool";
            this.tsbClose.Click += new System.EventHandler(this.tsbClose_Click);
            // 
            // tssSeparator1
            // 
            this.tssSeparator1.Name = "tssSeparator1";
            this.tssSeparator1.Size = new System.Drawing.Size(6, 31);
            // 
            // splitMain
            // 
            this.splitMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitMain.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitMain.Location = new System.Drawing.Point(0, 31);
            this.splitMain.Name = "splitMain";
            this.splitMain.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitMain.Panel1
            // 
            this.splitMain.Panel1.Controls.Add(this.lblBot);
            this.splitMain.Panel1.Controls.Add(this.cboBot);
            // 
            // splitMain.Panel2
            // 
            this.splitMain.Panel2.Controls.Add(this.grpTopics);
            this.splitMain.Size = new System.Drawing.Size(559, 269);
            this.splitMain.SplitterDistance = 30;
            this.splitMain.TabIndex = 5;
            // 
            // lblBot
            // 
            this.lblBot.AutoSize = true;
            this.lblBot.Location = new System.Drawing.Point(3, 9);
            this.lblBot.Name = "lblBot";
            this.lblBot.Size = new System.Drawing.Size(56, 13);
            this.lblBot.TabIndex = 1;
            this.lblBot.Text = "Select Bot";
            // 
            // cboBot
            // 
            this.cboBot.FormattingEnabled = true;
            this.cboBot.Location = new System.Drawing.Point(65, 6);
            this.cboBot.Name = "cboBot";
            this.cboBot.Size = new System.Drawing.Size(243, 21);
            this.cboBot.TabIndex = 0;
            this.cboBot.SelectedValueChanged += new System.EventHandler(this.cboBot_SelectedIndexChanged);
            // 
            // grpTopics
            // 
            this.grpTopics.Controls.Add(this.gvTopics);
            this.grpTopics.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpTopics.Location = new System.Drawing.Point(0, 0);
            this.grpTopics.Name = "grpTopics";
            this.grpTopics.Size = new System.Drawing.Size(559, 235);
            this.grpTopics.TabIndex = 0;
            this.grpTopics.TabStop = false;
            this.grpTopics.Text = "Topics";
            // 
            // gvTopics
            // 
            this.gvTopics.AllowUserToAddRows = false;
            this.gvTopics.AllowUserToDeleteRows = false;
            this.gvTopics.AllowUserToOrderColumns = true;
            this.gvTopics.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gvTopics.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gvTopics.Location = new System.Drawing.Point(3, 16);
            this.gvTopics.Name = "gvTopics";
            this.gvTopics.ReadOnly = true;
            this.gvTopics.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.gvTopics.Size = new System.Drawing.Size(553, 216);
            this.gvTopics.TabIndex = 0;
            // 
            // btnCreateVisio
            // 
            this.btnCreateVisio.Image = global::LinkeD365.BotToVisio.Properties.Resources.visio_icon;
            this.btnCreateVisio.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnCreateVisio.Name = "btnCreateVisio";
            this.btnCreateVisio.Size = new System.Drawing.Size(97, 28);
            this.btnCreateVisio.Text = "Create Visio";
            this.btnCreateVisio.Click += new System.EventHandler(this.btnCreateVisio_Click);
            // 
            // BotToVisioCtl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitMain);
            this.Controls.Add(this.toolStripMenu);
            this.Name = "BotToVisioCtl";
            this.Size = new System.Drawing.Size(559, 300);
            this.Load += new System.EventHandler(this.MyPluginControl_Load);
            this.toolStripMenu.ResumeLayout(false);
            this.toolStripMenu.PerformLayout();
            this.splitMain.Panel1.ResumeLayout(false);
            this.splitMain.Panel1.PerformLayout();
            this.splitMain.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitMain)).EndInit();
            this.splitMain.ResumeLayout(false);
            this.grpTopics.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gvTopics)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ToolStrip toolStripMenu;
        private System.Windows.Forms.ToolStripButton tsbClose;
        private System.Windows.Forms.ToolStripSeparator tssSeparator1;
        private System.Windows.Forms.SplitContainer splitMain;
        private System.Windows.Forms.Label lblBot;
        private System.Windows.Forms.ComboBox cboBot;
        private System.Windows.Forms.GroupBox grpTopics;
        private System.Windows.Forms.DataGridView gvTopics;
        private System.Windows.Forms.ToolStripButton btnCreateVisio;
    }
}

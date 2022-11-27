
namespace core.audiamus.connect.ui {
  partial class BookLibDGVControl {
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose (bool disposing) {
      if (disposing && (components != null)) {
        components.Dispose ();
      }
      base.Dispose (disposing);
    }

    #region Component Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify 
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent () {
      System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
      System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
      this.dataGridView1 = new core.audiamus.connect.ui.DataGridViewEx();
      this.panelDownloadSelect = new System.Windows.Forms.Panel();
      this.panelDnloadList = new System.Windows.Forms.Panel();
      this.lblDnloadList = new System.Windows.Forms.Label();
      this.panelResync = new System.Windows.Forms.Panel();
      this.btnResync = new System.Windows.Forms.Button();
      this.panelDownloadSelectButtons = new System.Windows.Forms.Panel();
      this.lblDnloadCaption = new System.Windows.Forms.Label();
      this.btnRemAll = new System.Windows.Forms.Button();
      this.btnAddSel = new System.Windows.Forms.Button();
      this.btnRemSel = new System.Windows.Forms.Button();
      this.btnOk = new System.Windows.Forms.Button();
      this.panel1 = new System.Windows.Forms.Panel();
      this.panelOk = new System.Windows.Forms.Panel();
      ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
      this.panelDownloadSelect.SuspendLayout();
      this.panelDnloadList.SuspendLayout();
      this.panelResync.SuspendLayout();
      this.panelDownloadSelectButtons.SuspendLayout();
      this.panel1.SuspendLayout();
      this.panelOk.SuspendLayout();
      this.SuspendLayout();
      // 
      // dataGridView1
      // 
      this.dataGridView1.AllowUserToResizeRows = false;
      this.dataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.DisplayedCells;
      this.dataGridView1.ClientAreaEnabled = true;
      dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
      dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
      dataGridViewCellStyle1.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
      dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
      dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Control;
      dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.WindowText;
      dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
      this.dataGridView1.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
      this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
      dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
      dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
      dataGridViewCellStyle2.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
      dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
      dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.GradientActiveCaption;
      dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.ControlText;
      dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
      this.dataGridView1.DefaultCellStyle = dataGridViewCellStyle2;
      this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.dataGridView1.Location = new System.Drawing.Point(0, 37);
      this.dataGridView1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
      this.dataGridView1.Name = "dataGridView1";
      this.dataGridView1.ReadOnly = true;
      this.dataGridView1.RowHeadersVisible = false;
      this.dataGridView1.RowTemplate.Height = 23;
      this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
      this.dataGridView1.Size = new System.Drawing.Size(933, 482);
      this.dataGridView1.TabIndex = 0;
      this.dataGridView1.BeginSorting += new System.EventHandler(this.dataGridView1_BeginSorting);
      this.dataGridView1.EndSorting += new System.EventHandler(this.dataGridView1_EndSorting);
      this.dataGridView1.SortingCompleteToSetVerticalPosition += new System.EventHandler(this.dataGridView1_SortingCompleteToSetVerticalPosition);
      this.dataGridView1.CellToolTipTextNeeded += new System.Windows.Forms.DataGridViewCellToolTipTextNeededEventHandler(this.dataGridView1_CellToolTipTextNeeded);
      this.dataGridView1.DataBindingComplete += new System.Windows.Forms.DataGridViewBindingCompleteEventHandler(this.dataGridView1_DataBindingComplete);
      this.dataGridView1.SelectionChanged += new System.EventHandler(this.dataGridView1_SelectionChanged);
      // 
      // panelDownloadSelect
      // 
      this.panelDownloadSelect.Controls.Add(this.panelDnloadList);
      this.panelDownloadSelect.Controls.Add(this.panelResync);
      this.panelDownloadSelect.Controls.Add(this.panelDownloadSelectButtons);
      this.panelDownloadSelect.Dock = System.Windows.Forms.DockStyle.Fill;
      this.panelDownloadSelect.Enabled = false;
      this.panelDownloadSelect.Location = new System.Drawing.Point(0, 0);
      this.panelDownloadSelect.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
      this.panelDownloadSelect.Name = "panelDownloadSelect";
      this.panelDownloadSelect.Size = new System.Drawing.Size(835, 37);
      this.panelDownloadSelect.TabIndex = 1;
      // 
      // panelDnloadList
      // 
      this.panelDnloadList.Controls.Add(this.lblDnloadList);
      this.panelDnloadList.Dock = System.Windows.Forms.DockStyle.Fill;
      this.panelDnloadList.Location = new System.Drawing.Point(393, 0);
      this.panelDnloadList.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
      this.panelDnloadList.Name = "panelDnloadList";
      this.panelDnloadList.Padding = new System.Windows.Forms.Padding(6, 7, 6, 5);
      this.panelDnloadList.Size = new System.Drawing.Size(343, 37);
      this.panelDnloadList.TabIndex = 6;
      // 
      // lblDnloadList
      // 
      this.lblDnloadList.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
      this.lblDnloadList.Dock = System.Windows.Forms.DockStyle.Fill;
      this.lblDnloadList.Location = new System.Drawing.Point(6, 7);
      this.lblDnloadList.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
      this.lblDnloadList.Name = "lblDnloadList";
      this.lblDnloadList.Size = new System.Drawing.Size(331, 25);
      this.lblDnloadList.TabIndex = 2;
      this.lblDnloadList.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
      // 
      // panelResync
      // 
      this.panelResync.Controls.Add(this.btnResync);
      this.panelResync.Dock = System.Windows.Forms.DockStyle.Right;
      this.panelResync.Location = new System.Drawing.Point(736, 0);
      this.panelResync.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
      this.panelResync.Name = "panelResync";
      this.panelResync.Size = new System.Drawing.Size(99, 37);
      this.panelResync.TabIndex = 7;
      // 
      // btnResync
      // 
      this.btnResync.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.btnResync.Location = new System.Drawing.Point(8, 6);
      this.btnResync.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
      this.btnResync.Name = "btnResync";
      this.btnResync.Size = new System.Drawing.Size(88, 27);
      this.btnResync.TabIndex = 4;
      this.btnResync.Text = "Resync";
      this.btnResync.UseVisualStyleBackColor = true;
      this.btnResync.Click += new System.EventHandler(this.btnResync_Click);
      // 
      // panelDownloadSelectButtons
      // 
      this.panelDownloadSelectButtons.Controls.Add(this.lblDnloadCaption);
      this.panelDownloadSelectButtons.Controls.Add(this.btnRemAll);
      this.panelDownloadSelectButtons.Controls.Add(this.btnAddSel);
      this.panelDownloadSelectButtons.Controls.Add(this.btnRemSel);
      this.panelDownloadSelectButtons.Dock = System.Windows.Forms.DockStyle.Left;
      this.panelDownloadSelectButtons.Location = new System.Drawing.Point(0, 0);
      this.panelDownloadSelectButtons.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
      this.panelDownloadSelectButtons.Name = "panelDownloadSelectButtons";
      this.panelDownloadSelectButtons.Size = new System.Drawing.Size(393, 37);
      this.panelDownloadSelectButtons.TabIndex = 5;
      // 
      // lblDnloadCaption
      // 
      this.lblDnloadCaption.AutoSize = true;
      this.lblDnloadCaption.Location = new System.Drawing.Point(7, 12);
      this.lblDnloadCaption.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
      this.lblDnloadCaption.Name = "lblDnloadCaption";
      this.lblDnloadCaption.Size = new System.Drawing.Size(64, 15);
      this.lblDnloadCaption.TabIndex = 1;
      this.lblDnloadCaption.Text = "Download:";
      // 
      // btnRemAll
      // 
      this.btnRemAll.Enabled = false;
      this.btnRemAll.Location = new System.Drawing.Point(288, 6);
      this.btnRemAll.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
      this.btnRemAll.Name = "btnRemAll";
      this.btnRemAll.Size = new System.Drawing.Size(99, 27);
      this.btnRemAll.TabIndex = 2;
      this.btnRemAll.Text = "Remove all";
      this.btnRemAll.UseVisualStyleBackColor = true;
      this.btnRemAll.Click += new System.EventHandler(this.btnRemAll_Click);
      // 
      // btnAddSel
      // 
      this.btnAddSel.Enabled = false;
      this.btnAddSel.Location = new System.Drawing.Point(78, 6);
      this.btnAddSel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
      this.btnAddSel.Name = "btnAddSel";
      this.btnAddSel.Size = new System.Drawing.Size(99, 27);
      this.btnAddSel.TabIndex = 0;
      this.btnAddSel.Text = "Add selected";
      this.btnAddSel.UseVisualStyleBackColor = true;
      this.btnAddSel.Click += new System.EventHandler(this.btnAddSel_Click);
      // 
      // btnRemSel
      // 
      this.btnRemSel.Enabled = false;
      this.btnRemSel.Location = new System.Drawing.Point(183, 6);
      this.btnRemSel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
      this.btnRemSel.Name = "btnRemSel";
      this.btnRemSel.Size = new System.Drawing.Size(99, 27);
      this.btnRemSel.TabIndex = 1;
      this.btnRemSel.Text = "Rem. selected";
      this.btnRemSel.UseVisualStyleBackColor = true;
      this.btnRemSel.Click += new System.EventHandler(this.btnRemSel_Click);
      // 
      // btnOk
      // 
      this.btnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.btnOk.Location = new System.Drawing.Point(7, 6);
      this.btnOk.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
      this.btnOk.Name = "btnOk";
      this.btnOk.Size = new System.Drawing.Size(88, 27);
      this.btnOk.TabIndex = 0;
      this.btnOk.Text = "OK";
      this.btnOk.UseVisualStyleBackColor = true;
      this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
      // 
      // panel1
      // 
      this.panel1.Controls.Add(this.panelDownloadSelect);
      this.panel1.Controls.Add(this.panelOk);
      this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
      this.panel1.Location = new System.Drawing.Point(0, 0);
      this.panel1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
      this.panel1.Name = "panel1";
      this.panel1.Size = new System.Drawing.Size(933, 37);
      this.panel1.TabIndex = 0;
      // 
      // panelOk
      // 
      this.panelOk.Controls.Add(this.btnOk);
      this.panelOk.Dock = System.Windows.Forms.DockStyle.Right;
      this.panelOk.Location = new System.Drawing.Point(835, 0);
      this.panelOk.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
      this.panelOk.Name = "panelOk";
      this.panelOk.Size = new System.Drawing.Size(98, 37);
      this.panelOk.TabIndex = 0;
      // 
      // BookLibDGVControl
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.dataGridView1);
      this.Controls.Add(this.panel1);
      this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
      this.Name = "BookLibDGVControl";
      this.Size = new System.Drawing.Size(933, 519);
      ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
      this.panelDownloadSelect.ResumeLayout(false);
      this.panelDnloadList.ResumeLayout(false);
      this.panelResync.ResumeLayout(false);
      this.panelDownloadSelectButtons.ResumeLayout(false);
      this.panelDownloadSelectButtons.PerformLayout();
      this.panel1.ResumeLayout(false);
      this.panelOk.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    private core.audiamus.connect.ui.DataGridViewEx dataGridView1;
    private System.Windows.Forms.Button btnOk;
    private System.Windows.Forms.Panel panel1;
    private System.Windows.Forms.Panel panelOk;
    internal System.Windows.Forms.Panel panelDownloadSelect;
    internal System.Windows.Forms.Button btnAddSel;
    internal System.Windows.Forms.Label lblDnloadList;
    internal System.Windows.Forms.Label lblDnloadCaption;
    internal System.Windows.Forms.Button btnRemAll;
    internal System.Windows.Forms.Button btnRemSel;
    internal System.Windows.Forms.Panel panelDnloadList;
    internal System.Windows.Forms.Panel panelResync;
    internal System.Windows.Forms.Panel panelDownloadSelectButtons;
    internal System.Windows.Forms.Button btnResync;
  }
}

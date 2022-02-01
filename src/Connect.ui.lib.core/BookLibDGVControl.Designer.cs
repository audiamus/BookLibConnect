
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
      System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
      System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
      this.dataGridView1 = new core.audiamus.connect.ui.DataGridViewEx();
      this.panelDownloadSelect = new System.Windows.Forms.Panel();
      this.lblDnloadList = new System.Windows.Forms.Label();
      this.lblDnloadCaption = new System.Windows.Forms.Label();
      this.btnRemAll = new System.Windows.Forms.Button();
      this.btnRemSel = new System.Windows.Forms.Button();
      this.btnAddSel = new System.Windows.Forms.Button();
      this.btnOk = new System.Windows.Forms.Button();
      this.panel1 = new System.Windows.Forms.Panel();
      this.panel2 = new System.Windows.Forms.Panel();
      ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
      this.panelDownloadSelect.SuspendLayout();
      this.panel1.SuspendLayout();
      this.panel2.SuspendLayout();
      this.SuspendLayout();
      // 
      // dataGridView1
      // 
      this.dataGridView1.AllowUserToResizeRows = false;
      this.dataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.DisplayedCells;
      dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
      dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Control;
      dataGridViewCellStyle3.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
      dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
      dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Control;
      dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.WindowText;
      dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
      this.dataGridView1.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle3;
      this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
      dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
      dataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.Window;
      dataGridViewCellStyle4.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
      dataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.ControlText;
      dataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.GradientActiveCaption;
      dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.ControlText;
      dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
      this.dataGridView1.DefaultCellStyle = dataGridViewCellStyle4;
      this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.dataGridView1.Location = new System.Drawing.Point(0, 32);
      this.dataGridView1.Name = "dataGridView1";
      this.dataGridView1.ReadOnly = true;
      this.dataGridView1.RowHeadersVisible = false;
      this.dataGridView1.RowTemplate.Height = 23;
      this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
      this.dataGridView1.Size = new System.Drawing.Size(800, 418);
      this.dataGridView1.TabIndex = 0;
      this.dataGridView1.BeginSorting += new System.EventHandler(this.dataGridView1_BeginSorting);
      this.dataGridView1.EndSorting += new System.EventHandler(this.dataGridView1_EndSorting);
      this.dataGridView1.SortingCompleteToSetVerticalPosition += new System.EventHandler(this.dataGridView1_SortingCompleteToSetVerticalPosition);
      this.dataGridView1.DataBindingComplete += new System.Windows.Forms.DataGridViewBindingCompleteEventHandler(this.dataGridView1_DataBindingComplete);
      this.dataGridView1.SelectionChanged += new System.EventHandler(this.dataGridView1_SelectionChanged);
      // 
      // panelDownloadSelect
      // 
      this.panelDownloadSelect.Controls.Add(this.lblDnloadList);
      this.panelDownloadSelect.Controls.Add(this.lblDnloadCaption);
      this.panelDownloadSelect.Controls.Add(this.btnRemAll);
      this.panelDownloadSelect.Controls.Add(this.btnRemSel);
      this.panelDownloadSelect.Controls.Add(this.btnAddSel);
      this.panelDownloadSelect.Dock = System.Windows.Forms.DockStyle.Fill;
      this.panelDownloadSelect.Location = new System.Drawing.Point(0, 0);
      this.panelDownloadSelect.Name = "panelDownloadSelect";
      this.panelDownloadSelect.Size = new System.Drawing.Size(716, 32);
      this.panelDownloadSelect.TabIndex = 1;
      // 
      // lblDnloadList
      // 
      this.lblDnloadList.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.lblDnloadList.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
      this.lblDnloadList.Location = new System.Drawing.Point(337, 5);
      this.lblDnloadList.Name = "lblDnloadList";
      this.lblDnloadList.Size = new System.Drawing.Size(373, 23);
      this.lblDnloadList.TabIndex = 2;
      this.lblDnloadList.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
      // 
      // lblDnloadCaption
      // 
      this.lblDnloadCaption.AutoSize = true;
      this.lblDnloadCaption.Location = new System.Drawing.Point(4, 10);
      this.lblDnloadCaption.Name = "lblDnloadCaption";
      this.lblDnloadCaption.Size = new System.Drawing.Size(58, 13);
      this.lblDnloadCaption.TabIndex = 1;
      this.lblDnloadCaption.Text = "Download:";
      // 
      // btnRemAll
      // 
      this.btnRemAll.Enabled = false;
      this.btnRemAll.Location = new System.Drawing.Point(245, 5);
      this.btnRemAll.Name = "btnRemAll";
      this.btnRemAll.Size = new System.Drawing.Size(85, 23);
      this.btnRemAll.TabIndex = 2;
      this.btnRemAll.Text = "Remove all";
      this.btnRemAll.UseVisualStyleBackColor = true;
      this.btnRemAll.Click += new System.EventHandler(this.btnRemAll_Click);
      // 
      // btnRemSel
      // 
      this.btnRemSel.Enabled = false;
      this.btnRemSel.Location = new System.Drawing.Point(155, 5);
      this.btnRemSel.Name = "btnRemSel";
      this.btnRemSel.Size = new System.Drawing.Size(85, 23);
      this.btnRemSel.TabIndex = 1;
      this.btnRemSel.Text = "Rem. selected";
      this.btnRemSel.UseVisualStyleBackColor = true;
      this.btnRemSel.Click += new System.EventHandler(this.btnRemSel_Click);
      // 
      // btnAddSel
      // 
      this.btnAddSel.Enabled = false;
      this.btnAddSel.Location = new System.Drawing.Point(65, 5);
      this.btnAddSel.Name = "btnAddSel";
      this.btnAddSel.Size = new System.Drawing.Size(85, 23);
      this.btnAddSel.TabIndex = 0;
      this.btnAddSel.Text = "Add selected";
      this.btnAddSel.UseVisualStyleBackColor = true;
      this.btnAddSel.Click += new System.EventHandler(this.btnAddSel_Click);
      // 
      // btnOk
      // 
      this.btnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.btnOk.Location = new System.Drawing.Point(6, 5);
      this.btnOk.Name = "btnOk";
      this.btnOk.Size = new System.Drawing.Size(75, 23);
      this.btnOk.TabIndex = 3;
      this.btnOk.Text = "OK";
      this.btnOk.UseVisualStyleBackColor = true;
      this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
      // 
      // panel1
      // 
      this.panel1.Controls.Add(this.panelDownloadSelect);
      this.panel1.Controls.Add(this.panel2);
      this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
      this.panel1.Location = new System.Drawing.Point(0, 0);
      this.panel1.Name = "panel1";
      this.panel1.Size = new System.Drawing.Size(800, 32);
      this.panel1.TabIndex = 4;
      // 
      // panel2
      // 
      this.panel2.Controls.Add(this.btnOk);
      this.panel2.Dock = System.Windows.Forms.DockStyle.Right;
      this.panel2.Location = new System.Drawing.Point(716, 0);
      this.panel2.Name = "panel2";
      this.panel2.Size = new System.Drawing.Size(84, 32);
      this.panel2.TabIndex = 2;
      // 
      // BookLibDGVControl
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.dataGridView1);
      this.Controls.Add(this.panel1);
      this.Name = "BookLibDGVControl";
      this.Size = new System.Drawing.Size(800, 450);
      ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
      this.panelDownloadSelect.ResumeLayout(false);
      this.panelDownloadSelect.PerformLayout();
      this.panel1.ResumeLayout(false);
      this.panel2.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    private core.audiamus.connect.ui.DataGridViewEx dataGridView1;
    private System.Windows.Forms.Button btnAddSel;
    private System.Windows.Forms.Label lblDnloadList;
    private System.Windows.Forms.Label lblDnloadCaption;
    private System.Windows.Forms.Button btnOk;
    private System.Windows.Forms.Button btnRemAll;
    private System.Windows.Forms.Button btnRemSel;
    private System.Windows.Forms.Panel panel1;
    private System.Windows.Forms.Panel panel2;
    internal System.Windows.Forms.Panel panelDownloadSelect;
  }
}

namespace lotus;

partial class DashboardForm
{
    private System.ComponentModel.IContainer components = null;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components is not null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    private void InitializeComponent()
    {
        DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
        QueryEditorField = new RichTextBox();
        ExecuteQueryButton = new Button();
        DashboardTreeView = new TreeView();
        QueryResultTab = new TabControl();
        QueryResultTabGrid = new TabPage();
        QueryResultGrid = new DataGridView();
        QueryResultTabMessages = new TabPage();
        QueryResultTabMessagesLabel = new Label();
        QueryPanel = new Panel();
        menuStrip1 = new MenuStrip();
        ObjectExplorerPanel = new Panel();
        ObjectExplorerLabel = new Label();
        RefreshButton = new Button();
        QueryResultTab.SuspendLayout();
        QueryResultTabGrid.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)QueryResultGrid).BeginInit();
        QueryResultTabMessages.SuspendLayout();
        QueryPanel.SuspendLayout();
        ObjectExplorerPanel.SuspendLayout();
        SuspendLayout();
        // 
        // QueryEditorField
        // 
        QueryEditorField.Font = new Font("Consolas", 9F);
        QueryEditorField.Location = new Point(436, 149);
        QueryEditorField.Name = "QueryEditorField";
        QueryEditorField.Size = new Size(1184, 428);
        QueryEditorField.TabIndex = 1;
        QueryEditorField.Text = "";
        QueryEditorField.BorderStyle = BorderStyle.FixedSingle;
        // 
        // ExecuteQueryButton
        // 
        ExecuteQueryButton.Location = new Point(9, 64);
        ExecuteQueryButton.Name = "ExecuteQueryButton";
        ExecuteQueryButton.Size = new Size(116, 40);
        ExecuteQueryButton.TabIndex = 2;
        ExecuteQueryButton.Text = "Execute";
        ExecuteQueryButton.UseVisualStyleBackColor = true;
        ExecuteQueryButton.Click += ExecuteQueryButton_Click;
        // 
        // DashboardTreeView
        // 
        DashboardTreeView.Location = new Point(12, 79);
        DashboardTreeView.Name = "DashboardTreeView";
        DashboardTreeView.Size = new Size(427, 812);
        DashboardTreeView.TabIndex = 0;
        // 
        // QueryResultTab
        // 
        QueryResultTab.Controls.Add(QueryResultTabGrid);
        QueryResultTab.Controls.Add(QueryResultTabMessages);
        QueryResultTab.Location = new Point(436, 571);
        QueryResultTab.Name = "QueryResultTab";
        QueryResultTab.SelectedIndex = 0;
        QueryResultTab.Size = new Size(1184, 320);
        QueryResultTab.TabIndex = 0;
        // 
        // QueryResultTabGrid
        // 
        QueryResultTabGrid.Controls.Add(QueryResultGrid);
        QueryResultTabGrid.Location = new Point(4, 39);
        QueryResultTabGrid.Name = "QueryResultTabGrid";
        QueryResultTabGrid.Size = new Size(1176, 277);
        QueryResultTabGrid.TabIndex = 0;
        QueryResultTabGrid.Text = "Query Result";
        QueryResultTabGrid.UseVisualStyleBackColor = true;
        // 
        // QueryResultGrid
        // 
        QueryResultGrid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        dataGridViewCellStyle1.Alignment = DataGridViewContentAlignment.MiddleLeft;
        dataGridViewCellStyle1.BackColor = SystemColors.Window;
        dataGridViewCellStyle1.Font = new Font("Segoe UI", 9F);
        dataGridViewCellStyle1.ForeColor = SystemColors.ControlText;
        dataGridViewCellStyle1.SelectionBackColor = Color.LightBlue;
        dataGridViewCellStyle1.SelectionForeColor = Color.Black;
        dataGridViewCellStyle1.WrapMode = DataGridViewTriState.False;
        QueryResultGrid.DefaultCellStyle = dataGridViewCellStyle1;
        QueryResultGrid.Location = new Point(4, 0);
        QueryResultGrid.Name = "QueryResultGrid";
        QueryResultGrid.RowHeadersWidth = 72;
        QueryResultGrid.Size = new Size(1169, 288);
        QueryResultGrid.TabIndex = 3;
        // 
        // QueryResultTabMessages
        // 
        QueryResultTabMessages.Controls.Add(QueryResultTabMessagesLabel);
        QueryResultTabMessages.Location = new Point(4, 39);
        QueryResultTabMessages.Name = "QueryResultTabMessages";
        QueryResultTabMessages.Size = new Size(1176, 277);
        QueryResultTabMessages.TabIndex = 0;
        QueryResultTabMessages.Text = "Messages";
        QueryResultTabMessages.UseVisualStyleBackColor = true;
        // 
        // QueryResultTabMessagesLabel
        // 
        QueryResultTabMessagesLabel.AutoSize = true;
        QueryResultTabMessagesLabel.Location = new Point(16, 18);
        QueryResultTabMessagesLabel.Name = "QueryResultTabMessagesLabel";
        QueryResultTabMessagesLabel.Size = new Size(0, 30);
        QueryResultTabMessagesLabel.TabIndex = 3;
        // 
        // QueryPanel
        // 
        QueryPanel.BackColor = Color.White;
        QueryPanel.BorderStyle = BorderStyle.FixedSingle;
        QueryPanel.Controls.Add(RefreshButton);
        QueryPanel.Controls.Add(ExecuteQueryButton);
        QueryPanel.Location = new Point(438, 37);
        QueryPanel.Name = "QueryPanel";
        QueryPanel.Size = new Size(1180, 114);
        QueryPanel.TabIndex = 3;
        // 
        // menuStrip1
        // 
        menuStrip1.ImageScalingSize = new Size(28, 28);
        menuStrip1.Location = new Point(0, 0);
        menuStrip1.Name = "menuStrip1";
        menuStrip1.Size = new Size(1639, 24);
        menuStrip1.TabIndex = 0;
        menuStrip1.Text = "menuStrip1";
        // 
        // ObjectExplorerPanel
        // 
        ObjectExplorerPanel.BackColor = Color.White;
        ObjectExplorerPanel.BorderStyle = BorderStyle.FixedSingle;
        ObjectExplorerPanel.Controls.Add(ObjectExplorerLabel);
        ObjectExplorerPanel.Location = new Point(12, 37);
        ObjectExplorerPanel.Name = "ObjectExplorerPanel";
        ObjectExplorerPanel.Size = new Size(427, 44);
        ObjectExplorerPanel.TabIndex = 4;
        // 
        // ObjectExplorerLabel
        // 
        ObjectExplorerLabel.AutoSize = true;
        ObjectExplorerLabel.Location = new Point(3, 4);
        ObjectExplorerLabel.Name = "ObjectExplorerLabel";
        ObjectExplorerLabel.Size = new Size(155, 30);
        ObjectExplorerLabel.TabIndex = 0;
        ObjectExplorerLabel.Text = "Object Explorer";
        // 
        // RefreshButton
        // 
        RefreshButton.Location = new Point(131, 64);
        RefreshButton.Name = "RefreshButton";
        RefreshButton.Size = new Size(131, 40);
        RefreshButton.TabIndex = 3;
        RefreshButton.Text = "Refresh";
        RefreshButton.UseVisualStyleBackColor = true;
        RefreshButton.Click += RefreshButton_Click;
        // 
        // DashboardForm
        // 
        AutoScaleDimensions = new SizeF(12F, 30F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(1639, 910);
        Controls.Add(ObjectExplorerPanel);
        Controls.Add(QueryPanel);
        Controls.Add(QueryResultTab);
        Controls.Add(DashboardTreeView);
        Controls.Add(QueryEditorField);
        Controls.Add(menuStrip1);
        MainMenuStrip = menuStrip1;
        Name = "DashboardForm";
        Text = "Lotus";
        Load += DashboardForm_Load;
        QueryResultTab.ResumeLayout(false);
        QueryResultTabGrid.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)QueryResultGrid).EndInit();
        QueryResultTabMessages.ResumeLayout(false);
        QueryResultTabMessages.PerformLayout();
        QueryPanel.ResumeLayout(false);
        ObjectExplorerPanel.ResumeLayout(false);
        ObjectExplorerPanel.PerformLayout();
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion
    private RichTextBox QueryEditorField;
    private Button ExecuteQueryButton;
    private TreeView DashboardTreeView;
    private TabControl QueryResultTab;
    private TabPage QueryResultTabGrid;
    private TabPage QueryResultTabMessages;
    private DataGridView QueryResultGrid;
    private Label QueryResultTabMessagesLabel;
    private Panel QueryPanel;
    private MenuStrip menuStrip1;
    private Panel ObjectExplorerPanel;
    private Label ObjectExplorerLabel;
    private Button RefreshButton;
}

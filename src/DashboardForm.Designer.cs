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
        DataGridViewCellStyle dataGridViewCellStyle2 = new DataGridViewCellStyle();
        menuStrip1 = new MenuStrip();
        QueryEditorField = new RichTextBox();
        ExecuteQueryButton = new Button();
        DashboardTreeView = new TreeView();
        QueryResultTab = new TabControl();
        QueryResultTabGrid = new TabPage();
        QueryResultGrid = new DataGridView();
        QueryResultTabMessages = new TabPage();
        QueryResultTabMessagesLabel = new Label();
        QueryResultTab.SuspendLayout();
        QueryResultTabGrid.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)QueryResultGrid).BeginInit();
        QueryResultTabMessages.SuspendLayout();
        SuspendLayout();
        // 
        // menuStrip1
        // 
        menuStrip1.ImageScalingSize = new Size(28, 28);
        menuStrip1.Location = new Point(0, 0);
        menuStrip1.Name = "menuStrip1";
        menuStrip1.Size = new Size(1353, 24);
        menuStrip1.TabIndex = 0;
        menuStrip1.Text = "menuStrip1";
        // 
        // QueryEditorField
        // 
        QueryEditorField.Font = new Font("Consolas", 9F);
        QueryEditorField.Location = new Point(363, 89);
        QueryEditorField.Name = "QueryEditorField";
        QueryEditorField.Size = new Size(971, 289);
        QueryEditorField.TabIndex = 1;
        QueryEditorField.Text = "";
        QueryEditorField.TextChanged += QueryEditorField_TextChanged;
        QueryEditorField.SelectionIndent = 16;
        QueryEditorField.SelectionRightIndent = 16;

        // 
        // ExecuteQueryButton
        // 
        ExecuteQueryButton.Location = new Point(363, 43);
        ExecuteQueryButton.Name = "ExecuteQueryButton";
        ExecuteQueryButton.Size = new Size(150, 40);
        ExecuteQueryButton.TabIndex = 2;
        ExecuteQueryButton.Text = "Execute";
        ExecuteQueryButton.UseVisualStyleBackColor = true;
        ExecuteQueryButton.Click += ExecuteQueryButton_Click;
        // 
        // DashboardTreeView
        // 
        DashboardTreeView.Location = new Point(12, 43);
        DashboardTreeView.Name = "DashboardTreeView";
        DashboardTreeView.Size = new Size(333, 642);
        DashboardTreeView.TabIndex = 0;
        // 
        // QueryResultTab
        // 
        QueryResultTab.Controls.Add(QueryResultTabGrid);
        QueryResultTab.Controls.Add(QueryResultTabMessages);
        QueryResultTab.Location = new Point(363, 384);
        QueryResultTab.Name = "QueryResultTab";
        QueryResultTab.SelectedIndex = 0;
        QueryResultTab.Size = new Size(978, 301);
        QueryResultTab.TabIndex = 0;
        // 
        // QueryResultTabGrid
        // 
        QueryResultTabGrid.Controls.Add(QueryResultGrid);
        QueryResultTabGrid.Location = new Point(4, 39);
        QueryResultTabGrid.Name = "QueryResultTabGrid";
        QueryResultTabGrid.Size = new Size(970, 258);
        QueryResultTabGrid.TabIndex = 0;
        QueryResultTabGrid.Text = "Query Result";
        QueryResultTabGrid.UseVisualStyleBackColor = true;
        // 
        // QueryResultGrid
        // 
        QueryResultGrid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        dataGridViewCellStyle2.Alignment = DataGridViewContentAlignment.MiddleLeft;
        dataGridViewCellStyle2.BackColor = SystemColors.Window;
        dataGridViewCellStyle2.Font = new Font("Segoe UI", 9F);
        dataGridViewCellStyle2.ForeColor = SystemColors.ControlText;
        dataGridViewCellStyle2.SelectionBackColor = Color.LightBlue;
        dataGridViewCellStyle2.SelectionForeColor = Color.Black;
        dataGridViewCellStyle2.WrapMode = DataGridViewTriState.False;
        QueryResultGrid.DefaultCellStyle = dataGridViewCellStyle2;
        QueryResultGrid.Location = new Point(4, 0);
        QueryResultGrid.Name = "QueryResultGrid";
        QueryResultGrid.RowHeadersWidth = 72;
        QueryResultGrid.Size = new Size(963, 255);
        QueryResultGrid.TabIndex = 3;
        // 
        // QueryResultTabMessages
        // 
        QueryResultTabMessages.Controls.Add(QueryResultTabMessagesLabel);
        QueryResultTabMessages.Location = new Point(4, 39);
        QueryResultTabMessages.Name = "QueryResultTabMessages";
        QueryResultTabMessages.Size = new Size(970, 258);
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
        // DashboardForm
        // 
        AutoScaleDimensions = new SizeF(12F, 30F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(1353, 697);
        Controls.Add(QueryResultTab);
        Controls.Add(DashboardTreeView);
        Controls.Add(ExecuteQueryButton);
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
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private MenuStrip menuStrip1;
    private RichTextBox QueryEditorField;
    private Button ExecuteQueryButton;
    private TreeView DashboardTreeView;
    private TabControl QueryResultTab;
    private TabPage QueryResultTabGrid;
    private TabPage QueryResultTabMessages;
    private DataGridView QueryResultGrid;
    private Label QueryResultTabMessagesLabel;
}

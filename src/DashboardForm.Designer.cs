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
        menuStrip1.Size = new Size(1123, 24);
        menuStrip1.TabIndex = 0;
        menuStrip1.Text = "menuStrip1";
        // 
        // QueryEditorField
        // 
        QueryEditorField.Location = new Point(363, 89);
        QueryEditorField.Name = "QueryEditorField";
        QueryEditorField.Size = new Size(748, 268);
        QueryEditorField.TabIndex = 1;
        QueryEditorField.Text = "";
        QueryEditorField.TextChanged += QueryEditorField_TextChanged;
        QueryEditorField.SelectionIndent = 10;
        QueryEditorField.SelectionRightIndent = 10;
        // 
        // ExecuteQueryButton
        // 
        ExecuteQueryButton.Location = new Point(363, 43);
        ExecuteQueryButton.Name = "ExecuteQueryButton";
        ExecuteQueryButton.Size = new Size(166, 40);
        ExecuteQueryButton.TabIndex = 2;
        ExecuteQueryButton.Text = "Execute Query";
        ExecuteQueryButton.UseVisualStyleBackColor = true;
        ExecuteQueryButton.Click += ExecuteQueryButton_Click;
        // 
        // DashboardTreeView
        // 
        DashboardTreeView.Location = new Point(12, 43);
        DashboardTreeView.Name = "DashboardTreeView";
        DashboardTreeView.Size = new Size(333, 577);
        DashboardTreeView.TabIndex = 0;
        // 
        // QueryResultTab
        // 
        QueryResultTab.Controls.Add(QueryResultTabGrid);
        QueryResultTab.Controls.Add(QueryResultTabMessages);
        QueryResultTab.Location = new Point(363, 384);
        QueryResultTab.Name = "QueryResultTab";
        QueryResultTab.SelectedIndex = 0;
        QueryResultTab.Size = new Size(748, 230);
        QueryResultTab.TabIndex = 0;
        // 
        // QueryResultTabGrid
        // 
        QueryResultTabGrid.Controls.Add(QueryResultGrid);
        QueryResultTabGrid.Location = new Point(4, 39);
        QueryResultTabGrid.Name = "QueryResultTabGrid";
        QueryResultTabGrid.Size = new Size(740, 187);
        QueryResultTabGrid.TabIndex = 0;
        QueryResultTabGrid.Text = "Query Result";
        QueryResultTabGrid.UseVisualStyleBackColor = true;
        // 
        // QueryResultGrid
        // 
        QueryResultGrid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        QueryResultGrid.Location = new Point(4, 0);
        QueryResultGrid.Name = "QueryResultGrid";
        QueryResultGrid.RowHeadersWidth = 72;
        QueryResultGrid.Size = new Size(733, 181);
        QueryResultGrid.TabIndex = 3;
        // 
        // QueryResultTabMessages
        // 
        QueryResultTabMessages.Controls.Add(QueryResultTabMessagesLabel);
        QueryResultTabMessages.Location = new Point(4, 39);
        QueryResultTabMessages.Name = "QueryResultTabMessages";
        QueryResultTabMessages.Size = new Size(740, 187);
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
        ClientSize = new Size(1123, 632);
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

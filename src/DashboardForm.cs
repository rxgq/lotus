using lotus.src.Database;
using lotus.src.Enums;
using lotus.src.Models;

namespace lotus;

public partial class DashboardForm : Form
{
    private readonly DatabaseEngine _engine = new();

    public DashboardForm()
    {
        InitializeComponent();
    }

    private void DashboardForm_Load(object sender, EventArgs e)
    {
        var table = new DatabaseTable()
        {
            Name = "flowers",
            Columns = [
                new() {
                    Title = "flower_name",
                    DataType = DataColumnType.VarChar
                },
                new() {
                    Title = "flower_count",
                    DataType = DataColumnType.Int
                }
            ],
            Rows = [
                new() {
                    Values = new() {
                        { "flower_name", "aster" },
                        { "flower_count", 3 },
                    }
                },
                new() {
                    Values = new() {
                        { "flower_name", "tulip" },
                        { "flower_count", 8 },
                    }
                },
                new() {
                    Values = new() {
                        { "flower_name", "daisy" },
                        { "flower_count", 2 },
                    }
                }
            ],
        };

        _engine.CreateTable(table);

        InitialiseTreeNodes();
    }

    private void InitialiseTreeNodes()
    {
        DashboardTreeView.Nodes.Add(new TreeNode()
        {
            Text = "tables",
            Name = "tables"
        });

        foreach (var table in _engine.Tables)
        {
            var node = DashboardTreeView.Nodes.Find("tables", false).First();

            node.Nodes.Add(new TreeNode()
            {
                Text = table.Name
            });
        }
    }

    private void RefreshTables()
    {
        var tablesNode = DashboardTreeView.Nodes
            .Find("tables", false).FirstOrDefault();

        if (tablesNode is not null)
        {
            tablesNode.Nodes.Clear();

            foreach (var table in _engine.Tables)
            {
                tablesNode.Nodes.Add(new TreeNode()
                {
                    Text = table.Name
                });
            }
        }
        else
        {
            InitialiseTreeNodes();
        }
    }


    private void ExecuteQueryButton_Click(object sender, EventArgs e)
    {
        var stopwatch = new System.Diagnostics.Stopwatch();
        stopwatch.Start();

        try
        {
            var results = _engine.ParseSql(QueryEditorField.Text);

            RefreshTables();
            foreach (var result in results)
            {
                QueryResultGrid.Rows.Clear();
                QueryResultGrid.Columns.Clear();

                if (!result.IsSuccess)
                {
                    QueryResultTabMessagesLabel.Text = result.Message;
                    continue;
                }

                var rows = result.Value;

                foreach (var columnName in result.TableAffected.Columns.Select(x => x.Title))
                {
                    QueryResultGrid.Columns.Add(columnName, columnName);
                }

                foreach (var row in rows ?? [])
                {
                    var values = row.Values.Select(x => x.Value ?? "NULL").ToArray();
                    QueryResultGrid.Rows.Add(values);
                }

                stopwatch.Stop();
                var elapsedTime = stopwatch.ElapsedMilliseconds;

                QueryResultTabMessagesLabel.Text = $"Query completed in {elapsedTime} ms.";
            }
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            QueryResultTabMessagesLabel.Text = $"Error: {ex.Message}";
        }
    }

    private void QueryEditorField_TextChanged(object sender, EventArgs e)
    {

    }
}

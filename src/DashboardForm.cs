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
        var table = new DatabaseTable() {
            Name = "flowers",
            Columns = [
                new() { 
                    Title = "flower_name",
                    DataType = DataColumnType.String
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
        DashboardTreeView.Nodes.Add(new TreeNode() { 
            Text = "tables",
            Name = "tables"
        });

        foreach (var table in _engine.Tables) {
            var node = DashboardTreeView.Nodes.Find("tables", false).First();
            
            node.Nodes.Add(new TreeNode() { 
                Text = table.Name 
            });
        }
    }

    private void ExecuteQueryButton_Click(object sender, EventArgs e)
    {
        QueryResultGrid.Rows.Clear();
        QueryResultGrid.Columns.Clear();

        var stopwatch = new System.Diagnostics.Stopwatch();
        stopwatch.Start();

        try
        {
            var results = _engine.ParseSql(QueryEditorField.Text);

            foreach (var result in results)
            {
                if (!result.IsSuccess) 
                {
                    QueryResultTabMessagesLabel.Text = result.Message;
                    continue;
                }

                var rows = result.Value;

                if (rows.Count > 0)
                {
                    foreach (var columnName in rows.First().Values.Keys)
                    {
                        QueryResultGrid.Columns.Add(columnName, columnName);
                    }

                    foreach (var row in rows)
                    {
                        var values = row.Values.Select(x => x.Value).ToArray();
                        QueryResultGrid.Rows.Add(values);
                    }
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


}

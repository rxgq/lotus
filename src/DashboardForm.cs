using lotus.src.Database;
using lotus.src.Enums;
using lotus.src.Models;
using lotus.src.Sql.Utils;
using System.Diagnostics;
using System.Linq;

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
                        { "flower_name", "aster" },
                        { "flower_count", 4 },
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
                        { "flower_count", 8 },
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
        tablesNode.Nodes.Clear();

        foreach (var table in _engine.Tables)
        {
            tablesNode.Nodes.Add(new TreeNode() {
                Text = table.Name
            });
        }
    }

    private void ExecuteQueryButton_Click(object sender, EventArgs e)
    {
        var stopwatch = new Stopwatch();
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

                AddColumnsAndRows(result);

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

    private void AddColumnsAndRows(QueryResult<List<DatabaseRow>> result)
    {
        foreach (var columnName in result.TableResult.Columns.Select(x => x.Title))
        {
            QueryResultGrid.Columns.Add(columnName, columnName);
        }

        foreach (var row in result.Value ?? [])
        {
            var values = result.TableResult.Columns
                .Select(column =>
                {
                    row.Values.TryGetValue(column.Title, out var value);
                    return value ?? "NULL";
                })
                .ToArray();

            if (!values.Contains("NULL"))
            {
                QueryResultGrid.Rows.Add(values);
            }
        }
    }

}

using lotus.src.Database;
using lotus.src.Database.Enums;
using lotus.src.Database.Models;
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
        InitialiseTreeNodes();
    }

    private void InitialiseTreeNodes()
    {
        DashboardTreeView.Nodes.Add(new TreeNode()
        {
            Text = "Databases",
            Name = "Databases"
        });

        foreach (var database in _engine.Databases) 
        {
            var dbNode = DashboardTreeView.Nodes.Find("Databases", false).First();
            dbNode.Nodes.Add(new TreeNode()
            {
                Text = "Tables",
                Name = "Tables"
            });

            foreach (var table in database.Tables)
            {
                var node = DashboardTreeView.Nodes.Find("Tables", false).First();

                node.Nodes.Add(new TreeNode()
                {
                    Text = table.Name
                });
            }
        }
    }

    private void RefreshNodes()
    {
        DashboardTreeView.Nodes.Clear();

        var databasesNode = new TreeNode("Databases")
        {
            Name = "Databases"
        };

        foreach (var db in _engine.Databases)
        {
            var dbNode = new TreeNode(db.Name)
            {
                Name = db.Name
            };

            var tablesNode = new TreeNode("Tables")
            {
                Name = "Tables"
            };

            foreach (var table in db.Tables)
            {
                tablesNode.Nodes.Add(new TreeNode
                {
                    Text = table.Name,
                    Name = table.Name
                });
            }

            dbNode.Nodes.Add(tablesNode);

            databasesNode.Nodes.Add(dbNode);
        }

        DashboardTreeView.Nodes.Add(databasesNode);

        databasesNode.Expand();
    }


    private void ExecuteQueryButton_Click(object sender, EventArgs e)
    {
        var stopwatch = new Stopwatch();
        stopwatch.Start();

        try
        {
            var execResult = _engine.ParseSql(QueryEditorField.Text);
            RefreshNodes();

            foreach (var result in execResult.Results)
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
        if (result.TableResult is null) return;

        foreach (var columnName in result.TableResult.Columns.Select(x => x.Title))
        {
            QueryResultGrid.Columns.Add(columnName, columnName);
        }

        foreach (var row in result.Value ?? [])
        {
            var values = result.TableResult.Columns
                .Select(column => {
                    row.Values.TryGetValue(column.Title, out var value);
                    return value ?? "NULL";
                })
                .ToArray();

            QueryResultGrid.Rows.Add(values);
        }
    }

}

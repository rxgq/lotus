using lotus.src.Database;
using lotus.src.Database.Enums;
using lotus.src.Database.Models;
using lotus.src.Sql.Enums;
using lotus.src.Sql.Utils;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;

namespace lotus;

public partial class DashboardForm : Form
{
    private readonly DatabaseEngine _engine = new();

    public DashboardForm()
    {
        InitializeComponent();
        QueryEditorField.TextChanged += HighlightKeywordsInQuery;
    }

    private void DashboardForm_Load(object sender, EventArgs e)
    {
        try
        {
            InitialiseTreeNodes();

            _engine.ParseSql("""
                create database garden
                use garden

                create table flowers (
                    flower_name varchar,
                    flower_count int,
                    is_available bool,
                )

                insert into flowers (flower_name, flower_count, is_available)
                values ('tulip', 5, false)

                insert into flowers (flower_name, flower_count, is_available)
                values ('daisy', 8, true)
            
                insert into flowers (flower_name, flower_count, is_available)
                values ('aster', 0, true)
            
                insert into flowers (flower_name, flower_count, is_available)
                values ('bluebell', 3, false)

                insert into flowers (flower_name, flower_count, is_available)
                values ('rose', 4, true)

                select * from flowers
            """);

        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error during initialization: {ex.Message}", "Initialization Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }


    private TreeNode GetNode(string name)
    {
        return DashboardTreeView.Nodes.Find(name, false).First();
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
            var dbNode = GetNode("Databases");
            dbNode.Nodes.Add(new TreeNode()
            {
                Text = "Tables",
                Name = "Tables"
            });

            foreach (var table in database.Tables)
            {
                var node = GetNode("Tables");

                node.Nodes.Add(new TreeNode()
                {
                    Text = table.Name
                });
            }
        }
    }

    private void ExecuteQueryButton_Click(object sender, EventArgs e)
    {
        if (QueryEditorField.Text == "") return;

        var stopwatch = new Stopwatch();
        stopwatch.Start();

        try
        {
            var execResult = _engine.ParseSql(QueryEditorField.Text);
            if (execResult.Results is null) 
            {
                QueryResultTabMessagesLabel.Text = execResult.ExecutionErrors[0];
                return;
            }

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
                .Select(column =>
                {
                    row.Values.TryGetValue(column.Title, out var value);
                    return value ?? "NULL";
                })
                .ToArray();

            QueryResultGrid.Rows.Add(values);
        }
    }

    private void RefreshButton_Click(object sender, EventArgs e)
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

    private void HighlightKeywordsInQuery(object sender, EventArgs e)
    {
        var keywordsBlue = new[] {
            "SELECT", "FROM", "CREATE", "TABLE", "INSERT", "INTO", "VALUES", "DROP", "ALTER",
            "COLUMN", "ADD", "RENAME", "TO", "DELETE", "WHERE", "AND", "OR", "NOT", "LIMIT",
            "DISTINCT", "USE", "DATABASE"
        };

        var keywordsPurple = new[] {
            "VARCHAR", "INT", "BOOL", "DATESTAMP", "FLOAT"
        };

        int originalSelectionStart = QueryEditorField.SelectionStart;
        int originalSelectionLength = QueryEditorField.SelectionLength;

        QueryEditorField.SelectAll();
        QueryEditorField.SelectionColor = Color.Black;
        QueryEditorField.SelectionBackColor = Color.White;

        var regexLiterals = new Regex(@"'([^']*)'", RegexOptions.IgnoreCase);
        foreach (Match match in regexLiterals.Matches(QueryEditorField.Text))
        {
            QueryEditorField.Select(match.Index, match.Length);
            QueryEditorField.SelectionColor = Color.Green;
        }

        foreach (var keyword in keywordsBlue)
        {
            var regex = new Regex($@"\b{Regex.Escape(keyword)}\b", RegexOptions.IgnoreCase);

            foreach (Match match in regex.Matches(QueryEditorField.Text))
            {
                if (!IsInsideStringLiteral(match.Index, regexLiterals))
                {
                    QueryEditorField.Select(match.Index, match.Length);
                    QueryEditorField.SelectionColor = Color.Blue;
                }
            }
        }

        foreach (var keyword in keywordsPurple)
        {
            var regex = new Regex($@"\b{Regex.Escape(keyword)}\b", RegexOptions.IgnoreCase);

            foreach (Match match in regex.Matches(QueryEditorField.Text))
            {
                if (!IsInsideStringLiteral(match.Index, regexLiterals))
                {
                    QueryEditorField.Select(match.Index, match.Length);
                    QueryEditorField.SelectionColor = Color.Purple;
                }
            }
        }

        QueryEditorField.Select(originalSelectionStart, originalSelectionLength);
    }

    private bool IsInsideStringLiteral(int position, Regex literalRegex)
    {
        foreach (Match literalMatch in literalRegex.Matches(QueryEditorField.Text))
        {
            if (position >= literalMatch.Index && position < literalMatch.Index + literalMatch.Length)
            {
                return true;
            }
        }
        return false;
    }
}

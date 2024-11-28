using lotus.src.Database.Models;
using lotus.src.Sql.Utils;

namespace lotus.src.Database.Utils;

public sealed class ExecutionResult(List<QueryResult<List<DatabaseRow>>> results, List<string> executionErrors)
{
    public readonly List<QueryResult<List<DatabaseRow>>> Results = results;
    public readonly List<string> ExecutionErrors = executionErrors;
}

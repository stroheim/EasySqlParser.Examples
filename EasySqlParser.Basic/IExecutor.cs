using System;
using System.Collections.Generic;
using System.Text;

namespace EasySqlParser.Basic
{
    public interface IExecutor
    {
        void Execute(string connectionString, SqlParserResult parserResult);

        void ExecutePaginated(string connectionString, SqlParserResult parserResult, bool canDisplayRowNumber);

        void ExecuteCount(string connectionString, SqlParserResult parserResult);
    }
}

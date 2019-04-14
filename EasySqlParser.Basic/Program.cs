using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using EasySqlParser.Configurations;

namespace EasySqlParser.Basic
{
    class Program
    {
        // db connection string
        private const string ConnectionString = @"Data Source=localhost\SQLEXPRESS;Initial Catalog=AdventureWorks2012;Integrated Security=True";

        private const string FilePath = "SelectEmployees.sql";

        static void Main(string[] args)
        {
            // create config
            ConfigContainer.AddDefault(
                DbConnectionKind.SqlServer,
                () => new SqlParameter()
            );
            ConfigContainer.EnableCache = true;
            // Dapper execute
            ExecuteSimple(new DapperExecutor());
            ExecutePaginated(new DapperExecutor());
            ExecutePaginated(new DapperExecutor(), false);

            Console.WriteLine("---- Dapper end");

            // Ef execute
            ExecuteSimple(new EfExecutor());
            ExecutePaginated(new EfExecutor());
            ExecutePaginated(new EfExecutor(), false);

            Console.WriteLine("---- Ef end");

            Console.Read();
        }

        /// <summary>
        /// ExecutePaginated sql file most simple
        /// </summary>
        static void ExecuteSimple(IExecutor executor)
        {
            // create parameter object instance for 2-way-sql
            var condition = new SqlCondition
                            {
                                MiddleNames = new List<string> {"A", "J", "M"}
                            };
            var watch = new Stopwatch();
            watch.Start();
            // create SqlParse instance
            var parser = new SqlParser(FilePath, condition);

            // parse sql file
            var result = parser.Parse();
            watch.Stop();
            Console.WriteLine($"time\t{watch.Elapsed}");

            watch.Reset();

            watch.Start();
            Debug.WriteLine($"+++>{result.DebugSql}");
            executor.Execute(ConnectionString, result);
            watch.Stop();
            Console.WriteLine($"time\t{watch.Elapsed}");

        }

        /// <summary>
        /// Execute for pagination
        /// </summary>
        static void ExecutePaginated(IExecutor executor, bool canDisplayRowNumber = true)
        {

            // create parameter object instance for 2-way-sql
            var condition = new SqlCondition
                            {
                                BirthDateFrom = new DateTime(1980, 1, 1),
                                BirthDateTo = new DateTime(1990, 1, 1)
                            };
            var watch = new Stopwatch();
            watch.Start();
            // create SqlParse instance
            var parser = new SqlParser(FilePath, condition);



            // parse paginated
            string rowNumberColumn = null;
            if (canDisplayRowNumber)
            {
                rowNumberColumn = "SerialNumber";
            }
            var result = parser.ParsePaginated(5, 10, rowNumberColumn);
            watch.Stop();
            Console.WriteLine($"time\t{watch.Elapsed}");

            watch.Reset();

            watch.Start();
            Debug.WriteLine($"+++>{result.Result.DebugSql}");
            Debug.WriteLine($"+++>{result.CountResult.DebugSql}");

            executor.ExecutePaginated(ConnectionString, result.Result, canDisplayRowNumber);
            executor.ExecuteCount(ConnectionString, result.CountResult);
            watch.Stop();
            Console.WriteLine($"time\t{watch.Elapsed}");

            result = parser.ParsePaginated(15, 10, rowNumberColumn);
            watch.Reset();

            watch.Start();
            Debug.WriteLine($"+++>{result.Result.DebugSql}");
            Debug.WriteLine($"+++>{result.CountResult.DebugSql}");
            executor.ExecutePaginated(ConnectionString, result.Result, canDisplayRowNumber);
            executor.ExecuteCount(ConnectionString, result.CountResult);
            watch.Stop();
            Console.WriteLine($"time\t{watch.Elapsed}");
        }
    }
}

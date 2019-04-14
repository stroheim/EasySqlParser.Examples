using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using Dapper;

namespace EasySqlParser.Basic
{
    public class DapperExecutor : IExecutor
    {
        public void Execute(string connectionString, SqlParserResult parserResult)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var result = connection.Query<Employee>(parserResult.ParsedSql,
                    parserResult.DbDataParameters.ToDynamicParameters());
                Console.WriteLine("FirstName\tMiddleName\tLastName\tBirthDate");
                foreach (var employee in result)
                {
                    Console.WriteLine(
                        $"{employee.FirstName}\t{employee.MiddleName}\t{employee.LastName}\t{employee.BirthDate}");
                }

                Console.WriteLine();
            }
        }

        public void ExecutePaginated(string connectionString, SqlParserResult parserResult, bool canDisplayRowNumber)
        {
            if (!canDisplayRowNumber)
            {
                Execute(connectionString, parserResult);
                return;
            }
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var result = connection.Query<EmployeePaginated>(parserResult.ParsedSql,
                    parserResult.DbDataParameters.ToDynamicParameters());

                Console.WriteLine("SerialNumber\tFirstName\tMiddleName\tLastName\tBirthDate");
                foreach (var employee in result)
                {
                    Console.WriteLine(
                        $"{employee.SerialNumber}\t{employee.FirstName}\t{employee.MiddleName}\t{employee.LastName}\t{employee.BirthDate}");
                }

                Console.WriteLine();
            }
        }

        public void ExecuteCount(string connectionString, SqlParserResult parserResult)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var count = connection.ExecuteScalar<int>(parserResult.ParsedSql,
                    parserResult.DbDataParameters.ToDynamicParameters());
                Console.WriteLine($"TotalCount:{count}");
            }
        }
    }

    public static class DapperExtension
    {
        public static DynamicParameters ToDynamicParameters(this List<IDbDataParameter> parameters)
        {
            var result = new DynamicParameters();
            foreach (var parameter in parameters)
            {

                result.Add(parameter.ParameterName,
                    parameter.Value,
                    parameter.DbType,
                    parameter.Direction,
                    parameter.Size,
                    parameter.Precision,
                    parameter.Scale);
            }

            return result;
        }
    }
}

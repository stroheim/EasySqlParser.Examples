using System;
using System.Data;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace EasySqlParser.Basic
{
    public class EfExecutor : IExecutor
    {
        public void Execute(string connectionString, SqlParserResult parserResult)
        {

            using (var context = new EfContext(connectionString))
            {
                var result = context.Employees
                    .AsNoTracking()
                    .FromSql(
                        parserResult.ParsedSql,
                        parserResult.DbDataParameters.Cast<object>().ToArray())
                    .ToList();
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
            using (var context = new EfContext(connectionString))
            {
                var result = context.EmployeePaginatedSet
                    .AsNoTracking()
                    .FromSql(
                        parserResult.ParsedSql,
                        parserResult.DbDataParameters.Cast<object>().ToArray())
                    .ToList();
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
            using (var context = new EfContext(connectionString))
            {
                var count = context.ExecuteScalar<int>(parserResult.ParsedSql, parserResult.DbDataParameters.ToArray());
                Console.WriteLine($"TotalCount:{count}");
            }
        }

    }

    public class EfContext : DbContext
    {
        private readonly string _connectionString;
        public EfContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        public EfContext(DbContextOptions<EfContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Employee> Employees { get; set; }

        public virtual DbSet<EmployeePaginated> EmployeePaginatedSet { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_connectionString);
        }

    }

    public static class EntityExtension
    {
        public static T ExecuteScalar<T>(this DbContext context, string sql, params IDbDataParameter[] parameters)
        {
            context.Database.OpenConnection();
            var connection = context.Database.GetDbConnection();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = sql;
                command.Parameters.Clear();
                command.Parameters.AddRange(parameters);
                var scalar = command.ExecuteScalar();
                if (scalar == null || scalar is DBNull)
                {
                    return default(T);
                }

                return (T) scalar;
            }

        }
    }
}

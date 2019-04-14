using System;
using System.Collections.Generic;

namespace EasySqlParser.Basic
{

    public class SqlCondition
    {
        public List<string> MiddleNames { get; set; }
        public DateTime? BirthDateFrom { get; set; }
        public DateTime? BirthDateTo { get; set; }
        public string FirstName { get; set; }

    }
}

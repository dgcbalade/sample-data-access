using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dapper.Data;
using System.Data;


namespace AA.Common.Internal
{

    using AA.Common.Enums;

    internal class InternalDatabaseHelper : DbContext
    {
        internal InternalDatabaseHelper(string connectionName) : base(connectionName) { }

        internal static CommandType Parse(DbHelperCommandType type)
        {
            return (CommandType)Enum.Parse(typeof(CommandType), type.ToString());
        }
    }
}

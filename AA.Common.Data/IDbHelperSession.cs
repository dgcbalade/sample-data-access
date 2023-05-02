
using System;
using System.Collections.Generic;

namespace AA.Common
{

    using AA.Common.Enums;

    public interface IDbHelperSession
    {
        int Execute(string sql, object parameters = null, DbHelperCommandType type = DbHelperCommandType.Text);
        IEnumerable<dynamic> Query(string sql, object parameters = null, DbHelperCommandType type = DbHelperCommandType.Text);
        IEnumerable<T> Query<T>(string sql, object parameters = null, DbHelperCommandType type = DbHelperCommandType.Text);
        dynamic QuerySingle(string sql, object parameters = null, DbHelperCommandType type = DbHelperCommandType.Text);
        T QuerySingle<T>(string sql, object parameters = null, DbHelperCommandType type = DbHelperCommandType.Text);

    }
}


using System;
using System.Collections.Generic;

namespace AA.Common
{

    using AA.Common.Enums;

    public interface IDbHelper
    {
        void MultiQuery(string sql, object parameters = null, DbHelperCommandType type = DbHelperCommandType.Text, Action<IEnumerable<dynamic>> action = null);

        bool Batch(Action<IDbHelperSession> action);
        bool Batch<T>(out T result, Func<IDbHelperSession, T> action);

        int Execute(string sql, object parameters = null, DbHelperCommandType type = DbHelperCommandType.Text);
        
        IEnumerable<dynamic> Query(string sql, object parameters = null, DbHelperCommandType type = DbHelperCommandType.Text);
        IEnumerable<T> Query<T>(string sql, object parameters = null, DbHelperCommandType type = DbHelperCommandType.Text);

        IEnumerable<dynamic> Query(string sql, int page, int pageSize, out long total, object parameters = null, DbHelperCommandType type = DbHelperCommandType.Text);
        IEnumerable<T> Query<T>(string sql, int page, int pageSize, out long total, object parameters = null, DbHelperCommandType type = DbHelperCommandType.Text);


        dynamic QuerySingle(string sql, object parameters = null, DbHelperCommandType type = DbHelperCommandType.Text);
        T QuerySingle<T>(string sql, object parameters = null, DbHelperCommandType type = DbHelperCommandType.Text);
        
        dynamic QueryValue(string sql, object parameters = null, DbHelperCommandType type = DbHelperCommandType.Text);
    }
}

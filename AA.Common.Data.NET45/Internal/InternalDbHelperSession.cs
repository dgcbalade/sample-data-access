using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Dapper;
using Dapper.Data;
using System.Reflection;


namespace AA.Common.Internal
{
    using AA.Common.Enums;

    /// <summary>
    /// Sesión conectada a una base de datos que ejecuta consultas SQL dentro de un BATCH (y de forma transaccional)
    /// </summary>
    internal class InternalDbHelperSession : IDbHelperSession
    {
        private ISession _session;

        internal InternalDbHelperSession(ISession session) { _session = session; }

        /// <summary>
        /// Ejecuta una consulta SQL del tipo INSERT, DELETE UPDATE dentro de una transacción (es una de las tantas consultas a realizar en un ambiente BATCH)
        /// </summary>
        /// <param name="sql">consulta SQL</param>
        /// <param name="parameters">Parametros de la consula SQL</param>
        /// <returns>resultado de le ejecución de la consulta</returns>
        public int Execute(string sql, object parameters = null, DbHelperCommandType type = DbHelperCommandType.Text)
        {            
            return _session.Execute(sql, parameters, Internal.InternalDatabaseHelper.Parse(type));
        }


        /// <summary>
        /// Ejecuta una consulta SQL dentro de una transacción (es una de las tantas consultas a realizar en un ambiente BATCH) pero que solo retorna un registro
        /// </summary>
        /// <typeparam name="T">Tipo de registro retornado (los campos retornados por la consulta debe tener
        /// el mismo nombre que campos en el tipo)</typeparam>
        /// <param name="sql">consulta SQL</param>
        /// <param name="parameters">Parametros de la consula SQL</param>
        /// <returns>Registro retornado</returns>
        public T QuerySingle<T>(string sql, object parameters = null, DbHelperCommandType type = DbHelperCommandType.Text)
        {
            return _session.Query<T>(sql, parameters, Internal.InternalDatabaseHelper.Parse(type)).ToList().FirstOrDefault();
        }


        /// <summary>
        /// Ejecuta una consulta SQL dentro de una transacción (es una de las tantas consultas a realizar en un ambiente BATCH) pero que solo retorna un registro
        /// </summary>
        /// <param name="sql">consulta SQL</param>
        /// <param name="parameters">Parametros de la consula SQL</param>
        /// <returns>Registro Retornado</returns>
        public dynamic QuerySingle(string sql, object parameters = null, DbHelperCommandType type = DbHelperCommandType.Text)
        {
            return _session.Query(sql, parameters, Internal.InternalDatabaseHelper.Parse(type)).ToList().FirstOrDefault();
        }


        /// <summary>
        ///  Ejecuta una consulta SQL dentro de una transacción (es una de las tantas consultas a realizar en un ambiente BATCH)
        /// </summary>
        /// <typeparam name="T">Tipo de registro retornado (los campos retornados por la consulta debe tener
        /// el mismo nombre que campos en el tipo)</typeparam>
        /// <param name="sql">consulta SQL</param>
        /// <param name="parameters">Parametros de la consula SQL</param>
        /// <returns>Registros Retornados</returns>
        public IEnumerable<T> Query<T>(string sql, object parameters = null, DbHelperCommandType type = DbHelperCommandType.Text)
        {
            return _session.Query<T>(sql, parameters, Internal.InternalDatabaseHelper.Parse(type)).ToList();
        }


        /// <summary>
        /// Ejecuta una consulta SQL dentro de una transacción (es una de las tantas consultas a realizar en un ambiente BATCH)
        /// </summary>
        /// <param name="sql">consulta SQL</param>
        /// <param name="parameters">Parametros de la consula SQL</param>
        /// <returns>registros retornados</returns>
        public IEnumerable<dynamic> Query(string sql, object parameters = null, DbHelperCommandType type = DbHelperCommandType.Text)
        {
            return _session.Query(sql, parameters, Internal.InternalDatabaseHelper.Parse(type)).ToList();
        }

    }
}

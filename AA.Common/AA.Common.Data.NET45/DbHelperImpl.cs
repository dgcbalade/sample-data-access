using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data;
using System.Data.Common;

using Dapper;
using Dapper.Data;
using System.Reflection;
using System.Threading.Tasks;


namespace AA.Common
{

    using AA.Common.Events;
    using AA.Common.Enums;

    /// <summary>
    /// Gestionador de base de datos independiente del motor de base de datos subyacente
    /// </summary>
    public class DbHelperImpl : IDbHelper
    {
        private DbContext _dbContext;

        public EventHandler<DbHelperExceptionEventArgs> OnExceptionOcurred;
        public DbHelperImpl(): this("default")
        {
            
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="connectionName">Nombre del cadena de conexión a la base de datos (connectionStrings en el app.config)</param>
        public DbHelperImpl(string connectionName)            
        {            
            _dbContext = new Internal.InternalDatabaseHelper(connectionName);
        }

        public IEnumerable<dynamic> Query(string sql, int page, int pageSize, out long total, object parameters = null, DbHelperCommandType type = DbHelperCommandType.Text)
        {
            total = 0;

            var result = _dbContext.Query(sql, parameters, Internal.InternalDatabaseHelper.Parse(type));

            total = result.Count();

            return result.Skip(page * pageSize).Take(pageSize).ToList();
        }

        public IEnumerable<T> Query<T>(string sql, int page, int pageSize, out long total, object parameters = null, DbHelperCommandType type = DbHelperCommandType.Text)
        {
            total = 0;

            var result = _dbContext.Query<T>(sql, parameters, Internal.InternalDatabaseHelper.Parse(type));

            total = result.Count();

            return result.Skip(page * pageSize).Take(pageSize).ToList();
        }


        /// <summary>
        /// Permite el ejecutar varias consultas SQL en la base de datos de manera transaccional
        /// </summary>
        /// <param name="action">Acción que se ejecutará (y que realizará las consultas SQL)</param>
        /// <returns>Indicador de exito o fracaso de la ejecución de las consultas</returns>
        public bool Batch(Action<IDbHelperSession> action)
        {
            var result = false;

            _dbContext.Batch((session) =>
            {
                try
                {
                    var _session = new Internal.InternalDbHelperSession(session);
                    session.BeginTransaction(IsolationLevel.ReadCommitted);
                    action(_session);
                    session.CommitTransaction();

                    result = true;
                }
                catch (Exception e) 
                { 
                    session.RollbackTransaction();

                    if (OnExceptionOcurred != null)
                        OnExceptionOcurred(this, new DbHelperExceptionEventArgs(e));
                }
            });

            return result;
        }
      
        /// <summary>
        /// Permite el ejecutar varias consultas SQL en la base de datos de manera transaccional
        /// </summary>
        /// <typeparam name="T">Tipo de registro retornado (los campos retornados por la consulta debe tener
        /// el mismo nombre que campos en el tipo)</typeparam>
        /// <param name="result">Registro retornado</param>
        /// <param name="action">Acción que se ejecutará (y que realizará las consultas SQL)</param>
        /// <returns>Indicador de exito o fracaso de la ejecución de las consultas</returns>
        public bool Batch<T>(out T result, Func<IDbHelperSession,T> action)
        {
            result = _dbContext.Batch<T>((session) =>
            {
                try
                {
                    var _session = new Internal.InternalDbHelperSession(session);

                    session.BeginTransaction();
                    T ret = action(_session);
                    session.CommitTransaction();
                    return ret;
                }
                catch (Exception e) 
                { 
                    session.RollbackTransaction();

                    if (OnExceptionOcurred != null)
                        OnExceptionOcurred(this, new DbHelperExceptionEventArgs(e));
                }
                return default(T);
            });

            return result.Equals(default(T));
        }

        /// <summary>
        /// Permite ejecutar una consulta SQL en la base de datos (de manera transaccional)
        /// </summary>
        /// <param name="sql">Consulta SQL a ejecutar</param>
        /// <param name="parameters">Parametros de la consulta SQL (por ejemplo en ORACLE empiezan con ":" y
        /// en SQL SERVER con un "@"</param>
        /// <returns>Valor devuelto por la consulta</returns>
        public int Execute(string sql, object parameters = null, DbHelperCommandType type = DbHelperCommandType.Text)
        {
            return _dbContext.Execute(sql, parameters, Internal.InternalDatabaseHelper.Parse(type));
        }

        public void MultiQuery(string sql, object parameters = null, DbHelperCommandType type = DbHelperCommandType.Text, Action<IEnumerable<dynamic>> action = null)
        {
            if (action != null)
            {
                using (var cnn = _dbContext.ConnectionFactory.CreateAndOpen())
                {
                    try
                    {
                        using (var result = cnn.QueryMultiple(sql, parameters, commandType: Internal.InternalDatabaseHelper.Parse(type)))
                        {
                            do
                            {
                                var ret = result.Read();
                                if (ret != null) action(ret);
                                else break;

                            } while (true);

                        }
                    }
                    catch
                    {
                        cnn.Close();
                        throw;
                    }
                }
            }
        }

        /// <summary>
        /// Permite el ejecutar una consulta SQL que devuelve un solo valor (ejemplo) un count(*)
        /// </summary>
        /// <param name="sql">Consulta SQL a ejecutar</param>
        /// <param name="parameters">Parametros de la consulta SQL (por ejemplo en ORACLE empiezan con ":" y
        /// en SQL SERVER con un "@"</param>
        /// <returns>valor retornado</returns>
        public dynamic QueryValue(string sql, object parameters = null, DbHelperCommandType type = DbHelperCommandType.Text)
        {
            return _dbContext.Query(sql, parameters, Internal.InternalDatabaseHelper.Parse(type)).ToList().FirstOrDefault();
        }


        /// <summary>
        /// Permite el ejecutar una consulta SQL que devuelve un solo registro
        /// </summary>
        /// <param name="sql">Consulta SQL a ejecutar</param>
        /// <param name="parameters">Parametros de la consulta SQL (por ejemplo en ORACLE empiezan con ":" y
        /// en SQL SERVER con un "@"</param>
        /// <returns>registro retornado</returns>
        public dynamic QuerySingle(string sql, object parameters = null, DbHelperCommandType type = DbHelperCommandType.Text)
        {
            return _dbContext.Query(sql, parameters, Internal.InternalDatabaseHelper.Parse(type)).ToList().FirstOrDefault();
        }


        
        /// <summary>
        /// Permite el ejecutar una consulta SQL que devuelve un solo registro
        /// </summary>
        /// <typeparam name="T">Tipo de registro retornado (los campos retornados por la consulta debe tener
        /// el mismo nombre que campos en el tipo)</typeparam>
        /// <param name="sql">Consulta SQL a ejecutar</param>
        /// <param name="parameters">Parametros de la consulta SQL (por ejemplo en ORACLE empiezan con ":" y
        /// en SQL SERVER con un "@"</param>
        /// <returns>registro retornado</returns>
        public T QuerySingle<T>(string sql, object parameters = null, DbHelperCommandType type = DbHelperCommandType.Text)
        {
            return _dbContext.Query<T>(sql, parameters, Internal.InternalDatabaseHelper.Parse(type)).ToList().FirstOrDefault();
        }

        /// <summary>
        /// Permite el ejecutar una consulta SQL que devuelve un muchos registro
        /// </summary>
        /// <param name="sql">Consulta SQL a ejecutar</param>
        /// <param name="parameters">Parametros de la consulta SQL (por ejemplo en ORACLE empiezan con ":" y
        /// en SQL SERVER con un "@"</param>
        /// <returns>registros retornados</returns>
        public IEnumerable<dynamic> Query(string sql, object parameters = null, DbHelperCommandType type = DbHelperCommandType.Text)
        {
            return _dbContext.Query(sql, parameters, Internal.InternalDatabaseHelper.Parse(type)).ToList();
        }
       
       
        /// <summary>
        /// Permite el ejecutar una consulta SQL que devuelve un muchos registro
        /// </summary>
        /// <typeparam name="T">Tipo de registro retornado (los campos retornados por la consulta debe tener
        /// el mismo nombre que campos en el tipo)</typeparam>
        /// <param name="sql">Consulta SQL a ejecutar</param>
        /// <param name="parameters">Parametros de la consulta SQL (por ejemplo en ORACLE empiezan con ":" y
        /// en SQL SERVER con un "@"</param>
        /// <returns>registros retornados</returns>
        public IEnumerable<T> Query<T>(string sql, object parameters = null, DbHelperCommandType type = DbHelperCommandType.Text)
        {

            return _dbContext.Query<T>(sql, parameters, Internal.InternalDatabaseHelper.Parse(type)).ToList();
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Dapper;
using Dapper.Contrib.Extensions;
using MySql.Data.MySqlClient;

namespace VueNote.Core.Util
{
    public static class DbHelper
    {
        static DbHelper()
        {
            SqlMapperExtensions.TableNameMapper = (Type type) =>
            {
                var tableAttribute = type.GetCustomAttributes(false).OfType<TableAttribute>().FirstOrDefault();
                if (tableAttribute != null)
                {
                    return tableAttribute.Name;
                }
                else
                {
                    return type.Name;
                }
            };
        }

        private static MySqlConnection CreateConnection()
        {
            var connection = new MySqlConnection(ConfigHelper.ConnectionString);
            return connection;
        }

        public static async Task<IEnumerable<T>> Query<T>(string sql, object param = null)
        {
            using (var connection = CreateConnection())
            {
                await connection.OpenAsync();

                if (typeof(T).IsPrimitive)
                {
                    var records = await connection.QueryAsync<T>(sql, param);
                    return records;
                }
                else
                {
                    var records = await connection.QueryAsync<dynamic>(sql, param);
                    if (records == null)
                    {
                        return null;
                    }
                    else
                    {
                        // 将关联表数据映射为对象的属性。这里关闭keepCache选项，Slapper缓存机制有bug，可能导致拿到错误结果
                        var result = Slapper.AutoMapper.MapDynamic<T>(records, keepCache: false);
                        return result;
                    }
                }
            }
        }

        public static async Task<T> QueryFirst<T>(string sql, object param = null)
        {
            using (var connection = CreateConnection())
            {
                await connection.OpenAsync();

                var records = await connection.QueryAsync<dynamic>(sql, param);
                if (records == null)
                {
                    return default(T);
                }
                else
                {
                    // 将关联表数据映射为对象的属性。这里关闭keepCache选项，Slapper缓存机制有bug，可能导致拿到错误结果
                    var result = Slapper.AutoMapper.MapDynamic<T>(records, keepCache: false)
                        .FirstOrDefault();
                    return result;
                }
            }
        }
        public static async Task<Pageable<T>> QueryPageable<T>(string sql, object param = null, int pageSize = 10, int pageNum = 1, string rankBy = "Id")
        {
            using (var connection = CreateConnection())
            {
                await connection.OpenAsync();

                var result = new Pageable<T>();

                int begin = (pageNum - 1) * pageSize + 1;
                int end = begin + pageSize - 1;

                string pageRecordsSql = $@"
                select * from(
                    select *, dense_rank() over(order by {rankBy}) as rankId
                    from(
                        {sql}
                    ) as rawRecords
                ) as rankedRecords
                where rankedRecords.rankId between {begin} and {end}";
                result.PageRecords = await Query<T>(pageRecordsSql, param);

                string totalRecordCountSql = $@"
                select max(rankId) from(
                    select *, dense_rank() over(order by {rankBy}) as rankId
                    from(
                        {sql}
                    ) as rawRecords
                ) as rankedRecords";
                result.TotalCount = await connection.ExecuteScalarAsync<int>(totalRecordCountSql, param);

                return result;
            }
        }

        public static async Task<T> Get<T>(object id) where T : class
        {
            using (var connection = CreateConnection())
            {
                await connection.OpenAsync();
                var result = await connection.GetAsync<T>(id);
                return result;
            }
        }
        public static async Task<long> Insert<T>(T entity) where T : class
        {
            using (var connection = CreateConnection())
            {
                await connection.OpenAsync();
                var result = await connection.InsertAsync<T>(entity);
                return result;
            }
        }
        public static async Task<bool> Update<T>(T entity) where T : class
        {
            using (var connection = CreateConnection())
            {
                await connection.OpenAsync();
                var result = await connection.UpdateAsync<T>(entity);
                return result;
            }
        }
        public static async Task<bool> Delete<T>(T entity) where T : class
        {
            using (var connection = CreateConnection())
            {
                await connection.OpenAsync();
                var result = await connection.DeleteAsync<T>(entity);
                return result;
            }
        }

        public static async Task<int> Execute(string sql, object param = null, bool useTransaction = false)
        {
            using (var connection = CreateConnection())
            {
                await connection.OpenAsync();
                if (useTransaction)
                {
                    var transaction = await connection.BeginTransactionAsync();
                    using (transaction)
                    {
                        try
                        {
                            var result = await connection.ExecuteAsync(sql, param);
                            transaction.Commit();
                            return result;
                        }
                        catch
                        {
                            transaction.Rollback();
                            throw;
                        }
                    }
                }
                else
                {
                    var result = await connection.ExecuteAsync(sql, param);
                    return result;
                }
            }
        }
        public static async Task<T> ExecuteScalar<T>(string sql, object param = null, bool useTransaction = false)
        {
            using (var connection = CreateConnection())
            {
                await connection.OpenAsync();
                if (useTransaction)
                {
                    var transaction = await connection.BeginTransactionAsync();
                    using (transaction)
                    {
                        try
                        {
                            var result = await connection.ExecuteScalarAsync<T>(sql, param);
                            transaction.Commit();
                            return result;
                        }
                        catch
                        {
                            transaction.Rollback();
                            throw;
                        }
                    }
                }
                else
                {
                    var result = await connection.ExecuteScalarAsync<T>(sql, param);
                    return result;
                }
            }
        }
        public static async Task ExecuteAction(Action action)
        {
            using (var connection = CreateConnection())
            {
                await connection.OpenAsync();
                var transaction = await connection.BeginTransactionAsync();
                using (transaction)
                {
                    try
                    {
                        action.Invoke();
                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }
        public static async Task<T> ExecuteAction<T>(Func<T> action)
        {
            using (var connection = CreateConnection())
            {
                await connection.OpenAsync();
                var transaction = await connection.BeginTransactionAsync();
                using (transaction)
                {
                    try
                    {
                        T result = action.Invoke();
                        transaction.Commit();
                        return result;
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }
    }
}

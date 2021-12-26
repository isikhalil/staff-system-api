using Dapper;
using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace WishaLink.Models
{
    public static class Entity
	{
        #region Init
        
		public static IDbConnection Connection => new SqlConnection(ConnectionStr);

        private static string ConnectionStr { get; set; }

		public static void InitConnection(string connectionString)
		{
			ConnectionStr = connectionString;
		}

		#endregion

		#region Methods

		public static async Task<int> ExecAsync(string sql, object param = null)
		{
			int result = 0;

			using (IDbConnection conn = Connection)
			{
				try
				{
					if (conn.State != ConnectionState.Open) conn.Open();

					result = await conn.ExecuteAsync(sql, param);
				}
				catch (Exception e)
				{
					System.Console.WriteLine(e);
				}
				finally
				{
					conn.Close();
				}
			}

			return result;
		}

		public static async Task<T> ScalarAsync<T>(string sql, object param = null)
		{
			T result;

			using (IDbConnection conn = Connection)
			{
				try
				{
					if (conn.State != ConnectionState.Open) conn.Open();

					result = await conn.ExecuteScalarAsync<T>(sql, param);
				}
				finally
				{
					conn.Close();
				}
			}

			return result;
		}

		public static T One<T>(string sql, object param = null)
		{
			T result;

			using (IDbConnection conn = Connection)
			{
				try
				{
					if (conn.State != ConnectionState.Open) conn.Open();

					result = conn.QueryFirstOrDefault<T>(sql, param);
				}
				finally
				{
					conn.Close();
				}
			}
			return result;
		}

		public static List<T> All<T>(string sql, object param = null)
		{
			var result = new List<T>();

			using (IDbConnection conn = Connection)
			{
				try
				{
					if (conn.State != ConnectionState.Open) conn.Open();

					result = conn.Query<T>(sql, param).AsList();
				}
				finally
				{
					conn.Close();
				}
			}
			return result;
		}

		public static T Get<T>(long id) where T : class
		{
			T result;

			using (IDbConnection conn = Connection)
			{
				try
				{
					if (conn.State != ConnectionState.Open) conn.Open();

					result = conn.Get<T>(id);
				}
				finally
				{
					conn.Close();
				}
			}

			return result;
		}

		public static List<T> GetAll<T>() where T : class
		{
			List<T> result = new List<T>();

			using (IDbConnection conn = Connection)
			{
				try
				{
					if (conn.State != ConnectionState.Open) conn.Open();

					result = conn.GetAll<T>().ToList();
				}
				finally
				{
					conn.Close();
				}
			}

			return result;
		}

		public static long Insert<T>(T value) where T : class
		{
			long result = 0;

			using (IDbConnection conn = Connection)
			{
				try
				{
					if (conn.State != ConnectionState.Open) conn.Open();

					result = conn.Insert(value);
				}
				catch (Exception e)
				{
                    Console.WriteLine(e);
				}
				finally
				{
					conn.Close();
				}
			}

			return result;
		}

		public static bool Update<T>(T value) where T : class
		{
			bool result = false;

			using (IDbConnection conn = Connection)
			{
				try
				{
					if (conn.State != ConnectionState.Open) conn.Open();

					result = conn.Update(value);
				}
				catch (Exception e)
				{
                    Console.WriteLine(e);
				}
				finally
				{
					conn.Close();
				}
			}

			return result;
		}

		public static bool Delete<T>(T value) where T : class
		{
			bool result;

			using (IDbConnection conn = Connection)
			{
				try
				{
					if (conn.State != ConnectionState.Open) conn.Open();

					result = conn.Delete(value);
				}
				finally
				{
					conn.Close();
				}
			}

			return result;
		}

		#endregion
	}
}
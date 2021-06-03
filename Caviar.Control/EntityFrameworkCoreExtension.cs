using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.Control
{
    public static class EntityFrameworkCoreExtension
    {
		private static DbCommand CreateCommand(DatabaseFacade facade, string sql, out DbConnection connection, params object[] parameters)
		{
			var conn = facade.GetDbConnection();
			connection = conn;
			conn.Open();
			var cmd = conn.CreateCommand();
			if (facade.IsSqlServer())
			{
				cmd.CommandText = sql;
				cmd.Parameters.AddRange(parameters);
			}
			return cmd;
		}

		public static DataTable SqlQuery(this DatabaseFacade facade, string sql, params object[] parameters)
		{
			var command = CreateCommand(facade, sql, out DbConnection conn, parameters);
			var reader = command.ExecuteReader();
			var dt = new DataTable();
			dt.Load(reader);
			reader.Close();
			conn.Close();
			return dt;
		}

		public static List<object> ToList(this DataTable dt,Type type)
		{
			var propertyInfos = type.GetProperties();
			var list = new List<object>();
			foreach (DataRow row in dt.Rows)
			{
				
				var t = Activator.CreateInstance(type);
				foreach (PropertyInfo p in propertyInfos)
				{
					if (dt.Columns.IndexOf(p.Name) != -1 && row[p.Name] != DBNull.Value)
						p.SetValue(t, row[p.Name], null);
				}
				list.Add(t);
			}
			return list;
		}


		/// <summary>
		/// 取消跟踪DbContext中所有被跟踪的实体
		/// </summary>
		public static void DetachAll(this DbContext dbContext)
		{
			//循环遍历DbContext中所有被跟踪的实体
			while (true)
			{
				//每次循环获取DbContext中一个被跟踪的实体
				var currentEntry = dbContext.ChangeTracker.Entries().FirstOrDefault();

				//currentEntry不为null，就将其State设置为EntityState.Detached，即取消跟踪该实体
				if (currentEntry != null)
				{
					//设置实体State为EntityState.Detached，取消跟踪该实体，之后dbContext.ChangeTracker.Entries().Count()的值会减1
					currentEntry.State = EntityState.Detached;
				}
				//currentEntry为null，表示DbContext中已经没有被跟踪的实体了，则跳出循环
				else
				{
					break;
				}
			}
		}
	}
}

using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using SalvadorCaps.API.Data.Base;
using SalvadorCaps.API.Model;

namespace SalvadorCaps.API.Data.Repository
{
	public class CategoryDetailRepository
	{
		private readonly ILoggerFactory _loggerFactory;
		private readonly ILogger<CategoryDetailRepository> _log;
		private readonly IConfiguration _config;

		public CategoryDetailRepository(ILoggerFactory loggerFactory, IConfiguration config)
		{
			_loggerFactory = loggerFactory;
			_log = loggerFactory.CreateLogger<CategoryDetailRepository>();
			_config = config;
		}

		#region LoadModel

		private List<CategoryDetail> Load(DataSet data)
		{
			List<CategoryDetail> categoryDetails;
			CategoryDetail categoryDetail;

			try
			{
				categoryDetails = new List<CategoryDetail>();

				foreach (DataRow row in data.Tables[0].Rows)
				{
					categoryDetail = new CategoryDetail();

					categoryDetail.ID = row.Field<int>("ID");
					categoryDetail.CategoryID = row.Field<int>("CategoryID");

					categoryDetail.Name = row.Field<string>("Name");
					categoryDetail.TitleIconID = row.Field<string>("TitleIconID");

					categoryDetail.LoadUrls(_config);

					categoryDetails.Add(categoryDetail);
				}
			}
			catch (Exception ex)
			{
				throw ex;
			}

			return categoryDetails;
		}

		#endregion

		#region Change Data

		public CategoryDetail Insert(CategoryDetail categoryDetail)
		{
			SqlHelper dataConnection;
			SqlCommand command;

			try
			{
				dataConnection = new SqlHelper(_loggerFactory, _config);

				command = new SqlCommand($@"INSERT INTO CategoryDetail
											(
												 Name
												,CategoryID
												,TitleIconID
											)
										 OUTPUT inserted.ID 
										 VALUES
											(
												 @Name
												,@CategoryID
												,@TitleIconID
											)");

				command.Parameters.AddWithValue("Name", categoryDetail.Name.AsDbValue());
				command.Parameters.AddWithValue("CategoryID", categoryDetail.CategoryID.AsDbValue());
				command.Parameters.AddWithValue("TitleIconID", categoryDetail.TitleIconID.AsDbValue());

				categoryDetail.ID = (int)dataConnection.ExecuteScalar(command);
			}
			catch (Exception ex)
			{
				throw ex;
			}

			return categoryDetail;
		}

		public CategoryDetail Update(CategoryDetail categoryDetail)
		{
			SqlHelper dataConnection;
			SqlCommand command;

			try
			{
				dataConnection = new SqlHelper(_loggerFactory, _config);

				command = new SqlCommand($@"UPDATE CategoryDetail SET

											 Name = @Name
											,CategoryID = @CategoryID
											,TitleIconID = @TitleIconID

											WHERE ID = @ID");

				command.Parameters.AddWithValue("ID", categoryDetail.ID.AsDbValue());
				command.Parameters.AddWithValue("Name", categoryDetail.Name.AsDbValue());
				command.Parameters.AddWithValue("CategoryID", categoryDetail.CategoryID.AsDbValue());
				command.Parameters.AddWithValue("TitleIconID", categoryDetail.TitleIconID.AsDbValue());

				dataConnection.ExecuteNonQuery(command);
			}
			catch (Exception ex)
			{
				throw ex;
			}

			return categoryDetail;
		}

		public bool Delete(long id)
		{
			SqlHelper dataConnection;
			SqlCommand command;
			int result;

			try
			{
				dataConnection = new SqlHelper(_loggerFactory, _config);

				command = new SqlCommand($@"DELETE from CategoryDetail WHERE ID = @ID");

				command.Parameters.AddWithValue("ID", id.AsDbValue());

				result = dataConnection.ExecuteNonQuery(command);
			}
			catch (Exception ex)
			{
				throw ex;
			}

			return (result > 0);
		}

		#endregion

		#region Retrieve Data

		public CategoryDetail Get(int id)
		{
			SqlHelper dataConnection;
			SqlCommand command;
			DataSet dataSet;

			CategoryDetail product;

			try
			{
				dataConnection = new SqlHelper(_loggerFactory, _config);

				command = new SqlCommand($"SELECT * FROM CategoryDetail WHERE ID = @ID");
				command.Parameters.AddWithValue("ID", id.AsDbValue());

				dataSet = dataConnection.ExecuteDataSet(command);

				product = Load(dataSet).FirstOrDefault();
			}
			catch (Exception ex)
			{
				throw ex;
			}

			return product;
		}

		public List<CategoryDetail> Get(int? categoryID = null)
		{
			SqlHelper dataConnection;
			SqlCommand command;
			DataSet dataSet;

			List<CategoryDetail> categoryDetails;
			List<string> clauses;

			try
			{
				dataConnection = new SqlHelper(_loggerFactory, _config);

				command = new SqlCommand($"SELECT * FROM CategoryDetail");

				clauses = new List<string>();
				if (categoryID.HasValue)
				{
					clauses.Add($"CategoryID = @CategoryID");
					command.Parameters.AddWithValue("CategoryID", categoryID.AsDbValue());
				}

				if (clauses.Count > 0)
				{
					command.CommandText += $" WHERE {string.Join(" and ", clauses)}";
				}

				dataSet = dataConnection.ExecuteDataSet(command);

				categoryDetails = Load(dataSet);
			}
			catch (Exception ex)
			{
				throw ex;
			}

			return categoryDetails;
		}

		#endregion
	}
}

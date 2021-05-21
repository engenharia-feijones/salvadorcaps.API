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
	public class CategoryRepository
	{
		private readonly ILoggerFactory _loggerFactory;
		private readonly ILogger<CategoryRepository> _log;
		private readonly IConfiguration _config;

		public CategoryRepository(ILoggerFactory loggerFactory, IConfiguration config)
		{
			_loggerFactory = loggerFactory;
			_log = loggerFactory.CreateLogger<CategoryRepository>();
			_config = config;
		}

		#region LoadModel

		private List<Category> Load(DataSet data)
		{
			List<Category> categorys;
			Category category;

			try
			{
				categorys = new List<Category>();

				foreach (DataRow row in data.Tables[0].Rows)
				{
					category = new Category();

					category.ID = row.Field<int>("ID");
					category.Name = row.Field<string>("Name");
					category.DesktopSpotlight = row.Field<bool>("DesktopSpotlight");
					category.DesktopSpotlightImageID = row.Field<string>("DesktopSpotlightImageID");
					category.MobileSpotlight = row.Field<bool>("MobileSpotlight");
					category.MobileSpotlightImageID = row.Field<string>("MobileSpotlightImageID");

					category.LoadUrls(_config);

					categorys.Add(category);
				}
			}
			catch (Exception ex)
			{
				throw ex;
			}

			return categorys;
		}

		#endregion

		#region Change Data

		public Category Insert(Category category)
		{
			SqlHelper dataConnection;
			SqlCommand command;

			try
			{
				dataConnection = new SqlHelper(_loggerFactory, _config);

				command = new SqlCommand($@"INSERT INTO Category
											(
												 Name
												,DesktopSpotlight
												,DesktopSpotlightImageID
												,MobileSpotlight
												,MobileSpotlightImageID
											)
										 OUTPUT inserted.ID 
										 VALUES
											(
												 @Name
												,@DesktopSpotlight
												,@DesktopSpotlightImageID
												,@MobileSpotlight
												,@MobileSpotlightImageID
											)");

				command.Parameters.AddWithValue("Name", category.Name.AsDbValue());
				command.Parameters.AddWithValue("DesktopSpotlight", category.DesktopSpotlight.AsDbValue());
				command.Parameters.AddWithValue("DesktopSpotlightImageID", category.DesktopSpotlightImageID.AsDbValue());
				command.Parameters.AddWithValue("MobileSpotlight", category.MobileSpotlight.AsDbValue());
				command.Parameters.AddWithValue("MobileSpotlightImageID", category.MobileSpotlightImageID.AsDbValue());

				category.ID = (int)dataConnection.ExecuteScalar(command);
			}
			catch (Exception ex)
			{
				throw ex;
			}

			return category;
		}

		public Category Update(Category category)
		{
			SqlHelper dataConnection;
			SqlCommand command;

			try
			{
				dataConnection = new SqlHelper(_loggerFactory, _config);

				command = new SqlCommand($@"UPDATE Category SET

											 Name = @Name
											,DesktopSpotlight = @DesktopSpotlight
											,DesktopSpotlightImageID = @DesktopSpotlightImageID
											,MobileSpotlight = @MobileSpotlight
											,MobileSpotlightImageID = @MobileSpotlightImageID

											WHERE ID = @ID");

				command.Parameters.AddWithValue("ID", category.ID.AsDbValue());
				command.Parameters.AddWithValue("Name", category.Name.AsDbValue());
				command.Parameters.AddWithValue("DesktopSpotlight", category.DesktopSpotlight.AsDbValue());
				command.Parameters.AddWithValue("DesktopSpotlightImageID", category.DesktopSpotlightImageID.AsDbValue());
				command.Parameters.AddWithValue("MobileSpotlight", category.MobileSpotlight.AsDbValue());
				command.Parameters.AddWithValue("MobileSpotlightImageID", category.MobileSpotlightImageID.AsDbValue());

				dataConnection.ExecuteNonQuery(command);
			}
			catch (Exception ex)
			{
				throw ex;
			}

			return category;
		}

		public bool Delete(int id)
		{
			SqlHelper dataConnection;
			SqlCommand command;
			int result;

			try
			{
				dataConnection = new SqlHelper(_loggerFactory, _config);

				command = new SqlCommand($@"DELETE from CategoryDetail where CategoryID=@ID;DELETE from Category WHERE ID = @ID;");

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

		public Category Get(int id)
		{
			SqlHelper dataConnection;
			SqlCommand command;
			DataSet dataSet;

			Category category;

			try
			{
				dataConnection = new SqlHelper(_loggerFactory, _config);

				command = new SqlCommand($"SELECT * FROM Category WHERE ID = @ID");
				command.Parameters.AddWithValue("ID", id.AsDbValue());

				dataSet = dataConnection.ExecuteDataSet(command);

				category = Load(dataSet).FirstOrDefault();
			}
			catch (Exception ex)
			{
				throw ex;
			}

			return category;
		}

		public List<Category> Get(string name = null)
		{
			SqlHelper dataConnection;
			SqlCommand command;
			DataSet dataSet;

			List<Category> categorys;
			List<string> clauses;

			try
			{
				dataConnection = new SqlHelper(_loggerFactory, _config);

				command = new SqlCommand($"SELECT * FROM Category");

				clauses = new List<string>();
				if (!string.IsNullOrEmpty(name))
				{
					clauses.Add($"Name like '%' + @Name + '%'");
					command.Parameters.AddWithValue("Name", name.AsDbValue());
				}

				if (clauses.Count > 0)
				{
					command.CommandText += $" WHERE {string.Join(" and ", clauses)}";
				}

				dataSet = dataConnection.ExecuteDataSet(command);

				categorys = Load(dataSet);
			}
			catch (Exception ex)
			{
				throw ex;
			}

			return categorys;
		}

		#endregion
	}
}

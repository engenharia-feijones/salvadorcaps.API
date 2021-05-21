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
	public class BrandRepository
	{
		private readonly ILoggerFactory _loggerFactory;
		private readonly ILogger<BrandRepository> _log;
		private readonly IConfiguration _config;

		public BrandRepository(ILoggerFactory loggerFactory, IConfiguration config)
		{
			_loggerFactory = loggerFactory;
			_log = loggerFactory.CreateLogger<BrandRepository>();
			_config = config;
		}

		#region LoadModel

		private List<Brand> Load(DataSet data)
		{
			List<Brand> brands;
			Brand brand;

			try
			{
				brands = new List<Brand>();

				foreach (DataRow row in data.Tables[0].Rows)
				{
					brand = new Brand();

					brand.ID = row.Field<int>("ID");
					brand.Name = row.Field<string>("Name");
					brand.DesktopSpotlight = row.Field<bool>("DesktopSpotlight");
					brand.DesktopSpotlightImageID = row.Field<string>("DesktopSpotlightImageID");
					brand.MobileSpotlight = row.Field<bool>("MobileSpotlight");
					brand.MobileSpotlightImageID = row.Field<string>("MobileSpotlightImageID");

					brand.LoadUrls(_config);

					brands.Add(brand);
				}
			}
			catch (Exception ex)
			{
				throw ex;
			}

			return brands;
		}

		#endregion

		#region Change Data

		public Brand Insert(Brand brand)
		{
			SqlHelper dataConnection;
			SqlCommand command;

			try
			{
				dataConnection = new SqlHelper(_loggerFactory, _config);

				command = new SqlCommand($@"INSERT INTO Brand
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

				command.Parameters.AddWithValue("Name", brand.Name.AsDbValue());
				command.Parameters.AddWithValue("DesktopSpotlight", brand.DesktopSpotlight.AsDbValue());
				command.Parameters.AddWithValue("DesktopSpotlightImageID", brand.DesktopSpotlightImageID.AsDbValue());
				command.Parameters.AddWithValue("MobileSpotlight", brand.MobileSpotlight.AsDbValue());
				command.Parameters.AddWithValue("MobileSpotlightImageID", brand.MobileSpotlightImageID.AsDbValue());

				brand.ID = (int)dataConnection.ExecuteScalar(command);
			}
			catch (Exception ex)
			{
				throw ex;
			}

			return brand;
		}

		public Brand Update(Brand brand)
		{
			SqlHelper dataConnection;
			SqlCommand command;

			try
			{
				dataConnection = new SqlHelper(_loggerFactory, _config);

				command = new SqlCommand($@"UPDATE Brand SET

											 Name = @Name
											,DesktopSpotlight = @DesktopSpotlight
											,DesktopSpotlightImageID = @DesktopSpotlightImageID
											,MobileSpotlight = @MobileSpotlight
											,MobileSpotlightImageID = @MobileSpotlightImageID

											WHERE ID = @ID");

				command.Parameters.AddWithValue("ID", brand.ID.AsDbValue());
				command.Parameters.AddWithValue("Name", brand.Name.AsDbValue());
				command.Parameters.AddWithValue("DesktopSpotlight", brand.DesktopSpotlight.AsDbValue());
				command.Parameters.AddWithValue("DesktopSpotlightImageID", brand.DesktopSpotlightImageID.AsDbValue());
				command.Parameters.AddWithValue("MobileSpotlight", brand.MobileSpotlight.AsDbValue());
				command.Parameters.AddWithValue("MobileSpotlightImageID", brand.MobileSpotlightImageID.AsDbValue());

				dataConnection.ExecuteNonQuery(command);
			}
			catch (Exception ex)
			{
				throw ex;
			}

			return brand;
		}

		public bool Delete(int id)
		{
			SqlHelper dataConnection;
			SqlCommand command;
			int result;

			try
			{
				dataConnection = new SqlHelper(_loggerFactory, _config);

				command = new SqlCommand($@"DELETE from Brand WHERE ID = @ID");

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

		public Brand Get(int id)
		{
			SqlHelper dataConnection;
			SqlCommand command;
			DataSet dataSet;

			Brand brand;

			try
			{
				dataConnection = new SqlHelper(_loggerFactory, _config);

				command = new SqlCommand($"SELECT * FROM Brand WHERE ID = @ID");
				command.Parameters.AddWithValue("ID", id.AsDbValue());

				dataSet = dataConnection.ExecuteDataSet(command);

				brand = Load(dataSet).FirstOrDefault();
			}
			catch (Exception ex)
			{
				throw ex;
			}

			return brand;
		}

		public List<Brand> Get(string name = null)
		{
			SqlHelper dataConnection;
			SqlCommand command;
			DataSet dataSet;

			List<Brand> brands;
			List<string> clauses;

			try
			{
				dataConnection = new SqlHelper(_loggerFactory, _config);

				command = new SqlCommand($"SELECT * FROM Brand");

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

				brands = Load(dataSet);
			}
			catch (Exception ex)
			{
				throw ex;
			}

			return brands;
		}

		#endregion
	}
}

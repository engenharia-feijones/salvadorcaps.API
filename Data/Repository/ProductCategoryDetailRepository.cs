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
	public class ProductCategoryDetailDetailRepository
	{
		private readonly ILoggerFactory _loggerFactory;
		private readonly ILogger<ProductCategoryDetailDetailRepository> _log;
		private readonly IConfiguration _config;

		public ProductCategoryDetailDetailRepository(ILoggerFactory loggerFactory, IConfiguration config)
		{
			_loggerFactory = loggerFactory;
			_log = loggerFactory.CreateLogger<ProductCategoryDetailDetailRepository>();
			_config = config;
		}

		#region LoadModel

		private List<ProductCategoryDetail> Load(DataSet data)
		{
			List<ProductCategoryDetail> productCategoryDetails;
			ProductCategoryDetail productCategoryDetail;

			try
			{
				productCategoryDetails = new List<ProductCategoryDetail>();

				foreach (DataRow row in data.Tables[0].Rows)
				{
					productCategoryDetail = new ProductCategoryDetail();

					productCategoryDetail.ProductID = row.Field<long>("ProductID");
					productCategoryDetail.CategoryDetailID = row.Field<int>("CategoryDetailID");

					productCategoryDetails.Add(productCategoryDetail);
				}
			}
			catch (Exception ex)
			{
				throw ex;
			}

			return productCategoryDetails;
		}

		#endregion

		#region Change Data

		public void Insert(List<ProductCategoryDetail> productCategories)
		{
			SqlHelper dataConnection;
			SqlCommand command;

			ProductCategoryDetail productCategoryDetail;
			List<string> clauses;

			try
			{
				dataConnection = new SqlHelper(_loggerFactory, _config);

				command = new SqlCommand($@"INSERT INTO ProductCategoryDetail
											(
												 ProductID
												,CategoryDetailID
											)
										 VALUES");

				clauses = new List<string>();

				for (int i = 0; i < productCategories.Count; i++)
				{
					productCategoryDetail = productCategories[i];

					clauses.Add($"(@ProductID{i}, @CategoryDetailID{i})");
					command.Parameters.AddWithValue($"ProductID{i}", productCategoryDetail.ProductID.AsDbValue());
					command.Parameters.AddWithValue($"CategoryDetailID{i}", productCategoryDetail.CategoryDetailID.AsDbValue());
				}

				if (clauses.Count > 0)
				{
					command.CommandText += $"{string.Join(" , ", clauses)}";
					dataConnection.ExecuteScalar(command);
				}
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		public void Delete(List<ProductCategoryDetail> productCategories)
		{
			SqlHelper dataConnection;
			SqlCommand command;

			ProductCategoryDetail productCategoryDetail;
			List<string> clauses;

			try
			{
				dataConnection = new SqlHelper(_loggerFactory, _config);

				command = new SqlCommand($@"DELETE from ProductCategoryDetail");

				clauses = new List<string>();

				for (int i = 0; i < productCategories.Count; i++)
				{
					productCategoryDetail = productCategories[i];

					clauses.Add($"(ProductID = @ProductID AND CategoryDetailID = @CategoryDetailID{i})");
					command.Parameters.AddWithValue($"ProductID", productCategoryDetail.ProductID.AsDbValue());
					command.Parameters.AddWithValue($"CategoryDetailID", productCategoryDetail.CategoryDetailID.AsDbValue());
				}

				if (clauses.Count > 0)
				{
					command.CommandText += $" WHERE {string.Join(" OR ", clauses)}";
				}

				dataConnection.ExecuteNonQuery(command);
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		#endregion

		#region Retrieve Data

		public List<ProductCategoryDetail> Get(long? productID = null)
		{
			SqlHelper dataConnection;
			SqlCommand command;
			DataSet dataSet;

			List<ProductCategoryDetail> productCategoryDetails;
			List<string> clauses;

			try
			{
				dataConnection = new SqlHelper(_loggerFactory, _config);

				command = new SqlCommand($"SELECT * FROM ProductCategoryDetail");

				clauses = new List<string>();
				if (productID.HasValue)
				{
					clauses.Add($"ProductID = @ProductID");
					command.Parameters.AddWithValue("ProductID", productID.AsDbValue());
				}

				if (clauses.Count > 0)
				{
					command.CommandText += $" WHERE {string.Join(" and ", clauses)}";
				}

				dataSet = dataConnection.ExecuteDataSet(command);

				productCategoryDetails = Load(dataSet);
			}
			catch (Exception ex)
			{
				throw ex;
			}

			return productCategoryDetails;
		}

		#endregion
	}
}

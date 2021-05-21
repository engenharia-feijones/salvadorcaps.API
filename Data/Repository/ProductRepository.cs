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
	public class ProductRepository
	{
		private readonly ILoggerFactory _loggerFactory;
		private readonly ILogger<ProductRepository> _log;
		private readonly IConfiguration _config;

		public ProductRepository(ILoggerFactory loggerFactory, IConfiguration config)
		{
			_loggerFactory = loggerFactory;
			_log = loggerFactory.CreateLogger<ProductRepository>();
			_config = config;
		}

		#region LoadModel

		private List<Product> Load(DataSet data)
		{
			List<Product> products;
			Product product;

			try
			{
				products = new List<Product>();

				foreach (DataRow row in data.Tables[0].Rows)
				{
					product = new Product();

					product.ID = row.Field<long>("ID");
					product.BrandID = row.Field<int>("BrandID");
					product.Code = row.Field<long>("Code");
					product.Name = row.Field<string>("Name");
					product.ExternalName = row.Field<string>("ExternalName");
					product.Description = row.Field<string>("Description");
					product.Price = row.Field<decimal?>("Price");
					product.ImageID = row.Field<string>("ImageID");

					product.LoadUrls(_config);

					products.Add(product);
				}
			}
			catch (Exception ex)
			{
				throw ex;
			}

			return products;
		}

		#endregion

		#region Change Data

		public Product Insert(Product product)
		{
			SqlHelper dataConnection;
			SqlCommand command;

			try
			{
				dataConnection = new SqlHelper(_loggerFactory, _config);

				command = new SqlCommand($@"INSERT INTO Product
											(
												 BrandID
												,Code
												,Name
												,ExternalName
												,Description
												,Price
												,ImageID
											)
										 OUTPUT inserted.ID 
										 VALUES
											(
												 @BrandID
												,@Code
												,@Name
												,@ExternalName
												,@Description
												,@Price
												,@ImageID
											)");

				command.Parameters.AddWithValue("BrandID", product.BrandID.AsDbValue());
				command.Parameters.AddWithValue("Code", product.Code.AsDbValue());
				command.Parameters.AddWithValue("Name", product.Name.AsDbValue());
				command.Parameters.AddWithValue("ExternalName", product.ExternalName.AsDbValue());
				command.Parameters.AddWithValue("Description", product.Description.AsDbValue());
				command.Parameters.AddWithValue("Price", product.Price.AsDbValue());
				command.Parameters.AddWithValue("ImageID", product.ImageID.AsDbValue());

				product.ID = (long)dataConnection.ExecuteScalar(command);
			}
			catch (Exception ex)
			{
				throw ex;
			}

			return product;
		}

		public Product Update(Product product)
		{
			SqlHelper dataConnection;
			SqlCommand command;

			try
			{
				dataConnection = new SqlHelper(_loggerFactory, _config);

				command = new SqlCommand($"UPDATE Product SET " +
											$" BrandID = @BrandID," +
											$" Code = @Code," +
											$" Name = @Name," +
											$" ExternalName = @ExternalName," +
											$" Description = @Description," +
											$" Price = @Price," +
											$" ImageID = @ImageID" +
											$" WHERE ID = @ID");

				command.Parameters.AddWithValue("ID", product.ID.AsDbValue());
				command.Parameters.AddWithValue("BrandID", product.BrandID.AsDbValue());
				command.Parameters.AddWithValue("Code", product.Code.AsDbValue());
				command.Parameters.AddWithValue("Name", product.Name.AsDbValue());
				command.Parameters.AddWithValue("ExternalName", product.ExternalName.AsDbValue());
				command.Parameters.AddWithValue("Description", product.Description.AsDbValue());
				command.Parameters.AddWithValue("Price", product.Price.AsDbValue());
				command.Parameters.AddWithValue("ImageID", product.ImageID.AsDbValue());

				dataConnection.ExecuteNonQuery(command);
			}
			catch (Exception ex)
			{
				throw ex;
			}

			product.LoadUrls(_config);
			return product;
		}

		public bool Delete(long id)
		{
			SqlHelper dataConnection;
			SqlCommand command;
			int result;

			try
			{
				dataConnection = new SqlHelper(_loggerFactory, _config);

				command = new SqlCommand($@"DELETE from ProductCategoryDetail where ProductID=@ID ;
											DELETE from Product WHERE ID = @ID;"  );

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

		public Product Get(long id)
		{
			SqlHelper dataConnection;
			SqlCommand command;
			DataSet dataSet;

			Product product;

			try
			{
				dataConnection = new SqlHelper(_loggerFactory, _config);

				command = new SqlCommand($"SELECT * FROM Product WHERE ID = @ID");
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

		public List<Product> Get(long? code = null, string name = null, int? categoryID = null, int? categoryDetailID = null, int? brandID = null)
		{
			SqlHelper dataConnection;
			SqlCommand command;
			DataSet dataSet;

			List<Product> products;
			List<string> clauses;

			try
			{
				dataConnection = new SqlHelper(_loggerFactory, _config);

				command = new SqlCommand($"SELECT P.*" +
										$" FROM Product P LEFT JOIN" +

										$" ProductCategoryDetail PC ON P.ID = PC.ProductID LEFT JOIN " +
										$" CategoryDetail D ON PC.CategoryDetailID = D.ID");

				clauses = new List<string>();
				if (code.HasValue)
				{
					clauses.Add($"P.Code = @Code");
					command.Parameters.AddWithValue("Code", code.AsDbValue());
				}
				if (!string.IsNullOrEmpty(name))
				{
					clauses.Add($"P.Name like '%' + @Name + '%'");
					command.Parameters.AddWithValue("Name", name.AsDbValue());
				}
				if (categoryID.HasValue)
				{
					clauses.Add($"D.CategoryID = @CategoryID");
					command.Parameters.AddWithValue("CategoryID", categoryID.AsDbValue());
				}
				if (categoryDetailID.HasValue)
				{
					clauses.Add($"D.ID = @CategoryDetailID");
					command.Parameters.AddWithValue("CategoryDetailID", categoryDetailID.AsDbValue());
				}
				if (brandID.HasValue)
				{
					clauses.Add($"P.BrandID = @BrandID");
					command.Parameters.AddWithValue("BrandID", brandID.AsDbValue());
				}

				if (clauses.Count > 0)
				{
					command.CommandText += $" WHERE {string.Join(" and ", clauses)}";
				}

				dataSet = dataConnection.ExecuteDataSet(command);

				products = Load(dataSet);
			}
			catch (Exception ex)
			{
				throw ex;
			}

			return products;
		}

		#endregion
	}
}

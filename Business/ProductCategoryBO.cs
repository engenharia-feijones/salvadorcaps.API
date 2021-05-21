using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SalvadorCaps.API.Data.Repository;
using SalvadorCaps.API.Model;

namespace SalvadorCaps.API.Business
{
	public class ProductCategoryDetailBO
	{
		private readonly ILoggerFactory _loggerFactory;
		private readonly ILogger<ProductCategoryDetailBO> _log;
		private readonly IConfiguration _config;

		public ProductCategoryDetailBO(ILoggerFactory loggerFactory, IConfiguration config)
		{
			_loggerFactory = loggerFactory;
			_log = loggerFactory.CreateLogger<ProductCategoryDetailBO>();
			_config = config;
		}

		#region Change Data

		public void Save(long id, List<ProductCategoryDetail> productCategories)
		{
			ProductCategoryDetailDetailRepository productCategoryDetailRepository;

			List<ProductCategoryDetail> oldLinks;
			List<ProductCategoryDetail> toDelete;
			List<ProductCategoryDetail> toSave;

			try
			{
				productCategoryDetailRepository = new ProductCategoryDetailDetailRepository(_loggerFactory, _config);
				toDelete = new List<ProductCategoryDetail>();
				toSave = new List<ProductCategoryDetail>();

				if (productCategories != null && productCategories.Count > 0)
				{
					productCategories.ForEach(x => x.ProductID = id);

					oldLinks = Get(id);
					if (oldLinks.Count > 0)
					{
						toDelete = oldLinks.Where(o => !productCategories.Any(n => n.CategoryDetailID == o.CategoryDetailID && n.ProductID == o.ProductID)).ToList();

						if (toDelete.Count > 0)
						{
							productCategoryDetailRepository.Delete(toDelete);
						}
					}

					toSave = productCategories.Where(o => !oldLinks.Any(n => n.CategoryDetailID == o.CategoryDetailID && n.ProductID == o.ProductID)).ToList();

					if (toSave.Count > 0)
					{
						productCategoryDetailRepository.Insert(toSave);
					}
				}
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		#endregion

		#region Retrieve Repository

		public List<ProductCategoryDetail> Get(long? productID = null)
		{
			ProductCategoryDetailDetailRepository productCategoryDetailRepository;
			List<ProductCategoryDetail> productCategoryDetails;

			try
			{
				productCategoryDetailRepository = new ProductCategoryDetailDetailRepository(_loggerFactory, _config);

				productCategoryDetails = productCategoryDetailRepository.Get(productID);
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

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using SalvadorCaps.API.Data.Repository;
using SalvadorCaps.API.Model;

namespace SalvadorCaps.API.Business
{
	public class ProductBO
	{
		private readonly ILoggerFactory _loggerFactory;
		private readonly ILogger<ProductBO> _log;
		private readonly IConfiguration _config;

		public ProductBO(ILoggerFactory loggerFactory, IConfiguration config)
		{
			_loggerFactory = loggerFactory;
			_log = loggerFactory.CreateLogger<ProductBO>();
			_config = config;
		}

		#region Change Data

		public Product Insert(Product product)
		{
			ProductRepository productRepository;
			ProductCategoryDetailBO productCategoryDetailBO;

			BlobFileBO blobFileBO;

			try
			{
				productRepository = new ProductRepository(_loggerFactory, _config);
				productCategoryDetailBO = new ProductCategoryDetailBO(_loggerFactory, _config);
				blobFileBO = new BlobFileBO(_loggerFactory, _config);

				if (product.ID == 0)
				{
					product = productRepository.Insert(product);
					productCategoryDetailBO.Save(product.ID, product.ProductCategoryDetails);
				}
				else
				{
					throw new Exception("ID diferente de 0, avalie a utilização do PUT");
				}
			}
			catch (Exception ex)
			{
				throw ex;
			}

			return product;
		}

		public Product Update(Product product)
		{
			ProductRepository productRepository;
			ProductCategoryDetailBO productCategoryDetailBO;

			BlobFileBO blobFileBO;

			try
			{
				productRepository = new ProductRepository(_loggerFactory, _config);
				productCategoryDetailBO = new ProductCategoryDetailBO(_loggerFactory, _config);
				blobFileBO = new BlobFileBO(_loggerFactory, _config);

				if (product.ID == 0)
				{
					throw new Exception("ID diferente de 0, avalie a utilização do POST");
				}
				else
				{
					productRepository.Update(product);
					productCategoryDetailBO.Save(product.ID, product.ProductCategoryDetails);
				}
			}
			catch (Exception ex)
			{
				throw ex;
			}

			return product;
		}

		public void Delete(long id)
		{
			ProductRepository productRepository;
			BlobFileBO blobFileBO;
			Product product;

			try
			{
				if (id == 0)
				{
					throw new Exception("ID inválido");
				}
				else
				{
					productRepository = new ProductRepository(_loggerFactory, _config);
					blobFileBO = new BlobFileBO(_loggerFactory, _config);

					product = Get(id);
					if (product != null)
					{
						if (!string.IsNullOrEmpty(product.ImageID))
						{
							blobFileBO.Delete(product.ImageID);
						}
						productRepository.Delete(id);
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

		public Product Get(long id)
		{
			ProductRepository productRepository;
			ProductCategoryDetailBO productCategoryDetailBO;
			BlobFileBO blobFileBO;

			Product product;

			try
			{
				productRepository = new ProductRepository(_loggerFactory, _config);
				productCategoryDetailBO = new ProductCategoryDetailBO(_loggerFactory, _config);
				blobFileBO = new BlobFileBO(_loggerFactory, _config);

				product = productRepository.Get(id);
				product.ProductCategoryDetails = productCategoryDetailBO.Get(product.ID);
			}
			catch (Exception ex)
			{
				throw ex;
			}

			return product;
		}

		public List<Product> Get(long? code = null, string name = null, int? categoryID = null, int? categoryDetailID = null)
		{
			ProductRepository productRepository;
			List<Product> products;

			try
			{
				productRepository = new ProductRepository(_loggerFactory, _config);

				products = productRepository.Get(code: code, name: name, categoryID: categoryID, categoryDetailID: categoryDetailID);
			}
			catch (Exception ex)
			{
				throw ex;
			}

			return products;
		}

		#endregion
	}

	public class ProductImageBO
	{
		private readonly ILoggerFactory _loggerFactory;
		private readonly ILogger<ProductImageBO> _log;
		private readonly IConfiguration _config;

		public ProductImageBO(ILoggerFactory loggerFactory, IConfiguration config)
		{
			_loggerFactory = loggerFactory;
			_log = loggerFactory.CreateLogger<ProductImageBO>();
			_config = config;
		}

		#region Change Data

		public ProductImage Insert(ProductImage image)
		{
			ProductBO productBO;
			BlobFileBO blobFileBO;
			Product product;

			try
			{
				productBO = new ProductBO(_loggerFactory, _config);
				blobFileBO = new BlobFileBO(_loggerFactory, _config);

				if (string.IsNullOrEmpty(image.BlobFile?.ID))
				{
					product = productBO.Get(image.ProductID);

					if (product != null)
					{
						image.BlobFile = blobFileBO.Insert(image.BlobFile);

						product.ImageID = image.BlobFile.ID;
						product = productBO.Update(product);
					}
					else
					{
						throw new Exception("Produto não encontrado");
					}
				}
				else
				{
					throw new Exception("ID diferente de vazio, avalie a utilização do PUT");
				}
			}
			catch (Exception ex)
			{
				throw ex;
			}

			return image;
		}

		public ProductImage Update(ProductImage image)
		{
			ProductBO productBO;
			BlobFileBO blobFileBO;
			Product product;

			try
			{
				productBO = new ProductBO(_loggerFactory, _config);
				blobFileBO = new BlobFileBO(_loggerFactory, _config);

				if (string.IsNullOrEmpty(image.BlobFile?.ID))
				{
					throw new Exception("ID vazio, avalie a utilização do POST");
				}
				else
				{
					product = productBO.Get(image.ProductID);

					if (product != null)
					{
						image.BlobFile = blobFileBO.Update(image.BlobFile);

						product.ImageID = image.BlobFile.ID;
						productBO.Update(product);
					}
					else
					{
						throw new Exception("Produto não encontrado");
					}
				}
			}
			catch (Exception ex)
			{
				throw ex;
			}

			return image;
		}

		public void Delete(string id)
		{
			BlobFileBO blobFileBO;

			try
			{
				if (!string.IsNullOrEmpty(id))
				{
					throw new Exception("ID inválido");
				}
				else
				{
					blobFileBO = new BlobFileBO(_loggerFactory, _config);

					blobFileBO.Delete(id);
				}
			}
			catch (Exception ex)
			{
				throw ex;
			}

		}

		#endregion
	}
}

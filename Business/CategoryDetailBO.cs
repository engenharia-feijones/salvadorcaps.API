using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using SalvadorCaps.API.Data.Repository;
using SalvadorCaps.API.Model;

namespace SalvadorCaps.API.Business
{
	public class CategoryDetailBO
	{
		private readonly ILoggerFactory _loggerFactory;
		private readonly ILogger<CategoryDetailBO> _log;
		private readonly IConfiguration _config;

		public CategoryDetailBO(ILoggerFactory loggerFactory, IConfiguration config)
		{
			_loggerFactory = loggerFactory;
			_log = loggerFactory.CreateLogger<CategoryDetailBO>();
			_config = config;
		}

		#region Change Data

		public CategoryDetail Insert(CategoryDetail categoryDetail)
		{
			CategoryDetailRepository categoryDetailRepository;

			try
			{
				categoryDetailRepository = new CategoryDetailRepository(_loggerFactory, _config);

				if (categoryDetail.ID == 0)
				{
					categoryDetail = categoryDetailRepository.Insert(categoryDetail);
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

			return categoryDetail;
		}

		public CategoryDetail Update(CategoryDetail categoryDetail)
		{
			CategoryDetailRepository categoryDetailRepository;

			try
			{
				categoryDetailRepository = new CategoryDetailRepository(_loggerFactory, _config);

				if (categoryDetail.ID == 0)
				{
					throw new Exception("ID diferente de 0, avalie a utilização do POST");
				}
				else
				{
					categoryDetailRepository.Update(categoryDetail);
				}
			}
			catch (Exception ex)
			{
				throw ex;
			}

			return categoryDetail;
		}

		public void Delete(int id)
		{
			CategoryDetailRepository categoryDetailRepository;
			BlobFileBO blobFileBO;
			CategoryDetail categoryDetail;

			try
			{
				if (id == 0)
				{
					throw new Exception("ID inválido");
				}
				else
				{
					categoryDetailRepository = new CategoryDetailRepository(_loggerFactory, _config);
					blobFileBO = new BlobFileBO(_loggerFactory, _config);

					categoryDetail = Get(id);
					if (categoryDetail != null)
					{
						if (!string.IsNullOrEmpty(categoryDetail.TitleIconID))
						{
							blobFileBO.Delete(categoryDetail.TitleIconID);
						}
						categoryDetailRepository.Delete(id);
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

		public CategoryDetail Get(int id)
		{
			CategoryDetailRepository categoryDetailRepository;
			ProductBO productBO;
			CategoryDetail categoryDetail;

			try
			{
				categoryDetailRepository = new CategoryDetailRepository(_loggerFactory, _config);
				productBO = new ProductBO(_loggerFactory, _config);

				categoryDetail = categoryDetailRepository.Get(id);
				categoryDetail.Products = productBO.Get(categoryDetailID: categoryDetail.ID);
			}
			catch (Exception ex)
			{
				throw ex;
			}

			return categoryDetail;
		}

		public List<CategoryDetail> Get(int? categoryID = null)
		{
			CategoryDetailRepository categoryDetailRepository;
			List<CategoryDetail> categoryDetails;

			try
			{
				categoryDetailRepository = new CategoryDetailRepository(_loggerFactory, _config);

				categoryDetails = categoryDetailRepository.Get(categoryID);
			}
			catch (Exception ex)
			{
				throw ex;
			}

			return categoryDetails;
		}

		#endregion
	}

	public class CategoryDetailImageBO
	{
		private readonly ILoggerFactory _loggerFactory;
		private readonly ILogger<CategoryDetailImageBO> _log;
		private readonly IConfiguration _config;

		public CategoryDetailImageBO(ILoggerFactory loggerFactory, IConfiguration config)
		{
			_loggerFactory = loggerFactory;
			_log = loggerFactory.CreateLogger<CategoryDetailImageBO>();
			_config = config;
		}

		#region Change Data

		public CategoryDetailImage Insert(CategoryDetailImage image)
		{
			CategoryDetailBO categoryDetailBO;
			BlobFileBO blobFileBO;
			CategoryDetail categoryDetail;

			try
			{
				categoryDetailBO = new CategoryDetailBO(_loggerFactory, _config);
				blobFileBO = new BlobFileBO(_loggerFactory, _config);

				if (string.IsNullOrEmpty(image.BlobFile?.ID))
				{
					categoryDetail = categoryDetailBO.Get(image.CategoryDetailID);

					if (categoryDetail != null)
					{
						image.BlobFile = blobFileBO.Insert(image.BlobFile);

						categoryDetail.TitleIconID = image.BlobFile.ID;
						categoryDetail = categoryDetailBO.Update(categoryDetail);
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

		public CategoryDetailImage Update(CategoryDetailImage image)
		{
			CategoryDetailBO categoryDetailBO;
			BlobFileBO blobFileBO;
			CategoryDetail categoryDetail;

			try
			{
				categoryDetailBO = new CategoryDetailBO(_loggerFactory, _config);
				blobFileBO = new BlobFileBO(_loggerFactory, _config);

				if (string.IsNullOrEmpty(image.BlobFile?.ID))
				{
					throw new Exception("ID vazio, avalie a utilização do POST");
				}
				else
				{
					categoryDetail = categoryDetailBO.Get(image.CategoryDetailID);

					if (categoryDetail != null)
					{
						image.BlobFile = blobFileBO.Update(image.BlobFile);

						categoryDetail.TitleIconID = image.BlobFile.ID;
						categoryDetailBO.Update(categoryDetail);
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

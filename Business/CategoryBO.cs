using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using SalvadorCaps.API.Data.Repository;
using SalvadorCaps.API.Model;

namespace SalvadorCaps.API.Business
{
	public class CategoryBO
	{
		private readonly ILoggerFactory _loggerFactory;
		private readonly ILogger<CategoryBO> _log;
		private readonly IConfiguration _config;

		public CategoryBO(ILoggerFactory loggerFactory, IConfiguration config)
		{
			_loggerFactory = loggerFactory;
			_log = loggerFactory.CreateLogger<CategoryBO>();
			_config = config;
		}

		#region Change Data

		public Category Insert(Category category)
		{
			CategoryRepository categoryRepository;

			try
			{
				categoryRepository = new CategoryRepository(_loggerFactory, _config);

				if (category.ID == 0)
				{
					category = categoryRepository.Insert(category);
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

			return category;
		}

		public Category Update(Category category)
		{
			CategoryRepository categoryRepository;

			try
			{
				categoryRepository = new CategoryRepository(_loggerFactory, _config);

				if (category.ID == 0)
				{
					throw new Exception("ID diferente de 0, avalie a utilização do POST");
				}
				else
				{
					categoryRepository.Update(category);
				}
			}
			catch (Exception ex)
			{
				throw ex;
			}

			return category;
		}

		public void Delete(int id)
		{
			CategoryRepository categoryRepository;
			BlobFileBO blobFileBO;
			Category category;

			try
			{
				if (id == 0)
				{
					throw new Exception("ID inválido");
				}
				else
				{
					categoryRepository = new CategoryRepository(_loggerFactory, _config);
					blobFileBO = new BlobFileBO(_loggerFactory, _config);

					category = Get(id);
					if (category != null)
					{
						if (!string.IsNullOrEmpty(category.DesktopSpotlightImageID))
						{
							blobFileBO.Delete(category.DesktopSpotlightImageID);
						}
						if (!string.IsNullOrEmpty(category.MobileSpotlightImageID))
						{
							blobFileBO.Delete(category.MobileSpotlightImageID);
						}

						categoryRepository.Delete(id);
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

		public Category Get(int id)
		{
			CategoryRepository categoryRepository;
			Category category;

			try
			{
				categoryRepository = new CategoryRepository(_loggerFactory, _config);

				category = categoryRepository.Get(id);
			}
			catch (Exception ex)
			{
				throw ex;
			}

			return category;
		}

		public List<Category> Get(string name = null)
		{
			CategoryRepository categoryRepository;
			List<Category> categorys;

			try
			{
				categoryRepository = new CategoryRepository(_loggerFactory, _config);

				categorys = categoryRepository.Get(name);
			}
			catch (Exception ex)
			{
				throw ex;
			}

			return categorys;
		}

		#endregion
	}

	public class CategoryImageBO
	{
		private readonly ILoggerFactory _loggerFactory;
		private readonly ILogger<CategoryImageBO> _log;
		private readonly IConfiguration _config;

		public CategoryImageBO(ILoggerFactory loggerFactory, IConfiguration config)
		{
			_loggerFactory = loggerFactory;
			_log = loggerFactory.CreateLogger<CategoryImageBO>();
			_config = config;
		}

		#region Change Data

		public CategoryImage Insert(CategoryImage image)
		{
			CategoryBO categoryBO;
			BlobFileBO blobFileBO;
			Category category;

			try
			{
				categoryBO = new CategoryBO(_loggerFactory, _config);
				blobFileBO = new BlobFileBO(_loggerFactory, _config);

				category = categoryBO.Get(image.CategoryID);

				if (string.IsNullOrEmpty(image.BlobFile?.ID))
				{
					if (category != null)
					{
						image.BlobFile = blobFileBO.Insert(image.BlobFile);

						switch (image.Destination)
						{
							case CategoryImage.CategoryImageDestination.Desktop:
								category.DesktopSpotlightImageID = image.BlobFile.ID;
								break;
							case CategoryImage.CategoryImageDestination.Mobile:
								category.MobileSpotlightImageID = image.BlobFile.ID;
								break;
							default:
								break;
						}

						category = categoryBO.Update(category);
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

		public CategoryImage Update(CategoryImage image)
		{
			CategoryBO categoryBO;
			BlobFileBO blobFileBO;
			Category category;

			try
			{
				categoryBO = new CategoryBO(_loggerFactory, _config);
				blobFileBO = new BlobFileBO(_loggerFactory, _config);

				if (string.IsNullOrEmpty(image.BlobFile?.ID))
				{
					throw new Exception("ID vazio, avalie a utilização do POST");
				}
				else
				{
					category = categoryBO.Get(image.CategoryID);

					if (category != null)
					{
						image.BlobFile = blobFileBO.Update(image.BlobFile);

						switch (image.Destination)
						{
							case CategoryImage.CategoryImageDestination.Desktop:
								category.DesktopSpotlightImageID = image.BlobFile.ID;
								break;
							case CategoryImage.CategoryImageDestination.Mobile:
								category.MobileSpotlightImageID = image.BlobFile.ID;
								break;
							default:
								break;
						}

						categoryBO.Update(category);
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

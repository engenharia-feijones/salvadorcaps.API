using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using SalvadorCaps.API.Data.Repository;
using SalvadorCaps.API.Model;

namespace SalvadorCaps.API.Business
{
	public class BrandBO
	{
		private readonly ILoggerFactory _loggerFactory;
		private readonly ILogger<BrandBO> _log;
		private readonly IConfiguration _config;

		public BrandBO(ILoggerFactory loggerFactory, IConfiguration config)
		{
			_loggerFactory = loggerFactory;
			_log = loggerFactory.CreateLogger<BrandBO>();
			_config = config;
		}

		#region Change Data

		public Brand Insert(Brand brand)
		{
			BrandRepository brandRepository;

			try
			{
				brandRepository = new BrandRepository(_loggerFactory, _config);

				if (brand.ID == 0)
				{
					brand = brandRepository.Insert(brand);
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

			return brand;
		}

		public Brand Update(Brand brand)
		{
			BrandRepository brandRepository;

			try
			{
				brandRepository = new BrandRepository(_loggerFactory, _config);

				if (brand.ID == 0)
				{
					throw new Exception("ID diferente de 0, avalie a utilização do POST");
				}
				else
				{
					brandRepository.Update(brand);
				}
			}
			catch (Exception ex)
			{
				throw ex;
			}

			return brand;
		}

		public void Delete(int id)
		{
			BrandRepository brandRepository;
			BlobFileBO blobFileBO;
			Brand brand;

			try
			{
				if (id == 0)
				{
					throw new Exception("ID inválido");
				}
				else
				{
					brandRepository = new BrandRepository(_loggerFactory, _config);
					blobFileBO = new BlobFileBO(_loggerFactory, _config);

					brand = Get(id);
					if (brand != null)
					{
						if (!string.IsNullOrEmpty(brand.DesktopSpotlightImageID))
						{
							blobFileBO.Delete(brand.DesktopSpotlightImageID);
						}
						if (!string.IsNullOrEmpty(brand.MobileSpotlightImageID))
						{
							blobFileBO.Delete(brand.MobileSpotlightImageID);
						}

						brandRepository.Delete(id);
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

		public Brand Get(int id)
		{
			BrandRepository brandRepository;
			Brand brand;

			try
			{
				brandRepository = new BrandRepository(_loggerFactory, _config);

				brand = brandRepository.Get(id);
			}
			catch (Exception ex)
			{
				throw ex;
			}

			return brand;
		}

		public List<Brand> Get(string name = null)
		{
			BrandRepository brandRepository;
			List<Brand> brands;

			try
			{
				brandRepository = new BrandRepository(_loggerFactory, _config);

				brands = brandRepository.Get(name);
			}
			catch (Exception ex)
			{
				throw ex;
			}

			return brands;
		}

		#endregion
	}

	public class BrandImageBO
	{
		private readonly ILoggerFactory _loggerFactory;
		private readonly ILogger<BrandImageBO> _log;
		private readonly IConfiguration _config;

		public BrandImageBO(ILoggerFactory loggerFactory, IConfiguration config)
		{
			_loggerFactory = loggerFactory;
			_log = loggerFactory.CreateLogger<BrandImageBO>();
			_config = config;
		}

		#region Change Data

		public BrandImage Insert(BrandImage image)
		{
			BrandBO brandBO;
			BlobFileBO blobFileBO;
			Brand brand;

			try
			{
				brandBO = new BrandBO(_loggerFactory, _config);
				blobFileBO = new BlobFileBO(_loggerFactory, _config);

				brand = brandBO.Get(image.BrandID);

				if (string.IsNullOrEmpty(image.BlobFile?.ID))
				{
					if (brand != null)
					{
						image.BlobFile = blobFileBO.Insert(image.BlobFile);

						switch (image.Destination)
						{
							case BrandImage.BrandImageDestination.Desktop:
								brand.DesktopSpotlightImageID = image.BlobFile.ID;
								break;
							case BrandImage.BrandImageDestination.Mobile:
								brand.MobileSpotlightImageID = image.BlobFile.ID;
								break;
							default:
								break;
						}

						brand = brandBO.Update(brand);
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

		public BrandImage Update(BrandImage image)
		{
			BrandBO brandBO;
			BlobFileBO blobFileBO;
			Brand brand;

			try
			{
				brandBO = new BrandBO(_loggerFactory, _config);
				blobFileBO = new BlobFileBO(_loggerFactory, _config);

				if (string.IsNullOrEmpty(image.BlobFile?.ID))
				{
					throw new Exception("ID vazio, avalie a utilização do POST");
				}
				else
				{
					brand = brandBO.Get(image.BrandID);

					if (brand != null)
					{
						image.BlobFile = blobFileBO.Update(image.BlobFile);

						switch (image.Destination)
						{
							case BrandImage.BrandImageDestination.Desktop:
								brand.DesktopSpotlightImageID = image.BlobFile.ID;
								break;
							case BrandImage.BrandImageDestination.Mobile:
								brand.MobileSpotlightImageID = image.BlobFile.ID;
								break;
							default:
								break;
						}

						brandBO.Update(brand);
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

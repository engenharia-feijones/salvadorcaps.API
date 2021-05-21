using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using SalvadorCaps.API.Data.Base;
using SalvadorCaps.API.Model;

namespace SalvadorCaps.API.Business
{
	public class BlobFileBO
	{
		private readonly ILoggerFactory _loggerFactory;
		private readonly ILogger<BlobFileBO> _log;
		private readonly IConfiguration _config;

		public BlobFileBO(ILoggerFactory loggerFactory, IConfiguration config)
		{
			_loggerFactory = loggerFactory;
			_log = loggerFactory.CreateLogger<BlobFileBO>();
			_config = config;
		}

		#region Change Data

		public BlobFile Insert(BlobFile blobFile)
		{
			BlobHelper blobHelper;

			try
			{
				blobHelper = new BlobHelper(_loggerFactory, _config);

				if (string.IsNullOrEmpty(blobFile.ID))
				{
					if (!string.IsNullOrEmpty(blobFile?.Name) && blobFile?.Data != null)
					{
						blobFile.ID = Guid.NewGuid().ToString();
						blobFile.ID += Path.GetExtension(blobFile.Name);
						blobFile = blobHelper.InsertOrUpdate(blobFile);
					}
					else
					{
						throw new Exception("Informações insuficientes");
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

			return blobFile;
		}

		public BlobFile Update(BlobFile blobFile)
		{
			BlobHelper blobHelper;
			string oldID;

			try
			{
				blobHelper = new BlobHelper(_loggerFactory, _config);

				if (string.IsNullOrEmpty(blobFile.ID))
				{
					throw new Exception("ID vazio, avalie a utilização do POST");
				}
				else
				{
					if (!string.IsNullOrEmpty(blobFile?.Name) && blobFile?.Data != null)
					{
						oldID = blobFile.ID;
						blobHelper.InsertOrUpdate(blobFile, oldID);
					}
					else
					{
						throw new Exception("Informações insuficientes");
					}
				}
			}
			catch (Exception ex)
			{
				throw ex;
			}

			return blobFile;
		}

		public void Delete(string id)
		{
			BlobHelper blobHelper;

			try
			{
				if (string.IsNullOrEmpty(id))
				{
					throw new Exception("ID inválido");
				}
				else
				{
					blobHelper = new BlobHelper(_loggerFactory, _config);

					blobHelper.Delete(id);
				}
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		#endregion

		#region Retrieve Repository

		public BlobFile Get(string id)
		{
			BlobHelper blobHelper;
			BlobFile blobFile;

			try
			{
				blobHelper = new BlobHelper(_loggerFactory, _config);

				blobFile = blobHelper.Get(id);
			}
			catch (Exception ex)
			{
				throw ex;
			}

			return blobFile;
		}

		#endregion
	}
}

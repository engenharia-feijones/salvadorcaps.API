using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.File;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using SalvadorCaps.API.Model;

namespace SalvadorCaps.API.Data.Base
{
	public class BlobHelper
	{
		private readonly string blobConnection;
		private readonly string blobFolder;

		private readonly ILogger<BlobHelper> _log;
		private readonly IConfiguration _config;

		public BlobHelper(ILoggerFactory loggerFactory, IConfiguration config)
		{
			_log = loggerFactory.CreateLogger<BlobHelper>();
			_config = config;
			blobConnection = _config.GetValue("Blob:Connection")[0];
			blobFolder = _config.GetValue("Blob:Folder")[0];
		}

		public BlobFile Get(string id)
		{
			CloudStorageAccount storageAccount;

			CloudBlobClient blobClient;
			CloudBlobContainer container;
			CloudBlobDirectory directory;

			CloudBlob cloudBlob;

			BlobFile blobFile;

			try
			{
				storageAccount = CloudStorageAccount.Parse(blobConnection);
				blobClient = storageAccount.CreateCloudBlobClient();
				container = blobClient.GetContainerReference(blobFolder);

				directory = container.GetDirectoryReference(string.Empty);
				cloudBlob = directory.GetBlobReference(id);

				cloudBlob.FetchAttributesAsync();

				blobFile = new BlobFile();
				blobFile.ID = cloudBlob.Name;
				blobFile.Created = cloudBlob.Properties.Created?.DateTime;

				using (var downloadFileStream = new MemoryStream())
				{
					cloudBlob.DownloadToStreamAsync(downloadFileStream).Wait();
					blobFile.Data = Convert.ToBase64String(downloadFileStream.ToArray());
				}
			}
			catch (Exception ex)
			{
				throw (ex);
			}

			return blobFile;
		}

		public BlobFile InsertOrUpdate(BlobFile blobFile, string oldID = null)
		{
			CloudStorageAccount storageAccount;

			CloudBlobClient blobClient;
			CloudBlobContainer container;
			CloudBlockBlob blockBlob;

			string baseName;
			string currentExtension;
			string newName;

			byte[] currentData;

			try
			{
				//Getting new ID with fixed extension				
				baseName = Path.GetFileNameWithoutExtension(blobFile.ID);
				currentExtension = Path.GetExtension(blobFile.Name);
				newName = baseName + currentExtension;

				storageAccount = CloudStorageAccount.Parse(blobConnection);
				blobClient = storageAccount.CreateCloudBlobClient();

				container = blobClient.GetContainerReference(blobFolder);

				if (oldID != null)
				{
					blockBlob = container.GetBlockBlobReference(oldID);
					blockBlob.DeleteIfExistsAsync().Wait();
				}

				blockBlob = container.GetBlockBlobReference(blobFile.ID);
				blockBlob.DeleteIfExistsAsync().Wait();

				//Preventing wrong file format
				blobFile.ID = newName;
				blockBlob = container.GetBlockBlobReference(blobFile.ID);
				blockBlob.DeleteIfExistsAsync().Wait();

				currentData = Convert.FromBase64String(blobFile.Data);
				blockBlob.UploadFromByteArrayAsync(currentData, 0, currentData.Length);
			}
			catch (Exception ex)
			{
				throw (ex);
			}

			return blobFile;
		}

		public void Delete(string id)
		{
			CloudStorageAccount storageAccount;

			CloudBlobClient blobClient;
			CloudBlobContainer container;
			CloudBlobDirectory directory;

			CloudBlob cloudBlob;

			try
			{
				storageAccount = CloudStorageAccount.Parse(blobConnection);
				blobClient = storageAccount.CreateCloudBlobClient();
				container = blobClient.GetContainerReference(blobFolder);

				directory = container.GetDirectoryReference(string.Empty);
				cloudBlob = directory.GetBlobReference(id);

				cloudBlob.DeleteIfExistsAsync();
			}
			catch (Exception ex)
			{
				throw (ex);
			}
		}
	}
}

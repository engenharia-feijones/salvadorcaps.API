using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;

namespace SalvadorCaps.API.Model
{
	public class Product
	{
		public long ID { get; set; }

		public int BrandID { get; set; }

		public long Code { get; set; }
		public string Name { get; set; }
		public string ExternalName { get; set; }
		public string Description { get; set; }
		public decimal? Price { get; set; }

		public string ImageID { get; set; }
		public string Image { get; private set; }

		public void LoadUrls(IConfiguration config)
		{
			if (!string.IsNullOrEmpty(ImageID))
				Image = config.GetValue("Blob:BaseURL")[0] + ImageID;
		}

		public List<ProductCategoryDetail> ProductCategoryDetails { get; set; }
	}

	public class ProductImage
	{
		public long ProductID { get; set; }

		public BlobFile BlobFile { get; set; }
	}

	public class ProductCategoryDetail
	{
		public long ProductID { get; set; }
		public int CategoryDetailID { get; set; }
	}
}

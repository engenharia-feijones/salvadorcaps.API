using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SalvadorCaps.API.Model
{
	public class Category
	{
		public int ID { get; set; }

		public string Name { get; set; }

		public bool DesktopSpotlight { get; set; }
		public string DesktopSpotlightImageID { get; set; }
		public string DesktopSpotlightImage { get; set; }

		public bool MobileSpotlight { get; set; }
		public string MobileSpotlightImageID { get; set; }
		public string MobileSpotlightImage { get; set; }

		public void LoadUrls(IConfiguration config)
		{
			if (!string.IsNullOrEmpty(DesktopSpotlightImageID))
				DesktopSpotlightImage = config.GetValue("Blob:BaseURL")[0] + DesktopSpotlightImageID;

			if (!string.IsNullOrEmpty(MobileSpotlightImageID))
				MobileSpotlightImage = config.GetValue("Blob:BaseURL")[0] + MobileSpotlightImageID;
		}
	}

	public class CategoryImage
	{
		public int CategoryID { get; set; }

		public BlobFile BlobFile { get; set; }
		public CategoryImageDestination Destination { get; set; }

		public enum CategoryImageDestination
		{
			Desktop = 1,
			Mobile = 2
		}
	}

	public class CategoryDetail
	{
		public int ID { get; set; }
		public int CategoryID { get; set; }

		public string Name { get; set; }

		public string TitleIconID { get; set; }
		public string TitleIcon { get; private set; }

		public List<Product> Products { get; set; }

		public void LoadUrls(IConfiguration config)
		{
			if (!string.IsNullOrEmpty(TitleIconID))
				TitleIcon = config.GetValue("Blob:BaseURL")[0] + TitleIconID;
		}
	}

	public class CategoryDetailImage
	{
		public int CategoryDetailID { get; set; }

		public BlobFile BlobFile { get; set; }
	}
}

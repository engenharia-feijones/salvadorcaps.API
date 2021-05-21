using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SalvadorCaps.API.Model
{
	public class Brand
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

	public class BrandImage
	{
		public int BrandID { get; set; }

		public BlobFile BlobFile { get; set; }
		public BrandImageDestination Destination { get; set; }

		public enum BrandImageDestination
		{
			Desktop = 1,
			Mobile = 2
		}
	}
}

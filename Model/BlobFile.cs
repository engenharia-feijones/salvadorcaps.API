using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SalvadorCaps.API.Model
{
	public class BlobFile
	{
		public string ID { get; set; }
		public string Name { get; set; }
		public DateTime? Created { get; set; }
		public string Data { get; set; }
	}
}

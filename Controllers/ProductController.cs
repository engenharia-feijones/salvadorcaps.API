using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SalvadorCaps.API.Business;
using SalvadorCaps.API.Model;

namespace SalvadorCaps.API.Controllers
{
	[EnableCors("Policy1")]
	[ApiController]
	[Route("api/[controller]")]
	public class ProductController : ControllerBase
	{
		private readonly ILoggerFactory _loggerFactory;
		private readonly ILogger<ProductController> _log;
		private readonly IConfiguration _config;

		public ProductController(ILoggerFactory loggerFactory, IConfiguration config)
		{
			_loggerFactory = loggerFactory;
			_log = loggerFactory.CreateLogger<ProductController>();
			_config = config;
		}


		[HttpGet]
		public IActionResult Get(long? code = null, string name = null, int? categoryID = null, int? categoryDetailID = null)
		{
			ProductBO productBO;
			List<Product> products;
			ObjectResult response;

			try
			{
				_log.LogInformation("Starting Get()");

				productBO = new ProductBO(_loggerFactory, _config);
				products = productBO.Get(code: code, name: name, categoryID: categoryID, categoryDetailID: categoryDetailID);

				response = Ok(products);

				_log.LogInformation($"Finishing Get() with '{products.Count}' results");
			}
			catch (Exception ex)
			{
				_log.LogError(ex.Message);
				response = StatusCode(500, ex.Message);
			}

			return response;
		}

		// GET: api/Product/5
		[HttpGet("{id}", Name = "GetProduct")]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public IActionResult Get(long id)
		{
			ProductBO productBO;
			Product product;
			ObjectResult response;

			try
			{
				_log.LogInformation($"Starting Get( {id} )");

				productBO = new ProductBO(_loggerFactory, _config);

				product = productBO.Get(id);

				if (product != null)
				{
					response = Ok(product);
				}
				else
				{
					response = NotFound(string.Empty);
				}

				_log.LogInformation($"Finishing Get( {id} )");
			}
			catch (Exception ex)
			{
				_log.LogError(ex.Message);
				response = StatusCode(500, ex.Message);
			}

			return response;
		}

		[HttpPost]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public IActionResult Post([FromBody] Product product)
		{
			ProductBO productBO;
			ObjectResult response;

			try
			{
				_log.LogInformation($"Starting Post('{JsonConvert.SerializeObject(product, Formatting.None)}')");

				productBO = new ProductBO(_loggerFactory, _config);

				product = productBO.Insert(product);

				response = Ok(product);

				_log.LogInformation($"Finishing Post");
			}
			catch (Exception ex)
			{
				_log.LogError(ex.Message);
				response = StatusCode(500, ex.Message);
			}

			return response;
		}

		// PUT: api/Product/5
		[HttpPut("{id}")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public IActionResult Put(long id, [FromBody] Product product)
		{
			ProductBO productBO;
			ObjectResult response;

			try
			{
				_log.LogInformation($"Starting Put( {id}, '{JsonConvert.SerializeObject(product, Formatting.None)}')");

				productBO = new ProductBO(_loggerFactory, _config);

				product.ID = id;
				product = productBO.Update(product);

				response = Ok(product);

				_log.LogInformation($"Finishing Put( {id} )");
			}
			catch (Exception ex)
			{
				_log.LogError(ex.Message);
				response = StatusCode(500, ex.Message);
			}

			return response;
		}

		// DELETE: api/ApiWithActions/5
		[HttpDelete("{id}")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public IActionResult Delete(long id)
		{
			ProductBO productBO;
			ObjectResult response;

			try
			{
				_log.LogInformation($"Starting Delete( {id} )");

				productBO = new ProductBO(_loggerFactory, _config);
				productBO.Delete(id);

				response = Ok(string.Empty);

				_log.LogInformation($"Finishing Delete( {id} )");
			}
			catch (Exception ex)
			{
				_log.LogError(ex.Message);
				response = StatusCode(500, ex.Message);
			}

			return response;
		}
	}
}

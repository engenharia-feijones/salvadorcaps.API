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
	public class BrandController : ControllerBase
	{
		private readonly ILoggerFactory _loggerFactory;
		private readonly ILogger<BrandController> _log;
		private readonly IConfiguration _config;

		public BrandController(ILoggerFactory loggerFactory, IConfiguration config)
		{
			_loggerFactory = loggerFactory;
			_log = loggerFactory.CreateLogger<BrandController>();
			_config = config;
		}


		[HttpGet]
		public IActionResult Get(string name = null)
		{
			BrandBO brandBO;
			List<Brand> brands;
			ObjectResult response;

			try
			{
				_log.LogInformation("Starting Get()");

				brandBO = new BrandBO(_loggerFactory, _config);
				brands = brandBO.Get(name);

				response = Ok(brands);

				_log.LogInformation($"Finishing Get() with '{brands.Count}' results");
			}
			catch (Exception ex)
			{
				_log.LogError(ex.Message);
				response = StatusCode(500, ex.Message);
			}

			return response;
		}

		// GET: api/Brand/5
		[HttpGet("{id}", Name = "GetProductCatagory1")]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public IActionResult Get(int id)
		{
			BrandBO brandBO;
			Brand brand;
			ObjectResult response;

			try
			{
				_log.LogInformation($"Starting Get( {id} )");

				brandBO = new BrandBO(_loggerFactory, _config);

				brand = brandBO.Get(id);

				if (brand != null)
				{
					response = Ok(brand);
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
		public IActionResult Post([FromBody] Brand brand)
		{
			BrandBO brandBO;
			ObjectResult response;

			try
			{
				_log.LogInformation($"Starting Post('{JsonConvert.SerializeObject(brand, Formatting.None)}')");

				brandBO = new BrandBO(_loggerFactory, _config);

				brand = brandBO.Insert(brand);

				response = Ok(brand);

				_log.LogInformation($"Finishing Post");
			}
			catch (Exception ex)
			{
				_log.LogError(ex.Message);
				response = StatusCode(500, ex.Message);
			}

			return response;
		}

		// PUT: api/Brand/5
		[HttpPut("{id}")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public IActionResult Put(int id, Brand brand)
		{
			BrandBO brandBO;
			ObjectResult response;

			try
			{
				_log.LogInformation($"Starting Put( {id}, '{JsonConvert.SerializeObject(brand, Formatting.None)}')");

				brandBO = new BrandBO(_loggerFactory, _config);

				brand.ID = id;
				brand = brandBO.Update(brand);

				response = Ok(brand);

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
		public IActionResult Delete(int id)
		{
			BrandBO brandBO;
			ObjectResult response;

			try
			{
				_log.LogInformation($"Starting Delete( {id} )");

				brandBO = new BrandBO(_loggerFactory, _config);
				brandBO.Delete(id);

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

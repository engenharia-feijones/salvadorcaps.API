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
	public class CategoryDetailController : ControllerBase
	{
		private readonly ILoggerFactory _loggerFactory;
		private readonly ILogger<CategoryDetailController> _log;
		private readonly IConfiguration _config;

		public CategoryDetailController(ILoggerFactory loggerFactory, IConfiguration config)
		{
			_loggerFactory = loggerFactory;
			_log = loggerFactory.CreateLogger<CategoryDetailController>();
			_config = config;
		}


		[HttpGet]
		public IActionResult Get(int? categoryID = null)
		{
			CategoryDetailBO categoryDetailBO;
			List<CategoryDetail> categoryDetails;
			ObjectResult response;

			try
			{
				_log.LogInformation("Starting Get()");

				categoryDetailBO = new CategoryDetailBO(_loggerFactory, _config);
				categoryDetails = categoryDetailBO.Get(categoryID);

				response = Ok(categoryDetails);

				_log.LogInformation($"Finishing Get() with '{categoryDetails.Count}' results");
			}
			catch (Exception ex)
			{
				_log.LogError(ex.Message);
				response = StatusCode(500, ex.Message);
			}

			return response;
		}

		// GET: api/CategoryDetail/5
		[HttpGet("{id}", Name = "GetCategoryDetail")]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public IActionResult Get(int id)
		{
			CategoryDetailBO categoryDetailBO;
			CategoryDetail categoryDetail;
			ObjectResult response;

			try
			{
				_log.LogInformation($"Starting Get( {id} )");

				categoryDetailBO = new CategoryDetailBO(_loggerFactory, _config);

				categoryDetail = categoryDetailBO.Get(id);

				if (categoryDetail != null)
				{
					response = Ok(categoryDetail);
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
		public IActionResult Post([FromBody] CategoryDetail categoryDetail)
		{
			CategoryDetailBO categoryDetailBO;
			ObjectResult response;

			try
			{
				_log.LogInformation($"Starting Post('{JsonConvert.SerializeObject(categoryDetail, Formatting.None)}')");

				categoryDetailBO = new CategoryDetailBO(_loggerFactory, _config);

				categoryDetail = categoryDetailBO.Insert(categoryDetail);

				response = Ok(categoryDetail);

				_log.LogInformation($"Finishing Post");
			}
			catch (Exception ex)
			{
				_log.LogError(ex.Message);
				response = StatusCode(500, ex.Message);
			}

			return response;
		}

		// PUT: api/CategoryDetail/5
		[HttpPut("{id}")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public IActionResult Put(int id, CategoryDetail categoryDetail)
		{
			CategoryDetailBO categoryDetailBO;
			ObjectResult response;

			try
			{
				_log.LogInformation($"Starting Put( {id}, '{JsonConvert.SerializeObject(categoryDetail, Formatting.None)}')");

				categoryDetailBO = new CategoryDetailBO(_loggerFactory, _config);

				categoryDetail.ID = id;
				categoryDetail = categoryDetailBO.Update(categoryDetail);

				response = Ok(categoryDetail);

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
			CategoryDetailBO categoryDetailBO;
			ObjectResult response;

			try
			{
				_log.LogInformation($"Starting Delete( {id} )");

				categoryDetailBO = new CategoryDetailBO(_loggerFactory, _config);
				categoryDetailBO.Delete(id);

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

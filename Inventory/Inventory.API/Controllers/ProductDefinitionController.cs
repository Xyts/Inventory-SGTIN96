using Inventory.API.Models.DTOs;
using Inventory.API.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;

namespace Inventory.API.Controllers
{
    [ApiController]
    [Produces(MediaTypeNames.Application.Json)]
    public class ProductDefinitionController : ControllerBase
    {
        private readonly IProductDefinitionService productDefinitionService;

        public ProductDefinitionController(IProductDefinitionService productDefinitionService)
        {
            this.productDefinitionService = productDefinitionService;
        }

        /// <summary>
        /// Posting product definition data
        /// </summary>
        /// <param name="productDefinitionDTO"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/product/productdefinition")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult PostProductDefinition(ProductDefinitionDTO productDefinitionDTO)
        {
            if (productDefinitionDTO == null)
            {
                return BadRequest();
            }

            var saveProductDefinitionResult = productDefinitionService.SaveProductDefinition(productDefinitionDTO.CompanyPrefix, productDefinitionDTO.CompanyName, productDefinitionDTO.ItemReference, productDefinitionDTO.ProductName);

            if (saveProductDefinitionResult.IsFailure)
            {
                return BadRequest(saveProductDefinitionResult.Error);
            }

            return Ok();

        }

        /// <summary>
        /// Delete product definition
        /// </summary>
        /// <param name="companyPrefix"></param>
        /// <param name="reference"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("api/product/Delete")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult DeleteProductByCompanyPrefixAndReference(long companyPrefix, int reference)
        {
            if (companyPrefix == 0 || reference == 0)
            {
                return BadRequest("CompanyPrefix and reference are mandatory");
            }
            productDefinitionService.DeleteProductByCompanyPrefixAndReference(companyPrefix, reference);
            return Ok();
        }
    }
}

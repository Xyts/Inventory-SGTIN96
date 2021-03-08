using Inventory.API.Models.DTOs;
using Inventory.API.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;

namespace Inventory.API.Controllers
{
    [Produces("application/json")]
    [ApiController]
    public class InventoryController : ControllerBase
    {
        private readonly IInventoryService inventoryService;

        public InventoryController(IInventoryService inventoryService)
        {
            this.inventoryService = inventoryService;
        }

        /// <summary>
        /// Posting inventory data
        /// </summary>
        /// <param name="inventoryDTO"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/inventory/tags")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult PostInventoryData(InventoryDTO inventoryDTO)
        {
            if (inventoryDTO == null) 
            {
                return BadRequest("Inventory data is mandatory!");
            }

            var saveInventoryResult = inventoryService.SaveInventory(inventoryDTO.InventoryId, inventoryDTO.InventoryLocation, inventoryDTO.DateOfInventory, inventoryDTO.SGTIN96Tags);

            if (saveInventoryResult.IsFailure)
            {
                return BadRequest(saveInventoryResult.Error);
            }
            return Ok();
        }

        /// <summary>
        /// Get inventory items by inventory Id
        /// </summary>
        /// <param name="inventoryId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/inventory/items")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetItemsByInventoryId(string inventoryId)
        {
            if (string.IsNullOrWhiteSpace(inventoryId))
            {
                return BadRequest("Inventory id is mandatory");
            }
           
            var resultItemDetails = inventoryService.GetItemsByInventoryId(inventoryId);

            if (resultItemDetails.IsFailure)
            {
                return BadRequest(resultItemDetails.Error);
            }

            return Ok(resultItemDetails.Value);
            
        }

        /// <summary>
        /// Remove inventory items by inventory Id
        /// </summary>
        /// <param name="inventoryId"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("api/inventory/delete")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult DeleteItemsByInventoryId(string inventoryId)
        {
            if (string.IsNullOrWhiteSpace(inventoryId))
            {
                return BadRequest("Inventory is mandatory");
            }

            inventoryService.DeleteItemsByInventoryId(inventoryId);
            return Ok();
        }

        

        /// <summary>
        /// Count of inventoried items grouped by a specific product for a specific inventory 
        /// </summary>
        /// <param name="inventoryId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/inventory/productcount/invetoryid")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetQuantityByProductNumber(string inventoryId)
        {
            if (string.IsNullOrEmpty(inventoryId))
            {
                return BadRequest("Inventory id is blank!");
            }

            var resultQuantityByProductNumber = inventoryService.GetQuantityByProductNumberForInventoryId(inventoryId);
            if (resultQuantityByProductNumber.IsFailure)
            {
                return BadRequest(resultQuantityByProductNumber.Error);
            }
            return Ok(resultQuantityByProductNumber.Value);
        }

        /// <summary>
        /// Count of inventoried items grouped by a specific product per day
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/inventory/productcount/date")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetQuantityByProductNumberAndDate()
        {
            var resultQuantityByProductNumberAndDate = inventoryService.GetQuantityByProductNumberAndDate();
            if (resultQuantityByProductNumberAndDate.IsFailure)
            {
                return BadRequest(resultQuantityByProductNumberAndDate.Error);
            }
            return Ok(resultQuantityByProductNumberAndDate.Value);
        }

        /// <summary>
        /// Count of inventoried items grouped by a specific company
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/inventory/productcount/company")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetQuantityByCompany()
        {
            var resultQuantityByCompany = inventoryService.GetQuantityByCompany();
            if (resultQuantityByCompany.IsFailure)
            {
                return BadRequest(resultQuantityByCompany.Error);
            }

            return Ok(resultQuantityByCompany.Value);
        }
    }
}

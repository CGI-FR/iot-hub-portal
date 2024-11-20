// Copyright (c) CGI France. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace IoTHub.Portal.Server.Controllers.V10
{
    using IoTHub.Portal.Shared.Models.v10;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System;
    using Microsoft.Extensions.Logging;
    using IoTHub.Portal.Application.Services;
    //[Authorize]
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/scopes")]
    [ApiExplorerSettings(GroupName = "Scope Management")]
    public class ScopesController : ControllerBase
    {
        private readonly ILogger<ScopesController> logger;
        private readonly IScopesService scopeService;

        public ScopesController(ILogger<ScopesController> logger, IScopesService scopeService)
        {
            this.logger = logger;
            this.scopeService = scopeService;
        }

        [HttpGet("{id}", Name = "Get a scope By Id")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AccessControlModel))]
        public async Task<IActionResult> GetACById(string id)
        {
            try
            {
                var scope = await scopeService.GetScopeById(id);
                if (scope is null)
                {
                    logger.LogWarning("AccessControl with ID {AcId} not found", id);
                    return NotFound();
                }
                logger.LogInformation("Details retrieved for accessControl with ID {AcId}", id);
                return Ok(scope);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to get details for accessControl with ID {AcId}", id);
                throw;
            }

        }

        /// <summary>
        /// Gets the scopes list.
        /// </summary>
        /// <returns>An array representing the scopes.</returns>
        [HttpGet(Name = "GET Layer list")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ScopeModel>>> GetScopes()
        {
            return Ok(await this.scopeService.GetScopes());
        }

        /// <summary>
        /// Create a new role and the associated actions
        /// </summary>
        /// <param name="scope">Scope details</param>
        /// <returns>HTTP Post response</returns>
        [HttpPost(Name = "POST Create a Scope")]
        //[Authorize(Policy = Policies.CreateScope)]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(ScopeModel))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateScopeAsync(ScopeModel scope)
        {
            try
            {
                var result = await this.scopeService.CreateScope(scope);
                logger.LogInformation("Scope created successfully with ID {ScopeId}", result.Id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogError($"Failed to create scope. : {ex}");
                return BadRequest(ex);
            }
        }

        /// <summary>
        /// Edit an existing scope
        /// </summary>
        /// <param name="scope">Scope's details</param>
        /// <param name="id">Scope id</param>
        /// <returns>HTTP Put response, updated role</returns>
        [HttpPut("{id}", Name = "PUT Edit Scope")]
        //[Authorize(Policy = Policies.EditRole)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> EditScopeAsync(string id, ScopeModel scope)
        {
            try
            {
                var result = await this.scopeService.UpdateScope(id, scope);
                logger.LogInformation("Scope with ID {ScopeID} updated successfully", id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to update scope with ID {Scope}", id);
                throw;
            }
        }


        /// <summary>
        /// Delete a scope by id
        /// </summary>
        /// <param name="id">Scope id that we want to delete</param>
        /// <returns></returns>
        [HttpDelete("{id}", Name = "DELETE Scope")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteScope(string id)
        {
            try
            {
                var result = await this.scopeService.DeleteScope(id);
                if (!result)
                {
                    logger.LogWarning("Scope with ID {ScopeID} not found", id);
                    return NotFound("Scope not found.");
                }
                logger.LogInformation("Scope with ID {ScopeID} deleted successfully", id);
                return NoContent();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to delete Scope with ID {ScopeID}", id);
                throw;
            }
        }

    }
}

﻿// Copyright (c) CGI France. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace AzureIoTHub.Portal.Server.Controllers.V10
{
    using Azure;
    using Azure.Data.Tables;
    using AzureIoTHub.Portal.Server.Factories;
    using AzureIoTHub.Portal.Server.Mappers;
    using AzureIoTHub.Portal.Server.Services;
    using AzureIoTHub.Portal.Shared.Models.V10.Device;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    [Route("/api/settings/device-tags")]
    [ApiController]

    [ApiVersion("1.0")]
    [Produces("application/json")]
    [ApiExplorerSettings(GroupName = "Portal Settings")]
    public class DeviceTagSettingsController : ControllerBase
    {
        /// <summary>
        /// The table client factory.
        /// </summary>
        private readonly ITableClientFactory tableClientFactory;

        /// <summary>
        /// The device tag mapper.
        /// </summary>
        private readonly IDeviceTagMapper deviceTagMapper;

        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger<DeviceTagSettingsController> log;

        /// <summary>
        /// The DeviceTag service.
        /// </summary>
        private readonly IDeviceTagService deviceTagService;

        public const string DefaultPartitionKey = "0";

        /// <summary>
        /// Initializes a new instance of the <see cref="DeviceTagSettingsController"/> class.
        /// </summary>
        /// <param name="log">The logger.</param>
        /// <param name="deviceTagMapper">The device tag mapper.</param>
        /// <param name="tableClientFactory">The table client factory.</param>
        /// <param name="deviceTagService">The device tag service.</param>
        public DeviceTagSettingsController(
            ILogger<DeviceTagSettingsController> log,
            IDeviceTagMapper deviceTagMapper,
            ITableClientFactory tableClientFactory,
            IDeviceTagService deviceTagService)
        {
            this.log = log;
            this.deviceTagMapper = deviceTagMapper;
            this.tableClientFactory = tableClientFactory;
            this.deviceTagService = deviceTagService;
        }

        /// <summary>
        /// Updates the device tag settings to be used in the application.
        /// </summary>
        /// <param name="tags">List of tags.</param>
        /// <returns>The action result.</returns>
        [HttpPost(Name = "POST a set of device tag settings")]
        public async Task<IActionResult> Post(List<DeviceTag> tags)
        {
            await this.deviceTagService.UpdateTags(tags);
            return this.Ok();
        }

        /// <summary>
        /// Gets the device tag settings to be used in the application
        /// </summary>
        /// <returns>The list of tags</returns>
        [HttpGet(Name = "GET a set of device settings")]
        public ActionResult<List<DeviceTag>> Get()
        {
            return this.Ok(deviceTagService.GetAllTags());
        }
    }
}

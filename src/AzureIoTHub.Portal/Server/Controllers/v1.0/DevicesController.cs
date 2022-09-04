// Copyright (c) CGI France. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace AzureIoTHub.Portal.Server.Controllers.V10
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using AzureIoTHub.Portal.Domain;
    using AzureIoTHub.Portal.Domain.Exceptions;
    using AzureIoTHub.Portal.Domain.Repositories;
    using AzureIoTHub.Portal.Models.v10;
    using AzureIoTHub.Portal.Server.Helpers;
    using AzureIoTHub.Portal.Server.Managers;
    using AzureIoTHub.Portal.Server.Mappers;
    using AzureIoTHub.Portal.Server.Services;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using ITableClientFactory = Domain.ITableClientFactory;

    [Authorize]
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/devices")]
    [ApiExplorerSettings(GroupName = "IoT Devices")]
    public class DevicesController : DevicesControllerBase<DeviceListItem, DeviceDetails>
    {
        /// <summary>
        /// The table client factory.
        /// </summary>
        private readonly ITableClientFactory tableClientFactory;

        /// <summary>
        /// The devices service.
        /// </summary>
        private readonly IDeviceService devicesService;

        /// <summary>
        /// The portal database context.
        /// </summary>
        private readonly IUnitOfWork unitOfWork;

        /// <summary>
        /// The device model properties repository.
        /// </summary>
        private readonly IDeviceModelPropertiesRepository deviceModelPropertiesRepository;

        public DevicesController(
            IUnitOfWork unitOfWork,
            ILogger<DevicesController> logger,
            IDeviceService devicesService,
            IDeviceTagService deviceTagService,
            IDeviceProvisioningServiceManager deviceProvisioningServiceManager,
            IDeviceTwinMapper<DeviceListItem, DeviceDetails> deviceTwinMapper,
            IDeviceModelPropertiesRepository deviceModelPropertiesRepository,
            ITableClientFactory tableClientFactory)
            : base(logger, devicesService, deviceTagService, deviceTwinMapper, deviceProvisioningServiceManager, tableClientFactory)
        {
            this.unitOfWork = unitOfWork;
            this.devicesService = devicesService;
            this.tableClientFactory = tableClientFactory;
            this.deviceModelPropertiesRepository = deviceModelPropertiesRepository;
        }

        /// <summary>
        /// Gets the device list.
        /// </summary>
        /// <param name="continuationToken"></param>
        /// <param name="searchText"></param>
        /// <param name="searchStatus"></param>
        /// <param name="searchState"></param>
        /// <param name="pageSize"></param>
        [HttpGet(Name = "GET Device list")]
        public Task<PaginationResult<DeviceListItem>> SearchItems(
            string continuationToken = null,
            string searchText = null,
            bool? searchStatus = null,
            bool? searchState = null,
            int pageSize = 10)
        {
            return base.GetItems("GET Device list", continuationToken, searchText, searchStatus, searchState, pageSize);
        }

        /// <summary>
        /// Gets the specified device.
        /// </summary>
        /// <param name="deviceID">The device identifier.</param>
        [HttpGet("{deviceID}", Name = "GET Device details")]
        public override Task<DeviceDetails> GetItem(string deviceID)
        {
            return base.GetItem(deviceID);
        }

        /// <summary>
        /// Creates the device.
        /// </summary>
        /// <param name="device">The device.</param>
        [HttpPost(Name = "POST Create device")]
        public override Task<IActionResult> CreateDeviceAsync(DeviceDetails device)
        {
            return base.CreateDeviceAsync(device);
        }

        /// <summary>
        /// Updates the device.
        /// </summary>
        /// <param name="device">The device.</param>
        [HttpPut(Name = "PUT Update device")]
        public override Task<IActionResult> UpdateDeviceAsync(DeviceDetails device)
        {
            return base.UpdateDeviceAsync(device);
        }

        /// <summary>
        /// Deletes the specified device.
        /// </summary>
        /// <param name="deviceID">The device identifier.</param>
        [HttpDelete("{deviceID}", Name = "DELETE Remove device")]
        public override Task<IActionResult> Delete(string deviceID)
        {
            return base.Delete(deviceID);
        }

        /// <summary>
        /// Gets the device credentials.
        /// </summary>
        /// <param name="deviceID">The device identifier.</param>
        [HttpGet("{deviceID}/credentials", Name = "GET Device Credentials")]
        public override Task<ActionResult<EnrollmentCredentials>> GetCredentials(string deviceID)
        {
            return base.GetCredentials(deviceID);
        }

        /// <summary>
        /// Gets the device credentials.
        /// </summary>
        /// <param name="deviceID">The device identifier.</param>
        [HttpGet("{deviceID}/properties", Name = "GET Device Properties")]
        public async Task<ActionResult<IEnumerable<DevicePropertyValue>>> GetProperties(string deviceID)
        {
            var device = await this.devicesService.GetDeviceTwin(deviceID);

            if (device == null)
            {
                return NotFound();
            }

            var modelId = DeviceHelper.RetrieveTagValue(device, nameof(DeviceDetails.ModelId));

            if (string.IsNullOrEmpty(modelId))
            {
                return BadRequest("Device has no modelId tag value");
            }

            var result = new List<DevicePropertyValue>();
            JObject desiredPropertiesAsJson;
            JObject reportedPropertiesAsJson;

            try
            {
                desiredPropertiesAsJson = JObject.Parse(device.Properties.Desired.ToJson());
            }
            catch (JsonReaderException e)
            {
                throw new InternalServerErrorException($"Unable to read desired properties for device with id {deviceID}", e);
            }

            try
            {
                reportedPropertiesAsJson = JObject.Parse(device.Properties.Reported.ToJson());
            }
            catch (JsonReaderException e)
            {
                throw new InternalServerErrorException($"Unable to read reported properties for device with id {deviceID}", e);
            }

            foreach (var item in await this.deviceModelPropertiesRepository.GetModelProperties(modelId))
            {
                var value = item.IsWritable ? desiredPropertiesAsJson.SelectToken(item.Name)?.Value<string>() :
                        reportedPropertiesAsJson.SelectToken(item.Name)?.Value<string>();

                result.Add(new DevicePropertyValue
                {
                    DisplayName = item.DisplayName,
                    IsWritable = item.IsWritable,
                    Name = item.Name,
                    PropertyType = item.PropertyType,
                    Value = value
                });
            }

            return result;
        }

        /// <summary>
        /// Gets the device credentials.
        /// </summary>
        /// <param name="deviceID">The device identifier.</param>
        /// <param name="values">The properties values.</param>
        [HttpPost("{deviceID}/properties", Name = "POST Device Properties")]
        public async Task<ActionResult<IEnumerable<DevicePropertyValue>>> SetProperties(string deviceID, IEnumerable<DevicePropertyValue> values)
        {
            var device = await this.devicesService.GetDeviceTwin(deviceID);

            if (device == null)
            {
                return NotFound();
            }

            var modelId = DeviceHelper.RetrieveTagValue(device, nameof(DeviceDetails.ModelId));

            if (string.IsNullOrEmpty(modelId))
            {
                return BadRequest("Device has no modelId tag value");
            }

            var desiredProperties = new Dictionary<string, object>();

            foreach (var item in await this.deviceModelPropertiesRepository.GetModelProperties(modelId))
            {
                if (!item.IsWritable)
                {
                    continue;
                }

                _ = desiredProperties.TryAdd(item.Name, values.FirstOrDefault(x => x.Name == item.Name)?.Value);
            }

            device.Properties.Desired = DeviceHelper.PropertiesWithDotNotationToTwinCollection(desiredProperties);

            _ = await this.devicesService.UpdateDeviceTwin(device);

            return Ok();
        }
    }
}

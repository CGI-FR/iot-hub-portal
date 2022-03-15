// Copyright (c) CGI France. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace AzureIoTHub.Portal.Server.Controllers.V10
{
    using System.Linq;
    using System.Threading.Tasks;
    using Azure.Data.Tables;
    using AzureIoTHub.Portal.Server.Factories;
    using AzureIoTHub.Portal.Server.Filters;
    using AzureIoTHub.Portal.Server.Managers;
    using AzureIoTHub.Portal.Server.Mappers;
    using AzureIoTHub.Portal.Server.Services;
    using AzureIoTHub.Portal.Shared;
    using AzureIoTHub.Portal.Shared.Models.v10;
    using AzureIoTHub.Portal.Shared.Models.v10.Device;
    using AzureIoTHub.Portal.Shared.Models.v10.LoRaWAN.LoRaDevice;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;

    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/lorawan/devices")]
    [ApiExplorerSettings(GroupName = "LoRa WAN")]
    [LoRaFeatureActiveFilter]
    public class LoRaWANDevicesController : DevicesControllerBase<DeviceListItem, LoRaDeviceDetails>
    {
        private readonly ITableClientFactory tableClientFactory;
        private readonly ILoraDeviceMethodManager loraDeviceMethodManager;
        private readonly IDeviceModelCommandMapper deviceModelCommandMapper;
        private readonly IDeviceService deviceService;
        private readonly IDeviceTwinMapper<DeviceListItem, LoRaDeviceDetails> deviceTwinMapper;

        public LoRaWANDevicesController(
            IUrlHelper urlHelper,
            ILogger<LoRaWANDevicesController> logger,
            IDeviceService devicesService,
            IDeviceTagService deviceTagService,
            IDeviceTwinMapper<DeviceListItem, LoRaDeviceDetails> deviceTwinMapper,
            ITableClientFactory tableClientFactory,
            ILoraDeviceMethodManager loraDeviceMethodManager,
            IDeviceModelCommandMapper deviceModelCommandMapper,
            IDeviceProvisioningServiceManager deviceProvisioningServiceManager)
            : base(urlHelper, logger, devicesService, deviceTagService, deviceTwinMapper, deviceProvisioningServiceManager, tableClientFactory)
        {
            this.tableClientFactory = tableClientFactory;
            this.loraDeviceMethodManager = loraDeviceMethodManager;
            this.deviceModelCommandMapper = deviceModelCommandMapper;
            this.deviceService = devicesService;
            this.deviceTwinMapper = deviceTwinMapper;
        }

        /// <summary>
        /// Gets the device list.
        /// </summary>
        /// <returns></returns>
        [HttpGet(Name = "GET LoRaWAN device list")]
        public override Task<PaginationResult<DeviceListItem>> GetItems(
            string continuationToken = null,
            string searchText = null,
            bool? searchStatus = null,
            bool? searchState = null,
            int pageSize = 10)
        {
            return base.GetItems(continuationToken, searchText, searchStatus, searchState, pageSize);
        }

        /// <summary>
        /// Gets the specified device.
        /// </summary>
        /// <param name="deviceID">The device identifier.</param>
        /// <returns></returns>
        [HttpGet("{deviceID}", Name = "GET LoRaWAN device details")]
        public override Task<LoRaDeviceDetails> GetItem(string deviceID)
        {
            return base.GetItem(deviceID);
        }

        /// <summary>
        /// Creates the device.
        /// </summary>
        /// <param name="device">The device.</param>
        /// <returns></returns>
        [HttpPost(Name = "POST Create LoRaWAN device")]
        public override Task<IActionResult> CreateDeviceAsync(LoRaDeviceDetails device)
        {
            return base.CreateDeviceAsync(device);
        }

        /// <summary>
        /// Updates the device.
        /// </summary>
        /// <param name="device">The device.</param>
        /// <returns></returns>
        [HttpPut(Name = "PUT Update LoRaWAN device")]
        public override Task<IActionResult> UpdateDeviceAsync(LoRaDeviceDetails device)
        {
            return base.UpdateDeviceAsync(device);
        }

        /// <summary>
        /// Deletes the specified device.
        /// </summary>
        /// <param name="deviceID">The device identifier.</param>
        /// <returns></returns>
        [HttpDelete("{deviceID}", Name = "DELETE Remove LoRaWAN device")]
        public override Task<IActionResult> Delete(string deviceID)
        {
            return base.Delete(deviceID);
        }

        /// <summary>
        /// Executes the command on the device..
        /// </summary>
        /// <param name="deviceId">The device identifier.</param>
        /// <param name="commandId">The command identifier.</param>
        /// <returns></returns>
        /// <exception cref="System.FormatException">Incorrect port or invalid DevEui Format.</exception>
        [HttpPost("{deviceId}/_command/{commandId}", Name = "POST Execute LoRaWAN command")]
        public async Task<IActionResult> ExecuteCommand(string deviceId, string commandId)
        {

            var twin = await deviceService.GetDeviceTwin(deviceId);
            var modelId = deviceTwinMapper.CreateDeviceDetails(twin, null).ModelId;

            var commandEntity = this.tableClientFactory
                   .GetDeviceCommands()
                   .Query<TableEntity>(filter: $"RowKey eq '{commandId}' and PartitionKey eq '{modelId}'")
                   .Single();

            var deviceModelCommand = this.deviceModelCommandMapper.GetDeviceModelCommand(commandEntity);

            var result = await this.loraDeviceMethodManager.ExecuteLoRaDeviceMessage(deviceId, deviceModelCommand);

            if (!result.IsSuccessStatusCode)
            {
                this.Logger.LogError($"{deviceId} - Execute command on device failed \n{(int)result.StatusCode} - {result.ReasonPhrase}\n{await result.Content.ReadAsStringAsync()}");

                return this.BadRequest("Something went wrong when executing the command.");
            }

            this.Logger.LogInformation($"{deviceId} - Execute command: \n{(int)result.StatusCode} - {result.ReasonPhrase}\n{await result.Content.ReadAsStringAsync()}");

            return this.Ok();
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public override Task<ActionResult<EnrollmentCredentials>> GetCredentials(string deviceID)
        {
            return Task.FromResult<ActionResult<EnrollmentCredentials>>(this.NotFound());
        }
    }
}

// Copyright (c) CGI France. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace AzureIoTHub.Portal.Infrastructure.Services.AWS
{
    using System;
    using System.Threading.Tasks;
    using AzureIoTHub.Portal.Application.Services;
    using Models.v10;
    using AutoMapper;
    using AzureIoTHub.Portal.Domain.Repositories;
    using AzureIoTHub.Portal.Domain;
    using Amazon.IoT.Model;
    using AzureIoTHub.Portal.Application.Services.AWS;
    using Amazon.IotData.Model;
    using AzureIoTHub.Portal.Application.Managers;
    using Infrastructure;
    using Microsoft.Extensions.Logging;

    public class AWSDeviceService : DeviceService
    {
        private readonly IMapper mapper;
        private readonly IUnitOfWork unitOfWork;
        private readonly IDeviceRepository deviceRepository;
        private readonly IAWSExternalDeviceService externalDevicesService;

        public AWSDeviceService(PortalDbContext portalDbContext,
            IMapper mapper,
            IUnitOfWork unitOfWork,
            IDeviceRepository deviceRepository,
            IDeviceTagValueRepository deviceTagValueRepository,
            ILabelRepository labelRepository,
            IDeviceTagService deviceTagService,
            IDeviceModelImageManager deviceModelImageManager,
            IAWSExternalDeviceService externalDevicesService,
            ILogger<AWSDeviceService> logger)
            : base(mapper, unitOfWork, deviceRepository, deviceTagValueRepository, labelRepository, null!, deviceTagService, deviceModelImageManager, null!, portalDbContext, logger)
        {
            this.mapper = mapper;
            this.unitOfWork = unitOfWork;
            this.deviceRepository = deviceRepository;
            this.externalDevicesService = externalDevicesService;
        }

        public override async Task<DeviceDetails> CreateDevice(DeviceDetails device)
        {
            //Create Thing
            var createThingRequest = this.mapper.Map<CreateThingRequest>(device);
            var response = await this.externalDevicesService.CreateDevice(createThingRequest);
            device.DeviceID = response.ThingId;

            //Create Thing Shadow
            var shadowRequest = this.mapper.Map<UpdateThingShadowRequest>(device);
            _ = await this.externalDevicesService.UpdateDeviceShadow(shadowRequest);

            //Create Thing in DB
            return await CreateDeviceInDatabase(device);
        }

        public override async Task<DeviceDetails> UpdateDevice(DeviceDetails device)
        {
            //Update Thing
            var updateThingRequest = this.mapper.Map<UpdateThingRequest>(device);
            _ = await this.externalDevicesService.UpdateDevice(updateThingRequest);

            //Update Thing Shadow
            var shadowRequest = this.mapper.Map<UpdateThingShadowRequest>(device);
            _ = await this.externalDevicesService.UpdateDeviceShadow(shadowRequest);

            //Update Thing in DB
            return await UpdateDeviceInDatabase(device);
        }

        public override Task DeleteDevice(string deviceId)
        {
            throw new NotImplementedException();
        }
    }
}

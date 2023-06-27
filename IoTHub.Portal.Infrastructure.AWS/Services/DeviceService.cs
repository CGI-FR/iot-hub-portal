// Copyright (c) CGI France. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace IoTHub.Portal.Infrastructure.AWS.Services
{
    using Amazon.IoT;
    using Amazon.IoT.Model;
    using Amazon.IotData;
    using Amazon.IotData.Model;
    using AutoMapper;
    using IoTHub.Portal.Application.Managers;
    using IoTHub.Portal.Application.Services;
    using IoTHub.Portal.Domain;
    using IoTHub.Portal.Domain.Exceptions;
    using IoTHub.Portal.Domain.Repositories;
    using IoTHub.Portal.Infrastructure.Common;
    using Microsoft.Extensions.Logging;
    using Models.v10;
    using System.Threading.Tasks;
    using ResourceNotFoundException = Domain.Exceptions.ResourceNotFoundException;

    public class DeviceService : Common.Services.DeviceService
    {
        private readonly IMapper mapper;
        private readonly IDeviceRepository deviceRepository;
        private readonly IAmazonIoT amazonIoTClient;
        private readonly IAmazonIotData amazonIotDataClient;
        private readonly ILogger<DeviceService> logger;


        public DeviceService(PortalDbContext portalDbContext,
            IMapper mapper,
            IUnitOfWork unitOfWork,
            IDeviceRepository deviceRepository,
            IDeviceTagValueRepository deviceTagValueRepository,
            ILabelRepository labelRepository,
            IDeviceTagService deviceTagService,
            IDeviceModelImageManager deviceModelImageManager,
            IAmazonIoT amazonIoTClient,
            IAmazonIotData amazonIotDataClient,
            IExternalDeviceService externalDeviceService,
            ILogger<DeviceService> logger)
            : base(mapper, unitOfWork, deviceRepository, deviceTagValueRepository, labelRepository, externalDeviceService, deviceTagService, deviceModelImageManager, portalDbContext, logger)
        {
            this.mapper = mapper;
            this.deviceRepository = deviceRepository;
            this.amazonIoTClient = amazonIoTClient;
            this.amazonIotDataClient = amazonIotDataClient;
            this.logger = logger;
        }

        public override async Task<DeviceDetailsDto> CreateDevice(DeviceDetailsDto device)
        {
            //Create Thing
            try
            {
                var thingResponse = await amazonIoTClient.CreateThingAsync(mapper.Map<CreateThingRequest>(device));
                device.DeviceID = thingResponse.ThingId;

                try
                {
                    //Create Thing Shadow
                    var shadowResponse = await amazonIotDataClient.UpdateThingShadowAsync(mapper.Map<UpdateThingShadowRequest>(device));

                    //Create Thing in DB
                    return await CreateDeviceInDatabase(device);
                }
                catch (AmazonIotDataException e)
                {
                    throw new InternalServerErrorException($"Unable to create/update the thing shadow with device name : {device.DeviceName} due to an error in the Amazon IoT API.", e);
                }

            }
            catch (AmazonIoTException e)
            {
                throw new InternalServerErrorException($"Unable to create the thing with device name : {device.DeviceName} due to an error in the Amazon IoT API.", e);

            }

        }

        public override async Task<DeviceDetailsDto> UpdateDevice(DeviceDetailsDto device)
        {
            try
            {
                //Update Thing
                var thingResponse = await amazonIoTClient.UpdateThingAsync(mapper.Map<UpdateThingRequest>(device));

                //Update Thing in DB
                return await UpdateDeviceInDatabase(device);
            }
            catch (AmazonIoTException e)
            {
                throw new InternalServerErrorException($"Unable to update the thing with device name : {device.DeviceName} due to an error in the Amazon IoT API.", e);
            }

        }

        public override async Task DeleteDevice(string deviceId)
        {
            //Get device in DB
            var device = await deviceRepository.GetByIdAsync(deviceId);

            if (device == null)
            {
                throw new ResourceNotFoundException($"The device with id {deviceId} doesn't exist");
            }

            try
            {
                try
                {
                    //Retrieve all thing principals and detach it before deleting the thing
                    var principals = await amazonIoTClient.ListThingPrincipalsAsync(new ListThingPrincipalsRequest
                    {
                        NextToken = string.Empty,
                        ThingName = device.Name
                    });

                    try
                    {
                        foreach (var principal in principals.Principals)
                        {
                            _ = await amazonIoTClient.DetachThingPrincipalAsync(new DetachThingPrincipalRequest
                            {
                                Principal = principal,
                                ThingName = device.Name
                            });
                        }
                    }
                    catch (AmazonIoTException e)
                    {
                        logger.LogWarning("Can not detach Thing principal because it doesn't exist in AWS IoT", e);
                    }

                    try
                    {
                        //Delete the thing type after detaching the principal
                        _ = await amazonIoTClient.DeleteThingAsync(mapper.Map<DeleteThingRequest>(device));
                    }
                    catch (AmazonIoTException e)
                    {
                        logger.LogWarning("Can not delete the thing because it doesn't exist in AWS IoT", e);
                    }

                }
                catch (AmazonIoTException e)
                {
                    logger.LogWarning("Can not retreive Thing  because it doesn't exist in AWS IoT", e);
                }

            }
            catch (AmazonIoTException e)
            {
                logger.LogWarning("Can not delete the device because it doesn't exist in AWS IoT", e);

            }
            finally
            {
                //Delete Thing in DB
                await DeleteDeviceInDatabase(deviceId);
            }

        }
    }
}
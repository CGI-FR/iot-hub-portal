// Copyright (c) CGI France. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace AzureIoTHub.Portal.Infrastructure.Repositories
{
    using AzureIoTHub.Portal.Domain.Repositories;
    using Domain.Entities;

    public class DeviceTagRepository : GenericRepository<DeviceTag>, IDeviceTagRepository
    {
        public DeviceTagRepository(PortalDbContext context) : base(context)
        {
        }
    }
}

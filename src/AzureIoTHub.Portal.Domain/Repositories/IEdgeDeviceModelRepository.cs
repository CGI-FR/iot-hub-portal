// Copyright (c) CGI France. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace AzureIoTHub.Portal.Domain.Repositories
{
    using Entities;

    public interface IEdgeDeviceModelRepository : IRepository<EdgeDeviceModel>
    {
        Task<EdgeDeviceModel?> GetByNameAsync(string edgeModelDevice);
    }
}

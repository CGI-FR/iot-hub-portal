// Copyright (c) CGI France. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.


namespace AzureIoTHub.Portal.Infrastructure.Repositories
{
    using Domain.Entities;
    using Domain.Repositories;

    public class EdgeModelRepository : GenericRepository<EdgeDeviceModel>, IEdgeModelRepository
    {
        public EdgeModelRepository(PortalDbContext context) : base(context)
        {
        }
    }
}

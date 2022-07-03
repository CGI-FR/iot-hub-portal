// Copyright (c) CGI France. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace AzureIoTHub.Portal.Client.Services
{
    using System.Threading.Tasks;
    using Portal.Models.v10.LoRaWAN;

    public interface ILoRaWanConcentratorsClientService
    {
        Task<PaginationResult<Concentrator>> GetConcentrators(string continuationUri);
    }
}

// Copyright (c) CGI France. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace IoTHub.Portal.Client.Services
{
    using System.Net.Http;
    using System.Net.Http.Json;
    using System.Threading.Tasks;
    using IoTHub.Portal.Shared.Models.v10;

    public class GroupsClientService : IGroupsClientService
    {
        private readonly HttpClient http;

        public GroupsClientService(HttpClient http)
        {
            this.http = http;
        }

        public Task<PaginationResult<GroupModel>> GetDevices(string continuationUri)
        {
            return this.http.GetFromJsonAsync<PaginationResult<GroupModel>>(continuationUri)!;
        }
    }
}

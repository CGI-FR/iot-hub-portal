// Copyright (c) CGI France. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace IoTHub.Portal.Application.Services
{
    using IoTHub.Portal.Shared.Models.v10;

    public interface IScopesService
    {
        Task<IEnumerable<ScopeModel>> GetScopes();
        Task<ScopeModel> GetScopeById(string Id);

        Task<ScopeModel> CreateScope(ScopeModel scope);
        Task<ScopeModel?> UpdateScope(string id, ScopeModel scope);
        Task<bool> DeleteScope(string id);
    }
}

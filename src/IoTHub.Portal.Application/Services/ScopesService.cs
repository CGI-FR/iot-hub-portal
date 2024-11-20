// Copyright (c) CGI France. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace IoTHub.Portal.Application.Services
{
    using System;
    using System.Threading.Tasks;
    using AutoMapper;
    using IoTHub.Portal.Domain.Repositories;
    using IoTHub.Portal.Domain;
    using IoTHub.Portal.Shared.Models.v10;
    using IoTHub.Portal.Domain.Exceptions;
    using IoTHub.Portal.Crosscutting;
    using IoTHub.Portal.Domain.Entities;

    internal class ScopesService : IScopesService
    {
        private readonly IScopeRepository scopeRepository;
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public ScopesService(IScopeRepository scopeRepository, IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.scopeRepository = scopeRepository;
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }
        public async Task<ScopeModel> CreateScope(ScopeModel scope)
        {
            if (scope is null)
            {
                throw new ArgumentNullException(nameof(scope));
            }

            var scopeEntity = this.mapper.Map<Scope>(scope);
            await this.scopeRepository.InsertAsync(scopeEntity);
            await this.unitOfWork.SaveAsync();

            return this.mapper.Map<ScopeModel>(scopeEntity);
        }

        public async Task<bool> DeleteScope(string id)
        {
            var scopeEntity = await this.scopeRepository.GetByIdAsync(id);
            if (scopeEntity is null)
            {
                throw new ResourceNotFoundException($"The Scope with id {id} doesn't exist");
            }

            this.scopeRepository.Delete(id);

            await this.unitOfWork.SaveAsync();
            return true;
        }

        public async Task<ScopeModel> GetScopeById(string Id)
        {
            var scopeEntity = await this.scopeRepository.GetByIdAsync(Id);
            if (scopeEntity is null)
            {
                throw new ResourceNotFoundException($"The role with name {Id} doesn't exist");
            }
            var scopeModel = this.mapper.Map<ScopeModel>(scopeEntity);
            return scopeModel;
        }

        public async Task<IEnumerable<ScopeModel>> GetScopes()
        {
            var layerPredicate = PredicateBuilder.True<ScopeModel>();

            var layers = await this.scopeRepository.GetAllAsync();

            return layers
                .Select(entity =>
                {
                    var layerListItem = this.mapper.Map<ScopeModel>(entity);
                    return layerListItem;
                })
                .ToList();
        }

        public async Task<ScopeModel?> UpdateScope(string id, ScopeModel scope)
        {
            if (scope is null)
            {
                throw new ArgumentNullException(nameof(scope));
            }

            var scopeEntity = await this.scopeRepository.GetByIdAsync(id);
            if (scopeEntity is null)
            {
                throw new ResourceNotFoundException($"The scope with id {id} doesn't exist");
            }

            scopeEntity.Name = scope.Name;
            scopeEntity.Father = scope.Father;
            scopeRepository.Update(scopeEntity);
            await this.unitOfWork.SaveAsync();

            return this.mapper.Map<ScopeModel>(scopeEntity);

        }
    }
}

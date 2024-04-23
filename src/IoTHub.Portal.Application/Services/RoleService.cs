// Copyright (c) CGI France. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace IoTHub.Portal.Application.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;
    using AutoMapper;
    using IoTHub.Portal.Domain;
    using IoTHub.Portal.Domain.Entities;
    using IoTHub.Portal.Domain.Repositories;
    using IoTHub.Portal.Shared.Models.v1._0;
    using IoTHub.Portal.Shared.Models.v10;
    using IoTHub.Portal.Shared.Models.v10.Filters;
    using Action = Domain.Entities.Action;
    using IoTHub.Portal.Domain.Exceptions;
    using IoTHub.Portal.Crosscutting;

    internal class RoleService : IRoleManagementService
    {
        private readonly IRoleRepository roleRepository;
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly IActionRepository actionRepository;

        public RoleService(IRoleRepository roleRepository, IUnitOfWork unitOfWork, IMapper mapper, IActionRepository actionRepository)
        {
            this.roleRepository = roleRepository;
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.actionRepository = actionRepository;
        }


        public async Task<bool> DeleteRole(string id)
        {
            var actionsToRemove = new List<string>();
            var role = await this.roleRepository.GetByIdAsync(id, r => r.Actions);
            if (role == null)
            {
                return false;
            }
            foreach (var action in role.Actions)
            {
                actionsToRemove.Add(action.Id);
            }
            foreach (var action in actionsToRemove)
            {
                this.actionRepository.Delete(action);
            }
            roleRepository.Delete(role.Id);
            await unitOfWork.SaveAsync();
            return true;
        }

        public async Task<PaginatedResult<RoleModel>> GetRolePage(
            string? searchKeyword = null,
            int pageSize = 10,
            int pageNumber = 0,
            string[] orderBy = null)
        {
            var roleFilter = new  RoleFilter
            {
                Keyword = searchKeyword,
                PageSize = pageSize,
                PageNumber = pageNumber,
                OrderBy = orderBy
            };

            var rolePredicate = PredicateBuilder.True<Role>();
            if (!string.IsNullOrWhiteSpace(roleFilter.Keyword))
            {
                rolePredicate = rolePredicate.And(role => role.Name.ToLower().Contains(roleFilter.Keyword.ToLower()) ||
                role.Description.ToLower().Contains(roleFilter.Keyword.ToLower()));
            }

            var paginatedRole = await this.roleRepository.GetPaginatedListAsync(pageNumber,
                pageSize,
                orderBy,
                rolePredicate,
                includes: new Expression<Func<Role, object>>[] { role => role.Actions});

            var paginatedRoleDto = new PaginatedResult<RoleModel>
            {
                Data = paginatedRole.Data.Select(x => this.mapper.Map<RoleModel>(x)).ToList(),
                TotalCount = paginatedRole.TotalCount,
                CurrentPage = paginatedRole.CurrentPage,
                PageSize = pageSize
            };

            return new PaginatedResult<RoleModel>(paginatedRoleDto.Data, paginatedRoleDto.TotalCount);

        }

        async Task<RoleDetailsModel> IRoleManagementService.GetRoleDetailsAsync(string roleName)
        {
            var roleEntity = await this.roleRepository.GetByNameAsync(roleName, r => r.Actions);
            if (roleEntity is null)
            {
                throw new ResourceNotFoundException($"The role with name {roleName} doesn't exist");
            }
            var roleModel = this.mapper.Map<RoleDetailsModel>(roleEntity);
            return roleModel;
        }

        async Task<RoleDetailsModel> IRoleManagementService.CreateRole(RoleDetailsModel role)
        {
            var roleEntity = this.mapper.Map<Role>(role);
            await this.roleRepository.InsertAsync(roleEntity);
            await this.unitOfWork.SaveAsync();


            var createdRole = await this.roleRepository.GetByIdAsync(roleEntity.Id);
            var createdRoleDto = this.mapper.Map<RoleDetailsModel>(createdRole);

            return createdRoleDto;
        }

        async Task<RoleDetailsModel?> IRoleManagementService.UpdateRole(string roleName, RoleDetailsModel role)
        {
            var roleEntity = await this.roleRepository.GetByNameAsync(roleName, r => r.Actions);
            if (roleEntity is null)
            {
                throw new ResourceNotFoundException($"The role with name {roleName} doesn't exist");
            }

            roleEntity.Name = role.Name;
            roleEntity.Description = role.Description;
            roleEntity.Actions = UpdateRoleActions(roleEntity.Actions, role.Actions.Select(a => new Action { Name = a }).ToList());
            this.roleRepository.Update(roleEntity);
            await this.unitOfWork.SaveAsync();

            var updatedRole = await this.roleRepository.GetByIdAsync(roleEntity.Id);
            var updatedRoleDto = this.mapper.Map<RoleDetailsModel>(updatedRole);

            return updatedRoleDto;
        }

        private static ICollection<Action> UpdateRoleActions(ICollection<Action> existingActions, ICollection<Action> newActions)
        {
            var updatedActions = new HashSet<Action>(existingActions);
            foreach (var action in newActions)
            {
                if (!updatedActions.Any(a => a.Name == action.Name))
                {
                    _ = updatedActions.Add(action);
                }
            }
            _ = updatedActions.RemoveWhere(a => !newActions.Any(na => na.Name == a.Name));
            return updatedActions;
        }
    }
}
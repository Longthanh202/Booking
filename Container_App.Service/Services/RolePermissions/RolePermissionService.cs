using Container_App.Common.Shared;
using Container_App.Core.Model.RolePermissions;
using Container_App.Data;
using Container_App.Data.Connection;
using Container_App.Data.Repository.RolePermissions;
using Container_App.Service.Dtos.RolePermission;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Container_App.Service.Services.RolePermissions
{
    public class RolePermissionService : IRolePermissionService
    {
        private readonly IRolePermissionRepository _rolePermissionRepository;
        private readonly IUnitOfWork _unitOfWork;
        public RolePermissionService(IRolePermissionRepository rolePermissionRepository, IUnitOfWork unitOfWork)
        {
            _rolePermissionRepository = rolePermissionRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task Insert(RolePermissionRequset input)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {               
                List<RolePermission> rolePermissions = input.Permissions.Select(p => new RolePermission
                {
                    Id = Guid.NewGuid(),
                    RoleId = input.RoleId,
                    ResourceId = p.ResourceId,
                    PermissionId = p.PermissionId,
                    
                }).ToList();
                await _rolePermissionRepository.Insert(input.RoleId, rolePermissions);
                await _unitOfWork.CommitAsync();
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                Console.WriteLine($"Error inserting role permissions: {ex.Message}");
                throw;
            }
        }
    }
}

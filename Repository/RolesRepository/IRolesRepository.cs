namespace E_CommerceApi.Repository.RolesRepository.RolesRepository
{
    public interface IRolesRepository
    {
        Task<List<string>> GetAllRoles();
        Task<bool> AddRoleAsync(string Name);
        Task<bool> DeleteRole(string Name);
    }
}

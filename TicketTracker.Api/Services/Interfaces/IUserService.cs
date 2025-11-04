public interface IUserService
{
    Task<List<UserDto>?> GetAllAsync(CancellationToken cancellationToken = default);
}
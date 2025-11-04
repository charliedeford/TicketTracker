public class UserService(IUserRepository userRepository) : IUserService
{
    public async Task<List<UserDto>?> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await userRepository.GetAllAsync(cancellationToken);
    }
}
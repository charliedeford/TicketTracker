public class UserService(IUserRepository userRepository) : IUserService
{
    public async Task<List<UserDto>?> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var response = await userRepository.GetAllAsync(cancellationToken);
        return response?.Select(u => new UserDto(u.Id, u.Username)).ToList();
    }
}
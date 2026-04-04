using NearServe.DTOs;
using NearServe.Models;

namespace NearServe.Services.Interfaces
{
    public class IUserService
    {
        Task<string> RegisterUserAsync(RegisterUserDto dto);
        Task<string> LoginAsync(LoginDto dto);
        Task<User?> GetUserByIdAsync(int cid);
        Task<List<User>> GetAllUsersAsync();
        Task<string> UpdateUserAsync(int cid, UpdateUserDto dto);
        Task<string> DeleteUserAsync(int cid);
        Task<string> SetPinAsync(SetPinDto dto);
        Task<string> ResetPinAsync(ResetPinDto dto);
        Task<decimal> GetWalletBalanceAsync(int cid);
        Task<string> TransferWalletToBankAsync(WalletTransferDto dto);
    }
}

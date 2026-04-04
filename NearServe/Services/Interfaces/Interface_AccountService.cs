using NearServe.Models;
using NearServe.DTOs;

namespace NearServe.Services.Interfaces
{
    public class Interface_AccountService
    {
        Task<string> CreateAccountAsync(CreateAccountDto dto);
        Task<Account?> GetAccountByIdAsync(int id);
        Task<List<Account>> GetAllAccountsAsync();
        Task<string> UpdateBalanceAsync(UpdateBalanceDto dto);
        Task<string> DeleteAccountAsync(int id);

        Task<decimal> GetBalanceAsync(CheckBalanceDto dto);
        Task<string> TransferAsync(TransferDto dto);

    }
}

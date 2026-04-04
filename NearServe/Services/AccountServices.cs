using NearServe.DTOs;
using NearServe.Models;
using NearServe.Services.Interfaces;
using Microsoft.Data.SqlClient;

namespace NearServe.Services
{
    public class AccountServices : Interface_AccountService
    {
        private readonly string _connectionString;

        public AccountServices(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")!;
        }

        // Create Account
        public async Task<string> CreateAccountAsync(CreateAccountDto dto)
        {
            using SqlConnection conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            string checkQuery = @"SELECT COUNT(*) FROM Accounts 
                                 WHERE Account_No = @Account_No";

            using (SqlCommand checkCmd = new SqlCommand(checkQuery, conn))
            {
                checkCmd.Parameters.AddWithValue("@Account_No", dto.Account_No);
                object? countObj = await checkCmd.ExecuteScalarAsync();
                int count = (countObj != null && countObj != DBNull.Value) ? Convert.ToInt32(countObj) : 0;

                if (count > 0)
                    return "Account already exists.";
            }

            string insertQuery = @"INSERT INTO Accounts
                                  (Account_No, Customer_Name, Branch, Bank_Name, Balance)
                                  VALUES
                                  (@Account_No, @Customer_Name, @Branch, @Bank_Name, @Balance)";

            using SqlCommand cmd = new SqlCommand(insertQuery, conn);

            cmd.Parameters.AddWithValue("@Account_No", dto.Account_No);
            cmd.Parameters.AddWithValue("@Customer_Name", dto.Customer_Name);
            cmd.Parameters.AddWithValue("@Branch", dto.Branch);
            cmd.Parameters.AddWithValue("@Bank_Name", dto.Bank_Name);
            cmd.Parameters.AddWithValue("@Balance", dto.Balance);

            int rows = await cmd.ExecuteNonQueryAsync();
            return rows > 0 ? "Account created successfully." : "Failed to create account.";
        }

        // Get Account by ID
        public async Task<Account?> GetAccountByIdAsync(int cid)
        {
            using SqlConnection conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            string query = "SELECT * FROM Accounts WHERE CID = @CID";

            using SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@CID", cid);

            using SqlDataReader reader = await cmd.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return new Account
                {
                    CID = Convert.ToInt32(reader["CID"]),
                    Account_No = reader["Account_No"].ToString() ?? "",
                    Customer_Name = reader["Customer_Name"].ToString() ?? "",
                    Branch = reader["Branch"].ToString() ?? "",
                    Bank_Name = reader["Bank_Name"].ToString() ?? "",
                    Balance = Convert.ToDecimal(reader["Balance"])
                };
            }

            return null;
        }

        // Update Balance
        public async Task<string> UpdateBalanceAsync(UpdateBalanceDto dto)
        {
            using SqlConnection conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            string query = @"UPDATE Accounts 
                             SET Balance = Balance + @Amount
                             WHERE CID = @CID";

            using SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@Amount", dto.Amount);
            cmd.Parameters.AddWithValue("@CID", dto.AccountId);

            int rows = await cmd.ExecuteNonQueryAsync();
            return rows > 0 ? "Balance updated successfully." : "Account not found.";
        }

        // Check Balance
        public async Task<decimal> GetBalanceAsync(CheckBalanceDto dto)
        {
            using SqlConnection conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            string query = "SELECT Balance FROM Accounts WHERE CID = @CID";

            using SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@CID", dto.AccountId);

            object? result = await cmd.ExecuteScalarAsync();

            return result != null ? Convert.ToDecimal(result) : -1;
        }

        // Transfer Amount
        public async Task<string> TransferAsync(TransferAmountDto dto)
        {
            using SqlConnection conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            SqlTransaction transaction = conn.BeginTransaction();

            try
            {
                // Check Balance
                string checkQuery = "SELECT Balance FROM Accounts WHERE CID = @FromId";

                using SqlCommand checkCmd = new SqlCommand(checkQuery, conn, transaction);
                checkCmd.Parameters.AddWithValue("@FromId", dto.FromAccountId);

                object? result = await checkCmd.ExecuteScalarAsync();

                if (result == null)
                    return "Sender account not found";

                decimal balance = Convert.ToDecimal(result);

                if (balance < dto.Amount)
                    return "Insufficient balance";

                // Debit
                string deductQuery = @"UPDATE Accounts 
                                       SET Balance = Balance - @Amount 
                                       WHERE CID = @FromId";

                using SqlCommand deductCmd = new SqlCommand(deductQuery, conn, transaction);
                deductCmd.Parameters.AddWithValue("@Amount", dto.Amount);
                deductCmd.Parameters.AddWithValue("@FromId", dto.FromAccountId);

                await deductCmd.ExecuteNonQueryAsync();

                // Credit
                string addQuery = @"UPDATE Accounts 
                                    SET Balance = Balance + @Amount 
                                    WHERE CID = @ToId";

                using SqlCommand addCmd = new SqlCommand(addQuery, conn, transaction);
                addCmd.Parameters.AddWithValue("@Amount", dto.Amount);
                addCmd.Parameters.AddWithValue("@ToId", dto.ToAccountId);

                await addCmd.ExecuteNonQueryAsync();

                transaction.Commit();
                return "Transfer successful";
            }
            catch
            {
                transaction.Rollback();
                return "Transfer failed";
            }
        }
    }
}
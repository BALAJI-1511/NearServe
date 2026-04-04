using Microsoft.VisualBasic;
using NearServe.DTOs;
using NearServe.Models;
using NearServe.Services.Interfaces;
using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography.X509Certificates;

namespace NearServe.Services
{
    public class UserService : IUserService
    {
        private readonly string _connectionString;

        public UserService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")!;
        }
        public async Task<string> RegisterUserAsync(RegisterUserDto dto)
        {
            // Implementation for registering a user
            using SqlConnection conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            string checkQuery = @"SELECT COUNT(*) FROM Users
                                  WHERE Username = @Username OR Email = @Email OR PhoneNumber";

            using (SqlCommand checkCmd = new SqlCommand(checkQuery, conn))
            {
                checkCmd.Parameters.AddWithValue("@Username", dto.Username);
                checkCmd.Parameters.AddWithValue("@Email", dto.Email);
                checkCmd.Parameters.AddWithValue("@PhoneNumber", dto.PhoneNumber);
                int existingCount = (int)await checkCmd.ExecuteScalarAsync();
                if (existingCount > 0)
                
                    return "Username, email, or phone number already exists.";
                }
                string insertQuery = @"INSERT INTO Users
                                       (FullName, DOB, Gender. PhoneNumber, Email, Address, Username, PasswordHash, Role, CreatedAt, UpdatedAt)
                                       VALUES
                                       (@FullName, @DOB, @Gender, @PhoneNumber, @Email, @Address, @Username, @PasswordHash, @Role, GETDATE(), GETDATE())";
                using SqlCommand cmd = new SqlCommand(insertQuery, conn);
                cmd.Parameters.AddWithValue("@FullName", dto.FullName);
                cmd.Parameters.AddWithValue("@DOB", dto.DOB);
                cmd.Parameters.AddWithValue("@Gender", dto.Gender);
                cmd.Parameters.AddWithValue("@PhoneNumber", dto.PhoneNumber);
                cmd.Parameters.AddWithValue("@Email", dto.Email);
                cmd.Parameters.AddWithValue("@Address", dto.Address);
                cmd.Parameters.AddWithValue("@Username", dto.Username);
                cmd.Parameters.AddWithValue("@PasswordHash", dto.Password);
                cmd.Parameters.AddWithValue("@Role", dto.Role);

                int rows = await cmd.ExecuteNonQueryAsync();
                return rows > 0 ? "User registered successfully." : "Failed to register user.";
            }


        public async Task<string> LoginAsync(LoginDto dto)
        {
            // Implementation for user login
            using SqlConnection conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            string query = @"SELECT PasswordHash FROM Users WHERE Username = @Username";
            using SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@Username", dto.Username);

            object? result = await cmd.ExecuteScalarAsync();

            if (result == null)
                return "User not found";

            string storedHash = result.ToString()!;
            string inputHash = dto.Password;

            return storedHash == inputHash ? "Login successful." : "Invalid password.";
        }
        public async Task<User?> GetUserByIdAsync(int cid)
        {
            // Implementation for getting user by ID
            using SqlConnection conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();
            string query = "SELECT FROM Users WHERE CID = @CID AND IsActive = 1";
            using SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@CID", cid);
            using SqlDataReader reader = await cmd.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return new User
                {
                    CID = Convert.ToInt32(reader["CID"]),
                    FullName = reader["FullName"].ToString() ?? "",
                    DOB = Convert.ToDateTime(reader["DOB"]),
                    Gender = reader["Gender"].ToString() ?? "",
                    PhoneNumber = reader["PhoneNumber"].ToString() ?? "",
                    Email = reader["Email"].ToString() ?? "",
                    Address = reader["Address"].ToString() ?? "",
                    Username = reader["Username"].ToString() ?? "",
                    PasswordHash = reader["PasswordHash"].ToString() ?? "",
                    PinHash = reader["PinHash"]?.ToString(),
                    Role = reader["Role"].ToString() ?? "",
                    AccountNumber = reader["AccountNumber"]?.ToString(),
                    WalletBalance = Convert.ToDecimal(reader["WalletBalance"]),
                    IsActive = Convert.ToBoolean(reader["IsActive"]),
                    CreatedAt = Convert.ToDateTime(reader["CreatedAt"]),
                    UpdatedAt = Convert.ToDateTime(reader["UpdatedAt"])
                };

            }
            return null;

        }
        public async Task<List<User>> GetAllUsersAsync()
        {
            List<User> users = new List<User>();

            using SqlConnection conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            string query = "SELECT * FROM Users WHERE IsActive = 1";

            using SqlCommand cmd = new SqlCommand(query, conn);
            using SqlDataReader reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                users.Add(new User
                {
                    CID = Convert.ToInt32(reader["CID"]),
                    FullName = reader["FullName"].ToString() ?? "",
                    DOB = Convert.ToDateTime(reader["DOB"]),
                    Gender = reader["Gender"].ToString() ?? "",
                    PhoneNumber = reader["PhoneNumber"].ToString() ?? "",
                    Email = reader["Email"].ToString() ?? "",
                    Address = reader["Address"].ToString() ?? "",
                    Username = reader["Username"].ToString() ?? "",
                    PasswordHash = reader["PasswordHash"].ToString() ?? "",
                    PinHash = reader["PinHash"]?.ToString(),
                    Role = reader["Role"].ToString() ?? "",
                    AccountNumber = reader["AccountNumber"]?.ToString(),
                    WalletBalance = Convert.ToDecimal(reader["WalletBalance"]),
                    IsActive = Convert.ToBoolean(reader["IsActive"]),
                    CreatedAt = Convert.ToDateTime(reader["CreatedAt"]),
                    UpdatedAt = Convert.ToDateTime(reader["UpdatedAt"])
                });
            }

            return users;
        }

        public async Task<string> UpdateUserAsync(int cid, UpdateUserDto dto)
        {
            using SqlConnection conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            string query = @"UPDATE Users 
                             SET FullName = @FullName,
                                 PhoneNumber = @PhoneNumber,
                                 Email = @Email,
                                 Address = @Address,
                                 UpdatedAt = GETDATE()
                             WHERE CID = @CID AND IsActive = 1";

            using SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@FullName", dto.FullName);
            cmd.Parameters.AddWithValue("@PhoneNumber", dto.PhoneNumber);
            cmd.Parameters.AddWithValue("@Email", dto.Email);
            cmd.Parameters.AddWithValue("@Address", dto.Address);
            cmd.Parameters.AddWithValue("@CID", cid);

            int rows = await cmd.ExecuteNonQueryAsync();
            return rows > 0 ? "User updated successfully" : "User not found";
        }

        public async Task<string> DeleteUserAsync(int cid)
        {
            using SqlConnection conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            string query = @"UPDATE Users 
                             SET IsActive = 0, UpdatedAt = GETDATE()
                             WHERE CID = @CID";

            using SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@CID", cid);

            int rows = await cmd.ExecuteNonQueryAsync();
            return rows > 0 ? "User deleted successfully" : "User not found";
        }

        public async Task<string> SetPinAsync(SetPinDto dto)
        {
            using SqlConnection conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            string query = @"UPDATE Users 
                             SET PinHash = @PinHash, UpdatedAt = GETDATE()
                             WHERE CID = @CID AND IsActive = 1";

            using SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@PinHash", PasswordHasherHelper.Hash(dto.Pin));
            cmd.Parameters.AddWithValue("@CID", dto.CID);

            int rows = await cmd.ExecuteNonQueryAsync();
            return rows > 0 ? "PIN set successfully" : "User not found";
        }

        public async Task<string> ResetPinAsync(ResetPinDto dto)
        {
            using SqlConnection conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            string getQuery = "SELECT PinHash FROM Users WHERE CID = @CID AND IsActive = 1";

            using SqlCommand getCmd = new SqlCommand(getQuery, conn);
            getCmd.Parameters.AddWithValue("@CID", dto.CID);

            object? result = await getCmd.ExecuteScalarAsync();

            if (result == null)
                return "User not found";

            string oldPinHash = result.ToString()!;
            if (oldPinHash != PasswordHasherHelper.Hash(dto.OldPin))
                return "Old PIN is incorrect";

            string updateQuery = @"UPDATE Users 
                                   SET PinHash = @NewPinHash, UpdatedAt = GETDATE()
                                   WHERE CID = @CID";

            using SqlCommand updateCmd = new SqlCommand(updateQuery, conn);
            updateCmd.Parameters.AddWithValue("@NewPinHash", PasswordHasherHelper.Hash(dto.NewPin));
            updateCmd.Parameters.AddWithValue("@CID", dto.CID);

            int rows = await updateCmd.ExecuteNonQueryAsync();
            return rows > 0 ? "PIN reset successfully" : "PIN reset failed";
        }

        public async Task<decimal> GetWalletBalanceAsync(int cid)
        {
            using SqlConnection conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            string query = "SELECT WalletBalance FROM Users WHERE CID = @CID AND IsActive = 1";

            using SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@CID", cid);

            object? result = await cmd.ExecuteScalarAsync();
            return result != null ? Convert.ToDecimal(result) : -1;
        }

        public async Task<string> TransferWalletToBankAsync(WalletTransferDto dto)
        {
            using SqlConnection conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            string fetchQuery = "SELECT WalletBalance, AccountNumber FROM Users WHERE CID = @CID AND IsActive = 1";

            decimal currentBalance;
            string? accountNumber;

            using (SqlCommand fetchCmd = new SqlCommand(fetchQuery, conn))
            {
                fetchCmd.Parameters.AddWithValue("@CID", dto.CID);

                using SqlDataReader reader = await fetchCmd.ExecuteReaderAsync();
                if (!await reader.ReadAsync())
                    return "User not found";

                currentBalance = Convert.ToDecimal(reader["WalletBalance"]);
                accountNumber = reader["AccountNumber"]?.ToString();
            }

            if (string.IsNullOrWhiteSpace(accountNumber))
                return "Bank account not linked";

            if (dto.Amount <= 0)
                return "Amount must be greater than zero";

            if (currentBalance < dto.Amount)
                return "Insufficient wallet balance";

            string updateQuery = @"UPDATE Users 
                                   SET WalletBalance = WalletBalance - @Amount,
                                       UpdatedAt = GETDATE()
                                   WHERE CID = @CID";

            using SqlCommand updateCmd = new SqlCommand(updateQuery, conn);
            updateCmd.Parameters.AddWithValue("@Amount", dto.Amount);
            updateCmd.Parameters.AddWithValue("@CID", dto.CID);

            int rows = await updateCmd.ExecuteNonQueryAsync();
            return rows > 0 ? "Wallet money transferred to bank successfully" : "Transfer failed";
        }
    }
    } 


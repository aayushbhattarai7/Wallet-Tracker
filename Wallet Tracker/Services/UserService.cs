using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Wallet_Tracker.Models;

public class UserService
{
    private static readonly string DesktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
    private static readonly string FolderPath = Path.Combine(DesktopPath, "LocalDB");
    private static readonly string FilePath = Path.Combine(FolderPath, "users.json");
    public string? loggedInUserName { get; set; }

    public User? GetLoggedInUser()
    {
        var users = LoadUsers();
        return users.FirstOrDefault(user => user.Username == loggedInUserName);
    }

    public List<User> LoadUsers()
    {
        if (!File.Exists(FilePath))
        {

            return new List<User>();
        }

        var json = File.ReadAllText(FilePath);
        return JsonSerializer.Deserialize<List<User>>(json) ?? new List<User>();
    }

    public void SaveUsers(List<User> users)
    {
        if (!Directory.Exists(FolderPath))
        {
            Directory.CreateDirectory(FolderPath);
        }

        var json = JsonSerializer.Serialize(users, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(FilePath, json);
    }

    public string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(password);
        var hash = sha256.ComputeHash(bytes);
        return Convert.ToBase64String(hash);
    }

    public bool ValidatePassword(string inputPassword, string storedPassword)
    {
        var hashedInputPassword = HashPassword(inputPassword);
        return hashedInputPassword == storedPassword;
    }
}

using Microsoft.AspNetCore.Components;
using Microsoft.Maui.Controls;
using Wallet_Tracker.Models;
public class TransactionService
{
    private readonly UserService _userService;

    public TransactionService(UserService userService)
    {
        _userService = userService;
    }

    public List<Transaction> GetUserTransactions(string username)
    {
        var users = _userService.LoadUsers();
        var user = users.FirstOrDefault(user => user.Username == username);
        return user?.Transactions ?? new List<Transaction>();
    }

    public void ClearTransaction(string username)
    {
        var users = _userService.LoadUsers();
        var user = users.FirstOrDefault(u => u.Username == username);
        if (user != null)
        {
            user.Transactions.Clear();
            _userService.SaveUsers(users);
        }
    }

    public void UpdateCredit(string username, int newCredit, string tag, string remarks)
    {
        var users = _userService.LoadUsers();
        var user = users.FirstOrDefault(user => user.Username == username);
        if (user != null)
        {
            user.Credit += newCredit;

            user.Transactions ??= new List<Transaction>();

            user.Transactions.Add(new Transaction
            {
                Date = DateTime.Now,
                Type = "Credit",
                Amount = newCredit,
                Description = $"Added {newCredit} to Credit",
                Tag = tag,
                Remarks = remarks
            });

            _userService.SaveUsers(users);
        }
    }

    public void UpdateDebit(string username, int newDebit, string tag, string remarks)
    {
        var users = _userService.LoadUsers();
        var user = users.FirstOrDefault(user => user.Username == username);
        if (user != null)
        {
            if (user.Credit < newDebit)
            {
       
                throw new Exception("You don't have sufficient balance");
            }
            user.Debit += newDebit;
            user.Credit -= newDebit;

            user.Transactions ??= new List<Transaction>();

            user.Transactions.Add(new Transaction
            {
                Date = DateTime.Now,
                Type = "Debit",
                Amount = newDebit,
                Description = $"Added {newDebit} to Debit",
                Tag = tag,
                Remarks = remarks
            });

            _userService.SaveUsers(users);
        }
    }

    public void UpdateDebt(string username, int newDebt, string Tag, string Remarks)
    {
        var users = _userService.LoadUsers();
        var user = users.FirstOrDefault(user => user.Username == username);
        if (user != null)
        {
            user.Debt += newDebt;
            user.Credit += newDebt;

            if (user.TotalDebt >= 5)
            {
                throw new Exception("Your debt request limit exceeded. Clear your existing debt first.");
            }

            user.Transactions ??= new List<Transaction>();

            user.Transactions.Add(new Transaction
            {
                Date = DateTime.Now,
                Type = "TakenDebt",
                Amount = newDebt,
                Description = $"Added {newDebt} to Debt",
                Tag = Tag,
                Remarks = Remarks
            });


            user.TotalDebt++;
            user.DebtTaken += newDebt;
            _userService.SaveUsers(users);
        }
    }

    public void ClearDebt(string username, int credit, string Tag, string Remarks)
    {
        var users = _userService.LoadUsers();
        var user = users.FirstOrDefault(user => user.Username == username);
       if(user.Credit< credit)
        {
            throw new Exception("You do not have sufficient balance");
        }

            user.Transactions ??= new List<Transaction>();
            if(user.Debt< credit)
            {
                throw new Exception("Your debt is less than the credit");
            }
            user.Transactions.Add(new Transaction
            {
                Date = DateTime.Now,
                Type = "ClearedDebt",
                Amount = user.Debt,
                Description = $"Cleared {user.Debt} from Debt",
                  Tag = Tag,
                Remarks = Remarks
            });

            user.Credit -= credit;
            user.Debt -= credit;
        user.ClearedDebt += credit;
            if(user.Debt == 0)
            {
                user.TotalDebt = 0;
            }

            _userService.SaveUsers(users);
       
    }

    public void ResetDebit(string username)
    {
        var users = _userService.LoadUsers();
        var user = users.FirstOrDefault(user => user.Username == username);
        if (user != null)
        {
            user.Debit = 0;
            _userService.SaveUsers(users);
        }
    }
}

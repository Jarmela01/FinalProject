using FluentValidation.Validators;
using LOAN_API.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using Users.Data;
using Users.Domain;
using XAct;

namespace LOAN_API.Services
{
    public class UserRepository : IUserRepository
    {
        private readonly UsersContext _context;

        public UserRepository(UsersContext context)
        {
            _context = context;           
        }
        public User AddUser(User user)
        {
            var users = _context.Users.ToList();            
            bool sameUsername = users.Any(u => u.UserName.ToLower() == user.UserName.ToLower());
            bool sameEmail = users.Any(e => e.Email.ToLower() == user.Email.ToLower());
            if (sameUsername == true || sameEmail == true)
            {
                return null;
            }
            else
            {
                user.UserName = user.UserName.ToLower();
                user.Password = PasswordHasher.HashPassword(user.Password);
                user.Roles = "user";
                user.IsBlocked = false;
                
                _context.Users.Add(user);
                _context.SaveChanges();

                return user;
            }

            
        }

        public Loan AddLoan(int id,Loan loan)
        {
            var user = _context.Users.ToList();

            var userId = user[id-1];
            if (userId.IsBlocked == true)
            {
                return null;
            }
            else
            {
                if (userId == null)
                {
                    return null;
                }
                userId.Loans.Add(new Loan
                {
                    LoanType = loan.LoanType.ToLower(),
                    Currency = loan.Currency.ToUpper(),
                    Money = loan.Money,
                    Period = loan.Period,
                    Status = "Loading",
                });
                _context.Users.Attach(userId);
                _context.SaveChanges();

                var loans = _context.Loans.Where(x => x.UserId == id)
                    .OrderBy(id => id)
                    .ToList()
                    .LastOrDefault();

                return loans;
            }
            
        }

        public List<Loan> DeleteByUser(int userId,int id)
        {
            
            var loans = _context.Loans.ToList();
            List<Loan> newLoans = new List<Loan>();
            foreach (var item in loans)
            {
                if (item.UserId == userId)
                {
                    newLoans.Add(item);
                }
            }          
            
            if (newLoans.Count == 0)
            {
                return null;
            }
            var loanID = newLoans.Find(x => x.Id == id);
            if (loanID == null)
            {
                return null;
            }
            if (loanID.Status != "Loading")
            {
                return null;
            }
          
                var deletedLoan = _context.Loans
            .Where(x => x.Id == id)
            .FirstOrDefault();
                _context.Loans.Remove(deletedLoan);
                _context.SaveChanges();
         
            
            return newLoans.ToList();
        }
        public Loan Delete(int id)
        {
            var loans = _context.Loans.ToList();
            var loan = loans[id];

            var deletedLoan = _context.Loans
                .Where(x => x.Id == id)
                .FirstOrDefault();
            _context.Loans.Remove(deletedLoan);
            _context.SaveChanges();

            return deletedLoan;

        }

        public List<Loan> GetAll()
        {
            var loans = _context.Loans.ToList();

            return loans;
        }

        public Loan Update(int id, Loan loan)
        {
            var loans = _context.Loans.ToList();
            if (loans[id].Id == id)
            {
                loans[id].LoanType = loan.LoanType;
                loans[id].Money = loan.Money;
                loans[id].Currency = loan.Currency;
                loans[id].Period = loan.Period;
                _context.Loans.Update(loans[id]);
                _context.SaveChanges();
            }
            else
            {
                return null;
            }

            return loans[id];

        }

        public User Block(int id)
        {
            
            var users = _context.Users.ToList();
            users[id].IsBlocked = true;
            _context.Users.Update(users[id]);
            _context.SaveChanges();
            
            return users[id];
        }
        public Loan UpdateByUser(int userId, int id, Loan loan)
        {
            var loans = _context.Loans.ToList();
            List<Loan> newLoans = new List<Loan>();

            foreach (var item in loans)
            {
                if (item.UserId == userId)
                {
                    newLoans.Add(item);
                }
            }

            if (newLoans.Count == 0)
            {
                return null;
            }
            var loanID = newLoans.Find(x => x.Id == id);
            if (loanID == null)
            {
                return null;
            }
            if (loanID.Status != "Loading")
            {
                return null;
            }

            var updatedLoan = _context.Loans.Where(x => x.Id == id).FirstOrDefault();

            updatedLoan.LoanType = loan.LoanType;
            updatedLoan.Money = loan.Money;
            updatedLoan.Currency = loan.Currency;
            updatedLoan.Period = loan.Period;
            _context.Loans.Update(updatedLoan);
            _context.SaveChanges();

            return updatedLoan;
        }
        public Loan GetLoansId(int id)
        {
            var loans = _context.Loans.ToList();
            List<Loan> loans1 = new List<Loan>();

            foreach (var item in loans)
            {
                if (item.UserId == id)
                {
                    loans1.Add(item);
                }
            }           
            if (loans1.Count == 0)
            {
                return null;
            }
            else
            {
                var loans2 = loans1.Where(x => x.Id == id).ToList();
                return loans2[0];
            }
            
        }
    }
}

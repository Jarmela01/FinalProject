using System.Collections.Generic;
using Users.Domain;

namespace LOAN_API.Services
{
    public interface IUserRepository
    {
        List<Loan> GetAll();
        public User AddUser(User user);
        Loan AddLoan(int id,Loan loan);
        Loan Update(int id,Loan loan);
        Loan UpdateByUser(int userId,int id, Loan loan);
        List<Loan> DeleteByUser(int userId,int id);
        Loan Delete(int id);
        User Block(int id);
        Loan GetLoansId(int id);

    }
}

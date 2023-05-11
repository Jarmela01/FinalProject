using FinalProject.Services;
using FluentValidation.Validators;
using LOAN_API.Helpers;
using LOAN_API.Services;
using LOAN_API.Validators;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Users.Data;
using Users.Domain;


namespace FinalProject.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class LoanApiController : Controller
    {
        private readonly IUserRepository _userRepo;
        private readonly IUserService _userService;
        private readonly IAccountantService _accountantService;
        private readonly UsersContext _usersContext;
        private readonly AppSettings _appSettings;


        public LoanApiController(IUserRepository iUserRepository, IUserService userService,
            IOptions<AppSettings> appSettings, IAccountantService accountantService,
            UsersContext usersContext)
        {
            _userRepo = iUserRepository;
            _userService = userService;
            _accountantService = accountantService;
            _usersContext = usersContext;
            _appSettings = appSettings.Value;

        }

        [AllowAnonymous]
        [HttpPost("UserRegistration")]
        public IActionResult UserRegistration([FromQuery] User user)
        {
            var validator = new UserValidator();
            var validate = validator.Validate(user);

            if (!validate.IsValid)
            {
                return BadRequest(validate.Errors[0].ErrorMessage);
            }
            else
            {
                var registeredUser = _userRepo.AddUser(user);
                if (registeredUser == null)
                {
                    return BadRequest("This username is already taken or email adress is already taken!");
                }
                else
                {
                    return Accepted(registeredUser);
                }
            }
        }

        [AllowAnonymous]
        [HttpPost("LoginAsAccountant")]
        public IActionResult LoginAsAccountant([FromQuery] Accountant accountant)
        {
            var accountantLogin = _accountantService.Login(accountant);
            if (accountantLogin == null)
            {
                return BadRequest("Username or password is incorrect!");
            }
            else
            {
                var tokenString = _accountantService.GenerateToken(accountant);
                var accountants = _usersContext.Accountant.ToList();

                return Ok(new { accountants, tokenString });
            }
        }

        [AllowAnonymous]
        [HttpPost("Login")]
        public IActionResult Login([FromQuery] User user, int id)
        {
            var userLogin = _userService.Login(user);
            if (userLogin == null)
            {
                return BadRequest("Username or password is incorrect!");
            }
            try
            {
                var tokenString = _userService.GenerateToken(user, userLogin.Id - 1);
                return Ok(new
                {
                    userLogin.Id,
                    userLogin.UserName,
                    userLogin.FirstName,
                    userLogin.LastName,
                    Token = tokenString
                });
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost("AddLoan{id}")]
        public IActionResult MakeLoan(int id, [FromQuery] Loan loan)
        {
            var validator = new LoanValidator();
            var result = validator.Validate(loan);


            if (!result.IsValid)
            {
                return BadRequest(result.Errors[0].ErrorMessage);
            }
            else
            {
                var currentUserId = int.Parse(User.Identity.Name);

                if (currentUserId != id && !User.IsInRole(Role.Accountant))

                    return BadRequest("Tqven ar gaqvt wvdoma am ID-ze");

                var addLoan = _userRepo.AddLoan(currentUserId, loan);

                if (addLoan == null)
                    return BadRequest("Es momxmarebeli dablokilia da ar aqvs ufleba sesxi moitxovos!");

                return Ok(addLoan);
            }
        }

        [Authorize(Roles = Role.Accountant)]
        [HttpGet("GetAll")]
        public IActionResult GetAllLoan()
        {
            if (!User.IsInRole(Role.Accountant))
            {
                return Forbid("Only Accountant Can Get All Loans!");
            }
            else
            {
                var getLoans = _userRepo.GetAll();

                return Ok(getLoans);
            }
        }

        [HttpGet("GetLoan{id}")]
        public IActionResult GetLoan(int id)
        {
            var currentUserId = int.Parse(User.Identity.Name);
            if (currentUserId != id && !User.IsInRole(Role.Accountant))

                return BadRequest("Tqven ar gaqvt wvdoma am ID-ze");

            var user = _userService.GetById(id);

            if (user == null)
                return NotFound();

            return Ok(user);
        }

        [HttpPut("Update{id}")]
        public IActionResult ReplaceByUser([FromQuery] Loan loan, int id)
        {
            var validator = new LoanValidator();
            var result = validator.Validate(loan);
            if (!result.IsValid)

                return BadRequest(result.Errors[0].ErrorMessage);

            var loans = _usersContext.Loans.ToList();
            var currentUserId = int.Parse(User.Identity.Name);

            if (User.IsInRole(Role.Accountant))
            {
                var updateLoan = _userRepo.Update(id, loan);
            }
            else
            {
                var updateLoan = _userRepo.UpdateByUser(currentUserId, id, loan);
                if (updateLoan == null)
                {
                    return BadRequest("Tqven ar gaqvt wvdoma am ID-ze");
                }
            }
            return Ok(loan);
        }

        [Authorize(Roles = Role.Accountant)]
        [HttpPut("BlockUser{id}")]
        public IActionResult BlockUser(int id)
        {
            try
            {
                var blockUser = _userRepo.Block(id);
                return Ok(blockUser);
            }
            catch (Exception e)
            {
                return NotFound(e.Message);
            }
        }

        [HttpDelete("DeleteLoan{id}")]
        public IActionResult RemoveLoan(int id)
        {

            var users = _usersContext.Users.ToList();
            var currentUserId = int.Parse(User.Identity.Name);
            if (User.IsInRole(Role.Accountant))
            {
                var deletedLoan = _userRepo.Delete(id);
            }
            else
            {
                var deletedLoan = _userRepo.DeleteByUser(currentUserId, id);
                if (deletedLoan == null)
                {
                    return BadRequest("Tqven ar gaqvt wvdoma am ID-ze");
                }
            }
            return Ok(users[currentUserId - 1].Loans.ToList());
        }
    }
}

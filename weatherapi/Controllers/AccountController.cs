using weatherapi.Entities.Declarations.Generic;
using weatherapi.Entities;
using weatherapi.Models.Account;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace weatherapi.Controllers
{
    [ApiController]
    [Route("api/accounts")]

    public class AccountController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<AccountController> _logger;
        private readonly IRepositoryWrapper _repo;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILogger<AccountController> logger,
            IRepositoryWrapper repo)
        {

            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _repo = repo;
        }

        #region SETTING

        [HttpGet("update")]
        public async Task<IActionResult> UpdateAsync([FromBody] UpdateUserDto model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound("User not found");

            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.Gender = model.Gender;

            var response = await _userManager.UpdateAsync(user);
            if (response.Succeeded)
            {
                return Ok("Accouunt updated successfully");
            }
            return BadRequest("Account not updated");
        }

        #endregion

    }
}
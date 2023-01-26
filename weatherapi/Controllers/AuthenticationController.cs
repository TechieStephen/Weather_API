using weatherapi.Models.Account;
using weatherapi.Services.Authentication;
using AutoMapper;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using weatherapi.Entities;
using weatherapi.Entities.Declarations.Generic;
using weatherapi.Models.Authentication;

namespace weatherapi.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<AuthenticationController> _logger;
        private readonly IJwtAuthentication _jwtAuthentication;
        private readonly IRepositoryWrapper _repo;
        private readonly IMapper _mapper;
        private readonly IConfiguration _config;

        public AuthenticationController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILogger<AuthenticationController> logger,
            IConfiguration configuration,
            IJwtAuthentication jwtAuthenticationManager,
            IRepositoryWrapper repo, IMapper mapper, IConfiguration config)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _jwtAuthentication = jwtAuthenticationManager;
            _repo = repo;
            _mapper = mapper;
            _config = config;
        }


        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel registerModel)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = registerModel.Email, Email = registerModel.Email };

                var result = await _userManager.CreateAsync(user, registerModel.Password);

                if (result.Succeeded)
                {
                    _logger.LogInformation("NEW USER - new account created TIME:{0}.", DateTime.UtcNow);
                    await SendWelcomeEmail(user);
                    return Ok("Registration Successfull");
                }

                if (result.Errors.Any(x => x.Code == "DuplicateEmail"))
                {
                    return BadRequest($"Email {registerModel.Email.ToLower()} is already taken");
                }

                //Gets all errors if registration fails
                foreach (var error in result.Errors)
                {
                    _logger.LogError("{0} TIME:{1}", error.Description, DateTime.UtcNow);
                }

                //returns all errors
                return BadRequest(result.Errors);
            }
            return BadRequest("Please enter email and pasword, Password must be at least 6 charcters");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginModel login)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByNameAsync(login.Email);

                //var user = await _userManager.Users.Where(x => x.Email == login.Email)
                //    .Include(x => x.RefreshTokens).FirstOrDefaultAsync();

                if (user == null)
                {
                    return Unauthorized("Invalid username or password");
                }

                bool checkUserPassword = await _userManager.CheckPasswordAsync(user, login.Password);

                if (checkUserPassword)
                {
                    _logger.LogInformation("LOGIN - user loggedin TIME:{0}.", DateTime.UtcNow);

                    //Generate Token & Sign in user
                    var userRoles = await _userManager.GetRolesAsync(user);
                    var access_Token = _jwtAuthentication.GenerateAccessToken(user, userRoles);

                    if (!string.IsNullOrEmpty(access_Token))
                    {
                        var refresh_Token = "";

                        //Prepare & return auth response
                        var response = new AuthResponse()
                        {
                            AccessToken = access_Token,
                            RefreshToken = refresh_Token,
                        };

                        var data = _mapper.Map<UserDto>(user);

                        return Ok(new { auth = response, data });
                    }
                }

            }
            return Unauthorized("Invalid username or password");
        }


        // Get client ID (e.g google and facebook ID) from frontend then pass it to this endpoint
        [HttpPost("external")]
        public async Task<IActionResult> ExternalRegisterAndLogin(ExternalAuthDto externalAuth)
        {
            if (ModelState.IsValid)
            {
                var payload = await VerifyGoogleToken(externalAuth);

                if (payload == null) return BadRequest("Invalid External Authentication.");

                var info = new UserLoginInfo(externalAuth.Provider, payload.Subject, externalAuth.Provider);

                var user = await _userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);
                if (user == null)
                {
                    user = await _userManager.FindByEmailAsync(payload.Email);
                    if (user == null)
                    {
                        user = new ApplicationUser
                        {
                            Email = payload.Email,
                            UserName = payload.Email,
                            FirstName = payload.GivenName,
                            LastName = payload.FamilyName
                        };

                        var result = await _userManager.CreateAsync(user);
                        if (result.Succeeded)
                        {
                            _logger.LogInformation("NEW USER - new account created TIME:{0}.", DateTime.UtcNow);
                            await SendWelcomeEmail(user);
                        }

                    }
                    await _userManager.AddLoginAsync(user, info);
                }

                //if user is still null return Badrequest - Just to be safe
                if (user == null) return BadRequest("Invalid External Authentication.");


                //You can check for the Locked out account here if app requires it
                _logger.LogInformation("User logged in TIME:{0}.", DateTime.UtcNow);

                //Since this is external login/registeration sign in the user if everything goes well
                //Generate Token & Sign in user
                var userRoles = await _userManager.GetRolesAsync(user);
                var access_Token = _jwtAuthentication.GenerateAccessToken(user, userRoles);

                if (!string.IsNullOrEmpty(access_Token))
                {
                    var refresh_Token = "";

                    //Prepare & return response
                    var response = new AuthResponse()
                    {
                        AccessToken = access_Token,
                        RefreshToken = refresh_Token,
                    };

                    return Ok(new { auth = response, userData = _mapper.Map<UserDto>(user) });
                }
            }

            return BadRequest("Invalid External Authentication.");
        }


        #region NON ACTIONS
        // Verifies client ID sent from the frontend app and returns the user details if found and null if not found
        [NonAction]
        public async Task<GoogleJsonWebSignature.Payload> VerifyGoogleToken(ExternalAuthDto externalAuth)
        {
            try
            {
                string clientId = _config.GetSection("Authentication:Google:ClientId").Value;
                var settings = new GoogleJsonWebSignature.ValidationSettings()
                {
                    Audience = new List<string>() { clientId }
                };
                var payload = await GoogleJsonWebSignature.ValidateAsync(externalAuth.Token, settings);
                return payload;
            }
            catch (Exception)
            {
                //log an exception
                return null;
            }
        }

        
        //used in the Register and ExternalRegisterAndLogin methods to send email to new users
        [NonAction]
        public async Task<bool> SendWelcomeEmail(ApplicationUser user)
        {
            //Send Confirmation/Welcome Email Here
            //_emailSender.WelcomeEmailAsync();
            return true;
        }
        #endregion
    }
}

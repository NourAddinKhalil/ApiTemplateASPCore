using APITemplate.Constants;
using APITemplate.DBModels;
using APITemplate.Helpers;
using APITemplate.Models;
using APITemplate.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace APITemplate.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("Register")]
        public async Task<ActionResult<ResponseModel>> RegisterAsync([FromBody] RegisterModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var results = await _authService.RegisterAsync(model);

                return Ok(new ResponseModel()
                {
                    Message = "User registered Sucessfully!",
                    Model = results,
                });
            }
            catch (CustomAPIExceptionHelper ex)
            {
                return StatusCode(ex.StatusCode, new ResponseModel()
                {
                    Message = ex.Message ?? "",
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel()
                {
                    Message = ex.Message ?? "",
                });
            }

        }

        [HttpPost("Login")]
        public async Task<ActionResult<AuthModel>> LoginAsync([FromBody] LoginModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var results = await _authService.LoginAsync(model);

                return Ok(new ResponseModel()
                {
                    Message = "User Logged in Sucessfully!",
                    Model = results,
                });
            }
            catch(CustomAPIExceptionHelper ex)
            {
                return StatusCode(ex.StatusCode, new ResponseModel()
                {
                    Message = ex.Message ?? "",
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel()
                {
                    Message = ex.Message ?? "",
                });
            }
        }

        [HttpPost("AssignRole")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApplicationUser>> AssginRoleAsync(UserRoles role, string userID)
        {
            try
            {
                var result = await _authService.AssginRoleAsync(role, userID);

                return Ok(new ResponseModel()
                {
                    Message = "Role Assgined Sucessfully!",
                    Model = result,
                });
            }
            catch (CustomAPIExceptionHelper ex)
            {
                return StatusCode(ex.StatusCode, new ResponseModel()
                {
                    Message = ex.Message ?? "",
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel()
                {
                    Message = ex.Message ?? "",
                });
            }
        }

        [HttpPost("RevokeRole")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RevokeRoleAsync(List<UserRoles> roles, string userID)
        {
            try
            {
                var result = await _authService.RevokeRoleAsync(roles, userID);

                return Ok(new ResponseModel()
                {
                    Message = "Roles Revoked Sucessfully!",
                    Model = result,
                });
            }
            catch (CustomAPIExceptionHelper ex)
            {
                return StatusCode(ex.StatusCode, new ResponseModel()
                {
                    Message = ex.Message ?? "",
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel()
                {
                    Message = ex.Message ?? "",
                });
            }
        }
    }
}

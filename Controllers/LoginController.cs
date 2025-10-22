using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SchoolPortal.Model;
using SchoolPortal.Repository;
using Swashbuckle.AspNetCore.Annotations;

namespace SchoolPortal.Controllers;

[Route("api/[controller]")]
[ApiController]
public class LoginController : ControllerBase
{
    private readonly IAllRepository<Login> _repo;
    private readonly IRegisterRepository _register;
    public LoginController(IAllRepository<Login> repo, IRegisterRepository register)
    {
        _repo = repo;
        _register = register;
    }
    [HttpPost]
    [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ReturnObject))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(ReturnObject))]
    [Route("SignIn")]
    public IActionResult SignIn([FromBody] SignInRequestModel obj)
    {
        try
        {
            var r = new ReturnObject();
            var all = new AllHelper();
            var existingUser = _repo.GetAllForLogin().Data.FirstOrDefault(o => o.Email == obj.Email);
            if (existingUser == null || !(all.ComparePassword(obj.Password, existingUser.Password)))
            {
                r.Status = "Error";
                r.Message = "Invalid Credential";
            }
            else
            {
                var token = all.GenerateJwtToken(existingUser.Id.ToString(), existingUser.Role, existingUser.ClientId);

                r.Status = "success";
                r.Message = $"Welcome {existingUser.Email}";
                r.Data = new LoginReturn
                {
                    Token = token,
                    Role = existingUser.Role,
                    FirstLogin = existingUser.FirstLogin,
                    UserId = existingUser.Id.ToString(),
                    ClientId = existingUser.ClientId,
                };
            }
            return Ok(r);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new ReturnObject
            {
                Status = "error",
                Message = ex.Message
            });
        }
    }

    [HttpPost]
    [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ReturnObject))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(ReturnObject))]
    [Route("SignUp")]
    public async Task<IActionResult> SignUpAsync([FromBody] SignUpRequestModel obj)
    {
        try
        {
            return Ok(await _register.Register(obj.Email, obj.Password, "SuperAdmin","-1"));
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new ReturnObject
            {
                Status = "error",
                Message = ex.Message
            });
        }
    }
    [HttpPost]
    [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ReturnObject))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(ReturnObject))]
    [Route("ResetPassword")]
    public IActionResult ResetPassword([FromBody] SignUpRequestModel obj)
    {
        try
        {
            var r = new ReturnObject();
            r.Status = "error";
            r.Message = "Anerror occured";
            var all = new AllHelper();
            var existingUser = _repo.GetAllForLogin().Data.FirstOrDefault(o => o.Email == obj.Email);
            if (existingUser == null)
            {
                r.Status = "Error";
                r.Message = "User Not Found";

                return Ok(r);
            }
            existingUser.Password = all.HashPassword(obj.Password);
            existingUser.FirstLogin = false;

            var token = all.GenerateJwtToken(existingUser.Id.ToString(), existingUser.Role, existingUser.ClientId);
            var rec = _repo.Update(existingUser);
            if (rec.Status == "success")
            {
                r.Status = "success";
                r.Message = $"Welcome {existingUser.Email}";
                r.Data = new LoginReturn
                {
                    Token = token,
                    Role = existingUser.Role,
                    FirstLogin = existingUser.FirstLogin,
                    UserId = existingUser.Id.ToString(),
                    ClientId = existingUser.ClientId,
                };
            }
            return Ok(r);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new ReturnObject
            {
                Status = "error",
                Message = ex.Message
            });
        }
    }

}

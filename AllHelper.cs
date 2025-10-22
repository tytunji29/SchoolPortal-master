using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using SchoolPortal.Model;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SchoolPortal;

public class AllHelper
{

    private readonly PasswordHasher<object> _passwordHasher;

    public AllHelper()
    {
        _passwordHasher = new PasswordHasher<object>();
    }

    public string HashPassword(string password)
    {
        // Hash the password using PasswordHasher
        var hashedPassword = _passwordHasher.HashPassword(null, password);
        return hashedPassword;
    }

    public bool ComparePassword(string plainPassword, string hashedPassword)
    {
        // Verify the plain password against the hashed password using PasswordHasher
        var result = _passwordHasher.VerifyHashedPassword(null, hashedPassword, plainPassword);
        return result == PasswordVerificationResult.Success;
    }


    public static void RegisterMappings()
    {
        TypeAdapterConfig<SchoolRequestModel, School>
            .NewConfig();
          //  .Map(dest => dest.Logo,
            //src => ConvertIFormFilesToBase64(src.Logo)); // Custom mapping for the Logo

        TypeAdapterConfig<StaffRequestModel, Staff>.NewConfig();
        TypeAdapterConfig<ParentFormmodel, Parent>.NewConfig();
        TypeAdapterConfig<GeneralForFormModel, Subjects>.NewConfig();
        TypeAdapterConfig<ClassTeacherHistoryFormmodel, Classes>.NewConfig();
    }

    public static string ConvertIFormFilesToBase64(IFormFile file)
    {
        string val = "";
        if (file.Length > 0)
        {
            using (var ms = new MemoryStream())
            {
                file.CopyTo(ms);
                var fileBytes = ms.ToArray();
                val = Convert.ToBase64String(fileBytes);
            }
        }
        return val;
    }

    public string GenerateJwtToken(string userId, string role, string clientId)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("c2a8c91cf10b4989805adbda1778f1e79"));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId),
        new Claim(ClaimTypes.Role, role),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        new Claim("ClientId", clientId),
        new Claim("UserId", userId)
        };

        var token = new JwtSecurityToken(
            issuer: "yes!!~#",
            audience: "yes!!~#",
            claims: claims,
            expires: DateTime.Now.AddMinutes(30),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace USER_REGISTER.BLL.Security
{
    /// <summary>
    /// Provides JWT Token related functionalities
    /// </summary>
    public static class JWTTokenHelper
    {
        /// <summary>
        /// Generates a JWT Token
        /// </summary>
        /// <param name="jwtSecurityKey">The key to use when generating the token.</param>
        /// <param name="tokenDto"></param>
        /// <returns></returns>
        public static string GenerateToken(string jwtSecurityKey, JWTTokenDto tokenDto)
        {
            try
            {
                if (string.IsNullOrEmpty(jwtSecurityKey)) throw new ArgumentNullException(nameof(jwtSecurityKey));
                else if (tokenDto == null) throw new ArgumentNullException(nameof(tokenDto));

                // generate token that is valid for 7 days
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(jwtSecurityKey);
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(tokenDto.GenerateClaims()),
                    //Expires = DateTime.UtcNow.AddDays(7), no need as system is informational
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };
                var token = tokenHandler.CreateToken(tokenDescriptor);
                return tokenHandler.WriteToken(token);
            }
            catch (Exception e)
            {
                throw new Exception("Failed to generate user token! " + e.ExtractInnerExceptionMessage());
            }
        }

        /// <summary>
        /// Extract the Data from the supplied user token
        /// </summary>
        /// <param name="jwtSecurityKey"></param>
        /// <param name="jwtToken"></param>
        /// <returns></returns>
        public static JWTTokenDto ExtractDataFromToken(string jwtSecurityKey, string jwtToken)
        {
            try
            {
                if (string.IsNullOrEmpty(jwtSecurityKey)) throw new ArgumentNullException(nameof(jwtSecurityKey));
                else if (string.IsNullOrEmpty(jwtToken)) throw new ArgumentNullException(nameof(jwtToken));

                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(jwtSecurityKey);
                tokenHandler.ValidateToken(jwtToken, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    // set clockskew to zero so tokens expire exactly at token expiration time (instead of 5 minutes later)
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                return JWTTokenDto.Extract((JwtSecurityToken)validatedToken);
            }
            catch (Exception e)
            {
                throw new Exception("Failed to process user token! " + e.ExtractInnerExceptionMessage());
            }
        }
    }
}

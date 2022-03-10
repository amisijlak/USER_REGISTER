using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using USER_REGISTER.DAL.Security;

namespace USER_REGISTER.BLL.Security
{
    public class JWTTokenDto
    {
        /// <summary>
        /// The User Id
        /// </summary>
        public string Id { get; set; }
        public DateTime? LastPasswordChangeDate { get; set; }

        private JWTTokenDto()
        {

        }

        public JWTTokenDto(ApplicationUser user)
        {
            foreach (var property in this.GetType().GetProperties())
            {
                var userProperty = user.GetType().GetProperty(property.Name);
                userProperty.EnsureIsNotNULL($"Property {property.Name} Not Found In Type {nameof(ApplicationUser)}!");
                property.SetValue(this, userProperty.GetValue(user));
            }
        }

        public IEnumerable<Claim> GenerateClaims()
        {
            foreach (var property in this.GetType().GetProperties())
            {
                var value = property.GetValue(this);
                if (value != null) yield return new Claim(property.Name, GetClaimValue(property.Name, value));
            }
        }

        public static string GetClaimValue(string propertyName, object value)
        {
            if (propertyName == nameof(LastPasswordChangeDate)) return ((DateTime?)value).FormatAsParameter(true);

            return value?.ToString();
        }

        private static object GetTokenValue(PropertyInfo property, string value)
        {
            if (property.Name == nameof(LastPasswordChangeDate)) return DateTime.Parse(value);

            return Convert.ChangeType(value, property.PropertyType);
        }

        public static JWTTokenDto Extract(JwtSecurityToken rawToken)
        {
            var result = new JWTTokenDto();

            foreach (var property in typeof(JWTTokenDto).GetProperties())
            {
                var rawValue = rawToken.Claims.FirstOrDefault(x => x.Type == property.Name)?.Value;
                if (!rawValue.IsNullOrEmpty()) property.SetValue(result, GetTokenValue(property, rawValue));
            }

            return result;
        }
    }
}

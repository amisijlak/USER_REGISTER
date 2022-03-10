using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using USER_REGISTER.BLL.Security;
using USER_REGISTER.DAL.Security;

namespace USER_REGISTER.BLL.Tests.Security.GivenJWTTokenDto
{
    [TestClass]
    public class WhenExtract
    {
        [TestMethod]
        public void ThenPassForSuppliedToken()
        {
            RunTests(new JWTTokenDto(new ApplicationUser { Id = "red", LastPasswordChangeDate = DateTime.Now }));
            RunTests(new JWTTokenDto(new ApplicationUser { Id = "blue" }));
            RunTests(new JWTTokenDto(new ApplicationUser { LastPasswordChangeDate = DateTime.Now }));
        }

        private void RunTests(JWTTokenDto token)
        {
            var tokenKey = "Aoi&82_(U1A@21maW";

            var extractedToken = JWTTokenHelper.ExtractDataFromToken(tokenKey, JWTTokenHelper.GenerateToken(tokenKey, token));

            var tokenProperties = typeof(JWTTokenDto).GetProperties();

            foreach (var property in tokenProperties)
            {
                var originalValue = property.GetValue(token)?.ToString();
                var extractedValue = property.GetValue(extractedToken)?.ToString();

                Assert.AreEqual(originalValue, extractedValue, $"Invalid Token Value For {property.Name}!");
            }
        }
    }
}

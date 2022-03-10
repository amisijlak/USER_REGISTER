using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using USER_REGISTER.BLL.Security;
using USER_REGISTER.DAL.Security;

namespace USER_REGISTER.BLL.Tests.Security.GivenJWTTokenDto
{
    [TestClass]
    public class WhenGenerateClaims
    {
        [TestMethod]
        public void ThenPassForGivenUser()
        {
            RunTests(new ApplicationUser { Id = "red", LastPasswordChangeDate = DateTime.Now }, 2);
            RunTests(new ApplicationUser { Id = "blue" }, 1);
            RunTests(new ApplicationUser { LastPasswordChangeDate = DateTime.Now }, 1);
        }

        private void RunTests(ApplicationUser user, int noOfClaimsToFind)
        {
            var token = new JWTTokenDto(user);

            var claims = token.GenerateClaims().ToList();

            var tokenProperties = typeof(JWTTokenDto).GetProperties();

            var foundClaims = 0;

            foreach (var property in tokenProperties)
            {
                var originalValue = JWTTokenDto.GetClaimValue(property.Name, user.GetType().GetProperty(property.Name).GetValue(user));

                Assert.AreEqual(originalValue, JWTTokenDto.GetClaimValue(property.Name, property.GetValue(token)), $"Invalid Token Value For {property.Name}!");

                var claim = claims.Where(r => r.Type == property.Name).SingleOrDefault();

                if (originalValue == null) Assert.IsNull(claim, $"Claim {property.Name} Not Expected!");
                else
                {
                    Assert.IsNotNull(claim, $"Claim {property.Name} Missing!");
                    foundClaims++;
                    Assert.AreEqual(originalValue, claim.Value, $"Invalid Claim For {property.Name}!");
                }
            }

            Assert.AreEqual(noOfClaimsToFind, foundClaims, $"Invalid No. Of Claims!");
        }
    }
}

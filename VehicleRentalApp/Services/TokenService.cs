using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using VehicleRentalApp.Models;

namespace VehicleRentalApp.Services
{
    public class TokenService
    {
        private const string SecretKey = "MySuperLongSecureSecretKey1234567890!"; // Daha güvenli bir anahtar seçin
        private const int ExpirationHours = 1;

        public string GenerateToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: "VehicleRentalApp",
                audience: "VehicleRentalApp",
                claims: claims,
                expires: DateTime.Now.AddHours(ExpirationHours),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}

using Microsoft.IdentityModel.Tokens;
using System.Text.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Caviar.Models.SystemData;
using IdentityModel;

namespace Caviar.Control
{
    public static class JwtHelper
    {
        public static string secret = CaviarConfig.TokenConfig.Key.ToString();

        /// <summary>
        /// 创建jwtToken,采用微软内部方法，默认使用HS256加密
        /// </summary>
        public static string CreateTokenByHandler(UserToken userToken)
        {
            var now = DateTime.UtcNow;

            // Specifically add the jti (random nonce), iat (issued timestamp), and sub (subject/user) claims.
            // You can add other claims here, if you want:
            var claims = new List<Claim>();
            var json = JsonSerializer.Serialize(userToken);
            var tempClaim = new Claim(CurrencyConstant.TokenPayLoadName, json);
            claims.Add(tempClaim);
            tempClaim = new Claim("Duration", CaviarConfig.TokenConfig.Duration.ToString());
            claims.Add(tempClaim);
            // Create the JWT and write it to a string
            var jwt = new JwtSecurityToken(
                issuer: null,
                audience: null,
                claims: claims,
                notBefore: now,
                expires: now.Add(TimeSpan.FromMinutes(CaviarConfig.TokenConfig.Duration)),
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secret)), SecurityAlgorithms.HmacSha256));
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);
            return encodedJwt;
        }

        /// <summary>
        /// 验证身份 验证签名的有效性,
        /// </summary>
        /// <param name="encodeJwt"></param>
        /// <param name="validatePayLoad">自定义各类验证； 是否包含那种申明，或者申明的值， </param>
        /// 例如：payLoad["aud"]?.ToString() == "roberAuddience";
        /// 例如：验证是否过期 等
        public static bool Validate(string encodeJwt, Func<Dictionary<string, object>, bool> validatePayLoad = null)
        {
            try
            {
                var success = true;
                var jwtArr = encodeJwt.Split('.');
                var header = JsonSerializer.Deserialize<Dictionary<string, object>>(Base64UrlEncoder.Decode(jwtArr[0]));
                var payLoad = JsonSerializer.Deserialize<Dictionary<string, object>>(Base64UrlEncoder.Decode(jwtArr[1]));

                var hs256 = new HMACSHA256(Encoding.ASCII.GetBytes(secret));
                //首先验证签名是否正确（必须的）
                success = success && string.Equals(jwtArr[2], Base64UrlEncoder.Encode(hs256.ComputeHash(Encoding.UTF8.GetBytes(string.Concat(jwtArr[0], ".", jwtArr[1])))));
                if (!success)
                {
                    return success;//签名不正确直接返回
                }
                //其次验证是否在有效期内（也应该必须）
                var now = ToUnixEpochDate(DateTime.UtcNow);
                success = success && (now >= long.Parse(payLoad["nbf"].ToString()) && now < long.Parse(payLoad["exp"].ToString()));
                //进行自定义的验证
                var custom = true;
                if (validatePayLoad != null)
                {
                    custom = validatePayLoad(payLoad);
                }
                success = success && custom;

                return success;
            }
            catch
            {
                return false;
            }
        }
        public static long ToUnixEpochDate(DateTime date) =>
            (long)Math.Round((date.ToUniversalTime() - new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero)).TotalSeconds);
    }

}

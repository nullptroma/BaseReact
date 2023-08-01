using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace BaseReact;

public static class AuthOptions
{
    public const string Issuer = "BaseReact";
    public const string Audience = "BaseReact";
    const string Key = "!!#hghghgmysupersecret_secretkey!123";

    public static SymmetricSecurityKey GetSymmetricSecurityKey() =>
        new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Key));
}
A fork of [SingletonSean/firebase-authentication-net-demo](https://github.com/SingletonSean/firebase-authentication-net-demo) with added support for setting previously missing Firebase claims (such as `picture`) and custom claims as regular claims, to allow accessing in policy definitions, `HttpContext.User.Claims`, etc.

This also allows the use of [Role-based authorization](https://learn.microsoft.com/en-us/aspnet/core/security/authorization/roles?view=aspnetcore-8.0) by adding a custom claim of type `ClaimTypes.Role` containing either a single role name or an array of role names, for example:

```csharp
var user = await FirebaseAuth.DefaultInstance.GetUserAsync(uid);
var claims = new Dictionary<string, object>(user.CustomClaims)
{
    [ClaimTypes.Role] = "Administrator"
    // Or [ClaimTypes.Role] = new[] {"Administrator", "Member", "Human" ...}
    ["MyOtherNewCustomClaim"] = "someValue"
    ...
};
await FirebaseAuth.DefaultInstance.SetCustomUserClaimsAsync(uid, claims);
```

This fork also adds an option to enable `FirebaseAuth.VerifyIdTokenAsync`'s `checkRevoked`

To enable these changes:
```csharp
builder.Services.AddFirebaseAuthentication(options => 
{
    options.CheckRevoked = true;
    options.GetCustomClaims = true;
    options.SplitRoleClaimArrays = true;
});
```

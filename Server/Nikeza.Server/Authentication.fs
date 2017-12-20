module internal Nikeza.Server.Authentication

    open System
    open System.Security.Claims
    open System.Security.Cryptography
    open Microsoft.AspNetCore.Cryptography.KeyDerivation
    open Store
    open Nikeza.Common

    type LoginResponse = 
        | Authenticated of ClaimsPrincipal
        | UnAuthenticated 
                      
    let generateSalt =            
        let mutable salt = Array.init (128/8)  (fun i -> byte(i*i))
        let getSalt s =                
            use rng = RandomNumberGenerator.Create()                
            rng.GetBytes(s); s
            
        getSalt salt |> Convert.ToBase64String

    let getPasswordHash password salt = 
        let salt = Convert.FromBase64String(salt)
        let hashedPassword = 
            Convert.ToBase64String(
                KeyDerivation.Pbkdf2(
                    password,
                    salt,
                    KeyDerivationPrf.HMACSHA1,
                    10000,
                    (256/8)
            ))
        hashedPassword     

    let authenticate email password = 
        loginProfile email |> function
        | Some user -> let hashedPassword = getPasswordHash password user.Salt
                       hashedPassword = user.PasswordHash
        | None      -> false

    let getUserClaims userName authScheme =
        let claims = [ Claim(ClaimTypes.Name, userName,  ClaimValueTypes.String)]
        let identity = ClaimsIdentity(claims, authScheme)

        ClaimsPrincipal(identity)
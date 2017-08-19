module Nikeza.Server.Authentication

    open System.Security.Claims 
    open Nikeza.Server.DataStore

    [<CLIMutable>]
    type RegistrationRequest = {
            UserName: string 
            Password: string 
        }
            
    [<CLIMutable>]
    type LogInRequest = {
            UserName: string
            Password: string 
        }

    type LoginResponse = 
        | Authenticated of ClaimsPrincipal
        | UnAuthenticated 

    type RegistrationStatus = 
        | Success   
        | Failure 
    
    open System
    open System.Security.Cryptography
    open Microsoft.AspNetCore.Cryptography.KeyDerivation
                  
    let private generateSalt =            
        let mutable salt = Array.init (128/8)  (fun i -> byte(i*i))
        let getSalt s =                
            use rng = RandomNumberGenerator.Create()                
            rng.GetBytes(s)
            s
        getSalt salt |> Convert.ToBase64String

    let private getPasswordHash password saltString = 
        let salt = Convert.FromBase64String(saltString)
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

    let authenticate username password = 
        match findUser username with
        | Some user -> 
             let hashedPassword = getPasswordHash password user.Salt
             hashedPassword = user.PasswordHash
        | None -> false
            
    open Nikeza.Server.Models
    let register (r:RegistrationRequest) =
        match findUser r.UserName with
        | Some user -> Failure
        | None ->
            let salt = generateSalt
            let hashedPassword = getPasswordHash r.Password salt
            
            let user:Profile = {
                ProfileId = 0
                FirstName = ""
                LastName = ""
                Email = r.UserName
                ImageUrl = ""
                Bio = ""
                PasswordHash = hashedPassword
                Salt = salt
                Created = DateTime.Now
            }

            try
                execute <| Register user
                Success
            with
            | e -> Failure
    
    let getUserClaims userName authScheme =
        let claims =
            [
                Claim(ClaimTypes.Name, userName,  ClaimValueTypes.String)
            ]

        let identity = ClaimsIdentity(claims, authScheme)
        ClaimsPrincipal(identity)                      
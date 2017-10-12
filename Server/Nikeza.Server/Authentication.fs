module Nikeza.Server.Authentication

    open System.Security.Claims 
    open Nikeza.Server.Command
    open Nikeza.Server.Store
    open Nikeza.Server.Model

    [<CLIMutable>]
    type RegistrationRequest = {
            FirstName: string 
            LastName: string
            Email: string
            Password: string
            Confirm: string
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
        | Success of Profile
        | Failure 
    
    open System
    open System.Security.Cryptography
    open Microsoft.AspNetCore.Cryptography.KeyDerivation
                  
    let private generateSalt =            
        let mutable salt = Array.init (128/8)  (fun i -> byte(i*i))
        let getSalt s =                
            use rng = RandomNumberGenerator.Create()                
            rng.GetBytes(s); s
            
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
            
    let register (info:RegistrationRequest) =
        match findUser info.Email with
        | Some user -> Failure
        | None ->
            let salt = generateSalt
            let hashedPassword = getPasswordHash info.Password salt
            
            let profile = {
                ProfileId = 0
                FirstName = info.FirstName
                LastName =  info.LastName
                Email =     info.Email
                ImageUrl =  ""
                Bio =       ""
                PasswordHash = hashedPassword
                Salt =         salt
                Created =      DateTime.Now
            }

            try
                execute <| Register profile
                Success profile
            with
            | e -> Failure
    
    let getUserClaims userName authScheme =
        let claims =
            [
                Claim(ClaimTypes.Name, userName,  ClaimValueTypes.String)
            ]

        let identity = ClaimsIdentity(claims, authScheme)
        ClaimsPrincipal(identity)                      
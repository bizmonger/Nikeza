module Nikeza.Server.Authentication

    open System
    open System.Security.Claims
    open System.Security.Cryptography
    open Microsoft.AspNetCore.Cryptography.KeyDerivation
    open Nikeza.Server.Literals
    open Nikeza.Server.Command
    open Nikeza.Server.Store
    open Nikeza.Server.Model

    [<CLIMutable>]
    type RegistrationRequest = {
            FirstName: string 
            LastName:  string
            Email:     string
            Password:  string
        }
            
    [<CLIMutable>]
    type LogInRequest = {
            Email:    string
            Password: string 
        }

    type LoginResponse = 
        | Authenticated of ClaimsPrincipal
        | UnAuthenticated 
                  
    let private generateSalt =            
        let mutable salt = Array.init (128/8)  (fun i -> byte(i*i))
        let getSalt s =                
            use rng = RandomNumberGenerator.Create()                
            rng.GetBytes(s); s
            
        getSalt salt |> Convert.ToBase64String

    let private getPasswordHash password salt = 
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
        let result = loginProfile email
        match result with
        | Some user -> 
             let hashedPassword = getPasswordHash password user.Salt
             hashedPassword = user.PasswordHash
        | None -> false
            
    let register (info:RegistrationRequest) =
        let result = loginProfile info.Email
        match result with
        | Some _ -> Failure
        | None ->
            let salt = generateSalt
            let hashedPassword = getPasswordHash info.Password salt
            
            let profile = {
                ProfileId = "to be determined..."
                FirstName = info.FirstName
                LastName =  info.LastName
                Email =     info.Email
                ImageUrl =  ThumbnailUrl
                Bio =       ""
                Sources =   []
                PasswordHash = hashedPassword
                Salt =         salt
                Created =      DateTime.Now
            }

            try
                let profileId = execute <| Register profile
                Success { profile with ProfileId = profileId |> string }
            with
            | _ -> Failure
    
    let getUserClaims userName authScheme =
        let claims = [ Claim(ClaimTypes.Name, userName,  ClaimValueTypes.String)]
        let identity = ClaimsIdentity(claims, authScheme)

        ClaimsPrincipal(identity)
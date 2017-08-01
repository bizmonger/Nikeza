namespace Nikeza.Server.Models
    open System
    open System.Security.Claims 

    [<CLIMutable>]
    type RegistrationRequest = 
        {
            UserName: string 
            Password: string 
        }
    [<CLIMutable>]
    type UserRegistration = 
        {
            UserName:     string 
            Salt:         byte[] 
            PasswordHash: string 
        }      
    [<CLIMutable>]
    type LogInRequest = 
        {
            UserName: string
            Password: string 
        }
    type LoginResponse = 
        | Authenticated of ClaimsPrincipal
        | UnAuthenticated 

    type RegistrationStatus = 
        | Success   
        | Failure 
    
    module Authentication = 
        open Newtonsoft.Json
        open System.IO
        open System.Security.Cryptography
        open System
        open System.Text
        open Microsoft.AspNetCore.Cryptography.KeyDerivation
                      
        let private generateSalt =            
            let mutable salt = Array.init (128/8)  (fun i -> byte(i*i))
            let getSalt s =                
                (use rng = RandomNumberGenerator.Create()                
                 rng.GetBytes(s))
                s
            getSalt salt

        let private getPasswordHash password salt =            
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

        let private getUserFileName userName  =  sprintf "db/%s.json" userName
        let private userFileExists userName = 
            let userFile = getUserFileName userName
            File.Exists(userFile)
         
        let private createUserFile (ur:UserRegistration) = 
            let userFile = getUserFileName ur.UserName
            File.WriteAllText(userFile, JsonConvert.SerializeObject(ur))

        let authenticate username password = 
            if userFileExists username
            then let userFile = getUserFileName username
                 let userRegistrationJson = File.ReadAllText(userFile)
                 let user = JsonConvert.DeserializeObject<UserRegistration>(userRegistrationJson)
                 let hashedPassword = getPasswordHash password user.Salt
                 let matches = hashedPassword = user.PasswordHash
                 matches

            else false
                
        let register (r:RegistrationRequest) =            
            if userFileExists r.UserName
            then Failure
            else let salt = generateSalt
                 let hashedPassword = getPasswordHash r.Password salt
                 let userRegistration = { UserName = r.UserName; Salt = salt; PasswordHash = hashedPassword }    
                 createUserFile userRegistration
                 Success
        
        let getUserClaims userName authScheme =
            let issuer = "http://localhost:5000"
            let claims =
                [
                    Claim(ClaimTypes.Name, userName,  ClaimValueTypes.String, issuer)                  
                ]

            let identity = ClaimsIdentity(claims, authScheme)
            ClaimsPrincipal(identity)                      
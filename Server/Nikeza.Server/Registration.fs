module Nikeza.Server.Registration

open System
open DatabaseCommand
open Authentication
open Literals
open Store
open Model
open Commands
            
let register (info:RegistrationRequest) =
    loginProfile info.Email |> function
    | Some _ -> Failure
    | None -> let salt = generateSalt
              let hashedPassword = getPasswordHash info.Password salt
              let profile = {
                  ProfileId =    "to be determined..."
                  FirstName =    info.FirstName
                  LastName =     info.LastName
                  Email =        info.Email
                  ImageUrl =     DefaultThumbnail
                  Bio =          ""
                  Sources =      []
                  PasswordHash = hashedPassword
                  Salt =         salt
                  Created =      DateTime.Now
              }

              try let profileId = register profile
                  match getProfileByEmail creatorEmail with
                  | Some creator -> 
                      if profile.Email <> creator.Email
                        then follow { SubscriberId= profileId; ProfileId= creator.ProfileId } |> ignore
                        else ()
                      Success { profile with ProfileId = profileId |> string }
                  | None -> Failure
              with
              | _ -> Failure

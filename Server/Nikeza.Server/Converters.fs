module Nikeza.Server.Converters

open Nikeza.Server.Model

let toPortfolio links = { 
    Answers=  []
    Articles= []
    Videos=   []
    Podcasts= []
}


let toProfileEssentials (profile:Profile) : ProfileRequest =
    {
        ProfileId=    profile.ProfileId
        FirstName=    profile.FirstName
        LastName=     profile.LastName
        Email=        profile.Email
        ImageUrl=     profile.ImageUrl
        Bio=          profile.Bio
        Sources=      profile.Sources
    }

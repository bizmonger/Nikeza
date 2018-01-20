module Nikeza.Mobile.UILogic.TestAPI

open Nikeza.Mobile.Profile.Try
open Nikeza.Mobile.Profile.Query
open Nikeza.Mobile.Subscriptions.Events
open Nikeza.Mobile.Subscriptions.Query
open Nikeza.Mobile.Portfolio.Query
open Nikeza.Common

let someProfile = {
    Nikeza.Common.ProfileRequest.Id =        ""
    Nikeza.Common.ProfileRequest.FirstName = ""
    Nikeza.Common.ProfileRequest.LastName =  ""
    Nikeza.Common.ProfileRequest.Email =     "test@abc.com"
    Nikeza.Common.ProfileRequest.Bio =       ""
    Nikeza.Common.ProfileRequest.ImageUrl =  ""
    Nikeza.Common.ProfileRequest.Sources =   []
}

let someProfileRequest = {
    Nikeza.Common.ProfileRequest.Id =        ""
    Nikeza.Common.ProfileRequest.FirstName = ""
    Nikeza.Common.ProfileRequest.LastName =  ""
    Nikeza.Common.ProfileRequest.Email =     ""
    Nikeza.Common.ProfileRequest.Bio =       ""
    Nikeza.Common.ProfileRequest.ImageUrl =  ""
    Nikeza.Common.ProfileRequest.Sources =   []
}


let somePortfolio = { 
    Answers  = []
    Articles = []
    Videos =   []
    Podcasts = []
}

let someProvider = {
    Nikeza.Common.ProviderRequest.Profile =      someProfile
    Nikeza.Common.ProviderRequest.Topics =        []
    Nikeza.Common.ProviderRequest.Portfolio=     somePortfolio
    Nikeza.Common.ProviderRequest.RecentLinks=   []
    Nikeza.Common.ProviderRequest.Subscriptions= []
    Nikeza.Common.ProviderRequest.Followers=     []
}

open Nikeza.Mobile.Profile.Registration

let someValidatedForm = { 
   Form = { Email= Email ""
            Password= Password ""
            Confirm=  Password ""
          }
    }

let mockSubmit : SubmitFn =
    fun _ -> Ok someProfile

let mockFailedSubmit : SubmitFn =
    fun _ -> Error someValidatedForm

let mockSave : SaveProfileFn =
    fun _ -> Ok someProfileRequest

let mockSaveSources : SaveSourcesFn =
    fun _ -> Ok someProfileRequest

let mockTopics : TopicsFn =
    fun _ -> Ok [{ Id=0; Name="F#" }
                 { Id=1; Name="Elm"}
                 { Id=2; Name="Architecture" }
                ]

let mockPlatforms : PlatformsFn =
    fun _ -> Ok [ "YouTube"
                  "WordPress"
                  "StackOverflow"
                ]

let mockRecent : RecentFn =
    fun _ -> RecentQuery.RecentSucceeded []

let mockPortfolio : PortfolioFn =
    fun _ -> Ok someProvider
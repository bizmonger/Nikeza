module Nikeza.Mobile.UILogic.TestAPI

open Nikeza.Access.Specification.Try
open Nikeza.Mobile.Profile.Try
open Nikeza.Mobile.Profile.Queries
open Nikeza.Mobile.Subscriptions.Query
open Nikeza.Mobile.Portfolio.Query
open Nikeza.Common
open System

let someProfile = {
    Nikeza.Common.ProfileRequest.Id =        "0"
    Nikeza.Common.ProfileRequest.FirstName = "Brian"
    Nikeza.Common.ProfileRequest.LastName =  "Nimrod"
    Nikeza.Common.ProfileRequest.Email =     "brian@abc.com"
    Nikeza.Common.ProfileRequest.Bio =       ""
    Nikeza.Common.ProfileRequest.ImageUrl =  "http://www.ngu.edu/myimages/silhouette2230.jpg"
    Nikeza.Common.ProfileRequest.Sources =   []
}

let someProfile2 = {someProfile with FirstName="Cherice"; LastName="Johnson"}
let someProfile3 = {someProfile with FirstName="Joel";    LastName="Roach"}

let someProfileRequest = someProfile

let somePortfolio = { 
    Answers  = []
    Articles = []
    Videos =   []
    Podcasts = []
}

let someLink1 = { 
    Id=           123
    ProfileId=   "profileId1"
    Title=       "Some Link Title_1"
    Description= "Some link description"
    Url=         "http://some_url.com"
    Topics=       []
    ContentType= "article"
    IsFeatured=   false
    Timestamp=    DateTime.Now
}

let someLink2 = {someLink1 with Title="Some Link Title_2"}
let someLink3 = {someLink1 with Title="Some Link Title_3"}

let someProvider = {
    Nikeza.Common.ProviderRequest.Profile =      someProfile
    Nikeza.Common.ProviderRequest.Topics =        []
    Nikeza.Common.ProviderRequest.Portfolio=     somePortfolio
    Nikeza.Common.ProviderRequest.RecentLinks=   [someLink1;someLink2;someLink3]
    Nikeza.Common.ProviderRequest.Subscriptions= []
    Nikeza.Common.ProviderRequest.Followers=     []
}

let someProvider2 = {someProvider with Profile=someProfile2}
let someProvider3 = {someProvider with Profile=someProfile3}

open Nikeza.Access.Specification

let someFirstName =       "Scott"
let someLastName =        "Nimrod"
let someEmail =           "scott@abc.com"
let somePassword =        "some_password"
let someInvalidPassword = "some_invalid_password"

let someValidatedForm = { 
   Form = { Email=    Email ""
            Password= Password ""
            Confirm=  Password ""
          }
    }

let mockLogin : LoginFn =
    fun credentials -> 
        if credentials.Password = somePassword then 
             Ok <| Some someProvider

        else Ok <| None

let mockLogout : LogoutFn =
    fun _ -> Ok someProvider

let mockSubmit : SubmitFn =
    fun _ -> Ok someProfile

let mockFailedSubmit : SubmitFn =
    fun _ -> Error someValidatedForm

let mockSave : SaveProfileFn =
    fun _ -> Ok someProfileRequest

let mockSaveSources : SaveSourcesFn =
    fun _ -> Ok someProfileRequest

let mockTopics : TopicsFn =
    fun _ -> Ok 
                [{ Id=0; Name="F#" }
                 { Id=1; Name="Elm"}
                 { Id=2; Name="Architecture" }
                ]

let mockPlatforms : PlatformsFn =
    fun _ -> Ok 
                [ "YouTube"
                  "WordPress"
                  "StackOverflow"
                ]

let mockRecent : RecentFn =
    fun _ -> Ok [someProvider;someProvider2;someProvider3]

let mockSubscriptions : SubscriptionsFn =
    fun _ -> Ok [someProvider;someProvider2;someProvider3]

let mockPortfolio : PortfolioFn =
    fun _ -> Ok someProvider
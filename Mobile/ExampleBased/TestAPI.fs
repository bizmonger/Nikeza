module Nikeza.Mobile.TestAPI

open Nikeza.Common
open Nikeza.DataTransfer
open Nikeza.Mobile.Profile.Try
open Nikeza.Mobile.Profile.Query
open Nikeza.Mobile.Subscriptions.Try
open Nikeza.Mobile.Subscriptions.Events
open Nikeza.Mobile.Subscriptions.Query
open Nikeza.Mobile.Portfolio.Query
open Nikeza.Mobile.UILogic.Registration

let someFirstName = "Scott"
let someLastName =  "Nimrod"
let someEmail =     "scott@abc.com"
let somePassword =  "some_password"

let someProfile:ProfileRequest = { 
    Id =        ""
    FirstName = ""
    LastName =  ""
    Email =     ""
    ImageUrl =  ""
    Bio =       ""
    Sources =   []
}

let someUser:Profile = { 
    Id =        ""
    FirstName = ""
    LastName =  ""
    Email =     ""
    ImageUrl =  ""
    Bio =       ""
    Sources =   []
}

let somePortfolio = { 
    Answers  = []
    Articles = []
    Videos =   []
    Podcasts = []
}

let someProvider = {
    Profile =      someProfile
    Topics =        []
    Portfolio=     somePortfolio
    RecentLinks=   []
    Subscriptions= []
    Followers=     []
}

let mockSubmit : SubmitFn =
    fun _ -> Ok { Id =        ""
                  FirstName = ""
                  LastName =  ""
                  Email =     ""
                  Bio =       ""
                  ImageUrl =  ""
                  Sources =   []
                }

let mockRecent : RecentFn =
    fun _ -> Ok [someProvider]

let mockMembers : MembersFn =
    fun _ -> Ok [someProvider]

let mockSubscriptions : SubscriptionsFn =
    fun _ -> Ok [someProvider]

let mockSave : SaveProfileFn =
    fun _ -> Ok someProfile
    
let mockSaveSources : SaveSourcesFn =
    fun _ -> Ok someProfile

let mockTopics : TopicsFn =
    fun _ -> Ok [{Id=0; Name="F#" }]

let mockPlatforms : PlatformsFn =
    fun _ -> Ok ["YouTube"; "WordPress"; "StackOverflow"]

let mockPortfolio : PortfolioFn =
    fun _ -> Ok someProvider

let mockFollow : FollowFn =
    fun _ -> Ok {User= someProvider; Provider=someProvider}

let mockUnsubscribe : UnsubscribeFn =
    fun _ -> Ok {User= someProvider; Provider=someProvider}

module Registration =

    let viewmodelDependencies =

        let responders =   { ForRegistrationSubmission= [] }
        let functions =    { Submit=mockSubmit }
    
        { SideEffectFunctions=  functions; EventResponders= responders }


module Portfolio =
    open Nikeza.Mobile.UILogic.Portal.Portfolio

    let injected = {
        UserId =        ProviderId someProfile.Id
        ProviderId =    ProviderId someProvider.Profile.Id
        PortfolioFn =   mockPortfolio
        FollowFn =      mockFollow
        UnsubscribeFn = mockUnsubscribe
    }

type Nikeza.Mobile.UILogic.Registration.ViewModel with

    member x.FillOut () =
           x.Email    <- someEmail
           x.Password <- somePassword
           x.Confirm  <- somePassword
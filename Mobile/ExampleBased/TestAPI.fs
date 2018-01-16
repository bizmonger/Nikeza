module Nikeza.Mobile.TestAPI

open Nikeza.Common
open Nikeza.DataTransfer
open Nikeza.Mobile.Profile.Try
open Nikeza.Mobile.Profile.Query
open Nikeza.Mobile.Profile.Events
open Nikeza.Mobile.Subscriptions.Try
open Nikeza.Mobile.Subscriptions.Events
open Nikeza.Mobile.Subscriptions.Query
open Nikeza.Mobile.Portfolio.Query
open Nikeza.Mobile.Portfolio.Events

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
    fun _ -> Query.RecentSucceeded [someProvider]

let mockMembers : MembersFn =
    fun _ -> Query.MembersSucceeded [someProvider]

let mockSubscriptions : SubscriptionsFn =
    fun _ -> Query.SubscriptionsSucceeded [someProvider]

let mockSave : SaveFn =
    fun _ -> Ok someProfile

let mockTopics : TopicsFn =
    fun _ -> TopicsQuery.TopicsSucceeded [{Id=0; Name="F#" }]

let mockPlatforms : PlatformsFn =
    fun _ -> PlatformsQuery.PlatformsSucceeded [Platform "YouTube"; Platform "WordPress"; Platform "StackOverflow"]

let mockPortfolio : PortfolioFn =
    fun _ -> Query.Succeeded someProvider

let mockFollow : FollowFn =
    fun _ -> Ok {User= someProvider; Provider=someProvider}

let mockUnsubscribe : UnsubscribeFn =
    fun _ -> Ok {User= someProvider; Provider=someProvider}


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

    member x.FillIn () =
           x.Email    <- someEmail
           x.Password <- somePassword
           x.Confirm  <- somePassword
           x
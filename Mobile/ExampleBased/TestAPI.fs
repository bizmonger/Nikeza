module Nikeza.Mobile.TestAPI

open Nikeza.Common
open Nikeza.DataTransfer
open Nikeza.Mobile.Access
open Nikeza.Mobile.Profile.Try
open Nikeza.Mobile.Profile.Queries
open Nikeza.Mobile.Subscriptions.Try
open Nikeza.Mobile.Subscriptions.Query
open Nikeza.Mobile.Subscriptions.Events
open Nikeza.Mobile.Portfolio.Query

let someFirstName =       "Scott"
let someLastName =        "Nimrod"
let someEmail =           "scott@abc.com"
let somePassword =        "some_password"
let someInvalidPassword = "some_invalid_password"

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
    fun _ -> Ok [someProvider;someProvider;someProvider]

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

let onQueryFailed = function
    | GetRecentFailed        _ -> ()
    | GetMembersFailed       _ -> ()
    | GetSubscriptionsFailed _ -> ()

let onPageRequested = function _ -> ()

module Registration =

    open Nikeza.Mobile.UILogic.Registration

    let dependencies =

        let sideEffects =    { ForRegistrationSubmission= [] }
        let implementation = { Submit=mockSubmit }
    
        { Implementation=  implementation; SideEffects= sideEffects }

module ProfileEditor =

    open Nikeza.Mobile.UILogic.Portal.ProfileEditor
    
    let dependencies =

        let sideEffects =    { ForProfileSave= []; ForQueryTopicsFailed= [] }
        let implementation = { Save= mockSave }
    
        { Implementation= implementation
          SideEffects=    sideEffects 
          User =          someUser
          Query =       { Topics= mockTopics }
        }


module Portal =

    open Nikeza.Mobile.UILogic.Portal
    
    let dependencies =

        let sideEffects = { 
            ForQueryFailed=  []
            ForPageRequested=[] 
        }
    
        { SideEffects=  sideEffects 
          User =        someProfile
          Query =     { Subscriptions= mockSubscriptions }
        }

module DataSource =

    open Nikeza.Mobile.UILogic.Portal.DataSources
    
    let dependencies =

        let sideEffects =    { ForSaveSources= [] }
        let implementation = { Save= mockSaveSources }
    
        { Implementation=  implementation
          SideEffects=     sideEffects 
          UserId =         ProfileId someUser.Id
          Query =        { Platforms= mockPlatforms }
        }


module Portfolio =

    open Nikeza.Mobile.UILogic.Portal.Portfolio

    let dependencies =

        let sideEffects = { 
            ForFollow =        []
            ForUnsubscribe =   []
            ForQueryFailed =   []
            ForPageRequested = []
        }

        let implementation = { 
            Follow=      mockFollow 
            Unsubscribe= mockUnsubscribe
        }
    
        { UserId=         ProviderId someUser.Id
          ProviderId=     ProviderId someProvider.Profile.Id
          Query=        { Portfolio= mockPortfolio }
          Implementation= implementation
          SideEffects=    sideEffects 
        }
        
module Recent =

    open Nikeza.Mobile.UILogic.Portal.Recent

    let dependencies =

        let sideEffects = {
            ForQueryFailed =   { Head=onQueryFailed;   Tail=[] }
            ForPageRequested = { Head=onPageRequested; Tail=[] }
        }
    
        { UserId=      ProfileId someUser.Id
          Query=     { Portfolio= mockPortfolio; Recent= mockRecent }
          SideEffects= sideEffects 
        }

module Members =

    open Nikeza.Mobile.UILogic.Portal.Members

    let dependencies =

        let sideEffects = {
            ForQueryFailed =   { Head=onQueryFailed;   Tail=[] }
            ForPageRequested = { Head=onPageRequested; Tail=[] }
        }
    
        { Query=      { Members= mockMembers }
          SideEffects=  sideEffects 
        }

module Subscriptions =

    open Nikeza.Mobile.UILogic.Portal.Subscriptions

    let dependencies =
            
        let sideEffects = { 
            ForQueryFailed =   { Head=onQueryFailed;   Tail=[] }
            ForPageRequested = { Head=onPageRequested; Tail=[] }
        }

        let userId = ProfileId someUser.Id
    
        { UserId=       userId
          Query=      { Portfolio= mockPortfolio; Subscriptions=mockSubscriptions }
          SideEffects=  sideEffects 
        }

type Nikeza.Mobile.UILogic.Registration.ViewModel with

    member x.FillOut () =
           x.Email    <- someEmail
           x.Password <- somePassword
           x.Confirm  <- somePassword
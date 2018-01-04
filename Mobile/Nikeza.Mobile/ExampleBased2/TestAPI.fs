module TestAPI

open Nikeza.Common
open Nikeza.Mobile.Profile.Try
open Nikeza.Mobile.Subscription.Events
open Nikeza.Mobile.Subscriptions.Query

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
    fun _ -> [GetRecentSucceeded [someProvider]]

let mockMembers : MembersFn =
    fun _ -> [GetMembersSucceeded [someProvider]]

let mockSubscriptions : SubscriptionsFn =
    fun _ -> [GetSubscriptionsSucceeded [someProvider]]

let mockSave : SaveFn =
    fun _ -> Ok someProfile

type Nikeza.Mobile.UILogic.Registration.ViewModel with

    member x.FillIn () =
           x.Email    <- someEmail
           x.Password <- somePassword
           x.Confirm  <- somePassword
           x
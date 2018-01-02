namespace Nikeza.Mobile.UILogic.Portal.Recent

open Nikeza.Common
open Nikeza.DataTransfer
open Nikeza.Mobile.Subscriptions
open Nikeza.Mobile.UILogic.Publisher
open Nikeza.Mobile.Subscription.Events

type ViewModel(user:Provider) as x =

    let eventOccurred = new Event<_>()
    let mutable latest:Provider list = []

    member x.User =   user
    member x.Latest = latest

    member x.Load() =
        Query.latest <| ProfileId user.Profile.Id
         |> function
            | GetLatestSucceeded providers :: [] -> latest <- providers
            | otherEvents -> publish eventOccurred otherEvents
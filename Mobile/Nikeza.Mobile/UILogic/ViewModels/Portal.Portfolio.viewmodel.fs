namespace Nikeza.Mobile.UILogic.Portal.Portfolio

open Nikeza.Common
open Nikeza.Mobile.Portfolio.Events
open Nikeza.Mobile.Portfolio.Query
open Nikeza.Mobile.UILogic
open System.Windows.Input
open Nikeza.Mobile.UILogic.Publisher

type ViewModel(providerId, getPortfolio:PortfolioFn) =

    let mutable provider = uninitializedProvider
    let eventOccurred =    Event<_>()

    let getId = providerId |> function ProviderId id -> id
    
    let isAlreadyFollowing providerId =
        provider.Subscriptions |> List.contains providerId

    let follow =      DelegateCommand( (fun _ -> ()), fun _ -> not <| isAlreadyFollowing    getId ) :> ICommand
    let unsubscribe = DelegateCommand( (fun _ -> ()), fun _ ->        isAlreadyFollowing <| getId ) :> ICommand

    member x.Follow =      follow
    member x.Unsubscribe = unsubscribe

    member x.Provider =    provider

    member x.Load() =
             providerId 
              |> getPortfolio
              |> function
                 | GetPortfolioSucceeded p :: _ -> provider <- p
                 | otherEvents -> otherEvents |> publish eventOccurred

    member x.EventOccurred() = eventOccurred.Trigger
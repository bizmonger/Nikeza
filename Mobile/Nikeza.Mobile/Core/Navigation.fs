namespace Nikeza.Mobile.AppLogic

open Nikeza.Mobile.AppLogic.Pages
open Nikeza.Mobile.UILogic.Registration
open Nikeza.Mobile.Profile.Events

type Navigation() =

    let toPage = function
        | RegistrationSucceeded _ -> PageRequested.Portal
        | RegistrationFailed    _ -> PageRequested.ErrorRegistering

    let viewModel = ViewModel()
    do viewModel.EventOccured.Add(fun event -> event |> toPage |> ignore)
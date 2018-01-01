namespace Nikeza.Mobile.AppLogic

open Nikeza.Mobile.AppLogic.Pages
open Nikeza.Mobile.UILogic.Registration
open Nikeza.Mobile.Profile.Events

type Navigation() =

    let request page = () // Todo...

    let toPage = function
        | RegistrationSucceeded _ -> PageRequested.Portal
        | RegistrationFailed    _ -> PageRequested.ErrorRegistering

    let viewModel = ViewModel(Try.submit)
    do viewModel.EventOccured.Add(fun event -> event |> toPage |> request)
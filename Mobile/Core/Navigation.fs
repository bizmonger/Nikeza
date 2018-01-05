namespace Nikeza.Mobile.AppLogic

open Nikeza.Mobile.UILogic.Pages
open Nikeza.Mobile.UILogic.Registration
open Nikeza.Mobile.Profile.Events
open Nikeza.Mobile.Profile

type Navigation() =

    let request page = () // Todo...

    let toPage = function
        | RegistrationSucceeded _ -> PageRequested.Portal
        | RegistrationFailed    _ -> PageRequested.RegistrationError

    let viewModel = ViewModel(Try.submit)
    do viewModel.EventOccured.Add(fun event -> event |> toPage |> request)
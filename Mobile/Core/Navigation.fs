namespace Nikeza.Mobile.AppLogic

open System.Collections.Generic
open Nikeza.Mobile.AppLogic.TestAPI
open Nikeza.Mobile.UILogic
open Nikeza.Mobile.UILogic.Pages
open Nikeza.Mobile.UILogic.Registration
open Nikeza.Mobile.Profile.Events

type Navigation(viewmodels) =

    let requested = Event<_>()
    let request page = requested.Trigger(page)

    let toPage = function
        | RegistrationSucceeded profile -> profile |> PageRequested.Portal
        | RegistrationFailed    form    -> form    |> PageRequested.RegistrationError

    let viewModel = viewmodels.Registration
    do viewModel.PageRequested.Add(fun event -> event |> toPage |> request)

    [<CLIEvent>]
    member x.Requested = requested.Publish
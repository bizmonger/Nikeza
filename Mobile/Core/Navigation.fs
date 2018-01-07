namespace Nikeza.Mobile.AppLogic

open Nikeza.Mobile.UILogic.Pages
open Nikeza.Mobile.Profile.Events

type Navigation() =

    let requested = Event<_>()
    let request (page:PageRequested) = requested.Trigger(page)

    let toPage = function
        | RegistrationSucceeded profile -> profile |> PageRequested.Portal
        | RegistrationFailed    form    -> form    |> PageRequested.RegistrationError

    [<CLIEvent>]
    member x.Requested = requested.Publish
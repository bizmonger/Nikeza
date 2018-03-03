namespace Nikeza.Mobile.AppLogic

module ProfileEvents =

    open Nikeza.Mobile.Profile.Events
    open Nikeza.Mobile.AppLogic.Response

    let handle = function
        | RegistrationSubmissionEvent e -> 
                                      e |> function
                                           | RegistrationSucceeded p -> navigate <| NavigateToPortal p
                                           | RegistrationFailed    p -> ()

        | SessionEvent                e -> ()
        | SubscriptionEvent           e -> ()
        | ProfileSaveEvent            e -> ()
        | SourcesSaveEvent            e -> ()
        | _ -> ()
namespace Nikeza.Mobile.AppLogic

module HandleProfile =

    open Nikeza.Mobile.Profile.Events

    let events = function
        | RegistrationValidationEvent _ -> ()
        | RegistrationSubmissionEvent _ -> ()
        | SessionEvent                _ -> ()
        | SubscriptionEvent           _ -> ()
        | ProfileValidateEvent        _ -> ()
        | ProfileSaveEvent            _ -> ()
        | SourcesSaveEvent            _ -> ()
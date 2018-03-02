namespace Nikeza.Mobile.AppLogic

module HandleSubscriptions =

    open Nikeza.Mobile.Subscriptions.Events

    let events = function
        | NotificationEvent _ -> ()
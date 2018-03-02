namespace Nikeza.Mobile.AppLogic

module HandlePortfolio =

    open Nikeza.Mobile.Portfolio.Events

    let events = function
        | LinksEvent  _ -> ()
        | TopicsEvent _ -> ()
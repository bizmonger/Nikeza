module Execute

open Nikeza.Mobile.Portfolio.Commands
open Nikeza.Mobile.Portfolio.Events

module Links =
    type private LinksWorkflow = LinkCommand -> LinksEvent  list

    let workflow : LinksWorkflow = function
        | LinkCommand.Feature   linkId -> 
                                linkId |> Attempt.featureLink
                                       |> ResultOf.Link.Feature
                                       |> Are.Registration.events
                                   
        | LinkCommand.Unfeature linkId -> 
                                linkId |> Attempt.unfeatureLink
                                       |> ResultOf.Link.Unfeature
                                       |> Are.Registration.events
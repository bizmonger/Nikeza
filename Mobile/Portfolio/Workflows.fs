module Execute

open Nikeza.Mobile.Portfolio.Commands
open Nikeza.Mobile.Portfolio.Events

module Links =
    type private LinksWorkflow = LinkCommand -> LinksEvent  list

    let workflow = function
        | LinkCommand.Feature   linkId -> 
                                linkId |> Try.featureLink
                                       |> ResultOf.Link.Feature
                                       |> Are.Registration.events
                                   
        | LinkCommand.Unfeature linkId -> 
                                linkId |> Try.unfeatureLink
                                       |> ResultOf.Link.Unfeature
                                       |> Are.Registration.events
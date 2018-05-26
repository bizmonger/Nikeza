namespace Nikeza.Mobile.AppLogic

module ProfileEvents =

    open System.Diagnostics
    open Nikeza.Mobile.UILogic.Portal.ProfileEditor
    open Nikeza.Mobile.AppLogic.Specification.ProfileEditor

    module Save =

        let appendPersistence : Save.AddSideEffectsFn =

            fun app sideEffects ->
 
                let handle = function
                    | Ok    _ -> (Debug.WriteLine(sprintf "Request: Navigate to previous page"))
                    | Error _ -> ()

                let handlers = handle::sideEffects.ForProfileSave

                { sideEffects with SideEffects.ForProfileSave= handlers }

    module Topics =

        let appendQuery : QueryFailed.AddSideEffectsFn =

            fun app sideEffects ->
 
                let handle = function
                    | Error msg -> (Debug.WriteLine(sprintf "Request: Navigate to Error page\n %s" msg))
                    | Ok    _   -> ()

                let handlers = handle::sideEffects.ForQueryTopicsFailed

                { sideEffects with SideEffects.ForQueryTopicsFailed= handlers }
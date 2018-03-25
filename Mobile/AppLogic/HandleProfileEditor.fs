namespace Nikeza.Mobile.AppLogic

module ProfileEvents =

    open System.Diagnostics
    open Nikeza.Mobile.Profile.Events
    open Nikeza.Mobile.UILogic.Portal.ProfileEditor
    open Nikeza.Mobile.AppLogic.Design.ProfileEditor

    module Save =

        let appendPersistence : Save.SideEffects =

            fun app sideEffects ->
 
                let handle = function
                    | ProfileSaved      p -> (Debug.WriteLine(sprintf "Request: Navigate to previous page"))
                    | ProfileSaveFailed _ -> ()

                let handlers = handle::sideEffects.ForProfileSave

                { sideEffects with SideEffects.ForProfileSave= handlers }

    module Topics =

        let appendQuery : QueryFailed.SideEffects =

            fun app sideEffects ->
 
                let handle = function
                    | QueryTopicsFailed    msg -> (Debug.WriteLine(sprintf "Request: Navigate to Error page\n %s" msg))
                    | QueryTopicsSucceeded _   -> ()

                let handlers = handle::sideEffects.ForQueryTopicsFailed

                { sideEffects with SideEffects.ForQueryTopicsFailed= handlers }
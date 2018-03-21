namespace Nikeza.Mobile.AppLogic

module ProfileEvents =

    open System.Diagnostics
    open Nikeza.Mobile.Profile.Events

    module Save =
        
        open Nikeza.Mobile.UILogic.Portal.ProfileEditor
        open Nikeza.Mobile.AppLogic.Design.ProfileEditor

        let appendPersistence : Save.SideEffects =

            fun sideEffects ->
 
                let handle = function
                    | ProfileSaved      p -> (Debug.WriteLine(sprintf "Request: Navigate to previous page"))
                    | ProfileSaveFailed _ -> ()

                let handlers = handle::sideEffects.ForProfileSave

                { sideEffects with SideEffects.ForProfileSave= handlers }

    module Topics =
        
        open Nikeza.Mobile.UILogic.Portal.ProfileEditor
        open Nikeza.Mobile.AppLogic.Design.ProfileEditor

        let appendQuery : QueryFailed.SideEffects =

            fun sideEffects ->
 
                let handle = function
                    QueryTopicsFailed msg -> (Debug.WriteLine(sprintf "Request: Navigate to Error page\n %s" msg))

                let handlers = handle::sideEffects.ForQueryTopicsFailed

                { sideEffects with SideEffects.ForQueryTopicsFailed= handlers }
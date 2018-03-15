namespace Nikeza.Mobile.AppLogic

module ProfileEvents =

    open Nikeza.Mobile.Profile.Events
    open System.Diagnostics

    module Register =

        open Nikeza.Mobile.UILogic.Registration

        let addTo sideEffects =
 
            let handle = function
                | RegistrationSucceeded p -> (Debug.WriteLine(sprintf "Request: Navigate to Portal\n %A" p))
                | RegistrationFailed    _ -> ()

            let handlers = handle::sideEffects.ForRegistrationSubmission

            { sideEffects with SideEffects.ForRegistrationSubmission= handlers }

    module Save =
        
        open Nikeza.Mobile.UILogic.Portal.ProfileEditor

        let addTo sideEffects =
 
            let handle = function
                | ProfileSaved      p -> (Debug.WriteLine(sprintf "Request: Navigate to previous page"))
                | ProfileSaveFailed _ -> ()

            let handlers = handle::sideEffects.ForProfileSave

            { sideEffects with SideEffects.ForProfileSave= handlers }

    module Topics =
        
        open Nikeza.Mobile.UILogic.Portal.ProfileEditor

        let addTo sideEffects =
 
            let handle = function
                QueryTopicsFailed msg -> (Debug.WriteLine(sprintf "Request: Navigate to Error page\n %s" msg))

            let handlers = handle::sideEffects.ForQueryTopicsFailed

            { sideEffects with SideEffects.ForQueryTopicsFailed= handlers }
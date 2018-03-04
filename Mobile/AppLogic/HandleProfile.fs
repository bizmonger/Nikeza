namespace Nikeza.Mobile.AppLogic

module ProfileEvents =

    open Nikeza.Mobile.UILogic.Registration
    open Nikeza.Mobile.Profile.Events
    open System.Diagnostics

    let addTo responders =
 
        let handle =  function
         | RegistrationSubmissionEvent.RegistrationSucceeded p -> (Debug.WriteLine(sprintf "Request: Navigate to Portal\n %A" p))
         | RegistrationSubmissionEvent.RegistrationFailed    _ -> ()

        let handlers = handle::responders.ForRegistrationSubmission
        { responders with Responders.ForRegistrationSubmission= handlers }
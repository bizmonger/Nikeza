module Nikeza.Mobile.Profile.EventExtraction

open System.Runtime.CompilerServices
open Nikeza.Mobile.Profile.Events

[<Extension>]
type RegistrationSubmissionEventExtension () =
    [<Extension>]
    static member TryGetProfile (x : RegistrationSubmissionEvent) = 
         match x with
         | RegistrationSucceeded profile -> Some profile
         | _                             -> None

[<Extension>]
type ProfileEditorEventExtension () =
    [<Extension>]
    static member TryGetProfile(x:ProfileEditorEvent) =
           match x with
           | ProfileSaved profile -> Some profile
           | _                    -> None
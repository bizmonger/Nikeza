module Nikeza.Mobile.Profile.EventExtentions

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
    static member TryGetProfile(x:ProfileSaveEvent) =
           match x with
           | ProfileSaved profile -> Some profile
           | _                    -> None

[<Extension>]
type SourcesSaveEventExtension () =
    [<Extension>]
    static member TryGetProfile(x:SourcesSaveEvent) =
           match x with
           | SourcesSaved profile -> Some profile
           | _                    -> None
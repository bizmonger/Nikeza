module Nikeza.Mobile.Profile.EventExtentions

open System.Runtime.CompilerServices
open Nikeza.Access.Specification.Events

[<Extension>]
type RegistrationSubmissionEventExtension () =
    [<Extension>]
    static member TryGetProfile (x : RegistrationSubmissionEvent) = 
         match x with
         | RegistrationSucceeded profile -> Some profile
         | _                             -> None
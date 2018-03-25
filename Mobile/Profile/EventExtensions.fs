module Nikeza.Mobile.Profile.EventExtentions

open System.Runtime.CompilerServices
open Nikeza.DataTransfer

[<Extension>]
type ProfileEditorEventExtension () =
    [<Extension>]
    static member TryGetProfile(x:Result<Profile, ValidatedProfile>) =
           match x with
           | Ok profile -> Some profile
           | _          -> None

[<Extension>]
type SourcesSaveEventExtension () =
    [<Extension>]
    static member TryGetProfile(x:Result<Profile,DataSourceSubmit list>) =
           match x with
           | Ok profile -> Some profile
           | Error _    -> None
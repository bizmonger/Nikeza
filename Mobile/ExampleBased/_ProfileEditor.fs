module _ProfileEditor

open FsUnit
open NUnit.Framework
open Nikeza.Mobile.TestAPI
open Nikeza.Mobile.UILogic.Portal.ProfileEditor
open Nikeza.Mobile.Profile.Events

[<Test>]
let ``Save edited profile`` () =

    // Setup
    let mutable saveOccurred = false

    let profileEditor = ViewModel(someUser, mockSave)

    profileEditor.EventOccurred
                 .Add(fun events ->
                          events |> (fun event -> 
                                         event |> function
                                                  | ProfileSaved _ -> saveOccurred <- true
                                                  | _ -> ()))
    profileEditor.FirstName <- someFirstName
    profileEditor.LastName  <- someLastName
    profileEditor.Email     <- someEmail

    profileEditor.Validate.Execute()

    // Test
    profileEditor.Save.Execute()

    // Verify
    saveOccurred |> should equal true

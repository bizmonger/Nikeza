module _ProfileEditor

open FsUnit
open NUnit.Framework
open Nikeza.Mobile.TestAPI
open Nikeza.Mobile.UILogic.Portal.ProfileEditor
open Nikeza.Mobile.Profile.Events

[<Test>]
let ``Save edited profile`` () =

    // Setup
    let mutable saveRequested = false
    let inject =      { User= someUser; SaveFn= mockSave; TopicsFn= mockTopics }
    let profileEditor = ViewModel(inject)

    profileEditor.SaveRequest
                 .Add(fun events ->
                          events |> (fun event -> 
                                         event |> function
                                                  | ProfileSaved _ -> saveRequested <- true
                                                  | _ -> ()))
    profileEditor.FirstName <- someFirstName
    profileEditor.LastName  <- someLastName
    profileEditor.Email     <- someEmail

    profileEditor.Validate.Execute()

    // Test
    profileEditor.Save.Execute()

    // Verify
    saveRequested |> should equal true
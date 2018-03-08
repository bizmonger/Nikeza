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

    let response = function
        | ProfileSaved _ -> saveRequested <- true
        | _ -> ()

    let responders =   { ForProfileSave= [response]; ForTopicsFnFailed= [] }
    let dependencies = { ProfileEditor.dependencies with Observers= responders }

    let profileEditor = ViewModel(dependencies)

    profileEditor.FirstName <- someFirstName
    profileEditor.LastName  <- someLastName
    profileEditor.Email     <- someEmail

    profileEditor.Validate.Execute()

    // Test
    profileEditor.Save.Execute()

    // Verify
    saveRequested |> should equal true
module _ProfileEditor

open FsUnit
open NUnit.Framework
open Nikeza.Mobile.TestAPI
open Nikeza.Mobile.UILogic.Portal.ProfileEditor

[<Test>]
let ``Save edited profile`` () =

    // Setup
    let mutable saveRequested = false
    let response = function Ok _ -> saveRequested <- true | _ -> ()

    let responders =   { ForProfileSave= [response]; ForQueryTopicsFailed= [] }
    let dependencies = { ProfileEditor.dependencies with SideEffects= responders }

    let profileEditor = ViewModel(dependencies)

    profileEditor.FirstName <- someFirstName
    profileEditor.LastName  <- someLastName
    profileEditor.Email     <- someEmail

    profileEditor.Validate.Execute()

    // Test
    profileEditor.Save.Execute()

    // Verify
    saveRequested |> should equal true
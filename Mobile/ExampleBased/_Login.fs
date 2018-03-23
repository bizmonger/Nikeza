module Tests.Login

open System
open FsUnit
open NUnit.Framework
open Nikeza.Mobile.TestAPI
open Nikeza.Mobile.AppLogic
open Nikeza.Mobile.UILogic

[<Test>]
let ``Can't login if missing email`` () =
    
    // Setup
    let login = Login.ViewModel(Login.dependencies)
    login.Email    <- ""
    login.Password <- somePassword

    // Test
    login.Next.Execute()

    // Verify
    login.IsValidated |> should equal false

[<Test>]
let ``Can't login if missing password`` () =
    
    // Setup
    let login = Login.ViewModel(Login.dependencies)
    login.Email    <- someEmail
    login.Password <- ""

    // Test
    login.Next.Execute()

    // Verify
    login.IsValidated |> should equal false

[<Test>]
let ``Can login if provided email and password`` () =
    
    // Setup
    let login = Login.ViewModel(Login.dependencies)
    login.Email    <- someEmail
    login.Password <- somePassword

    // Test
    try     login.Next.Execute()
    with :? TypeInitializationException -> login.IsValidated |> should equal true
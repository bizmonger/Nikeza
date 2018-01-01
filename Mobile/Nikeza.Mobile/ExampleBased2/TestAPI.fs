module TestAPI

open Nikeza.Mobile.UILogic.Registration

let someEmail =    "scott@abc.com"
let somePassword = "some_password"

module Apply =

    let valuesTo (viewmodel:ViewModel) =
        viewmodel.Email    <- someEmail
        viewmodel.Password <- somePassword
        viewmodel.Confirm  <- somePassword
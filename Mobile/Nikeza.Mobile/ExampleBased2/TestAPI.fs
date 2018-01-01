module TestAPI

open Nikeza.Mobile.UILogic.Registration
open Nikeza.DataTransfer
open Try

let someEmail =    "scott@abc.com"
let somePassword = "some_password"

let mockSubmit : SubmitRegistration =
    fun validated -> Ok { Profile.Id =        ""
                          Profile.FirstName = ""
                          Profile.LastName =  ""
                          Profile.Email =     ""
                          Profile.Bio =       ""
                          Profile.ImageUrl =  ""
                          Profile.Sources =   []
                        }

module Apply =

    let valuesTo (viewmodel:ViewModel) =
        viewmodel.Email    <- someEmail
        viewmodel.Password <- somePassword
        viewmodel.Confirm  <- somePassword
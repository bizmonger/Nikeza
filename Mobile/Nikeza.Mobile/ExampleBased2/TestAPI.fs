module TestAPI

open Nikeza.Mobile.UILogic.Registration
open Try

let someEmail =    "scott@abc.com"
let somePassword = "some_password"

let mockSubmit : SubmitFn =
    fun _ -> Ok { Id =        ""
                  FirstName = ""
                  LastName =  ""
                  Email =     ""
                  Bio =       ""
                  ImageUrl =  ""
                  Sources =   []
                }

type ViewModel with

    member x.FillIn () =
           x.Email    <- someEmail
           x.Password <- somePassword
           x.Confirm  <- somePassword
           x
module Nikeza.Mobile.AppLogic.TestAPI

open Nikeza.Mobile.Profile.Try

    let mockSubmit : SubmitFn =
        fun _ -> Ok { Id =        ""
                      FirstName = ""
                      LastName =  ""
                      Email =     ""
                      Bio =       ""
                      ImageUrl =  ""
                      Sources =   []
                    }
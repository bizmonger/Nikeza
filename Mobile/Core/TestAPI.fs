module Nikeza.Mobile.AppLogic.TestAPI


open Nikeza.Mobile.Profile.Try
open Nikeza.DataTransfer

let someProfile = {
    Profile.Id =        ""
    Profile.FirstName = ""
    Profile.LastName =  ""
    Profile.Email =     ""
    Profile.Bio =       ""
    Profile.ImageUrl =  ""
    Profile.Sources =   []
}

let someProfileRequest = {
    Nikeza.Common.ProfileRequest.Id =        ""
    Nikeza.Common.ProfileRequest.FirstName = ""
    Nikeza.Common.ProfileRequest.LastName =  ""
    Nikeza.Common.ProfileRequest.Email =     ""
    Nikeza.Common.ProfileRequest.Bio =       ""
    Nikeza.Common.ProfileRequest.ImageUrl =  ""
    Nikeza.Common.ProfileRequest.Sources =   []
}

let mockSubmit : SubmitFn =
    fun _ -> Ok someProfile

let mockSave : SaveFn =
    fun _ -> Ok someProfileRequest
module Nikeza.Mobile.Profile.Attempt

open Nikeza
open Nikeza.Common
open Nikeza.DataTransfer

type SaveProfileFn = ValidatedProfile      -> Result<DataTransfer.Profile,ValidatedProfile error>
type SaveSourcesFn = DataSourceSubmit list -> Result<DataTransfer.Profile, (DataSourceSubmit list) error>

let saveProfile : SaveProfileFn = 
    fun profile -> Error { Context=profile; Description="Not implemented" }


let saveSources : SaveSourcesFn = 
    fun sources -> Error { Context=sources; Description="Not implemented" }  
module Nikeza.Mobile.Profile.Try

open Nikeza.DataTransfer

type SaveProfileFn = ValidatedProfile      -> Result<Profile, ValidatedProfile>
type SaveSourcesFn = DataSourceSubmit list -> Result<Profile, DataSourceSubmit list>

let internal saveProfile : SaveProfileFn = 
    fun profile -> Error profile

let internal saveSources : SaveSourcesFn = 
    fun sources -> Error sources
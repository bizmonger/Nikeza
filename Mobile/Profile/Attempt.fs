module Nikeza.Mobile.Profile.Attempt

open Nikeza.DataTransfer
open Events

type SaveProfileFn = ValidatedProfile      -> SaveProfileEvent
type SaveSourcesFn = DataSourceSubmit list -> SaveDataSourcesEvent

let internal saveProfile : SaveProfileFn = 
    fun profile -> Error profile

let internal saveSources : SaveSourcesFn = 
    fun sources -> Error sources
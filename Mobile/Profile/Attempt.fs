module Nikeza.Mobile.Profile.Attempt

open Nikeza
open Events
open Nikeza.Common
open Nikeza.DataTransfer
open Nikeza.Mobile.Profile.Commands.ProfileEditor

type SaveProfileFn = SaveCommand -> Result<DataTransfer.Profile,ValidatedProfile error>
type SaveSourcesFn = DataSourceSubmit list -> SaveDataSourcesEvent

let saveProfile : SaveProfileFn = 
    fun command ->
        command |> function Save profile -> Error { Context=profile; Description="Not implemented" }

let saveSources : SaveSourcesFn = 
    fun sources -> Error sources
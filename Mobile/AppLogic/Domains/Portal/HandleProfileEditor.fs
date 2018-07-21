namespace Nikeza.Mobile.AppLogic

open Xamarin.Forms
open Nikeza.Mobile.AppLogic.Navigation
open Nikeza.Mobile.AppLogic.PageFactory
open Nikeza.Mobile.UILogic.Pages

module ProfileEditor =

    let navigate' = function
        | PageRequested.EditProfile profile -> Application.Current |> navigate (profilePage profile) profile
        | _ -> ()
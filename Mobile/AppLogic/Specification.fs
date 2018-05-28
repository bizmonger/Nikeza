namespace Nikeza.Mobile.AppLogic.Specification

open Xamarin.Forms
open Nikeza.Access.Specification
open Nikeza.Mobile.UILogic
open Nikeza.Mobile.UILogic.Portal

module Access =

    module Login =
        type AddSideEffects = Application -> Login.SideEffects -> Login.SideEffects

    module Registration = 
        type AddSideEffects = Application -> Registration.SideEffects  -> Registration.SideEffects


module Portal =
    type AddSideEffects = Application -> Portal.SideEffects -> Portal.SideEffects
        

module ProfileEditor =

    module Save =
        type AddSideEffects =  Application -> ProfileEditor.SideEffects -> ProfileEditor.SideEffects
 
    module QueryFailed =
        type AddSideEffects =  Application -> ProfileEditor.SideEffects -> ProfileEditor.SideEffects
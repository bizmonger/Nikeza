namespace Nikeza.Mobile.AppLogic.Specification

open Xamarin.Forms
open Nikeza.Access.Specification
open Nikeza.Mobile.UILogic
open Nikeza.Mobile.UILogic.Portal

module Access =

    module Login =
        type AddSideEffectsFn = Application -> Login.SideEffects -> Login.SideEffects

    module Registration = 
        type AddSideEffectsFn = Application -> Registration.SideEffects  -> Registration.SideEffects


module Portal =
    type AddSideEffectsFn = Application -> Portal.SideEffects -> Portal.SideEffects
        

module ProfileEditor =

    module Save =
        type AddSideEffectsFn =  Application -> ProfileEditor.SideEffects -> ProfileEditor.SideEffects
 
    module QueryFailed =
        type AddSideEffectsFn =  Application -> ProfileEditor.SideEffects -> ProfileEditor.SideEffects
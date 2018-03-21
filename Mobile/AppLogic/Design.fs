namespace Nikeza.Mobile.AppLogic

module Design =

    open Xamarin.Forms
    open Nikeza.Mobile.UILogic
    open Nikeza.Mobile.UILogic.Portal

    type ``Side effects from login`` =        Application -> Login.SideEffects         -> Login.SideEffects
    type ``side effects from registration`` = Application -> Registration.SideEffects  -> Registration.SideEffects

    type ``side effects from save`` =  ProfileEditor.SideEffects -> ProfileEditor.SideEffects
    type ``side effects from query`` = ProfileEditor.SideEffects -> ProfileEditor.SideEffects
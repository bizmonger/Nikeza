namespace Nikeza.Mobile.AppLogic

module Design =

    open Xamarin.Forms
    open Nikeza.Mobile.UILogic

    type ``Side effects from login`` =        Application -> Login.SideEffects        -> Login.SideEffects
    type ``side effects from registration`` = Application -> Registration.SideEffects -> Registration.SideEffects
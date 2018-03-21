namespace Nikeza.Mobile.AppLogic

module Design =

    open Xamarin.Forms
    open Nikeza.Mobile.UILogic

    type ``login side effects`` =        Application -> Login.SideEffects        -> Login.SideEffects
    type ``registration side effects`` = Application -> Registration.SideEffects -> Registration.SideEffects
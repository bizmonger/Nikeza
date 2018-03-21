namespace Nikeza.Mobile.AppLogic

module Design =

    open Xamarin.Forms
    open Nikeza.Mobile.UILogic
    open Nikeza.Mobile.UILogic.Portal

    module Access =

        module Login =
            type SideEffects = Application -> Login.SideEffects -> Login.SideEffects

        module Registration = 
            type SideEffects = Application -> Registration.SideEffects  -> Registration.SideEffects


    module ProfileEditor =

        module Save =
            type SideEffects =  ProfileEditor.SideEffects -> ProfileEditor.SideEffects
 
        module QueryFailed =
            type SideEffects =  ProfileEditor.SideEffects -> ProfileEditor.SideEffects
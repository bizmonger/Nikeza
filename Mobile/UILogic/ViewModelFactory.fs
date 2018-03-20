module Nikeza.Mobile.UILogic.ViewModelFactory


module Registration = 

    open Nikeza.Mobile.Access
    open Nikeza.Mobile.Access.Commands
    open Nikeza.Mobile.UILogic.Registration

    let getViewModel : Nikeza.Mobile.UILogic.Registration.ViewModel =

        let implementation = { Submit= Try.submit }
        let sideEffects =    { ForRegistrationSubmission=[] }

        let dependencies = { Implementation=implementation
                             SideEffects=sideEffects 
        }

        Registration.ViewModel(dependencies)
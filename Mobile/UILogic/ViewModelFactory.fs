module Nikeza.Mobile.UILogic.ViewModelFactory


module Registration = 

    open Nikeza.Mobile.Access
    open Nikeza.Mobile.Access.Commands
    open Nikeza.Mobile.UILogic.Registration

    let getViewModel : Nikeza.Mobile.UILogic.Registration.ViewModel =

        let actions =    { Submit= Try.submit }
        let responders = { ForRegistrationSubmission=[] }

        let dependencies = { Implementation=actions; SideEffects=responders }

        Registration.ViewModel(dependencies)
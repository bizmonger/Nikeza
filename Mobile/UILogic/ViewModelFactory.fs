module Nikeza.Mobile.UILogic.ViewModelFactory


module Registration = 

    open Nikeza.Mobile.Profile.Commands
    open Nikeza.Mobile.Profile
    open Nikeza.Mobile.UILogic.Registration

    let getViewModel : Nikeza.Mobile.UILogic.Registration.ViewModel =

        let actions = { Submit= Try.submit }
        let responders = { ForRegistrationSubmission=[] }

        let dependencies = { Implementation=actions; SideEffects=responders }

        Registration.ViewModel(dependencies)
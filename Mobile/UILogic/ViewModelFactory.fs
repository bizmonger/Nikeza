module Nikeza.Mobile.UILogic.ViewModelFactory


module Registration = 

    open Nikeza.Mobile.Profile.Commands
    open Nikeza.Mobile.Profile
    open Nikeza.Mobile.Account
    open Nikeza.Mobile.Account.Registration

    let getViewModel : Registration.ViewModel =

        let actions = { Submit= Try.submit }
        let responders = { ForRegistrationSubmission=[] }

        let dependencies = { Actions=actions; Observers=responders }

        Registration.ViewModel(dependencies)
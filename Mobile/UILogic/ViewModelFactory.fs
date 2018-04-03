module Nikeza.Mobile.UILogic.ViewModelFactory

module Registration = 

    open Nikeza.Common
    open Nikeza.Mobile.Access
    open Nikeza.Mobile.Access.Commands
    open Nikeza.Mobile.Access.Events
    open Nikeza.Mobile.UILogic.Registration
    open System.Diagnostics

    let getViewModel : Registration.ViewModel =

        let log = function
            | RegistrationSucceeded p    -> Debug.WriteLine(sprintf "Registration succeeded for %A" p)
            | RegistrationFailed    form -> Debug.WriteLine(sprintf "Registration Failed for %A" form)

        let implementation = { Submit= Try.submit }
        let sideEffects =    { ForRegistrationSubmission= log::[] }

        let dependencies = { Implementation=implementation
                             SideEffects=sideEffects 
        }

        Registration.ViewModel(dependencies)


module Profile =

    module Portal = 

        open Nikeza.Mobile.UILogic.Portal

        let getViewModel user : Portal.ViewModel =

            let sideEffects = { ForPageRequested= []
                                ForQueryFailed=   []
            }

            let dependencies = { User=        user
                                 Query=     { Subscriptions= TestAPI.mockSubscriptions }
                                 SideEffects= sideEffects 
            }

            Portal.ViewModel(dependencies)
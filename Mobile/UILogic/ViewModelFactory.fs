module Nikeza.Mobile.UILogic.ViewModelFactory

module Registration = 

    open Nikeza.Mobile.Access
    open Nikeza.Access.Specification.Events
    open Nikeza.Access.Specification.Registration
    open System.Diagnostics

    let getViewModel : Registration.ViewModel =

        let log = function
            | RegistrationSucceeded p    -> Debug.WriteLine(sprintf "Registration succeeded for %A" p)
            | RegistrationFailed    form -> Debug.WriteLine(sprintf "Registration Failed for %A" form)

        let implementation = { Submit= Attempt.submit }
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
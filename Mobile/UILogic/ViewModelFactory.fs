module Nikeza.Mobile.UILogic.ViewModelFactory

module Registration = 

    open Nikeza.Mobile.Access
    open Nikeza.Mobile.Access.Commands
    open Nikeza.Mobile.UILogic.Registration

    let getViewModel : Registration.ViewModel =

        let implementation = { Submit= Try.submit }
        let sideEffects =    { ForRegistrationSubmission=[] }

        let dependencies = { Implementation=implementation
                             SideEffects=sideEffects 
        }

        Registration.ViewModel(dependencies)


module Profile =

    module Portal = 

        open Nikeza.Mobile.UILogic.Portal

        let getViewModel userId : Portal.ViewModel =

            let sideEffects = { ForPageRequested=[]
                                ForQueryFailed= []
            }

            let dependencies = { UserId=      userId
                                 Query=     { Subscriptions= TestAPI.mockSubscriptions }
                                 SideEffects= sideEffects 
            }

            Portal.ViewModel(dependencies)
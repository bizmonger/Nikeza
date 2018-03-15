module Nikeza.Mobile.UILogic.DependencyFactory


module Login =

    open Nikeza.Mobile.UILogic.Login

    let dependencies =

        let sideEffects = { ForLoginAttempt= [] }
    
        { SideEffects= sideEffects }
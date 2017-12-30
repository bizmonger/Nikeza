module internal Handle.Registration

open Commands
open Events
open Nikeza.Common

type private Registration = ResultOf.Registration -> RegistrationEvent list

let result : Registration =
    fun resultOf -> resultOf |> function
        | ResultOf.Registration.Submit   result -> 
                                            result |> function
                                                    | Ok    profile -> [ RegistrationSucceeded    profile
                                                                         LoginRequested        <| ProfileId profile.Id ]
                                                    | Error form    -> [ RegistrationFailed       form ]

        | ResultOf.Registration.Validate result -> 
                                            result |> function
                                                    | Ok    form -> [FormValidated    form]
                                                    | Error form -> [FormNotValidated form]


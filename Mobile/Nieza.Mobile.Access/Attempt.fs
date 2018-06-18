namespace Nikeza.Mobile.Access

open Nikeza.Access.Specification.Attempts
open Nikeza.Mobile.Access.Attempt

module ValidatedForm =

    let attempt : SubmitAttempt =
        fun form -> form |> submit


module Login =

    let attempt : LoginAttempt =
        fun credentials -> credentials |> login


module Logout =
    
    let attempt : LogoutAttempt =
        fun provider -> provider |> logout
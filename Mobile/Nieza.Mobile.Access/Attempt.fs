namespace Nikeza.Mobile.Access

open Nikeza.Access.Specification.Attempt
open Nikeza.Mobile.Access.Attempt

module ValidatedForm =

    let attempt : Submit =
        fun form -> form |> submit


module Login =

    let attempt : Login =
        fun credentials -> credentials |> login


module Logout =
    
    let attempt : Logout =
        fun provider -> provider |> logout
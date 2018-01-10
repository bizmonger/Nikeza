module Nikeza.Mobile.Portfolio.Query

open Nikeza.Mobile.Portfolio.Events
open Nikeza.Common

type PortfolioFn = ProviderId -> Query

let portfolio : PortfolioFn =
    fun providerId -> Failed providerId
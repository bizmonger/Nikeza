module Nikeza.Mobile.Portfolio.Query

open Nikeza.Mobile.Portfolio.Events
open Nikeza.Common

type PortfolioFn = ProviderId -> GetPortfolioEvent

let portfolio : PortfolioFn =
    fun providerId -> GetPortfolioFailed providerId
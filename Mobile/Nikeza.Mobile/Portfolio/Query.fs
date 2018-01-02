module Nikeza.Mobile.Portfolio.Query

open Nikeza.Mobile.Portfolio.Events
open Nikeza.Common

type TryGetPortfolio = ProviderId -> QueryEvent list

let portfolio : TryGetPortfolio =
    fun providerId -> [GetPortfolioFailed providerId]
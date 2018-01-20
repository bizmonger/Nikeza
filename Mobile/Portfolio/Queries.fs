module Nikeza.Mobile.Portfolio.Query

open Nikeza.Common

type PortfolioFn = ProviderId -> Result<ProviderRequest,ProviderId>

let portfolio : PortfolioFn =
    fun providerId -> Error providerId
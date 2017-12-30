module internal Query

open Nikeza.Common
open Nikeza.DataTransfer

type private TryGetPortfolio = ProviderId -> Result<Provider, ProviderId>

let portfolio : TryGetPortfolio =
    fun providerId -> Error providerId
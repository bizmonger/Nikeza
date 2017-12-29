module internal Portfolio

open Events
open Queries
open Queries.ResultOf

type private Handle = ResultOf.Portfolio -> PortfolioEvent list

let handle : Handle =
    fun response ->
        response |> function 
                    Portfolio.Query result -> 
                                    result |> function
                                              | Ok    provider   -> [PortfolioReturned provider  ]
                                              | Error providerId -> [PortfolioFailed   providerId]
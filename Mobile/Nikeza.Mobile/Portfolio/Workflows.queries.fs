module Workflows.Queries

open Events
open Queries

type private PortfolioWorkflow = Query -> PortfolioEvent list

let handlePortfolio : PortfolioWorkflow = fun query -> query |> function
    Query.Portfolio providerId ->
                    providerId |> Query.portfolio
                               |> ResultOf.Portfolio.Query
                               |> Portfolio.Result.handle
module Queries

open Nikeza.Common
open Nikeza.DataTransfer

type Query = Portfolio of ProviderId

module ResultOf =
    type Portfolio = Query of Result<Provider, ProviderId>
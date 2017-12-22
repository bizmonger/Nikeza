module Commands

open Nikeza.Common
open Nikeza.DataTransfer

type Command =
    | Subscribe   of Result<SubscriptionResponse, ProfileId>
    | Unsubscribe of Result<SubscriptionResponse, ProfileId>
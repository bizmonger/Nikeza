module Commands

open Nikeza.Common

type Command =
    | Follow      of FollowRequest
    | Unsubscribe of UnsubscribeRequest
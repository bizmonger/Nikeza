module Workflows

open Commands
open Try
open Events

type private Workflow = Command -> NotificationEvent list

let handle : Workflow = 
    fun command -> command |> function
    | Command.Follow      request -> 
                          request |> Try.follow 
                                  |> ResultOf.Follow 
                                  |> Are.Subscriptions.events
    | Command.Unsubscribe request -> 
                          request |> Try.unsubscribe 
                                  |> ResultOf.Unsubscribe  
                                  |> Are.Subscriptions.events
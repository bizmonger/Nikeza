module Workflows

open Commands
open Try
open Logic
open Events

type private Workflow = Command -> NotificationEvent list

let handle : Workflow = 
    fun command -> command |> function
    | Command.Follow      request -> 
                          request |> Try.follow 
                                  |> ResultOf.Follow 
                                  |> Handle.result
    | Command.Unsubscribe request -> 
                          request |> Try.unsubscribe 
                                  |> ResultOf.Unsubscribe  
                                  |> Handle.result
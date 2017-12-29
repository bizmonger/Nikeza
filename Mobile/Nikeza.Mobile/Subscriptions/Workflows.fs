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
                                  |> Result.handle
    | Command.Unsubscribe request -> 
                          request |> Try.unsubscribe 
                                  |> ResultOf.Unsubscribe  
                                  |> Result.handle
module Workflows

open Commands
open IO
open Logic
open Events

type private Workflow = Command -> NotificationEvent list

let handle : Workflow = 
    fun command -> command |> function
    | Command.Follow      request -> request 
                                      |> tryFollow 
                                      |> ResultOf.Follow 
                                      |> handle

    | Command.Unsubscribe request -> request 
                                      |> tryUnsubscribe 
                                      |> ResultOf.Unsubscribe  
                                      |> handle
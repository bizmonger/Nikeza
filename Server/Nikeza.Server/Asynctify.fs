module Asynctify
let toResult task = task |> Async.AwaitTask |> Async.RunSynchronously
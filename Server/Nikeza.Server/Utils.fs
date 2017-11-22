module Asynctify
let toResult task = task |> Async.AwaitTask |> Async.RunSynchronously

let replaceHtmlCodes (text:string) = text//.Replace("&#39","'")
                                         //.Replace("&#8217", "'")
                                         //.Replace("&#8230", "...")
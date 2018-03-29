module Nikeza.Mobile.AppLogic.Navigation

open Xamarin.Forms
open System.Diagnostics

let navigate page context (app:Application) =

    try 
        let expression = sprintf "\n\n** Request: Navigate to %s\n\n** Payload as follows:\n\n %A"
        Debug.WriteLine <| expression (page.ToString()) context

        app.MainPage <- page

    with ex -> Debug.WriteLine(ex.Message); raise ex
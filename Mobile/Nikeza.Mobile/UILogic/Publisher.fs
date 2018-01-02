module Nikeza.Mobile.UILogic.Publisher

let publishEvents (eventOccurred:Event<_>) events =
    events |> List.iter(fun event -> eventOccurred.Trigger(event))
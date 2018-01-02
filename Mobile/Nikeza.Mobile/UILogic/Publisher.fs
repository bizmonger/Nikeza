module Nikeza.Mobile.UILogic.Publisher

let publish (eventOccurred:Event<_>) events =
    events |> List.iter(fun event -> eventOccurred.Trigger(event))
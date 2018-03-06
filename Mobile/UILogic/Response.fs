namespace Nikeza.Mobile.UILogic

module Response =

    let handle event observers= 
        observers|> List.iter(fun handle -> handle event)
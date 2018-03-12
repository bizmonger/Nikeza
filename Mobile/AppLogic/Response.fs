namespace Nikeza.Mobile.AppLogic

module Response =

    let handle event observers= 
        observers|> List.iter(fun handle -> handle event)
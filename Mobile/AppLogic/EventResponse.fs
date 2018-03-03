namespace Nikeza.Mobile.AppLogic

module Response =
    open Nikeza.DataTransfer
    open System.Diagnostics

    type NavigationRequest =
        | NavigateToPortal    of Profile
        | NavigateToPortfolio of Profile

    let navigate = function
        | NavigateToPortal    _ -> Debug.WriteLine(NavigateToPortal.ToString())
        | NavigateToPortfolio _ -> Debug.WriteLine(NavigateToPortfolio.ToString())
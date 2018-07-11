namespace Nikeza.Mobile.AppLogic

open Nikeza.Mobile.AppLogic.Specification
open Nikeza.Mobile.AppLogic.Navigation
open Nikeza.Mobile.UILogic.Portal
open Nikeza.Mobile.UILogic.Pages
open PageFactory

module PortalEvents =

    let appendNavigation : Portal.AddSideEffects =

        fun app sideEffects ->
    
            let handle = function
                | PageRequested.Latest        user -> app |> navigate (latestPage        user) user
                | PageRequested.Followers     user -> app |> navigate (followersPage     user) user
                | PageRequested.Subscriptions user -> app |> navigate (subscriptionsPage user) user
                | PageRequested.Members            -> app |> navigate membersPage ()
                | PageRequested.Portfolio     _    -> app |> navigate membersPage ()

                | _ -> app |> navigate errorPage ()

            let handlers = handle::sideEffects.ForPageRequested

            { sideEffects with SideEffects.ForPageRequested= handlers }

module Portal =

    open Xamarin.Forms
    open Nikeza.Mobile.UILogic
    open PortalEvents

    let dependencies user =

        let sideEffects = 

            { ForPageRequested= []
              ForQueryFailed =  [] 

            } |> appendNavigation Application.Current
    
        { User=        user
          SideEffects= sideEffects; 
          Query=     { Subscriptions= TestAPI.mockRecent }  
        }
namespace Nikeza.Mobile.AppLogic

open Nikeza.Mobile.AppLogic.Navigation
open Nikeza.Mobile.AppLogic.Design
open PageFactory

module PortalEvents =
    open Nikeza.Mobile.UILogic.Pages
    open Nikeza.Mobile.UILogic.Portal

    let appendNavigation : Portal.SideEffects =

        fun app sideEffects ->
    
            let handle = function
                | PageRequested.Latest        userId -> app |> navigate latestPage        userId
                | PageRequested.Followers     userId -> app |> navigate followersPage     userId
                | PageRequested.Subscriptions userId -> app |> navigate subscriptionsPage userId
                | PageRequested.Members              -> app |> navigate membersPage       ()

                | _ -> app |> navigate errorPage ()

            let handlers = handle::sideEffects.ForPageRequested

            { sideEffects with SideEffects.ForPageRequested= handlers }

module Portal =

    open PortalEvents
    open Xamarin.Forms
    open Nikeza.Mobile.UILogic.Portal
    open Nikeza.Mobile.UILogic
    open Nikeza.Common

    let dependencies =

        let sideEffects = 

            { ForPageRequested= []
              ForQueryFailed =  [] 

            } |> appendNavigation Application.Current
    
        { UserId= ProfileId TestAPI.someProvider.Profile.Id
          SideEffects= sideEffects; 
          Query=     { Subscriptions= TestAPI.mockRecent }  
        }
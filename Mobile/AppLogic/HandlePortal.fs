namespace Nikeza.Mobile.AppLogic

open Nikeza.Mobile.AppLogic.Navigation
open Nikeza.Mobile.AppLogic.Design
open Nikeza.Mobile.UILogic.Portal
open Nikeza.Mobile.UILogic.Pages
open PageFactory

module PortalEvents =

    let appendNavigation : Portal.SideEffects =

        fun app sideEffects ->
    
            let handle = function
                | PageRequested.Latest        userId -> app |> navigate (latestPage        userId) userId
                | PageRequested.Followers     userId -> app |> navigate (followersPage     userId) userId
                | PageRequested.Subscriptions userId -> app |> navigate (subscriptionsPage userId) userId
                | PageRequested.Members              -> app |> navigate membersPage ()

                | _ -> app |> navigate errorPage ()

            let handlers = handle::sideEffects.ForPageRequested

            { sideEffects with SideEffects.ForPageRequested= handlers }

module Portal =

    open Xamarin.Forms
    open Nikeza.Mobile.UILogic
    open Nikeza.Common
    open PortalEvents

    let dependencies =

        let sideEffects = 

            { ForPageRequested= []
              ForQueryFailed =  [] 

            } |> appendNavigation Application.Current
    
        { UserId=      ProfileId TestAPI.someProvider.Profile.Id
          SideEffects= sideEffects; 
          Query=     { Subscriptions= TestAPI.mockRecent }  
        }
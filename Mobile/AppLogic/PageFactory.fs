module Nikeza.Mobile.AppLogic.PageFactory

open System.Diagnostics
open Nikeza.Mobile.UI
open Nikeza.Mobile.UILogic.ViewModelFactory.Profile
open Xamarin.Forms
open Nikeza.Common

let configurePage (page:ContentPage) viewmodel =
    page.BindingContext <- viewmodel; page

let membersPage : Page = 
    let mutable page:Page = null

    try page <- new ErrorPage()
    with ex -> Debug.WriteLine(ex.Message)
    page

let subscriptionsPage userId : Page = 
    let mutable page:Page = null

    try page <- new ErrorPage()
    with ex -> Debug.WriteLine(ex.Message)
    page

let followersPage userId : Page = 
    let mutable page:Page = null

    try page <- new ErrorPage()
    with ex -> Debug.WriteLine(ex.Message)
    page

let latestPage userId : Page =

    let mutable page:Page = null

    let viewmodel = Portal.getViewModel userId

    try page <- configurePage (new LatestPage()) viewmodel
    with ex -> Debug.WriteLine(ex.Message)
    page

let portalPage userId : Page =

    let mutable page:Page = null

    let viewmodel = Portal.getViewModel userId

    try page <- configurePage (new PortalPage()) viewmodel
    with ex -> Debug.WriteLine(ex.Message)
    page

let errorPage : Page = 
    let mutable page:Page = null

    try page <- new ErrorPage()
    with ex -> Debug.WriteLine(ex.Message)
    page
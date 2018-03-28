module Nikeza.Mobile.AppLogic.PageFactory

open System.Diagnostics
open Nikeza.Mobile.UI
open Nikeza.Mobile.UILogic.ViewModelFactory.Profile
open Xamarin.Forms

let configurePage (page:ContentPage) viewmodel =
    page.BindingContext <- viewmodel; page

let membersPage : Page = 
    let mutable page:Page = null

    try page <- new ErrorPage()
    with ex -> Debug.WriteLine(ex.Message)
    page

let subscriptionsPage : Page = 
    let mutable page:Page = null

    try page <- new ErrorPage()
    with ex -> Debug.WriteLine(ex.Message)
    page

let followersPage : Page = 
    let mutable page:Page = null

    try page <- new ErrorPage()
    with ex -> Debug.WriteLine(ex.Message)
    page

let latestPage : Page = 
    let mutable page:Page = null

    try page <- new ErrorPage()
    with ex -> Debug.WriteLine(ex.Message)
    page

let portalPage : Page =

    let mutable page:Page = null

    try page <- configurePage (new PortalPage()) Portal.getViewModel
    with ex -> Debug.WriteLine(ex.Message)
    page

let errorPage : Page = 
    let mutable page:Page = null

    try page <- new ErrorPage()
    with ex -> Debug.WriteLine(ex.Message)
    page
module Nikeza.Mobile.AppLogic.PageFactory

open System.Diagnostics
open Nikeza.Mobile.UI
open Nikeza.Mobile.UILogic.ViewModelFactory.Profile
open Xamarin.Forms

let configurePage (page:ContentPage) viewmodel =
    page.BindingContext <- viewmodel; page

let errorPage : Page = 
    let mutable page:Page = null

    try page <- new ErrorPage()
    with ex -> Debug.WriteLine(ex.Message)
    page

let membersPage : Page = 
    let mutable page:Page = null

    try page <- new ErrorPage()
    with ex -> Debug.WriteLine(ex.Message)
    page

let subscriptionsPage user : Page = 
    let mutable page:Page = null

    try page <- new ErrorPage()
    with ex -> Debug.WriteLine(ex.Message)
    page

let followersPage user : Page = 
    let mutable page:Page = null

    try page <- new ErrorPage()
    with ex -> Debug.WriteLine(ex.Message)
    page

let latestPage user : Page =

    let mutable page:Page = null

    let viewmodel = Portal.getViewModel user

    try page <- configurePage (new LatestPage()) viewmodel
    with ex -> Debug.WriteLine(ex.Message)
    page

let portalPage user : Page =

    let mutable page:Page = null

    let viewmodel = Portal.getViewModel user
    viewmodel.Init()

    try page <- configurePage (new PortalPage()) viewmodel
    with ex -> Debug.WriteLine(ex.Message)
    page


let portfolioPage : Page = 
    let mutable page:Page = null

    try page <- new PortfolioPage()
    with ex -> Debug.WriteLine(ex.Message)
    page
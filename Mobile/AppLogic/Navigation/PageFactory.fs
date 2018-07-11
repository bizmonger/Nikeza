module Nikeza.Mobile.AppLogic.PageFactory

open System.Diagnostics
open Xamarin.Forms
open Nikeza.Mobile.UI
open Nikeza.Mobile.UILogic.ViewModelFactory.Profile
open Nikeza.Mobile.AppLogic.Specification
open Nikeza.Mobile.UI.Profile
open Nikeza.Mobile.UILogic.ViewModelFactory

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

let profilePage profile : Page =

    let mutable page:Page = null

    let viewmodel = Profile.Editor.getViewModel profile
    viewmodel.Init()

    try page <- configurePage (new ProfilePage()) viewmodel
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
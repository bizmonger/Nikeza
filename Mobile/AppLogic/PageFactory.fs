module Nikeza.Mobile.AppLogic.PageFactory

open Nikeza.Mobile.UILogic.ViewModelFactory.Profile
open Xamarin.Forms
open Nikeza.Mobile.UI

let configurePage (page:ContentPage) viewmodel =
    page.BindingContext <- viewmodel; page

let portalPage = 
    configurePage (new PortalPage()) Portal.getViewModel

let errorPage = new ErrorPage()
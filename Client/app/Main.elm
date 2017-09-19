import Html exposing (..)
import Html.Attributes exposing (..)
import Html.Events exposing (onClick)
import Navigation exposing (Location)
import UrlParser exposing ((</>))
import Bootstrap.Navbar as Navbar
import Bootstrap.Grid as Grid
import Bootstrap.Grid.Col as Col
import Bootstrap.Card as Card
import Bootstrap.Button as Button
import Bootstrap.ListGroup as Listgroup
import Bootstrap.Modal as Modal


main : Program Never Model Msg
main =
    Navigation.program UrlChange
        { view = view
        , update = update
        , subscriptions = subscriptions
        , init = init
        }


type alias Model =
    { page : Page
    , navState : Navbar.State
    , modalState : Modal.State
    }


type Page
    = Home
    | GettingStarted
    | Modules
    | NotFound


init : Location -> ( Model, Cmd Msg )
init location =
    let
        ( navState, navCmd ) =
            Navbar.initialState NavMsg

        ( model, urlCmd ) =
            urlUpdate location { navState = navState, page = Home, modalState = Modal.hiddenState }
    in
        ( model, Cmd.batch [ urlCmd, navCmd ] )


type Msg
    = UrlChange Location
    | NavMsg Navbar.State
    | ModalMsg Modal.State


subscriptions : Model -> Sub Msg
subscriptions model =
    Navbar.subscriptions model.navState NavMsg


update : Msg -> Model -> ( Model, Cmd Msg )
update msg model =
    case msg of
        UrlChange location ->
            urlUpdate location model

        NavMsg state ->
            ( { model | navState = state }
            , Cmd.none
            )

        ModalMsg state ->
            ( { model | modalState = state }
            , Cmd.none
            )


urlUpdate : Navigation.Location -> Model -> ( Model, Cmd Msg )
urlUpdate location model =
    case decode location of
        Nothing ->
            ( { model | page = NotFound }, Cmd.none )

        Just route ->
            ( { model | page = route }, Cmd.none )


decode : Location -> Maybe Page
decode location =
    UrlParser.parseHash routeParser location


routeParser : UrlParser.Parser (Page -> a) a
routeParser =
    UrlParser.oneOf
        [ UrlParser.map Home UrlParser.top
        , UrlParser.map GettingStarted (UrlParser.s "getting-started")
        , UrlParser.map Modules (UrlParser.s "modules")
        ]


view : Model -> Html Msg
view model =
    div []
        [ menu model
        , mainContent model
        , modal model
        ]


menu : Model -> Html Msg
menu model =
    Navbar.config NavMsg
        |> Navbar.withAnimation
        |> Navbar.container
        |> Navbar.brand [ href "#" ] [ text "Elm Bootstrap" ]
        |> Navbar.items
            [ Navbar.itemLink [ href "#getting-started" ] [ text "Getting started" ]
            , Navbar.itemLink [ href "#modules" ] [ text "Modules" ]
            ]
        |> Navbar.view model.navState


mainContent : Model -> Html Msg
mainContent model =
    Grid.container [] <|
        case model.page of
            Home ->
                pageHome model

            GettingStarted ->
                pageGettingStarted model

            Modules ->
                pageModules model

            NotFound ->
                pageNotFound


pageHome : Model -> List (Html Msg)
pageHome model =
    [ h1 [] [ text "Home" ]
    , Grid.row []
        [ Grid.col []
            [ Card.config [ Card.outlinePrimary ]
                |> Card.headerH4 [] [ text "Getting started" ]
                |> Card.block []
                    [ Card.text [] [ text "Getting started is real easy. Just click the start button." ]
                    , Card.custom <|
                        Button.linkButton
                            [ Button.primary, Button.attrs [ href "#getting-started" ] ]
                            [ text "Start" ]
                    ]
                |> Card.view
            ]
        , Grid.col []
            [ Card.config [ Card.outlineDanger ]
                |> Card.headerH4 [] [ text "Modules" ]
                |> Card.block []
                    [ Card.text [] [ text "Check out the modules overview" ]
                    , Card.custom <|
                        Button.linkButton
                            [ Button.primary, Button.attrs [ href "#modules" ] ]
                            [ text "Module" ]
                    ]
                |> Card.view
            ]
        ]
    ]


pageGettingStarted : Model -> List (Html Msg)
pageGettingStarted model =
    [ h2 [] [ text "Getting started" ]
    , Button.button
        [ Button.success
        , Button.large
        , Button.block
        , Button.attrs [ onClick <| ModalMsg Modal.visibleState ]
        ]
        [ text "Click me" ]
    ]


pageModules : Model -> List (Html Msg)
pageModules model =
    [ h1 [] [ text "Modules" ]
    , Listgroup.ul
        [ Listgroup.li [] [ text "Alert" ]
        , Listgroup.li [] [ text "Badge" ]
        , Listgroup.li [] [ text "Card" ]
        ]
    ]


pageNotFound : List (Html Msg)
pageNotFound =
    [ h1 [] [ text "Not found" ]
    , text "SOrry couldn't find that page"
    ]


modal : Model -> Html Msg
modal model =
    Modal.config ModalMsg
        |> Modal.small
        |> Modal.h4 [] [ text "Getting started ?" ]
        |> Modal.body []
            [ Grid.containerFluid []
                [ Grid.row []
                    [ Grid.col
                        [ Col.xs6 ]
                        [ text "Col 1" ]
                    , Grid.col
                        [ Col.xs6 ]
                        [ text "Col 2" ]
                    ]
                ]
            ]
        |> Modal.view model.modalState

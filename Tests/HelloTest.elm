module HelloTest exposing (..)

import Controls.Login as Login exposing (Model)
import Home exposing (..)
import Test exposing (..)
import Expect


suite : Test
suite =
    describe "The Login module"
        [ test "runtime.tryLogin succeeds with valid credentials" <|
            \_ ->
                let
                    ( login, runtime ) =
                        ( Login.Model "test" "test" False, Home.runtime )

                    result =
                        runtime.tryLogin login
                in
                    Expect.equal result.loggedIn True
        , test "runtime.tryLogin fails with invalid credentials" <|
            \_ ->
                let
                    ( login, runtime ) =
                        ( Login.Model "test" "invalid_password" False, Home.runtime )

                    result =
                        runtime.tryLogin login
                in
                    Expect.equal result.loggedIn False
        ]

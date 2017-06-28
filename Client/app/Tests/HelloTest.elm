module HelloTest exposing (..)

import Controls.Login as Login exposing (Model)
import Settings exposing (..)
import Test exposing (..)
import Expect


suite : Test
suite =
    describe "Login module"
        [ test "runtime.tryLogin succeeds with valid credentials" <|
            \_ ->
                let
                    login =
                        Login.Model "test" "test" False

                    result =
                        runtime.tryLogin login
                in
                    Expect.equal result.loggedIn True
        , test "runtime.tryLogin fails with invalid credentials" <|
            \_ ->
                let
                    login =
                        Login.Model "test" "invalid_password" False

                    result =
                        runtime.tryLogin login
                in
                    Expect.equal result.loggedIn False
        ]

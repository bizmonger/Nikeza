module HelloTest exposing (..)

import Test exposing (..)
import Expect


suite : Test
suite =
    describe "The String module"
        [ describe "String.reverse"
            -- Nest as many descriptions as you like.
            [ test "has no effect on a palindrome" <|
                \_ ->
                    let
                        palindrome =
                            "hannah"
                    in
                        Expect.equal palindrome (String.reverse palindrome)
            ]
        ]

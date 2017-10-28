module Nikeza.Server.StackOverflow
// Get most popular tags (100 / page)
// https://api.stackexchange.com/docs/tags#page=5&pagesize=100&order=desc&sort=popular&filter=!-.G.68grSaJm&site=stackoverflow&run=true

// Answers
// https://api.stackexchange.com/2.2/users/492701/answers?order=desc&sort=activity&site=stackoverflow&filter=!Fcazzsr2b3M)LbUjGAu-Fs0Wf8

let getTags pages = []
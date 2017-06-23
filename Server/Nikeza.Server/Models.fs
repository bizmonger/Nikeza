namespace Nikeza.Server.Models

[<CLIMutable>]
type Message =
    {
        Text : string
    }

[<CLIMutable>]
type YoutubeView =
    {
        ApiKey : string
        ChannelId : string
    }
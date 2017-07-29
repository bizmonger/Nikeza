module Nikeza.Wordpress.Rss.Data

    open System.IO
    open System.Text
    open System.Xml
    open System.Xml.Serialization   

    [<CLIMutable>]
    type RssItem = {
        title:string; 
        link:string;
        description:string;
        author:string;
        category:string option;
        enclosure: string option;
        guid: string option;
        pubDate: string option;
        source: string option;
      }

    [<CLIMutable>]
    type RssChannel = {
        title:string;
        link:string;
        description:string;
        language: string option;
        copyright: string option;
        managingEditor: string option;
        webMaster: string option;
        pubDate: string option;
        lastBuildDate: string option;
        category:string option;
        generator:string option;
        docs: string option;
        cloud: string option;
        ttl: string option;
        image: string option;
        textInput: string option;
        skipHours: string option;
        skipDays: string option;
        // needed otherwise items
        // are not added to array
        [<XmlElement>]
        item:  RssItem[];
        }

    [<CLIMutable>]        
    type Rss = {channel: RssChannel}
   
    let deserializeXml content =
        let root = XmlRootAttribute("rss")
        let serializer = XmlSerializer(typeof<Rss>,root)
        use reader = new StringReader(content)
        serializer.Deserialize reader :?> Rss
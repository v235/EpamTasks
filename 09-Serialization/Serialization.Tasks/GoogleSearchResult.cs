
using System.Runtime.Remoting.Contexts;
using System.Runtime.Serialization;

namespace Serialization.Tasks
{

    // TODO: Implement GoogleSearchResult class to be deserialized from Google Search API response
    // Specification is available at: https://developers.google.com/custom-search/v1/using_rest#WorkingResults
    // The test json file is at Serialization.Tests\Resources\GoogleSearchJson.txt


    [DataContract]
    public class GoogleSearchResult
    {
        [DataMember(Name = "kind")] public string Kind;
        [DataMember(Name = "url")] public Url Url;
        [DataMember(Name = "queries")] public Queries Queries;
        [DataMember(Name = "context")] public Context Context;
        [DataMember(Name = "items")] public Item[] Items;
    }

    [DataContract]
    public class Url
    {
        [DataMember(Name = "type")] public string Type;
        [DataMember(Name = "template")] public string Template;
    }

    [DataContract]
    public class Page
    {
        [DataMember(Name = "title")] public string Title;
        [DataMember(Name = "totalResults")] public long TotalResults;
        [DataMember(Name = "searchTerms")] public string SearchTerms;
        [DataMember(Name = "count")] public int Count;
        [DataMember(Name = "startIndex")] public int StartIndex;
        [DataMember(Name = "inputEncoding")] public string InputEncoding;
        [DataMember(Name = "outputEncoding")] public string OutputEncoding;
        [DataMember(Name = "cx")] public string Cx;
    }

    [DataContract]
    public class Queries
    {
        [DataMember(Name = "previousPage")] public Page[] PreviousPage;
        [DataMember(Name = "nextPage")] public Page[] NextPage;
        [DataMember(Name = "request")] public Page[] Request;
    }

    [DataContract]
    public class Context
    {
        [DataMember(Name = "title")] public string Title;
    }

    [DataContract]
    public class Item
    {
        [DataMember(Name = "kind")] public string Kind;
        [DataMember(Name = "title")] public string Title;
        [DataMember(Name = "htmlTitle")] public string HtmlTitle;
        [DataMember(Name = "link")] public string Link;
        [DataMember(Name = "displayLink")] public string DisplayLink;
        [DataMember(Name = "snippet")] public string Snippet;
        [DataMember(Name = "htmlSnippet")] public string HtmlSnippet;
    }

}




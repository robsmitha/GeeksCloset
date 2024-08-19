
export interface ClaimsIdentity {
    clientPrincipal: ClientPrincipal;
}

export interface ClientPrincipal {
    userId:           string;
    userRoles:        string[];
    claims:           any[];
    identityProvider: string;
    userDetails:      string;
}

export interface WpPage {
    id:             number;
    date:           Date;
    date_gmt:       Date;
    guid:           GUID;
    modified:       Date;
    modified_gmt:   Date;
    slug:           string;
    status:         string;
    type:           string;
    link:           string;
    title:          Title;
    content:        Content;
    excerpt:        Content;
    author:         number;
    featured_media: number;
    parent:         number;
    menu_order:     number;
    comment_status: string;
    ping_status:    string;
    template:       string;
    meta:           Meta;
    _links:         WpPageLinks;
}

export interface WpPageLinks {
    self:                  About[];
    collection:            About[];
    about:                 About[];
    author:                Author[];
    replies:               Author[];
    "version-history":     VersionHistory[];
    "predecessor-version": PredecessorVersion[];
    "wp:attachment":       About[];
    curies:                Cury[];
}

export interface WpPost {
    id:             number;
    date:           Date;
    date_gmt:       Date;
    guid:           GUID;
    modified:       Date;
    modified_gmt:   Date;
    slug:           string;
    status:         string;
    type:           string;
    link:           string;
    title:          GUID;
    content:        Content;
    excerpt:        Content;
    author:         number;
    featured_media: number;
    comment_status: string;
    ping_status:    string;
    sticky:         boolean;
    template:       string;
    format:         string;
    meta:           Meta;
    categories:     number[];
    tags:           any[];
    _links:         WpPostLinks;
}

export interface WpPostLinks {
    self:              About[];
    collection:        About[];
    about:             About[];
    author:            Author[];
    replies:           Author[];
    "version-history": VersionHistory[];
    "wp:attachment":   About[];
    "wp:term":         WpTerm[];
    curies:            Cury[];
}

export interface WpTag {
    id:          number;
    count:       number;
    description: string;
    link:        string;
    name:        string;
    slug:        string;
    taxonomy:    string;
    meta:        any[];
    _links:      WpTagLinks;
}

export interface WpTagLinks {
    self:           About[];
    collection:     About[];
    about:          About[];
    "wp:post_type": About[];
    curies:         Cury[];
}

export interface WpCategory {
    id:          number;
    count:       number;
    description: string;
    link:        string;
    name:        string;
    slug:        string;
    taxonomy:    Taxonomy;
    parent:      number;
    meta:        any[];
    _links:      Links;
}

export interface WpCategoryLinks {
    self:           About[];
    collection:     About[];
    about:          About[];
    up?:            Up[];
    "wp:post_type": About[];
    curies:         Cury[];
}

export interface About {
    href: string;
}

export interface Author {
    embeddable: boolean;
    href:       string;
}

export interface Cury {
    name:      string;
    href:      string;
    templated: boolean;
}

export interface PredecessorVersion {
    id:   number;
    href: string;
}

export interface VersionHistory {
    count: number;
    href:  string;
}

export interface Content {
    rendered:  string;
    protected: boolean;
}

export interface GUID {
    rendered: string;
}

export interface Title {
    rendered: string;
}

export interface Meta {
    footnotes: string;
}

export interface WpTerm {
    taxonomy:   string;
    embeddable: boolean;
    href:       string;
}

export interface Up {
    embeddable: boolean;
    href:       string;
}

export enum Taxonomy {
    Category = "category",
}
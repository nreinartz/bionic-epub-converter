using System.Xml.Serialization;

namespace BionicEpubConverter.Models;

[XmlRoot(ElementName = "creator")]
public class Creator
{

	[XmlAttribute(AttributeName = "role")]
	public string Role { get; set; }

	[XmlAttribute(AttributeName = "file-as")]
	public string FileAs { get; set; }

	[XmlText]
	public string Text { get; set; }
}

[XmlRoot(ElementName = "meta")]
public class Meta
{

	[XmlAttribute(AttributeName = "name")]
	public string Name { get; set; }

	[XmlAttribute(AttributeName = "content")]
	public string Content { get; set; }
}

[XmlRoot(ElementName = "identifier")]
public class Identifier
{

	[XmlAttribute(AttributeName = "id")]
	public string Id { get; set; }

	[XmlAttribute(AttributeName = "scheme")]
	public string Scheme { get; set; }

	[XmlText]
	public string Text { get; set; }
}

[XmlRoot(ElementName = "date")]
public class Date
{

	[XmlAttribute(AttributeName = "event")]
	public string Event { get; set; }

	[XmlAttribute(AttributeName = "opf")]
	public string Opf { get; set; }

	[XmlText]
	public DateTime Text { get; set; }
}

[XmlRoot(ElementName = "metadata")]
public class Metadata
{

	[XmlElement(ElementName = "language")]
	public string Language { get; set; }

	[XmlElement(ElementName = "title")]
	public string Title { get; set; }

	[XmlElement(ElementName = "creator")]
	public Creator Creator { get; set; }

	[XmlElement(ElementName = "publisher")]
	public string Publisher { get; set; }

	[XmlElement(ElementName = "meta")]
	public List<Meta> Meta { get; set; }

	[XmlElement(ElementName = "identifier")]
	public Identifier Identifier { get; set; }

	[XmlElement(ElementName = "date")]
	public Date Date { get; set; }

	[XmlAttribute(AttributeName = "xsi")]
	public string Xsi { get; set; }

	[XmlAttribute(AttributeName = "dc")]
	public string Dc { get; set; }

	[XmlAttribute(AttributeName = "dcterms")]
	public string Dcterms { get; set; }

	[XmlAttribute(AttributeName = "opf")]
	public string Opf { get; set; }

	[XmlAttribute(AttributeName = "calibre")]
	public string Calibre { get; set; }

	[XmlText]
	public string Text { get; set; }
}

[XmlRoot(ElementName = "item")]
public class Item
{

	[XmlAttribute(AttributeName = "id")]
	public string Id { get; set; }

	[XmlAttribute(AttributeName = "href")]
	public string Href { get; set; }

	[XmlAttribute(AttributeName = "media-type")]
	public string MediaType { get; set; }
}

[XmlRoot(ElementName = "manifest")]
public class Manifest
{

	[XmlElement(ElementName = "item")]
	public List<Item> Items { get; set; }
}

[XmlRoot(ElementName = "itemref")]
public class Itemref
{

	[XmlAttribute(AttributeName = "idref")]
	public string Id { get; set; }
}

[XmlRoot(ElementName = "spine")]
public class Spine
{

	[XmlElement(ElementName = "itemref")]
	public List<Itemref> ItemReferences { get; set; }
}

[XmlRoot(ElementName = "reference")]
public class Reference
{

	[XmlAttribute(AttributeName = "type")]
	public string Type { get; set; }

	[XmlAttribute(AttributeName = "title")]
	public string Title { get; set; }

	[XmlAttribute(AttributeName = "href")]
	public string Href { get; set; }
}

[XmlRoot(ElementName = "guide")]
public class Guide
{

	[XmlElement(ElementName = "reference")]
	public List<Reference> Reference { get; set; }
}

[XmlRoot(Namespace = "http://www.idpf.org/2007/opf", ElementName = "package")]
public class Package
{

	[XmlElement(ElementName = "metadata")]
	public Metadata Metadata { get; set; }

	[XmlElement(ElementName = "manifest")]
	public Manifest Manifest { get; set; }

	[XmlElement(ElementName = "spine")]
	public Spine Spine { get; set; }

	[XmlElement(ElementName = "guide")]
	public Guide Guide { get; set; }

	[XmlAttribute(AttributeName = "version")]
	public double Version { get; set; }

	[XmlAttribute(AttributeName = "unique-identifier")]
	public string UniqueIdentifier { get; set; }

	[XmlAttribute(AttributeName = "xmlns")]
	public string Xmlns { get; set; }

	[XmlText]
	public string Text { get; set; }
}


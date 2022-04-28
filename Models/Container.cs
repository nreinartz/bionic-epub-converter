using System.Xml.Serialization;

namespace BionicEpubConverter.Models
{

	[XmlRoot(ElementName = "rootfile")]
	public class Rootfile
	{
		[XmlAttribute(AttributeName = "full-path")]
		public string FullPath { get; set; } = string.Empty;

		[XmlAttribute(AttributeName = "media-type")]
		public string MediaType { get; set; } = string.Empty;
	}

	[XmlRoot(ElementName = "rootfiles")]
	public class RootfileList
	{
		[XmlElement(ElementName = "rootfile")]
		public List<Rootfile> Rootfiles { get; set; } = new List<Rootfile>();
	}

	[XmlRoot(ElementName = "container", Namespace = "urn:oasis:names:tc:opendocument:xmlns:container")]
	public class Container
	{
		[XmlElement(ElementName = "rootfiles")]
		public RootfileList RootfileList { get; set; } = new RootfileList();

		[XmlAttribute(AttributeName = "version")]
		public double Version { get; set; }
	}


}

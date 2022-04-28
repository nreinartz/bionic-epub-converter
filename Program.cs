
using BionicEpubConverter.Models;
using System.IO.Compression;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;

if (args.Length == 0) {
	throw new Exception("No command line arguments provided");
}

ProcessEpubFile(args[0]);

void ProcessEpubFile(string path)
{
	FileInfo epubFile = new(args[0]);
	if (!epubFile.Extension.Equals(".epub", StringComparison.OrdinalIgnoreCase)) {
		throw new Exception("No epub file");
	}

	using ZipArchive epubArchive = new(
		epubFile.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.Read),
		ZipArchiveMode.Update,
		false
	);

	FileInfo targetEpubFile = new(Path.Combine(epubFile.Directory?.FullName ?? "", 
		$"{Path.GetFileNameWithoutExtension(epubFile.Name)} - bionic.epub"));
	using ZipArchive targetEpub = new(targetEpubFile.OpenWrite(), ZipArchiveMode.Create, false);

	HashSet<string> contentPaths = ReadContentPaths(epubArchive)
		.ToHashSet();

	foreach (ZipArchiveEntry sourceEntry in epubArchive.Entries) {
		ZipArchiveEntry targetEntry = targetEpub.CreateEntry(sourceEntry.FullName);

		using Stream sourceStream = sourceEntry.Open();
		using Stream targetStream = targetEntry.Open();

		if (!contentPaths.Contains(sourceEntry.FullName)) {
			sourceStream.CopyTo(targetStream);
		} else {
			ProcessContentEntry(sourceStream, targetStream);
		}
	}
}

void HandleText(XmlReader reader, XmlWriter writer)
{
	int currentPosition = 0;
	MatchCollection matches = Regex.Matches(reader.Value, @"(\w)*'?\w+");

	foreach (Match match in matches) {
		if (match.Index > currentPosition) {
			writer.WriteString(reader.Value[currentPosition..match.Index]);
		}

		writer.WriteStartElement(null, "b", null);

		writer.WriteString(
			match.Length switch {
				var x when x <= 3 => match.Value[0..1],
				var x when x == 4 => match.Value[0..2],
				_ => match.Value[0..(int)Math.Ceiling(match.Length * 0.4)]
			}
		);

		writer.WriteEndElement();

		writer.WriteString(
			match.Length switch {
				var x when x <= 3 => match.Value[1..],
				var x when x == 4 => match.Value[2..],
				_ => match.Value[(int)Math.Ceiling(match.Length * 0.4)..]
			}
		);

		currentPosition = match.Index + match.Length;
	}

	writer.WriteString(reader.Value[currentPosition..]);
}

void ProcessContentEntry(Stream source, Stream target)
{
	using XmlReader reader = XmlReader.Create(source, new XmlReaderSettings {
		DtdProcessing = DtdProcessing.Parse
	});

	using XmlWriter writer = XmlWriter.Create(target, new XmlWriterSettings {
		CloseOutput = true,
	});

	while (reader.Read()) {
		switch (reader.NodeType) {
			case XmlNodeType.Element:
				writer.WriteStartElement(reader.Prefix, reader.LocalName, reader.NamespaceURI);

				if (reader.HasAttributes) {
					writer.WriteAttributes(reader, false);
				}

				if (reader.IsEmptyElement) {
					writer.WriteEndElement();
				}
				break;
			case XmlNodeType.Text:
				HandleText(reader, writer);
				break;
			case XmlNodeType.Whitespace:
				writer.WriteWhitespace(reader.Value);
				break;
			case XmlNodeType.EndElement:
				writer.WriteEndElement();
				break;
		}
	}
}

IEnumerable<string> ReadContentPaths(ZipArchive epubArchive)
{
	ZipArchiveEntry? containerEntry = epubArchive.Entries.FirstOrDefault(
		entry => entry.FullName.EndsWith("container.xml", StringComparison.OrdinalIgnoreCase)
	);

	if (containerEntry == null) {
		throw new Exception("Invalid epub file");
	}

	using Stream containerDocStream = containerEntry.Open();
	using StreamReader reader = new(containerDocStream);

	List<ZipArchiveEntry> contentEntries = new();

	XmlSerializer serializer = new(typeof(Container));
	Container container = serializer.Deserialize(reader) as Container
		?? throw new InvalidOperationException();

	foreach (Rootfile rootfile in container.RootfileList.Rootfiles) {
		ZipArchiveEntry? opfFile = epubArchive.Entries.FirstOrDefault(
				entry => entry.FullName.Equals(rootfile.FullPath, StringComparison.OrdinalIgnoreCase)
		);

		if (opfFile != null) {
			foreach (string entry in ReadContentPathsForOpf(epubArchive, opfFile)) {
				yield return entry;
			}
		}
	}
}

IEnumerable<string> ReadContentPathsForOpf(ZipArchive archive, ZipArchiveEntry opfFile)
{
	using Stream opfDocStream = opfFile.Open();
	using StreamReader reader = new(opfDocStream);

	XmlSerializer serializer = new(typeof(Package));
	List<ZipArchiveEntry> bookItems = new();

	string basePath = opfFile.FullName[..^opfFile.Name.Length];
	Package? package = serializer.Deserialize(reader) as Package
		?? throw new InvalidOperationException();

	foreach (Itemref itemref in package.Spine.ItemReferences) {
		Item manifestItem = package.Manifest.Items.First(
			item => item.Id.Equals(itemref.Id, StringComparison.OrdinalIgnoreCase)
		);

		if (manifestItem.MediaType.Equals("application/xhtml+xml", StringComparison.OrdinalIgnoreCase)) {
			string itemPath = basePath + manifestItem.Href;

			ZipArchiveEntry? itemEntry = archive.Entries.FirstOrDefault(
				entry => entry.FullName.Equals(itemPath, StringComparison.OrdinalIgnoreCase)
			);

			if (itemEntry != null) {
				yield return itemEntry.FullName;
			}
		}
	}
}
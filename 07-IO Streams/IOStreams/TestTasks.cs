using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.IO.Packaging;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Linq;

namespace IOStreams
{

	public static class TestTasks
	{
        /// <summary>
        /// Parses Resourses\Planets.xlsx file and returns the planet data: 
        ///   Jupiter     69911.00
        ///   Saturn      58232.00
        ///   Uranus      25362.00
        ///    ...
        /// See Resourses\Planets.xlsx for details
        /// </summary>
        /// <param name="xlsxFileName">source file name</param>
        /// <returns>sequence of PlanetInfo</returns>
	    public static IEnumerable<PlanetInfo> ReadPlanetInfoFromXlsx(string xlsxFileName)
	    {
	        const string dictionarystringsPath = "/xl/sharedStrings.xml";
	        const string worksheetPath = "/xl/worksheets/sheet1.xml";

	        using (var package = Package.Open(xlsxFileName, FileMode.Open, FileAccess.Read, FileShare.Read))
	        {
	            var xDocument = LoadXDocument(package, dictionarystringsPath);	            
	            if (xDocument.Root == null)
	                throw new NullReferenceException();
                XNamespace ns = GetNamespace(xDocument);
	            var strings = xDocument.Root.Descendants(ns + "t").Select(x => x.Value).ToList();

	            xDocument = LoadXDocument(package, worksheetPath);
	            ns = GetNamespace(xDocument);
	            return xDocument.Descendants(ns + "row").Skip(1).Select(x =>
	                new PlanetInfo
	                {
	                    Name = strings[(int) x.Descendants(ns + "v").First()],
	                    MeanRadius = (double) x.Descendants(ns + "v").Skip(1).First()
	                });
	        }
	    }

	    private static XDocument LoadXDocument(Package package, string path)
		{
			using (var stream = GetPartStream(package, path))
			{
				var xDocument = XDocument.Load(stream);
				return xDocument;
			}
		}

		private static XNamespace GetNamespace(XDocument document)
		{
            if(document.Root==null)
                throw new NullReferenceException();
            return document.Root.Name.Namespace;
		}

		private static Stream GetPartStream(Package package, string path)
		{
			return package.GetPart(new Uri(path, UriKind.Relative)).GetStream();
		}


	    /// <summary>
	    /// Calculates hash of stream using specifued algorithm
	    /// </summary>
	    /// <param name="stream">source stream</param>
	    /// <param name="hashAlgorithmName">hash algorithm ("MD5","SHA1","SHA256" and other supported by .NET)</param>
	    /// <returns></returns>
	    public static
	        string CalculateHash(this Stream stream, string hashAlgorithmName)
	    {
	        var hashalgorithm = HashAlgorithm.Create(hashAlgorithmName);
            if (hashalgorithm == null) 
                throw new ArgumentException();
            return BitConverter.ToString(hashalgorithm.ComputeHash(stream)).Replace("-", string.Empty);
	    }


	    /// <summary>
	    /// Returns decompressed strem from file. 
	    /// </summary>
	    /// <param name="fileName">source file</param>
	    /// <param name="method">method used for compression (none, deflate, gzip)</param>
	    /// <returns>output stream</returns>
	    public static Stream DecompressStream(string fileName, DecompressionMethods method)
	    {
	        var streams = new Func<Stream, Stream>[3]
	        {
	            s => s,
	            s => new GZipStream(s, CompressionMode.Decompress),
	            s => new DeflateStream(s, CompressionMode.Decompress)
	        };
	        return streams[(int) method](new FileStream(fileName, FileMode.Open));
	    }


	    /// <summary>
		/// Reads file content econded with non Unicode encoding
		/// </summary>
		/// <param name="fileName">source file name</param>
		/// <param name="encoding">encoding name</param>
		/// <returns>Unicoded file content</returns>
		public static string ReadEncodedText(string fileName, string encoding)
		{
            return File.ReadAllText(fileName, Encoding.GetEncoding(encoding));
		}
	}


	public class PlanetInfo : IEquatable<PlanetInfo>
	{
		public string Name { get; set; }
		public double MeanRadius { get; set; }

		public override string ToString()
		{
			return string.Format("{0} {1}", Name, MeanRadius);
		}

		public bool Equals(PlanetInfo other)
		{
			return Name.Equals(other.Name)
				&& MeanRadius.Equals(other.MeanRadius);
		}
	}



}

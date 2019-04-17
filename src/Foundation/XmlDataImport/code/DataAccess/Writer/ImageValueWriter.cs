using Sitecore.Services.Core.Model;
using Sitecore.DataExchange.DataAccess;
using System;
using System.Globalization;
using System.Xml;
using System.Web.Hosting;
using System.IO;
using System.Net;
using System.Linq;
using Sitecore.Data;
using Sitecore;
using Sitecore.Configuration;
using Sitecore.Data.Items;
using Sitecore.Resources.Media;
using Sitecore.Data.Managers;
using Sitecore.SecurityModel;
using SaudiAramco.Foundation.XmlDataImport.Extensions;
using System.Collections.Generic;

namespace SaudiAramco.Foundation.XmlDataImport.DataAccess.Writer
{
    public class ImageValueWriter : IValueWriter
    {
        public string Field { get; set; }
        private string XmlValue = "<image mediaid=\"{0}\" />";
        private static Database _db;
        private static Database MasterDb => _db ?? (_db = Context.ContentDatabase ?? Factory.GetDatabase("master"));

        public ImageValueWriter(string fieldName) : base()
        {
            Field = fieldName;
        }

        public CanWriteResult CanWrite(object target, object value, DataAccessContext context)
        {
            return CanWriteResult.PositiveResult(true);
        }

        public bool Write(object target, object value, DataAccessContext context)
        {
            //Dictionary<string, ItemModel> model = (Dictionary<string, ItemModel>)target;
            ItemModel model = (ItemModel)target;
            if (model == null)
                return false;
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
                return false;

            XmlDocument xdoc = new XmlDocument();
            xdoc.LoadXml(value.ToString());

            string outerXml = value.ToString();

            var node = xdoc.SelectSingleNode("//hero/*[1]/@type")?.Value;

            string imageCaption = string.Empty;
            string imagePath = string.Empty;

            if (node == "image")
            {
                XmlAttributeCollection xnode = xdoc.SelectSingleNode("//hero/*").Attributes;
                foreach (XmlAttribute attr in xnode)
                {
                    if (attr.Name.Contains("heroimageCaption"))
                    {
                        imageCaption = attr.Value.ToString();
                        break;
                    }
                }

                xnode = xdoc.SelectSingleNode("//hero/*[1]/image").Attributes;
                foreach (XmlAttribute attr in xnode)
                {
                    if (attr.Name.Contains("fileReference"))
                    {
                        imagePath = attr.Value.ToString();
                        break;
                    }
                }
            }
            else
            {
                value = string.Empty;
            }

            if (!string.IsNullOrEmpty(imagePath))
            {
                value = imagePath.Remove(imagePath.Length - 4, 4);
                string hostname = "http://www.saudiaramco.com";
                string suffix = "/_jcr_content/renditions/original";

                string newsMediaFolderPath = "/SaudiAramco/News";

                var tempFolder = HostingEnvironment.ApplicationPhysicalPath + "temp\\dxfmediaimport";
                Directory.CreateDirectory(tempFolder);

                var fileName = Path.GetFileNameWithoutExtension(imagePath);
                var tempFilename = Guid.NewGuid() + Path.GetExtension(imagePath);
                string localPath = Path.Combine(tempFolder, tempFilename);

                string imageUrl = hostname + imagePath + suffix;

                using (WebClient client = new WebClient())
                {
                    client.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");
                    client.DownloadFile(imageUrl, localPath);
                }

                var mediaRoot = ItemIDs.MediaLibraryRoot;

                var mediaRootItem = MasterDb.GetItem(mediaRoot);
                var path = mediaRootItem.Paths.Path;

                var directoryName = Path.GetDirectoryName(localPath);

                var destination = Path.AltDirectorySeparatorChar + Combineparts(path, newsMediaFolderPath, fileName);
                var media = MasterDb.GetItem(destination);
                if (media == null)
                {
                    var options = new MediaCreatorOptions
                    {
                        FileBased = false,
                        Destination = destination,
                        AlternateText = fileName
                    };

                    var newMedia = MediaManager.Creator.CreateFromFile(localPath, options);

                    value = string.Format(XmlValue, newMedia.ID);
                }
                else
                {
                    value = string.Format(XmlValue, media.ID);
                }
            }

            // Update Image and Caption Field Value
            if (!string.IsNullOrEmpty(imageCaption))
            {
                model["Caption"] = imageCaption;
            }
            else
            {
                model["Caption"] = null;
            }

            if (!string.IsNullOrEmpty(value.ToString()))
            {
                model[Field] = value;
            }
            else
            {
                model[Field] = null;
            }

            ChildItemCreator obj = new ChildItemCreator(outerXml, model["ItemID"].ToString());
            obj.CreateChildItems();

            return true;
        }

        private static string Combineparts(params string[] pathparts)
        {
            var pathChr = Path.AltDirectorySeparatorChar;
            return string.Join(pathChr.ToString(),
                pathparts.Select(p => p.Replace(Path.DirectorySeparatorChar, pathChr).Trim(pathChr)));
        }
    }
}

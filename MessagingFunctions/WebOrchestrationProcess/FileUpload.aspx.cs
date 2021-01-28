using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System.Xml.Linq;
using Microsoft.Data.OData.Atom;

namespace WebOrchestrationProcess
{
    public partial class FileUpload : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void UploadFilesOfDirectory(object sender, EventArgs e)
        {
            string guidname = Guid.NewGuid().ToString().ToUpper();
            string blobname = RoleEnvironment.GetConfigurationSettingValue("BlobName");
            string storageString = RoleEnvironment.GetConfigurationSettingValue("StorageConnectionString");
            CloudStorageAccount myStorageAccount = CloudStorageAccount.Parse(storageString);
            CloudBlobClient blobclient = myStorageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobclient.GetContainerReference(blobname);
            if (container.CreateIfNotExists())
            {
                container.SetPermissions(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob });
            }
            XDocument document = new XDocument(
                new XElement("mail",
                    new XAttribute("Identificador", guidname),
                    new XElement("to", "Juan"),
                    new XElement("from", "Pedro"),
                    new XElement("Subject", "Reminder"),
                    new XElement("body", "Don't forget our meeting at: ")
                    )
                );
            Stream stream = new MemoryStream();
            document.Save(stream);
            stream.Seek(0, SeekOrigin.Begin);
            CloudBlockBlob cloudBlockBlob = container.GetBlockBlobReference("Example" + guidname + ".Xml");
            cloudBlockBlob.Properties.ContentType = "Xml";
            cloudBlockBlob.UploadFromStream(stream);
            labelReady.Text = "File " + guidname + " uploaded.";
            labelReady.Visible = true;
        }
    }
}
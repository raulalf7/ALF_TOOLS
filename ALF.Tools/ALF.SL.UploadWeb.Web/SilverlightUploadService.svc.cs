using System.IO;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Web.Hosting; 

namespace ALF.SL.UploadWeb.Web
{
    [ServiceContract(Namespace = "")]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class SilverlightUploadService
    {
        private string _tempExtension = "_temp";

        [OperationContract]
        public void StoreFileAdvanced(string fileName, byte[] data, int dataLength, string parameters, bool firstChunk, bool lastChunk)
        {
            string uploadFolder = GetUploadFolder();
            string tempFileName = fileName + _tempExtension;

            if (firstChunk)
            {
                //Delete temp file
                if (File.Exists(@HostingEnvironment.ApplicationPhysicalPath + "/" + uploadFolder + "/" + tempFileName))
                    File.Delete(@HostingEnvironment.ApplicationPhysicalPath + "/" + uploadFolder + "/" + tempFileName);

                //Delete target file
                if (File.Exists(@HostingEnvironment.ApplicationPhysicalPath + "/" + uploadFolder + "/" + fileName))
                    File.Delete(@HostingEnvironment.ApplicationPhysicalPath + "/" + uploadFolder + "/" + fileName);

            }


            FileStream fs = File.Open(@HostingEnvironment.ApplicationPhysicalPath + "/" + uploadFolder + "/" + tempFileName, FileMode.Append);
            fs.Write(data, 0, dataLength);
            fs.Close();

            if (lastChunk)
            {
                //Rename file to original file
                File.Move(HostingEnvironment.ApplicationPhysicalPath + "/" + uploadFolder + "/" + tempFileName, HostingEnvironment.ApplicationPhysicalPath + "/" + uploadFolder + "/" + fileName);

                //Finish stuff....
                FinishedFileUpload(fileName, parameters);
            }

        }

        [OperationContract]
        public void CancelUpload(string fileName)
        {
            string uploadFolder = GetUploadFolder();
            string tempFileName = fileName + _tempExtension;

            if (File.Exists(@HostingEnvironment.ApplicationPhysicalPath + "/" + uploadFolder + "/" + tempFileName))
                File.Delete(@HostingEnvironment.ApplicationPhysicalPath + "/" + uploadFolder + "/" + tempFileName);

        }

        protected void DeleteUploadedFile(string fileName)
        {
            string uploadFolder = GetUploadFolder();


            if (File.Exists(@HostingEnvironment.ApplicationPhysicalPath + "/" + uploadFolder + "/" + fileName))
                File.Delete(@HostingEnvironment.ApplicationPhysicalPath + "/" + uploadFolder + "/" + fileName);
        }

        protected virtual void FinishedFileUpload(string fileName, string parameters)
        {
        }

        protected virtual string GetUploadFolder()
        {
            return "Upload";
        }

    }
}

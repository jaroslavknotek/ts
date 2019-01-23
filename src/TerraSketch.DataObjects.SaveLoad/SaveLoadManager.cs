using System;
using System.IO;
using System.Xml.Serialization;

namespace TerraSketch.DataObjects.SaveLoad
{
    public class SaveLoadManager : ISaveLoadManager
    {
        private SaveItemConverter saveItemConverter = null;

        private readonly ILoadItemParameter parameters = null;
        public SaveLoadManager(ILoadItemParameter loadingparams)
        {
            parameters = loadingparams;
            saveItemConverter = new SaveItemConverter(parameters);
        }

        public ISaveItem Load(FileInfo fi)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(SaveItemXmlWrapper));
            TextReader reader = new StreamReader(fi.FullName);

            var deserializedWrapper = (SaveItemXmlWrapper)serializer.Deserialize(reader);
            reader.Close();

            return saveItemConverter.ToObject(deserializedWrapper);

        }

        public void Save(ISaveItem si, FileInfo fi)
        {

            try
            {

                using (TextWriter writer = new StreamWriter(fi.FullName))
                {
                    this.SaveToWriter(si, writer);
                }
            }
            catch (Exception)
            {
                // safe way to delete file without race condition
                try
                {
                    File.Delete(fi.FullName);
                }
                catch (Exception) { }



                throw;
            }
        }

        public void SaveToWriter(ISaveItem si, TextWriter tw)
        {
            var sxmlw = saveItemConverter.ToXmlWrapper(si);
            XmlSerializer serializer = new XmlSerializer(typeof(SaveItemXmlWrapper));
            serializer.Serialize(tw, sxmlw);
        }


    }
}

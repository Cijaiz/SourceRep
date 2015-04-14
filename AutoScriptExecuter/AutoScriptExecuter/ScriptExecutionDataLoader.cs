using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;
namespace AutoScriptExecuter
{
    class ScriptExecutionDataLoader
    {
        public List<ScriptLocation> ScriptFiles { get; set; }
        public List<string> lstConnectionString = null;
        string selectedFileName = string.Empty;
        string RootPath = string.Empty;
        public void InitializeLoader(string rootpath,StringBuilder log)
        {
            RootPath = rootpath;
            log.AppendLine(Constant.readXMLstart + DateTime.Now);
            loaderScriptData(rootpath);
        }

        static int textBoxCount = 0;
        public void loaderScriptData(string rootpath)
        {
            ScriptFiles = new List<ScriptLocation>();
            using (XmlReader reader = XmlReader.Create(selectedFileName))
                while (reader.Read())
                    if (reader.IsStartElement())
                        switch (reader.Name)
                        {
                            case Constant.script:
                                ScriptLocation SL = new ScriptLocation();
                                string tempString = reader.GetAttribute(Constant.scriptAttributename);
                                if (tempString.Contains(Constant.pathcontainsSeparator))
                                {
                                    var str = tempString.Split(Constant.pathsplitSeparator);
                                    if (str[str.Length - 1].Contains(Constant.filecontainsSeparator))
                                    {
                                        textBoxCount = textBoxCount + 1;
                                        var st = str[str.Length - 1].Split(Constant.filesplitSeparator);
                                        SL.NAME = st[0];
                                        SL.FILETYPE = st[1];
                                        SL.FILEPATH = tempString;
                                        SL.FILENAME = str[str.Length - 1];
                                        SL.TextName = SL.NAME + "#" + textBoxCount;
                                    }
                                    else
                                    {
                                        textBoxCount = textBoxCount + 1;
                                        SL.NAME = str[str.Length - 1];
                                        SL.FILETYPE = reader.GetAttribute(Constant.scriptAttributefiletype);
                                        SL.FILEPATH = string.Format("{0}.{1}", tempString, reader.GetAttribute(Constant.scriptAttributefiletype));
                                        SL.FILENAME = string.Format("{0}.{1}", str[str.Length - 1], reader.GetAttribute(Constant.scriptAttributefiletype));
                                        SL.TextName = SL.NAME + "#" + textBoxCount;
                                    }
                                }
                                else
                                {
                                    textBoxCount = textBoxCount + 1;
                                    SL.NAME = reader.GetAttribute(Constant.scriptAttributename);
                                    SL.FILETYPE = reader.GetAttribute(Constant.scriptAttributefiletype);
                                    SL.FILEPATH = string.Format("{0}\\{1}.{2}", rootpath, reader.GetAttribute(Constant.scriptAttributename), reader.GetAttribute(Constant.scriptAttributefiletype));
                                    SL.FILENAME = string.Format("{0}.{1}", reader.GetAttribute(Constant.scriptAttributename), reader.GetAttribute(Constant.scriptAttributefiletype));
                                    SL.TextName = SL.NAME + "#" + textBoxCount;
                                }
                                ScriptFiles.Add(SL);
                                break;
                            default:
                                break;
                        }

        }
        public void GetLoaderRootPath(out string rootpath,int selectedType)
        {
            rootpath = string.Empty;
            if (selectedType == 1)
                selectedFileName = Constant.LocalxmlFileName;
            else if (selectedType == 2)
                selectedFileName = Constant.AzurexmlFileName;

             using (XmlReader reader = XmlReader.Create(selectedFileName))
                while (reader.Read())
                    if (reader.IsStartElement())                  
                        switch (reader.Name)
                        {
                            case Constant.rootPath:
                                  rootpath = reader.GetAttribute(Constant.rootPathAttribute);
                                break;
                            default:
                                break;
                        }
        }

        public List<ExecutionType> GetExecutionTypes()
        {
            List<ExecutionType> executionTypes = new List<ExecutionType>();
            executionTypes.Add(new ExecutionType() { TypeId = 1, Type = "Local" });
            executionTypes.Add(new ExecutionType() { TypeId = 2, Type = "Azure" });
            return executionTypes;
        }

        public List<string> LoadConnectionString()
        {
             lstConnectionString = new List<string>();

            using (XmlReader reader = XmlReader.Create(Constant.ConnectionStringxmlFileName))
                while (reader.Read())
                    if (reader.IsStartElement())                  
                        switch (reader.Name)
                        {
                            case Constant.ConnectionStringxmlFileNameAttribute:
                                lstConnectionString.Add(reader.GetAttribute(Constant.ConnectionStringAttribute));
                                break;
                            default:
                                break;
                        }
                return lstConnectionString;
        }
        public List<string> WriteConnectionString(string txtConnectionSting)
        {
               if(!lstConnectionString.Contains(txtConnectionSting) )
                {

                    if (File.Exists(Constant.ConnectionStringxmlFileName) == false)
                    {
                        XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();
                        xmlWriterSettings.Indent = true;
                        xmlWriterSettings.NewLineOnAttributes = true;
                        using (XmlWriter xmlWriter = XmlWriter.Create(Constant.ConnectionStringxmlFileName, xmlWriterSettings))
                        {
                            xmlWriter.WriteStartDocument();
                            xmlWriter.WriteStartElement("connectionstring");

                            xmlWriter.WriteStartElement("String");
                            xmlWriter.WriteAttributeString("Value", txtConnectionSting);
                            xmlWriter.WriteEndElement();

                            xmlWriter.WriteEndElement();
                            xmlWriter.WriteEndDocument();
                            xmlWriter.Flush();
                            xmlWriter.Close();
                        }
                    }
                    else
                    {

                        XDocument doc = XDocument.Load(Constant.ConnectionStringxmlFileName);
                        XElement ConnectionString = doc.Element("connectionstring");
                        ConnectionString.Add(new XElement("string",
                                   new XAttribute("value", txtConnectionSting)));

                        doc.Save(Constant.ConnectionStringxmlFileName);
                    }
                    lstConnectionString.Add(txtConnectionSting);
                }
               return lstConnectionString;
        }

        public string WriteScriptDetails(List<string> NewScript,string rootpath)
        {
            XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();
            xmlWriterSettings.Indent = true;
            xmlWriterSettings.NewLineOnAttributes = true;
            using (XmlWriter xmlWriter = XmlWriter.Create(selectedFileName, xmlWriterSettings))
            {
                xmlWriter.WriteStartDocument();
                xmlWriter.WriteStartElement("ScriptSequence");
                xmlWriter.WriteStartElement("ScriptDetail");

                xmlWriter.WriteStartElement("Rootpath");
                xmlWriter.WriteAttributeString("path", rootpath);
                xmlWriter.WriteEndElement();

                xmlWriter.WriteEndElement();

                xmlWriter.WriteStartElement("ScriptDetail");
                
                foreach (var item in NewScript)
                {
                  
                    xmlWriter.WriteStartElement("Script");
                    xmlWriter.WriteAttributeString("name", item);
                    xmlWriter.WriteAttributeString("filetype", "sql");
                    xmlWriter.WriteEndElement();
                }

                xmlWriter.WriteEndElement();
                xmlWriter.WriteEndElement();
                xmlWriter.WriteEndDocument();
                xmlWriter.Flush();
                xmlWriter.Close();
            }
            loaderScriptData(rootpath);
            return "Success";
        }
    }
}

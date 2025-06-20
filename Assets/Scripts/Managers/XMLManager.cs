using Assets.Scripts.PatientData.AlgoData;
using Assets.Scripts.UI.TutorialContents;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Schema;
using UnityEngine;
using static Assets.Scripts.GameData;
using static Assets.Scripts.Managers.GameStateManager;

namespace Assets.Scripts.Managers
{
    /// <summary>
    /// XML Manager handles loading, saving, and validating game data using XML files.
    /// It supports deserialization of complex nested data structures (including dictionaries, lists, and custom structs),
    /// recursive serialization of C# objects into XML, and validation against XSD schemas to ensure data integrity.
    /// This utility facilitates persistent storage and retrieval of game progress and settings.
    /// </summary>
    public static class XmlManager
    {
        /// <summary>
        /// Serialize the given data object to XML and save it to the specified file path.
        /// </summary>
        /// <param name="data">The object to serialize (e.g., global game data).</param>
        /// <param name="path">The full file path where the XML file will be saved.</param>
        /// <param name="rootName">The root XML element name (default is "Root").</param>
        public static void SaveToXml(object data, string path, string rootName = "Root")
        {
            XmlDocument xmlDoc = new XmlDocument();
            XmlElement root = xmlDoc.CreateElement(rootName);
            xmlDoc.AppendChild(root);

            SerializedObject(xmlDoc, root, data);

            xmlDoc.Save(path);
            Debug.Log("File saved");
        }

        /// <summary>
        /// Recursively serialize an object to XML elements.
        /// </summary>
        /// <param name="xmlDocument">The XmlDocument to create elements in.</param>
        /// <param name="parentNode">The parent XmlElement to append serialized data to.</param>
        /// <param name="data">The object to serialize.</param>
        private static void SerializedObject(XmlDocument xmlDocument, XmlElement parentNode, object data)
        {
            if (data == null) return;

            Type objetType = data.GetType();
            if (objetType.IsPrimitive || objetType == typeof(string))
            {
                parentNode.InnerText = data.ToString();
            }
            else if (data is IDictionary dictionary)
            {
                foreach (var key in dictionary.Keys)
                {
                    XmlElement itemNode = xmlDocument.CreateElement(key.ToString());
                    SerializedObject(xmlDocument, itemNode, dictionary[key]);
                    parentNode.AppendChild(itemNode);
                }
            }
            else if (data is IEnumerable collection)
            {
                foreach (var item in collection)
                {
                    XmlElement itemNode = xmlDocument.CreateElement("Item");
                    SerializedObject(xmlDocument, itemNode, item);
                    parentNode.AppendChild(itemNode);
                }
            }
            else
            {
                foreach (FieldInfo field in objetType.GetFields())
                {
                    XmlElement fieldNode = xmlDocument.CreateElement(field.Name);
                    SerializedObject(xmlDocument, fieldNode, field.GetValue(data));
                    parentNode.AppendChild(fieldNode);
                }

                foreach (PropertyInfo property in objetType.GetProperties())
                {
                    if (property.CanRead)
                    {
                        XmlElement propNode = xmlDocument.CreateElement(property.Name);
                        SerializedObject(xmlDocument, propNode, property.GetValue(data));
                        parentNode.AppendChild(propNode);
                    }
                }
            }
        }

        /// <summary>
        /// Deserialize an XML file to a MainData struct.
        /// </summary>
        /// <param name="filePath">The path to the XML file to load.</param>
        /// <returns>A MainData struct populated from the XML.</returns>
        /// <exception cref="Exception">Throws if the XML format is invalid.</exception>
        public static MainData LoadGameData(string filePath)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(filePath);

            XmlNode root = doc.DocumentElement;
            if (root == null || root.Name != "GameData") throw new Exception("Invalid XML format.");

            MainData gameData = new MainData
            {
                totGames = int.Parse(root["totGames"].InnerText),
                nbGameSession = int.Parse(root["nbGameSession"].InnerText),
                gameTime = float.Parse(root["gameTime"].InnerText),
                sessionTimeQueue = LoadQueue(root.SelectSingleNode("sessionTimeQueue")),
                levelRecords = LoadLevelRecords(root.SelectSingleNode("levelRecords"))
            };

            return gameData;
        }

        /// <summary>
        /// Loads a tutorial entry from an XML TextAsset based on the given step name and entry ID.
        /// </summary>
        /// <param name="path">The TextAsset containing the tutorial XML data.</param>
        /// <param name="stepName">The name attribute of the Step node to search for.</param>
        /// <param name="id">The ID of the tutorial entry to load.</param>
        /// <returns>
        /// A TutorialEntry object with the data for the specified step and ID, or null if not found.
        /// </returns>
        /// <exception cref="Exception">Thrown if the XML root element is not "Tutorial".</exception>
        public static TutorialEntry LoadTutoriaDataByID(TextAsset path, string stepName, int id)
        {
            Debug.Log(path.text);
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(path.text);

            XmlNode node = doc.DocumentElement;
            if (node == null || node.Name != "Tutorial") throw new Exception("Invalid XML format.");

            node = doc.SelectSingleNode($"//Step[@Name='{stepName}']");
            if (node == null)
            {
                Debug.LogError($"Step '{stepName}' not found");
                return null;
            }

            TutorialEntry entry = new TutorialEntry();
            node = doc.SelectSingleNode($"//Entry[ID='{id}']");
            if (node != null)
            {
                entry.ID = int.Parse(node["ID"]?.InnerText);
                entry.Intitule = node["Intitule"]?.InnerText;
                entry.Text = node["Text"]?.InnerText;   
                return entry;
            }
            return null;
        }

        /// <summary>
        /// Validates an XML file against a given XSD schema.
        /// Should be called at the start of the game to ensure XML integrity.
        /// </summary>
        /// <param name="pathXmlFile">The file path of the XML document to validate.</param>
        /// <param name="pathXsdFile">The file path of the XSD schema to validate against.</param>
        /// <returns>True if the XML is valid according to the schema; otherwise, false.</returns>
        public static bool ValidateXML(string pathXmlFile, string pathXsdFile)
        {
            if (!File.Exists(pathXsdFile))
            {
                Debug.LogError("XSD file not found for validation");
                return false;
            }
            if (!File.Exists(pathXmlFile))
            {
                Debug.LogError("XML file not found for validation");
                return false;
            }

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(pathXmlFile);
            XmlSchemaSet schemaSet = new XmlSchemaSet();
            schemaSet.Add("", pathXsdFile);

            bool isValid = true;
            xmlDoc.Schemas = schemaSet;
            xmlDoc.Validate((sender, args) =>
            {
                if (args.Severity == XmlSeverityType.Error)
                {
                    Debug.LogError("XML validation error: " + args.Message);
                    isValid = false;
                }
            });
            return isValid;
        }

        /// <summary>
        /// Loads a dictionary mapping LevelState enums to LevelRecords structs from an XML node.
        /// Each child node represents a LevelRecord with data stored recursively.
        /// </summary>
        /// <param name="node">The XML node containing child nodes for each LevelRecord.</param>
        /// <returns>A dictionary where keys are LevelState enums and values are LevelRecords structs.</returns>
        private static Dictionary<LevelState, LevelRecords> LoadLevelRecords(XmlNode node)
        {
            Dictionary<LevelState, LevelRecords> levelRecords = new Dictionary<LevelState, LevelRecords>();
            foreach (XmlNode levelNode in node.ChildNodes)
            {
                LevelRecords records = new LevelRecords
                {
                    levelNbAttempt = int.Parse(levelNode["levelNbAttempt"].InnerText),
                    totPatientCompleted = int.Parse(levelNode["totPatientCompleted"].InnerText),
                    patientCaseRecords = LoadPatientCaseRecords(levelNode.SelectSingleNode("patientCaseRecords")),
                    
                };

                LevelState name = (LevelState)Enum.Parse(typeof(LevelState), levelNode.Name);
                levelRecords[name] = records;
            }
            return levelRecords;
        }

        /// <summary>
        /// Parses patient case records from an XML node and returns a dictionary mapping patient case names to their records.
        /// </summary>
        /// <param name="node">The XML node containing child nodes for each patient case record.</param>
        /// <returns>
        /// A dictionary where the key is the patient case name (XML node name) and the value is the corresponding PatientCaseRecords object.
        /// </returns>
        private static Dictionary<string, PatientCaseRecords> LoadPatientCaseRecords(XmlNode node)
        {
            Dictionary<string, PatientCaseRecords> patientCaseRecords = new Dictionary<string, PatientCaseRecords>();
            foreach (XmlNode patientNode in node.ChildNodes)
            {
                PatientCaseRecords record = new PatientCaseRecords
                {
                    nbAttempt = int.Parse(patientNode["nbAttempt"].InnerText),
                    totDiagnosticCorrect = int.Parse(patientNode["totDiagnosticCorrect"].InnerText),
                    totDiagnosticError = int.Parse(patientNode["totDiagnosticError"].InnerText),
                    totActionCorrect = int.Parse(patientNode["totActionCorrect"].InnerText),
                    totActionError = int.Parse(patientNode["totActionError"].InnerText),
                    totStepSucceed = int.Parse(patientNode["totStepSucceed"].InnerText),
                    totStepFailed = int.Parse(patientNode["totStepFailed"].InnerText),
                    timePassed = float.Parse(patientNode["timePassed"].InnerText),
                    stepRecords = LoadStepRecords(patientNode.SelectSingleNode("stepRecords")),
                }; 
                patientCaseRecords[patientNode.Name] = record;
            }
            return patientCaseRecords;
        }


        /// <summary>
        /// Loads a dictionary mapping AlgoState (as string keys) to StepRecords from an XML node.
        /// Each child node of the input node represents one StepRecord with its data.
        /// </summary>
        /// <param name="node">The XML node containing child nodes for each StepRecord.</param>
        /// <returns>A dictionary where keys are AlgoState names (strings) and values are StepRecords structs.</returns>
        private static Dictionary<string, StepRecords> LoadStepRecords(XmlNode node)
        {
            Dictionary<string, StepRecords> stepRecords = new Dictionary<string, StepRecords>();
            foreach (XmlNode stepNode in node.ChildNodes)
            {
                StepRecords step = new StepRecords
                {
                    diagnoticsAttempt = int.Parse(stepNode["diagnoticsAttempt"].InnerText),
                    actionAttempt = int.Parse(stepNode["actionAttempt"].InnerText),
                    actionAnswer = LoadStringList(stepNode.SelectSingleNode("actionAnswer")),
                    diagnosticAnswer = LoadStringList(stepNode.SelectSingleNode("diagnosticAnswer"))
                };
                stepRecords[stepNode.Name] = step;
            }
            return stepRecords;
        }

        /// <summary>
        /// Loads a list of strings from the child nodes of the given XML node.
        /// Each child node's inner text is added to the list.
        /// </summary>
        /// <param name="node">The XML node containing child nodes representing string values.</param>
        /// <returns>A list of strings extracted from the XML node's children.</returns>
        private static List<String> LoadStringList(XmlNode node)
        {
            List<String> list = new List<String>();
            foreach (XmlNode item in node.ChildNodes)
            {
                list.Add(item.InnerText);
            }
            return list;
        }

        /// <summary>
        /// Loads a queue of floats from the child nodes of the given XML node.
        /// Each child node's inner text is parsed as a float and enqueued.
        /// </summary>
        /// <param name="node">The XML node containing child nodes representing float values.</param>
        /// <returns>A queue containing the float values in the order they appear in the XML.</returns>
        private static Queue<float> LoadQueue(XmlNode node)
        {
            Queue<float> queue = new Queue<float>();
            foreach (XmlNode item in node.ChildNodes)
            {
                queue.Enqueue(float.Parse(item.InnerText));
            }
            return queue;
        }
    }
}

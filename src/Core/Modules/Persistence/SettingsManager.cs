using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Xml;
using Bannerlord.ButterLib.Common.Extensions;
using HarmonyLib;
using Path = System.IO.Path;

namespace Bannerlord.UIEditor.Core
{
    public class SettingsManager : Module, ISettingsManager
    {
        public static ISettingsManager? Instance { get; private set; }

#if STANDALONE_EDITOR
        public static string ModDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), 
            "Mount and Blade II Bannerlord", "Configs", "UIEditor");
#else
        public static string ModDirectory = Path.Combine(Utilities.GetConfigsPath(), "UIEditor");
#endif

        public static string UIEditorSettingsFileName = "UIEditor.settings";

        private readonly ConcurrentDictionary<string, SettingCategory> m_SettingCategory = new();
        private bool m_ChangesPending;
        private bool m_IsDisposing;

        private readonly object m_Lock = new();
        private Thread? m_WorkerThread;

        private readonly Stopwatch m_SaveSettingsDelay = new();
        private ISubModuleEventNotifier m_SubModuleEventNotifier = null!;

        /// <inheritdoc/>
        public T? GetSetting<T>(string _categoryName, string _name, T? _defaultValue)
        {
            SettingCategory settingCategory = GetSettingCategoryInternal(_categoryName)!;
            return settingCategory.GetSetting(_name, _defaultValue);
        }

        /// <inheritdoc/>
        public T? GetSetting<T>(string _categoryName, string _name)
        {
            var settingCategory = GetSettingCategoryInternal(_categoryName, false);
            return settingCategory is null ? default : settingCategory.GetSetting<T>(_name);
        }

        /// <inheritdoc/>
        public void SetSetting<T>(string _categoryName, string _name, T? _setting)
        {
            SettingCategory settingCategory = GetSettingCategoryInternal(_categoryName)!;

            settingCategory.SetSetting(_name, _setting);
        }

        /// <inheritdoc/>
        public ISettingCategory GetSettingCategory(string _categoryName)
        {
            return GetSettingCategoryInternal(_categoryName)!;
        }

        public bool SettingCategoryExists(string _categoryName)
        {
            return m_SettingCategory.TryGetValue(_categoryName, out _);
        }

        public override void Load()
        {
            base.Load();

            m_SubModuleEventNotifier = PublicContainer.GetModule<ISubModuleEventNotifier>();
            m_SubModuleEventNotifier.ApplicationTick += OnTick;
        }

        public override void Unload()
        {
            m_SubModuleEventNotifier.ApplicationTick -= OnTick;
            foreach (var (_, settingCategory) in m_SettingCategory)
            {
                settingCategory.ChangesPending -= OnSettingCategoryChangesPending;
            }

            base.Unload();
        }

        protected override void Dispose(bool _disposing)
        {
            m_IsDisposing = true;
            lock (m_Lock)
            {
                if (m_ChangesPending && (!m_WorkerThread?.IsAlive ?? true))
                {
                    SaveSettings();
                }
            }

            base.Dispose(_disposing);
        }

        public override void Create(IPublicContainer _publicContainer)
        {
            base.Create(_publicContainer);

            Instance = this;
            LoadSettings();

            RegisterModule<ISettingsManager>();
        }

        public void SaveSettings()
        {
            try
            {
                Directory.CreateDirectory(ModDirectory);

                XmlDocument xmlDocument = new();

                XmlDeclaration xmlDeclaration = xmlDocument.CreateXmlDeclaration("1.0", "UTF-8", null);
                xmlDocument.InsertBefore(xmlDeclaration, xmlDocument.DocumentElement);
                XmlElement uiEditorConfigNode = xmlDocument.CreateElement("UIEditorConfig");
                xmlDocument.AppendChild(uiEditorConfigNode);
                XmlElement settingsNode = xmlDocument.CreateElement("Settings");
                uiEditorConfigNode.AppendChild(settingsNode);

                TypeDictionary typeDictionary = new();

                foreach (var (_, settingCategory) in m_SettingCategory)
                {
                    settingCategory?.Serialize(settingsNode, typeDictionary);
                }

                typeDictionary.Serialize(uiEditorConfigNode);

                xmlDocument.Save(Path.Combine(ModDirectory, UIEditorSettingsFileName));
            }
            catch (Exception e)
            {
                FileLog.Log(e.ToString());
                Trace.WriteLine(e.ToString());
            }
        }

        public void LoadSettings()
        {
            string fullPath = Path.Combine(ModDirectory, UIEditorSettingsFileName);
            if (!File.Exists(fullPath))
            {
                return;
            }

            try
            {
                m_SettingCategory.Clear();

                XmlDocument xmlDocument = new();
                xmlDocument.Load(fullPath);

                if (xmlDocument.DocumentElement is null)
                {
                    return;
                }

                List<XmlNode> documentChildren = xmlDocument.DocumentElement.ChildNodes.OfType<XmlNode>().ToList();

                var typeDictionaryNode = documentChildren.FirstOrDefault(_node => _node.Name.Equals(TypeDictionary.NodeName));
                TypeDictionary typeDictionary = typeDictionaryNode is not null ? TypeDictionary.Deserialize(typeDictionaryNode) : new TypeDictionary();

                var settingsNode = documentChildren.FirstOrDefault(_node => _node.Name.Equals("Settings"));
                if (settingsNode is not null)
                {
                    foreach (XmlNode childNode in settingsNode.ChildNodes.OfType<XmlNode>())
                    {
                        var settingCategory = SettingCategory.Deserialize(childNode, typeDictionary);
                        if (settingCategory is null)
                        {
                            continue;
                        }

                        m_SettingCategory.TryAdd(settingCategory.Id, settingCategory);
                    }
                }
            }
            catch (Exception e)
            {
                FileLog.Log(e.ToString());
                Trace.WriteLine(e.ToString());
            }
        }

        private SettingCategory? GetSettingCategoryInternal(string _categoryName, bool _createIfNotExist = true)
        {
            if (!m_SettingCategory.TryGetValue(_categoryName, out var settingCategory) && _createIfNotExist)
            {
                settingCategory = new SettingCategory(_categoryName);
                m_SettingCategory.TryAdd(_categoryName, settingCategory);
            }

            if (settingCategory is not null)
            {
                settingCategory.ChangesPending += OnSettingCategoryChangesPending;
            }

            return settingCategory;
        }

        private void OnSettingCategoryChangesPending(object _sender, EventArgs _e)
        {
            lock (m_Lock)
            {
                m_ChangesPending = true;
            }
        }

        private void OnTick(object _sender, float _e)
        {
            lock (m_Lock)
            {
                if (m_IsDisposing)
                {
                    return;
                }

                if (m_SaveSettingsDelay.IsRunning && m_SaveSettingsDelay.ElapsedMilliseconds > 5000)
                {
                    m_ChangesPending = false;
                    m_SaveSettingsDelay.Reset();
                    m_SaveSettingsDelay.Stop();
                    m_WorkerThread = new Thread(SaveSettings);
                    m_WorkerThread.Start();
                }
                else if (m_ChangesPending && (!m_WorkerThread?.IsAlive ?? true))
                {
                    m_SaveSettingsDelay.Start();
                }
            }
        }
    }
}

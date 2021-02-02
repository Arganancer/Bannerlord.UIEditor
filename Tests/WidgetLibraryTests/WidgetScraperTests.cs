using System.Linq;
using Bannerlord.UIEditor.WidgetLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TaleWorlds.GauntletUI;

namespace Bannerlord.UIEditor.WidgetLibraryTests
{
    [TestClass]
    public class WidgetScraperTests
    {
        #region Public Constants Statics

        [ClassInitialize]
        public static void Init(TestContext _context)
        {
            m_MockUIResourceManager = new MockUIResourceManager();
            m_MockGauntletLayer = new MockGauntletLayer(m_MockUIResourceManager, 0);
        }

        [ClassCleanup]
        public static void Cleanup()
        {
            m_MockUIResourceManager = null;
            m_MockGauntletLayer = null;
        }

        #endregion

        #region Static/Const Fields

        private static MockUIResourceManager m_MockUIResourceManager;
        private static MockGauntletLayer m_MockGauntletLayer;

        #endregion

        #region Public Methods

        [TestMethod]
        public void WidgetScraper_ScrapeAssembly_NoExceptionsThrown()
        {
            var assembly = typeof( Widget ).Assembly;
            var widgetTemplates = WidgetScraper.ScrapeAssembly(assembly);
            Assert.IsTrue(widgetTemplates.Any());
        }

        [TestMethod]
        public void WidgetScraper_CreateWidgetTemplateFromType_IsNotNull()
        {
            var widgetTemplate = WidgetScraper.CreateWidgetTemplateFromType(typeof( Widget ));
            Assert.IsNotNull(widgetTemplate);
            Assert.AreEqual(nameof( Widget ), widgetTemplate.Name);
            Assert.AreEqual(typeof( Widget ).Assembly, widgetTemplate.Owner);
        }

        [TestMethod]
        public void WidgetTemplate_CreateInstance_IsNotNull()
        {
            var widgetTemplate = WidgetScraper.CreateWidgetTemplateFromType(typeof( Widget ));
            var instance = widgetTemplate.CreateInstance(m_MockUIResourceManager.WidgetFactory, m_MockGauntletLayer.GauntletUIContext);
            Assert.IsNotNull(instance);
            Assert.AreEqual(nameof( Widget ), instance.Name);
            Assert.IsTrue(instance.Attributes.Any());
        }

        #endregion
    }
}

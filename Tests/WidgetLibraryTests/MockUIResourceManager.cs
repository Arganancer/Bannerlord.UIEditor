using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.Data;
using TaleWorlds.GauntletUI.PrefabSystem;
using TaleWorlds.Library;
using TaleWorlds.TwoDimension;

namespace Bannerlord.UIEditor.WidgetLibraryTests
{
    public class MockUIResourceManager
    {
        public ResourceDepot UIResourceDepot { get; }

        public WidgetFactory WidgetFactory { get; }

        public SpriteData SpriteData { get; }

        public BrushFactory BrushFactory { get; }

        public FontFactory FontFactory { get; }

        public TwoDimensionEngineResourceContext ResourceContext { get; }

        public MockUIResourceManager()
        {
            UIResourceDepot = new ResourceDepot(BasePath.Name);
            WidgetFactory = new WidgetFactory(UIResourceDepot, "Prefabs");
            WidgetFactory.PrefabExtensionContext.AddExtension(new PrefabDatabindingExtension());
            WidgetFactory.Initialize();
            SpriteData = new SpriteData("SpriteData");
            SpriteData.Load(UIResourceDepot);
            //FontFactory = new FontFactory(UIResourceDepot);
            //FontFactory.LoadAllFonts(SpriteData);
            //BrushFactory = new BrushFactory(UIResourceDepot, "Brushes", SpriteData, FontFactory);
            //BrushFactory.Initialize();
            ResourceContext = new TwoDimensionEngineResourceContext();
        }
    }
}

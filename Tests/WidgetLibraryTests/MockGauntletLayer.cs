using System;
using System.Collections.Generic;
using TaleWorlds.Engine;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.Engine.Screens;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.Data;
using TaleWorlds.GauntletUI.PrefabSystem;
using TaleWorlds.Library;
using TaleWorlds.TwoDimension;

namespace Bannerlord.UIEditor.WidgetLibraryTests
{
    public class MockGauntletLayer : ScreenLayer
    {
        #region Public Fields

        public readonly TwoDimensionView TwoDimensionView;
        public readonly UIContext GauntletUIContext;
        public readonly List<Tuple<GauntletMovie, ViewModel>> MoviesAndDatasources;
        public readonly TwoDimensionEnginePlatform TwoDimensionEnginePlatform;
        public readonly EngineInputService EngineInputService;
        public readonly WidgetFactory WidgetFactory;

        #endregion

        #region Constructors

        public MockGauntletLayer(MockUIResourceManager _resourceManager, int _localOrder, string _categoryId = "GauntletLayer") : base(_localOrder, _categoryId)
        {
            MoviesAndDatasources = new List<Tuple<GauntletMovie, ViewModel>>();
            WidgetFactory = _resourceManager.WidgetFactory;
            var uiResourceDepot = _resourceManager.UIResourceDepot;
            TwoDimensionView = null;
            TwoDimensionEnginePlatform = new TwoDimensionEnginePlatform(TwoDimensionView);
            var twoDimensionContext = new TwoDimensionContext(TwoDimensionEnginePlatform, UIResourceManager.ResourceContext, uiResourceDepot);
            EngineInputService = new EngineInputService(Input);
            GauntletUIContext = new UIContext(twoDimensionContext, Input, EngineInputService, UIResourceManager.SpriteData, UIResourceManager.FontFactory, UIResourceManager.BrushFactory) {ScaleModifier = Scale};
            GauntletUIContext.Initialize();
            MouseEnabled = true;
        }

        #endregion
    }
}

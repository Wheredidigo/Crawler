using Buddy.Overlay;
using Buddy.Overlay.Controls;
using Crawler.Gui.Views;
using Crawler.Settings;
using ff14bot;

namespace Crawler.Gui.Overlays
{
    public class CrawlerOverlay
    {
        private static readonly CrawlerOverlayUiComponent CrawlerOverlayComponent = new CrawlerOverlayUiComponent();
        public static void Start()
        {
            if (!Core.OverlayManager.IsActive)
            {
                Core.OverlayManager.Activate();
            }

            Core.OverlayManager.AddUIComponent(CrawlerOverlayComponent);
        }

        public static void Stop()
        {
            if (!Core.OverlayManager.IsActive)
                return;

            Core.OverlayManager.RemoveUIComponent(CrawlerOverlayComponent);
        }

        internal class CrawlerOverlayUiComponent : OverlayUIComponent
        {
            public CrawlerOverlayUiComponent(): base(true) { }
            private OverlayControl _control;

            public override OverlayControl Control
            {
                get
                {
                    if (_control != null) return _control;
                    var overlayUc = new Overlay();
                    _control = new OverlayControl
                    {
                        Name = "CrawlerOverlay",
                        Content = overlayUc,
                        X = CrawlerSettings.Instance.OverlayPosX,
                        Y = CrawlerSettings.Instance.OverlayPosY,
                        AllowMoving = true
                    };

                    _control.MouseLeave += (sender, args) =>
                    {
                        CrawlerSettings.Instance.OverlayPosX = _control.X;
                        CrawlerSettings.Instance.OverlayPosY = _control.Y;
                        CrawlerSettings.Instance.Save();
                    };

                    _control.MouseLeftButtonDown += (sender, args) =>
                    {
                        _control.DragMove();
                    };
                    return _control;
                }
            }
        }
    }
}
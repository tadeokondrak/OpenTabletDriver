using System;
using System.Collections.Generic;
using System.Linq;
using TabletDriverLib.Interop;
using TabletDriverPlugin;
using TabletDriverPlugin.Attributes;
using TabletDriverPlugin.Platform.Pointer;
using TabletDriverPlugin.Tablet;

namespace TabletDriverLib.Output
{
    [PluginName("Relative Mode")]
    public class RelativeMode : BindingHandler, IRelativeMode
    {
        public float XSensitivity { set; get; }
        public float YSensitivity { set; get; }
        public TimeSpan ResetTime { set; get; } = TimeSpan.FromMilliseconds(100);
        
        private IEnumerable<IFilter> _filters, _preFilters, _postFilters;
        public IEnumerable<IFilter> Filters
        {
            set
            {
                _filters = value;
                _preFilters = value.Where(f => f.FilterStage == FilterStage.PreTranspose);
                _postFilters = value.Where(f => f.FilterStage == FilterStage.PostTranspose);
            }
            get => _filters;
        }

        private ICursorHandler CursorHandler { set; get; } = Platform.CursorHandler;
        private ITabletReport _lastReport;
        private DateTime _lastReceived;
        private Point _lastPosition;

        public void Read(IDeviceReport report)
        {
            if (report is ITabletReport tabletReport)
                Position(tabletReport);
        }

        public void Position(ITabletReport report)
        {
            if (TabletProperties.ActiveReportID != 0 && report.ReportID <= TabletProperties.ActiveReportID)
                return;
            
            var difference = DateTime.Now - _lastReceived;
            if (difference > ResetTime && _lastReceived != DateTime.MinValue)
            {
                _lastReport = null;
                _lastPosition = null;
            }

            if (_lastReport != null)
            {
                var pos = new Point(report.Position.X - _lastReport.Position.X, report.Position.Y - _lastReport.Position.Y);

                // Pre Filter
                foreach (IFilter filter in _preFilters)
                    pos = filter.Filter(pos);
                
                // Normalize (ratio of 1)
                pos.X /= TabletProperties.MaxX;
                pos.Y /= TabletProperties.MaxY;

                // Scale to tablet dimensions (mm)
                pos.X *= TabletProperties.Width;
                pos.Y *= TabletProperties.Height;

                // Sensitivity setting
                pos.X *= XSensitivity;
                pos.Y *= YSensitivity;
                
                // Translate by cursor position
                pos += GetCursorPosition();

                // Post Filter
                foreach (IFilter filter in _postFilters)
                    pos = filter.Filter(pos);

                CursorHandler.SetCursorPosition(pos);
                _lastPosition = pos;
            }
            
            _lastReport = report;
            _lastReceived = DateTime.Now;
        }

        private Point GetCursorPosition()
        {
            if (_lastPosition != null)
                return _lastPosition;
            else
                return CursorHandler.GetCursorPosition();
        }
    }
}
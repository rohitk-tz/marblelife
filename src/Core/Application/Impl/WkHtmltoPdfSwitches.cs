// Copyright 2019 Zone Defense LLC
using System;

namespace Core.Application.Impl
{
    public class WkHtmltoPdfSwitches
    {
        /// <summary>
        /// Set Margin Top. Specify in mm
        /// </summary>
        public decimal MarginTop { get; set; }

        /// <summary>
        /// Set Margin Bottom. Specify in mm
        /// </summary>
        public decimal MarginBottom { get; set; }

        /// <summary>
        /// Set Margin Left. Specify in mm
        /// </summary>
        public decimal MarginLeft { get; set; }

        /// <summary>
        /// Set Margin Right. Specify in mm
        /// </summary>
        public decimal MarginRight { get; set; }

        /// <summary>
        /// Page Size. Values like 'Letter', 'A4' etc.
        /// </summary>
        public string PageSize { get; set; }

        /// <summary>
        /// Specifies the delay (in milliseconds), after which HTML is considered for PDF conversion.
        /// </summary>
        public long RedirectDelay { get; set; }

        /// <summary>
        /// Orientation as in 'Landscape'/ 'Portrait'
        /// </summary>
        public string Orientation { get; set; }

        public int PageWidth { get; set; }

        public int PageHeight { get; set; }

        public string FooterUrl { get; set; }


        public override string ToString()
        {
            var pageSize = !string.IsNullOrEmpty(PageSize)
                               ? String.Concat("mm --page-size ", PageSize)
                               : String.Concat("mm --page-width ", PageWidth, "mm --page-height ", PageHeight, "mm");
            return String.Concat("--print-media-type --orientation ", Orientation, " --margin-top ", MarginTop, "mm --margin-bottom ", MarginBottom, "mm --margin-right ", MarginRight, "mm --margin-left ", MarginLeft, pageSize, " --header-spacing 5 --footer-spacing 5 --javascript-delay 5000");
        }

        public WkHtmltoPdfSwitches()
        {
            MarginTop = 10;
            MarginBottom = 10;
            MarginLeft = 10;
            MarginRight = 10;
            PageSize = "A4";
            RedirectDelay = 200;
            Orientation = "Portrait";
        }

    }
}

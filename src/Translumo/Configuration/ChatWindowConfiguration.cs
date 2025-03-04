﻿using System.Windows.Media;
using Translumo.Utils;

namespace Translumo.Configuration
{
    public class ChatWindowConfiguration : BindableBase
    {
        public Color BackgroundColor
        {
            get => _backgroundColor; 
            set
            {
                SetProperty(ref _backgroundColor, value);
            }
        }

        public Color FontColor
        {
            get => _fontColor;
            set
            {
                SetProperty(ref _fontColor, value);
            }
        }

        public float BackgroundOpacity
        {
            get => _backgroundOpacity;
            set
            {
                SetProperty(ref _backgroundOpacity, value);
            }
        }

        public int FontSize
        {
            get => _fontSize;
            set
            {
                SetProperty(ref _fontSize, value);
            }
        }

        public bool FontBold
        {
            get => _fontBold;
            set
            {
                SetProperty(ref _fontBold, value);
            }
        }

        public int LineSpacing
        {
            get => _lineSpacing;
            set
            {
                SetProperty(ref _lineSpacing, value);
            }
        }

        public static ChatWindowConfiguration Default => new()
        {
            BackgroundColor = Color.FromRgb(0, 0, 0),
            FontColor = Color.FromRgb(255, 128, 0),
            BackgroundOpacity = 0.2f,
            FontSize = 15,
            FontBold = true,
            LineSpacing = 14
        };

        private Color _backgroundColor;
        private Color _fontColor;
        private float _backgroundOpacity;
        private int _fontSize;
        private bool _fontBold;
        private int _lineSpacing;
    }
}

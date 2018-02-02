﻿using Mapsui.Widgets;
using SkiaSharp;
using System;

namespace Mapsui.Rendering.Skia
{
    public static class HyperlinkWidgetRenderer
    {
        public static void Draw(SKCanvas canvas, double screenWidth, double screenHeight, Hyperlink hyperlink,
            float layerOpacity)
        {
            if (string.IsNullOrEmpty(hyperlink.Text)) return;
            var textPaint = new SKPaint { Color = hyperlink.TextColor.ToSkia(layerOpacity), IsAntialias = true };
            var backPaint = new SKPaint { Color = hyperlink.BackColor.ToSkia(layerOpacity), };
            // The textRect has an offset which can be confusing. 
            // This is because DrawText's origin is the baseline of the text, not the bottom.
            // Read more here: https://developer.xamarin.com/guides/xamarin-forms/advanced/skiasharp/basics/text/
            var textRect = new SKRect();
            textPaint.MeasureText(hyperlink.Text, ref textRect);
            // The backRect is straight forward. It is leading for our purpose.
            var backRect = new SKRect(0, 0,
                textRect.Width + hyperlink.PaddingX * 2,
                textPaint.TextSize + hyperlink.PaddingY * 2); // Use the font's TextSize for consistency
            var offsetX = GetOffsetX(backRect.Width, hyperlink.MarginX, hyperlink.HorizontalAlignment, hyperlink.PositionX, screenWidth);
            var offsetY = GetOffsetY(backRect.Height, hyperlink.MarginY, hyperlink.VerticalAlignment, hyperlink.PositionY, screenHeight);
            backRect.Offset(offsetX, offsetY);
            canvas.DrawRoundRect(backRect, hyperlink.CornerRadius, hyperlink.CornerRadius, backPaint);
            hyperlink.Envelope = backRect.ToMapsui();
            // To position the text within the backRect correct using the textRect's offset.
            canvas.DrawText(hyperlink.Text,
                offsetX - textRect.Left + hyperlink.PaddingX,
                offsetY - textRect.Top + hyperlink.PaddingY, textPaint);
        }

        private static float GetOffsetX(float width, float offsetX, HorizontalAlignment horizontalAlignment, double position, double screenWidth)
        {
            if (horizontalAlignment == HorizontalAlignment.Left) return offsetX;
            if (horizontalAlignment == HorizontalAlignment.Right) return (float)(screenWidth - width - offsetX);
            if (horizontalAlignment == HorizontalAlignment.Center) return (float)(screenWidth * 0.5 - width * 0.5); // ignore offset
            if (horizontalAlignment == HorizontalAlignment.Position) return (float)position;
            throw new Exception($"Unknown {nameof(HorizontalAlignment)} type");
        }

        private static float GetOffsetY(float height, float offsetY, VerticalAlignment verticalAlignment, double position, double screenHeight)
        {
            if (verticalAlignment == VerticalAlignment.Top) return offsetY;
            if (verticalAlignment == VerticalAlignment.Bottom) return (float)(screenHeight - height - offsetY);
            if (verticalAlignment == VerticalAlignment.Center) return (float)(screenHeight * 0.5 - height * 0.5); // ignore offset
            if (verticalAlignment == VerticalAlignment.Position) return (float)position;
            throw new Exception($"Unknown {nameof(VerticalAlignment)} type");
        }
    }
}
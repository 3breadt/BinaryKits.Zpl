namespace BinaryKits.Zpl.Viewer.ElementDrawers;

using BinaryKits.Zpl.Label;
using BinaryKits.Zpl.Label.Elements;
using BinaryKits.Zpl.Viewer.Helpers;

using SkiaSharp;
using SkiaSharp.HarfBuzz;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>
/// Drawer for Field Block elements
/// </summary>
public class FieldBlockElementDrawer : ElementDrawerBase
{
    ///<inheritdoc/>
    public override bool CanDraw(ZplElementBase element)
    {
        return element is ZplFieldBlock;
    }

    ///<inheritdoc/>
    public override bool IsReverseDraw(ZplElementBase element)
    {
        if (element is ZplFieldBlock fieldBlock)
        {
            return fieldBlock.ReversePrint;
        }

        return false;
    }

    ///<inheritdoc/>
    public override void Draw(ZplElementBase element, DrawerOptions options)
    {
        if (element is ZplFieldBlock fieldBlock)
        {
            var font = fieldBlock.Font;

            float fontSize = font.FontHeight > 0 ? font.FontHeight : font.FontWidth;
            var scaleX = 1.00f;
            if (font.FontWidth != 0 && font.FontWidth != fontSize)
            {
                scaleX *= (float)font.FontWidth / fontSize;
            }

            var typeface = options.FontLoader(font.FontName);
            var text = fieldBlock.Text;
            if (fieldBlock.UseHexadecimalIndicator)
            {
                text = text.ReplaceHexEscapes();
            }

            if (options.ReplaceDashWithEnDash)
            {
                text = text.Replace("-", " \u2013 ");
            }

            var skFont = new SKFont(typeface, fontSize, scaleX);
            using var skPaint = new SKPaint()
            {
                IsAntialias = options.Antialias
            };

            skFont.MeasureText("X", out var textBoundBaseline, skPaint);

            float x = fieldBlock.PositionX;
            float y = fieldBlock.PositionY + textBoundBaseline.Height;

            var textLines = WordWrap(text, skFont, fieldBlock.Width);
            var hangingIndent = 0;
            var lineHeight = fontSize + fieldBlock.LineSpace;

            // actual ZPL printer does not include trailing line spacing in total height
            var totalHeight = lineHeight * fieldBlock.MaxLineCount - fieldBlock.LineSpace;
            // labelary
            //var totalHeight = lineHeight * fieldBlock.MaxLineCount;

            if (fieldBlock.FieldTypeset != null)
            {
                totalHeight = lineHeight * (fieldBlock.MaxLineCount - 1) + textBoundBaseline.Height;
                y -= totalHeight;
            }

            using (new SKAutoCanvasRestore(this._skCanvas))
            {
                SKMatrix matrix = SKMatrix.Empty;

                if (fieldBlock.FieldOrigin != null)
                {
                    switch (fieldBlock.Font.FieldOrientation)
                    {
                        case FieldOrientation.Rotated90:
                            matrix = SKMatrix.CreateRotationDegrees(90, fieldBlock.PositionX + totalHeight / 2, fieldBlock.PositionY + totalHeight / 2);
                            break;
                        case FieldOrientation.Rotated180:
                            matrix = SKMatrix.CreateRotationDegrees(180, fieldBlock.PositionX + fieldBlock.Width / 2, fieldBlock.PositionY + totalHeight / 2);
                            break;
                        case FieldOrientation.Rotated270:
                            matrix = SKMatrix.CreateRotationDegrees(270, fieldBlock.PositionX + fieldBlock.Width / 2, fieldBlock.PositionY + fieldBlock.Width / 2);
                            break;
                        case FieldOrientation.Normal:
                            break;
                    }
                }
                else
                {
                    switch (fieldBlock.Font.FieldOrientation)
                    {
                        case FieldOrientation.Rotated90:
                            matrix = SKMatrix.CreateRotationDegrees(90, fieldBlock.PositionX, fieldBlock.PositionY);
                            break;
                        case FieldOrientation.Rotated180:
                            matrix = SKMatrix.CreateRotationDegrees(180, fieldBlock.PositionX, fieldBlock.PositionY);
                            break;
                        case FieldOrientation.Rotated270:
                            matrix = SKMatrix.CreateRotationDegrees(270, fieldBlock.PositionX, fieldBlock.PositionY);
                            break;
                        case FieldOrientation.Normal:
                            break;
                    }
                }

                if (matrix != SKMatrix.Empty)
                {
                    var currentMatrix = _skCanvas.TotalMatrix;
                    var concatMatrix = SKMatrix.Concat(currentMatrix, matrix);
                    this._skCanvas.SetMatrix(concatMatrix);
                }

                foreach (var textLine in textLines)
                {
                    x = fieldBlock.PositionX + hangingIndent;

                    skFont.MeasureText(textLine, out var textBounds, skPaint);
                    var diff = fieldBlock.Width - textBounds.Width;

                    switch (fieldBlock.TextJustification)
                    {
                        case TextJustification.Center:
                            x += diff / 2 - textBounds.Left;
                            break;
                        case TextJustification.Right:
                            x += diff - textBounds.Left * 2;
                            hangingIndent = -fieldBlock.HangingIndent;
                            break;
                        case TextJustification.Left:
                        case TextJustification.Justified:
                        default:
                            hangingIndent = fieldBlock.HangingIndent;
                            break;
                    }

                    if (fieldBlock.ReversePrint)
                    {
                        skPaint.BlendMode = SKBlendMode.Xor;
                    }

                    this._skCanvas.DrawShapedText(textLine, x, y, SKTextAlign.Left, skFont, skPaint);
                    y += lineHeight;
                }
            }
        }
    }

    private IEnumerable<string> WordWrap(string text, SKFont font, int maxWidth)
    {
        using var tmpPaint = new SKPaint();
        var spaceWidth = font.MeasureText(" ", tmpPaint);
        var lines = new List<string>();

        var words = new Stack<string>(text.Split([' '], StringSplitOptions.None).Reverse());
        var line = new StringBuilder();
        float width = 0;
        while (words.Any())
        {
            var word = words.Pop();
            if (word.Contains(@"\&"))
            {
                var subwords = word.Split(new[] { @"\&" }, 2, StringSplitOptions.None);
                word = subwords[0];
                words.Push(subwords[1]);
                var wordWidth = font.MeasureText(word, tmpPaint);
                if (width + wordWidth <= maxWidth)
                {
                    line.Append(word);
                    lines.Add(line.ToString());
                    line = new StringBuilder();
                    width = 0;
                }
                else
                {
                    if (line.Length > 0)
                    {
                        lines.Add(line.ToString().Trim());
                    }
                    lines.Add(word.ToString());
                    line = new StringBuilder();
                    width = 0;
                }
            }
            else
            {
                var wordWidth = font.MeasureText(word, tmpPaint);
                if (width + wordWidth <= maxWidth)
                {
                    line.Append(word + " ");
                    width += wordWidth + spaceWidth;
                }
                else
                {
                    if (line.Length > 0)
                    {
                        lines.Add(line.ToString().Trim());
                    }
                    line = new StringBuilder(word + " ");
                    width = wordWidth + spaceWidth;
                }
            }
        }

        lines.Add(line.ToString().Trim());
        return lines;
    }

}

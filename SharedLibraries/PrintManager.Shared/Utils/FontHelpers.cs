using System;
using System.Windows;
using System.Windows.Media;

public static class FontHelpers
{
    public static FontFamily GetFontFamily(string s)
    {
        if (string.IsNullOrEmpty(s))
        {
            return new FontFamily("微软雅黑");
        }
        return new FontFamily(s);
    }
    public static FontWeight GetFontWeight(string s)
    {
        FontWeight fontWeight = FontWeights.Normal;
        if (string.IsNullOrEmpty(s)) { return fontWeight; }
        switch (s.Length)
        {
            case 4:
                if (s.Equals("Bold", StringComparison.OrdinalIgnoreCase))
                {
                    fontWeight = FontWeights.Bold;
                }

                if (s.Equals("Thin", StringComparison.OrdinalIgnoreCase))
                {
                    fontWeight = FontWeights.Thin;
                }

                break;
            case 5:
                if (s.Equals("Black", StringComparison.OrdinalIgnoreCase))
                {
                    fontWeight = FontWeights.Black;
                }

                if (s.Equals("Light", StringComparison.OrdinalIgnoreCase))
                {
                    fontWeight = FontWeights.Light;
                }

                if (s.Equals("Heavy", StringComparison.OrdinalIgnoreCase))
                {
                    fontWeight = FontWeights.Heavy;
                }

                break;
            case 6:
                if (s.Equals("Normal", StringComparison.OrdinalIgnoreCase))
                {
                    fontWeight = FontWeights.Normal;
                }

                if (s.Equals("Medium", StringComparison.OrdinalIgnoreCase))
                {
                    fontWeight = FontWeights.Medium;
                }

                break;
            case 7:
                if (s.Equals("Regular", StringComparison.OrdinalIgnoreCase))
                {
                    fontWeight = FontWeights.Regular;
                }

                break;
            case 8:
                if (s.Equals("SemiBold", StringComparison.OrdinalIgnoreCase))
                {
                    fontWeight = FontWeights.SemiBold;
                }

                if (s.Equals("DemiBold", StringComparison.OrdinalIgnoreCase))
                {
                    fontWeight = FontWeights.DemiBold;
                }

                break;
            case 9:
                if (s.Equals("ExtraBold", StringComparison.OrdinalIgnoreCase))
                {
                    fontWeight = FontWeights.ExtraBold;
                }

                if (s.Equals("UltraBold", StringComparison.OrdinalIgnoreCase))
                {
                    fontWeight = FontWeights.UltraBold;
                }

                break;
            case 10:
                if (s.Equals("ExtraLight", StringComparison.OrdinalIgnoreCase))
                {
                    fontWeight = FontWeights.ExtraLight;
                }

                if (s.Equals("UltraLight", StringComparison.OrdinalIgnoreCase))
                {
                    fontWeight = FontWeights.UltraLight;
                }

                if (s.Equals("ExtraBlack", StringComparison.OrdinalIgnoreCase))
                {
                    fontWeight = FontWeights.ExtraBlack;
                }

                if (s.Equals("UltraBlack", StringComparison.OrdinalIgnoreCase))
                {
                    fontWeight = FontWeights.UltraBlack;
                }

                break;
        }
        return fontWeight;
    }

    public static FontStyle GetFontStyle(string s)
    {
        FontStyle fontStyle = FontStyles.Normal;
        if(string.IsNullOrEmpty(s)) { return fontStyle; }

        if (s.Equals("Oblique", StringComparison.OrdinalIgnoreCase))
        {
            fontStyle = FontStyles.Oblique;
        }
        if (s.Equals("Italic", StringComparison.OrdinalIgnoreCase))
        {
            fontStyle = FontStyles.Italic;
        }

        return fontStyle;
    }
}
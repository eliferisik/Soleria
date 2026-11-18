using System.Globalization;

namespace Soleria.Web.Services;

public static class Bicim
{
    private static readonly CultureInfo Tr = new("tr-TR");

    /// <summary>1799.9 → "1.799,90 TL"</summary>
    public static string Tl(this decimal tutar) => tutar.ToString("N2", Tr) + " TL";

    public static CultureInfo TrKultur => Tr;
}

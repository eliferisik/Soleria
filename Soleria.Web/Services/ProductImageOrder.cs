using Soleria.Web.Models;

namespace Soleria.Web.Services;

public static class ProductImageOrder
{
    private static readonly HashSet<string> RequestedProducts = new(StringComparer.Ordinal)
    {
        "Kız Çocuk Hakiki Siyah Deri Bot",
        "Kız Çocuk Cat Hakiki Deri Hakiki Kürklü Suya Dayanıklı Bot Filet",
        "Kız Çocuk Hakiki Deri Suya Dayanıklı Siyah Postal",
        "Kız Çocuk Siyah Simli Hakiki Deri Anatomik Günlük Bot",
        "Kız Çocuk Siyah Kalp Tokalı Fermuarlı Bot",
        "Kız Çocuk Mavi Hakiki Deri Suya Dayanıklı Anatomik İlk Adım Bot"
    };

    public static void SwapFirstTwo(IEnumerable<Product> products)
    {
        foreach (var product in products)
        {
            var extraImages = product.EkGorseller.Split(
                '|', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            if (extraImages.Length == 0) continue;

            var firstImage = product.GorselYolu;
            product.GorselYolu = extraImages[0];
            product.EkGorseller = string.Join('|', new[] { firstImage }.Concat(extraImages.Skip(1)));
        }
    }

    public static void SwapRequestedProducts(IEnumerable<Product> products) =>
        SwapFirstTwo(products.Where(product => RequestedProducts.Contains(product.Ad)));
}

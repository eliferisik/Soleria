using Microsoft.EntityFrameworkCore;
using Soleria.Web.Models;

namespace Soleria.Web.Data;

/// <summary>
/// Başlangıç verisi: kategoriler ve mevcut siteden gelen 12 ürün.
/// Yalnızca ilgili tablo boşsa çalışır (tekrar çalıştırmak güvenlidir).
/// </summary>
public static class DbSeeder
{
    private const string KadinBedenler = "36,37,38,39,40,41";
    private const string CocukBedenler = "26,27,28,29,30,31,32,33,34,35";
    private const string ErkekBedenler = "40,41,42,43,44,45";

    private static readonly (string Grup, string Tur, string Slug)[] Kategoriler =
    {
        ("Kız Çocuk", "Bot & Çizme", "kiz-cocuk-bot-cizme"),
        ("Kız Çocuk", "Spor Ayakkabı", "kiz-cocuk-spor-ayakkabi"),
        ("Kız Çocuk", "Terlik", "kiz-cocuk-terlik"),
        ("Kız Çocuk", "Babet", "kiz-cocuk-babet"),
        ("Kız Çocuk", "Klasik Ayakkabı", "kiz-cocuk-klasik-ayakkabi"),
        ("Kadın", "Spor Ayakkabı", "kadin-spor-ayakkabi"),
        ("Kadın", "Bot & Çizme", "kadin-bot-cizme"),
        ("Kadın", "Terlik", "kadin-terlik"),
        ("Kadın", "Babet", "kadin-babet"),
        ("Kadın", "Klasik Ayakkabı", "kadin-klasik-ayakkabi"),
        ("Kadın", "Topuklu Ayakkabı", "kadin-topuklu-ayakkabi"),
        ("Erkek Çocuk", "Spor Ayakkabı", "erkek-cocuk-spor-ayakkabi"),
        ("Erkek Çocuk", "Bot & Çizme", "erkek-cocuk-bot-cizme"),
        ("Erkek Çocuk", "Terlik", "erkek-cocuk-terlik"),
        ("Erkek Çocuk", "Babet", "erkek-cocuk-babet"),
        ("Erkek Çocuk", "Klasik Ayakkabı", "erkek-cocuk-klasik-ayakkabi"),
        ("Erkek", "Spor Ayakkabı", "erkek-spor-ayakkabi"),
        ("Erkek", "Bot & Çizme", "erkek-bot-cizme"),
        ("Erkek", "Terlik", "erkek-terlik"),
        ("Erkek", "Babet", "erkek-babet"),
        ("Erkek", "Klasik Ayakkabı", "erkek-klasik-ayakkabi"),
    };

    public static async Task SeedAsync(AppDbContext db)
    {
        if (!await db.Categories.AnyAsync())
        {
            var sira = 1;
            foreach (var (grup, tur, slug) in Kategoriler)
            {
                db.Categories.Add(new Category { Grup = grup, Ad = $"{grup} {tur}", Slug = slug, Sira = sira++ });
            }
            await db.SaveChangesAsync();
        }

        if (!await db.Products.AnyAsync())
        {
            var kat = await db.Categories.ToDictionaryAsync(c => c.Slug, c => c.Id);

            Product P(string kategori, string ad, decimal fiyat, string gorsel, string bedenler,
                      string aciklama, params string[] ozellikler) => new Product
            {
                CategoryId = kat[kategori],
                Ad = ad,
                Fiyat = fiyat,
                GorselYolu = "/images/" + gorsel,
                Bedenler = bedenler,
                Aciklama = aciklama,
                Ozellikler = string.Join('\n', ozellikler),
            };

            const string topuklu = "kadin-topuklu-ayakkabi";
            const string kizBot = "kiz-cocuk-bot-cizme";

            db.Products.AddRange(
                // ---- Kadın (anasayfa) ----
                P("kadin-spor-ayakkabi", "Kadın Leopar Desenli Bağcıklı Spor Ayakkabı", 1799.90m, "leoparspor1-1.jpg", KadinBedenler,
                  "Leopar desenli, bağcıklı ve ince tabanlı günlük spor ayakkabı.",
                  "Desen: Leopar", "Model: Bağcıklı Günlük Spor Ayakkabı", "Renk: Kahverengi / Siyah"),
                P(topuklu, "Kadın Arkası Açık Kare Burunlu Baretli Düşük Klasik Topuklu Ayakkabı", 999.90m, "urun11.jpeg", KadinBedenler,
                  "Arkası açık, kare burunlu ve baretli düşük topuklu klasik ayakkabı.",
                  "Burun Tipi: Kare Burun", "Topuk: Düşük", "Model: Arkası Açık, Baretli"),
                P(topuklu, "Kadın Kahverengi Süet Slingback Topuklu Ayakkabı", 1750m, "yenitopuk1-1.jpg", KadinBedenler,
                  "Kahverengi süet dokulu, sivri burunlu ve ince topuklu slingback ayakkabı.",
                  "Renk: Kahverengi", "Malzeme: Süet Görünümlü", "Model: Slingback"),
                P(topuklu, "Kadın Kahverengi Kalın Tabanlı Süet Spor Ayakkabı", 4200m, "kadınyeni3-1.jpg", KadinBedenler,
                  "Kahverengi süet görünümlü, kalın tabanlı ve geniş bağcıklı spor ayakkabı.",
                  "Renk: Kahverengi", "Malzeme: Süet Görünümlü", "Model: Kalın Tabanlı Spor Ayakkabı"),
                P("kadin-spor-ayakkabi", "Kadın Kahverengi Süet Günlük Spor Ayakkabı", 2000m, "kadınspor1-1.jpg", KadinBedenler,
                  "Kahverengi süet görünümlü, beyaz detaylı günlük spor ayakkabı.",
                  "Renk: Kahverengi", "Malzeme: Süet Görünümlü", "Model: Günlük Spor Ayakkabı"),
                P(topuklu, "Kadın Bordo Rugan 3 Tokalı Kemer Detaylı Topuklu Ayakkabı", 1900m, "urun6.jpg", KadinBedenler,
                  "Üç tokalı kemer detayıyla dikkat çeken bordo rugan topuklu ayakkabı.",
                  "Renk: Bordo", "Malzeme: Rugan", "Detay: 3 Tokalı Kemer"),

                // ---- Kız çocuk bot & çizme ----
                P(kizBot, "Kız Çocuk Hakiki Siyah Deri Bot", 2199.90m, "urun15.jpeg", CocukBedenler,
                  "Hakiki deriden üretilmiş siyah kız çocuk botu.",
                  "Malzeme: Hakiki Deri", "Renk: Siyah"),
                P(kizBot, "Kız Çocuk Cat Hakiki Deri Hakiki Kürklü Suya Dayanıklı Bot Filet", 2099.95m, "urun16.jpeg", CocukBedenler,
                  "Hakiki deri, hakiki kürk astarlı ve suya dayanıklı kız çocuk botu.",
                  "Marka: Cat", "Malzeme: Hakiki Deri", "Astar: Hakiki Kürk", "Özellik: Suya Dayanıklı"),
                P(kizBot, "Kız Çocuk Hakiki Deri Suya Dayanıklı Siyah Postal", 1920.90m, "urun17.jpeg", CocukBedenler,
                  "Hakiki deri, suya dayanıklı siyah kız çocuk postalı.",
                  "Malzeme: Hakiki Deri", "Renk: Siyah", "Özellik: Suya Dayanıklı"),
                P(kizBot, "Kız Çocuk Siyah Simli Hakiki Deri Anatomik Günlük Bot", 3499.90m, "urun18.jpeg", CocukBedenler,
                  "Simli detaylı, anatomik tabanlı günlük kız çocuk botu.",
                  "Malzeme: Hakiki Deri", "Renk: Siyah, Simli", "Taban: Anatomik"),
                P(kizBot, "Kız Çocuk Siyah Kalp Tokalı Fermuarlı Bot", 699.90m, "urun19.jpeg", CocukBedenler,
                  "Kalp tokalı ve fermuarlı siyah kız çocuk botu.",
                  "Renk: Siyah", "Detay: Kalp Toka", "Kapanış: Fermuar"),
                P(kizBot, "Kız Çocuk Mavi Hakiki Deri Suya Dayanıklı Anatomik İlk Adım Bot", 2499.90m, "urun20.jpeg", CocukBedenler,
                  "Anatomik tabanlı, suya dayanıklı mavi ilk adım botu.",
                  "Malzeme: Hakiki Deri", "Renk: Mavi", "Taban: Anatomik", "Özellik: Suya Dayanıklı")
            );
            await db.SaveChangesAsync();
        }

        // Yeni kadın ürünlerini mevcut katalog korunarak ekle.
        var yeniUrunler = new (string Slug, string Ad, decimal Fiyat, string Resim, string EkResim, string Aciklama)[]
        {
            ("kadin-topuklu-ayakkabi", "Siyah Saten Sivri Burun Topuklu Ayakkabı", 3299m, "yeni1-1.jpg", "yeni1.jpg", "Saten dokulu, zarif sivri burunlu topuklu ayakkabı."),
            ("kadin-topuklu-ayakkabi", "Siyah Beyaz Bant Detaylı Rugan Topuklu Ayakkabı", 3599m, "yeni2-1.jpg", "yeni2.jpg", "Kontrast bant ve rugan yüzeyiyle öne çıkan topuklu ayakkabı."),
            ("kadin-topuklu-ayakkabi", "Leopar Desenli Sivri Burun Topuklu Ayakkabı", 3799m, "yeni3-1.jpg", "yeni3.jpg", "Leopar desenli şık sivri burun topuklu ayakkabı."),
            ("kadin-topuklu-ayakkabi", "Bej Süet Görünümlü Kalın Topuklu Ayakkabı", 2899m, "yeni4-1.jpg", "yeni4.jpg", "Bej tonlarında, rahat kalın topuklu slingback model."),
            ("kadin-topuklu-ayakkabi", "Siyah Bilekten Bantlı Topuklu Ayakkabı", 3199m, "yeni5-1.jpg", "yeni5.jpg", "Siyah, bilekten bantlı ve sivri burunlu topuklu model."),
            ("kadin-topuklu-ayakkabi", "Siyah Sivri Burun İnce Topuklu Ayakkabı", 3399m, "yeni6-1.jpg", "yeni6.jpg", "Klasik siyah sivri burunlu ince topuklu ayakkabı."),
            ("kadin-topuklu-ayakkabi", "Altın Zincir Detaylı Siyah Topuklu Ayakkabı", 3999m, "yeni7-1.jpg", "yeni7.jpg", "Altın zincir detaylı siyah topuklu ayakkabı."),
            ("kiz-cocuk-bot-cizme", "Kız Çocuk Pembe Kürklü Kışlık Bot", 1899m, "kız1-1.jpg", "kız1.jpg", "Yumuşak astarlı, bağcıklı pembe kışlık çocuk botu."),
            ("kiz-cocuk-bot-cizme", "Kız Çocuk Taba Süet Görünümlü Bot", 1699m, "kız2-2.jpg", "kız2.jpg", "Taba tonlarında günlük kullanıma uygun çocuk botu."),
            ("kiz-cocuk-bot-cizme", "Kız Çocuk Siyah Bağcıklı Postal Bot", 1799m, "kız3-3.jpg", "kız3.jpg", "Kalın tabanlı siyah bağcıklı çocuk postal bot."),
            ("kiz-cocuk-babet", "Kız Çocuk Puantiyeli Babet", 899m, "kız4-4.jpg", "kız4.jpg", "Puantiyeli, fiyonk detaylı rahat çocuk babeti."),
            ("kiz-cocuk-klasik-ayakkabi", "Kız Çocuk Beyaz Günlük Ayakkabı", 999m, "kız5-5.jpg", "kız5.jpg", "Bağcıklı, sade tasarımlı günlük çocuk ayakkabısı."),
            ("kiz-cocuk-spor-ayakkabi", "Kız Çocuk Gümüş Renkli Spor Ayakkabı", 1299m, "kız6-1.jpg", "kız6.jpg", "Gümüş renkli, hafif tabanlı çocuk spor ayakkabısı."),
            ("kiz-cocuk-babet", "Kız Çocuk Bordo Babet Ayakkabı", 949m, "kız7-1.jpg", "kız7.jpg", "Bordo renkli, çapraz bantlı çocuk babet ayakkabı."),
            ("erkek-cocuk-bot-cizme", "Erkek Çocuk Bej Trekking Bot", 1899m, "cerkek1-1.jpg", "cerkek1.jgp.jpg", "Bej ve siyah renkli, bağcıklı çocuk botu."),
            ("erkek-cocuk-bot-cizme", "Erkek Çocuk Kahverengi Günlük Bot", 1699m, "cerkek2-2.jpg", "cerkek2.jpg", "Kahverengi günlük kullanıma uygun çocuk botu."),
            ("erkek-cocuk-bot-cizme", "Erkek Çocuk Siyah Süet Bilekli Bot", 1599m, "cerkek3-3.jpg", "cerkek3.jpg", "Siyah süet görünümlü bilekli çocuk botu."),
            ("erkek-cocuk-klasik-ayakkabi", "Erkek Çocuk Bordo Bağcıklı Klasik Ayakkabı", 1799m, "cerkek4-4.jpg", "cerkek4.jpg", "Bordo tonlarında bağcıklı klasik çocuk ayakkabısı."),
            ("erkek-cocuk-bot-cizme", "Erkek Çocuk Taba Bağcıklı Bot", 1899m, "cerkek5-1.jpg", "cerkek5.jpg", "Taba tonlarında bağcıklı çocuk botu."),
            ("erkek-cocuk-spor-ayakkabi", "Erkek Çocuk Mavi Günlük Spor Ayakkabı", 1399m, "cerkek6-1.jpg", "cerkek6.jpg", "Mavi ve siyah renkli günlük çocuk spor ayakkabısı."),
            ("erkek-cocuk-spor-ayakkabi", "Erkek Çocuk Mavi Süet Spor Ayakkabı", 1299m, "cerkek7-1.jpg", "cerkek7.jpg", "Mavi süet görünümlü bağcıklı çocuk spor ayakkabısı."),
            ("erkek-cocuk-spor-ayakkabi", "Erkek Çocuk Yeşil Halı Saha Ayakkabısı", 1499m, "cerkek8-1.jpg", "cerkek8.jpg", "Yeşil renkli, sportif tabanlı çocuk ayakkabısı."),
            ("erkek-spor-ayakkabi", "Erkek Siyah Deri Günlük Sneaker", 2799m, "erkek1-1.jpg", "erkek1.jpg", "Siyah deri yüzeyi ve rahat tabanıyla günlük kullanıma uygun bağcıklı sneaker."),
            ("erkek-spor-ayakkabi", "Erkek Kahverengi Süet Spor Ayakkabı", 2499m, "erkek2-1.jpg", "erkek2.jpg", "Kahverengi süet dokulu, kalın tabanlı günlük spor ayakkabı."),
            ("erkek-bot-cizme", "Erkek Taba Süet Chelsea Bot", 3299m, "erkek3-1.jpg", "erkek3.jpg", "Taba süet görünümlü, yanları lastikli klasik Chelsea bot."),
            ("erkek-klasik-ayakkabi", "Erkek Siyah Deri Loafer", 2999m, "erkek4-1.jpg", "erkek4.jpg", "Sade tasarımlı siyah loafer, klasik ve günlük kombinlere uygundur."),
            ("erkek-spor-ayakkabi", "Erkek Siyah Bağcıklı Günlük Ayakkabı", 2299m, "erkek5-1.jpg", "erkek5.jpg", "Minimal tasarımlı siyah bağcıklı günlük ayakkabı."),
            ("erkek-klasik-ayakkabi", "Erkek Kahverengi Süet Loafer", 2899m, "erkek6-1.jpg", "erkek6.jpg", "Kahverengi süet dokulu loafer, dişli tabanıyla rahat bir kullanım sunar."),
            ("erkek-spor-ayakkabi", "Erkek Kahverengi Süet Sneaker", 2699m, "erkek7-1.jpg", "erkek7.jpg", "Koyu kahverengi süet detaylı, kontrast bağcıklı günlük sneaker."),
            ("erkek-spor-ayakkabi", "Erkek Mavi Denim Detaylı Spor Ayakkabı", 2599m, "erkek8-1.jpg", "erkek8.jpg", "Beyaz ve mavi denim görünümlü panelleriyle sportif bağcıklı ayakkabı.")
        };
        foreach (var item in yeniUrunler)
        {
            var mevcut = await db.Products.FirstOrDefaultAsync(p => p.Ad == item.Ad);
            if (mevcut != null)
            {
                var guncelKategori = await db.Categories.Where(c => c.Slug == item.Slug).Select(c => c.Id).FirstAsync();
                mevcut.CategoryId = guncelKategori;
                mevcut.GorselYolu = "/images/" + item.Resim;
                mevcut.EkGorseller = "/images/" + item.EkResim;
                continue;
            }
            var kategoriId = await db.Categories.Where(c => c.Slug == item.Slug).Select(c => c.Id).FirstAsync();
            db.Products.Add(new Product
            {
                CategoryId = kategoriId, Ad = item.Ad, Fiyat = item.Fiyat,
                GorselYolu = "/images/" + item.Resim,
                EkGorseller = "/images/" + item.EkResim,
                Bedenler = item.Slug.StartsWith("kiz-cocuk", StringComparison.Ordinal) || item.Slug.StartsWith("erkek-cocuk", StringComparison.Ordinal)
                    ? CocukBedenler
                    : item.Slug.StartsWith("erkek-", StringComparison.Ordinal) ? ErkekBedenler : KadinBedenler,
                Aciklama = item.Aciklama,
                Ozellikler = item.Slug.StartsWith("kiz-cocuk", StringComparison.Ordinal) || item.Slug.StartsWith("erkek-cocuk", StringComparison.Ordinal)
                    ? $"Beden: 26-35\nKategori: {(item.Slug.StartsWith("erkek-cocuk", StringComparison.Ordinal) ? "Erkek" : "Kız")} Çocuk Ayakkabı"
                    : item.Slug.StartsWith("erkek-", StringComparison.Ordinal)
                        ? "Renk: Siyah / Kahverengi / Taba\nBeden: 40-45\nKategori: Erkek Ayakkabı"
                        : "Renk: Siyah / Bej\nBeden: 36-41\nKategori: Kadın Ayakkabı"
            });
        }

        // Önceki katalogdaki sandalet kaydını yeni ürünle değiştir; sepet ve favori ilişkileri korunur.
        var eskiSandalet = await db.Products.FirstOrDefaultAsync(p => p.Ad == "Kadın Bilek Bantlı Topuklu Sandalet");
        if (eskiSandalet != null)
        {
            eskiSandalet.Ad = "Kadın Kahverengi Süet Slingback Topuklu Ayakkabı";
            eskiSandalet.CategoryId = await db.Categories.Where(c => c.Slug == "kadin-topuklu-ayakkabi").Select(c => c.Id).FirstAsync();
            eskiSandalet.Fiyat = 1750m;
            eskiSandalet.GorselYolu = "/images/yenitopuk1-1.jpg";
            eskiSandalet.EkGorseller = "/images/yenitopuk1.jpg";
            eskiSandalet.Bedenler = KadinBedenler;
            eskiSandalet.Aciklama = "Kahverengi süet dokulu, sivri burunlu ve ince topuklu slingback ayakkabı.";
            eskiSandalet.Ozellikler = "Renk: Kahverengi\nMalzeme: Süet Görünümlü\nModel: Slingback";
        }

        // Gönderilen yeni fotoğraflarla ürün kayıtlarını güncelle; eski ID'leri koru.
        var eskiBordoTopuklu = await db.Products.FirstOrDefaultAsync(p => p.Ad == "Kadın Bordo Rugan 3 Tokalı Kemer Detaylı Topuklu Ayakkabı");
        if (eskiBordoTopuklu != null)
        {
            eskiBordoTopuklu.Ad = "Kadın Siyah Deri Diz Üstü Çizme";
            eskiBordoTopuklu.CategoryId = await db.Categories.Where(c => c.Slug == "kadin-bot-cizme").Select(c => c.Id).FirstOrDefaultAsync();
            if (eskiBordoTopuklu.CategoryId == 0)
                eskiBordoTopuklu.CategoryId = await db.Categories.Where(c => c.Slug == "kadin-topuklu-ayakkabi").Select(c => c.Id).FirstAsync();
            eskiBordoTopuklu.Fiyat = 1900m;
            eskiBordoTopuklu.GorselYolu = "/images/kadınbot1.jpg";
            eskiBordoTopuklu.EkGorseller = "/images/kadınbot1-1.jpg";
            eskiBordoTopuklu.Bedenler = KadinBedenler;
            eskiBordoTopuklu.Aciklama = "Siyah deri görünümlü, sivri burunlu ve diz üstü uzun çizme.";
            eskiBordoTopuklu.Ozellikler = "Renk: Siyah\nModel: Diz Üstü Çizme\nMalzeme: Deri Görünümlü";
        }

        var eskiYves = await db.Products.FirstOrDefaultAsync(p => p.Ad == "Yves Saint Laurent Kadın Siyah Topuklu Ayakkabı");
        if (eskiYves != null)
        {
            eskiYves.Ad = "Kadın Kahverengi Kalın Tabanlı Süet Spor Ayakkabı";
            eskiYves.CategoryId = await db.Categories.Where(c => c.Slug == "kadin-spor-ayakkabi").Select(c => c.Id).FirstOrDefaultAsync();
            if (eskiYves.CategoryId == 0)
                eskiYves.CategoryId = await db.Categories.Where(c => c.Slug == "kadin-topuklu-ayakkabi").Select(c => c.Id).FirstAsync();
            eskiYves.GorselYolu = "/images/kadınyeni3-1.jpg";
            eskiYves.EkGorseller = "/images/kadınyeni3.jpg";
            eskiYves.Bedenler = KadinBedenler;
            eskiYves.Aciklama = "Kahverengi süet görünümlü, kalın tabanlı ve geniş bağcıklı spor ayakkabı.";
            eskiYves.Ozellikler = "Renk: Kahverengi\nMalzeme: Süet Görünümlü\nModel: Kalın Tabanlı Spor Ayakkabı";
        }

        var eskiWentes = await db.Products.FirstOrDefaultAsync(p => p.Ad == "Wentes Kadın Topuklu Ayakkabı Bordo Rugan");
        if (eskiWentes != null)
        {
            eskiWentes.Ad = "Kadın Kahverengi Süet Günlük Spor Ayakkabı";
            eskiWentes.CategoryId = await db.Categories.Where(c => c.Slug == "kadin-spor-ayakkabi").Select(c => c.Id).FirstAsync();
            eskiWentes.Fiyat = 2000m;
            eskiWentes.GorselYolu = "/images/kadınspor1-1.jpg";
            eskiWentes.EkGorseller = "/images/kadınspor1.jpg";
            eskiWentes.Bedenler = KadinBedenler;
            eskiWentes.Aciklama = "Kahverengi süet görünümlü, beyaz detaylı günlük spor ayakkabı.";
            eskiWentes.Ozellikler = "Renk: Kahverengi\nMalzeme: Süet Görünümlü\nModel: Günlük Spor Ayakkabı";
        }

        var eskiBilektenBaglamali = await db.Products.FirstOrDefaultAsync(p => p.Ad == "Kadın Bilekten Bağlamalı Rugan Topuklu Ayakkabı");
        if (eskiBilektenBaglamali != null)
        {
            eskiBilektenBaglamali.Ad = "Kadın Leopar Desenli Bağcıklı Spor Ayakkabı";
            eskiBilektenBaglamali.CategoryId = await db.Categories.Where(c => c.Slug == "kadin-spor-ayakkabi").Select(c => c.Id).FirstAsync();
            eskiBilektenBaglamali.Fiyat = 1799.90m;
            eskiBilektenBaglamali.GorselYolu = "/images/leoparspor1-1.jpg";
            eskiBilektenBaglamali.EkGorseller = "/images/leoparspor1.jpg";
            eskiBilektenBaglamali.Bedenler = KadinBedenler;
            eskiBilektenBaglamali.Aciklama = "Leopar desenli, bağcıklı ve ince tabanlı günlük spor ayakkabı.";
            eskiBilektenBaglamali.Ozellikler = "Desen: Leopar\nModel: Bağcıklı Günlük Spor Ayakkabı\nRenk: Kahverengi / Siyah";
        }

        var ekGorselEslesmeleri = new Dictionary<string, string>
        {
            ["Kadın Arkası Açık Kare Burunlu Baretli Düşük Klasik Topuklu Ayakkabı"] = "benekli-alt-yeni.png",
            ["Kız Çocuk Hakiki Siyah Deri Bot"] = "kızcocuk1.webp",
            ["Kız Çocuk Cat Hakiki Deri Hakiki Kürklü Suya Dayanıklı Bot Filet"] = "kızcocuk2.jpg",
            ["Kız Çocuk Hakiki Deri Suya Dayanıklı Siyah Postal"] = "kızcocuk3.jpg",
            ["Kız Çocuk Siyah Simli Hakiki Deri Anatomik Günlük Bot"] = "kızcocuk5.jpg",
            ["Kız Çocuk Mavi Hakiki Deri Suya Dayanıklı Anatomik İlk Adım Bot"] = "kızcocuk6.jpg",
            ["Erkek Çocuk Bej Trekking Bot"] = "cerkek1.jgp.jpg",
            ["Erkek Çocuk Kahverengi Günlük Bot"] = "cerkek2.jpg",
            ["Erkek Çocuk Siyah Süet Bilekli Bot"] = "cerkek3.jpg",
            ["Erkek Çocuk Bordo Bağcıklı Klasik Ayakkabı"] = "cerkek4.jpg",
            ["Erkek Çocuk Taba Bağcıklı Bot"] = "cerkek5.jpg",
            ["Erkek Çocuk Mavi Günlük Spor Ayakkabı"] = "cerkek6.jpg",
            ["Erkek Çocuk Mavi Süet Spor Ayakkabı"] = "cerkek7.jpg",
            ["Erkek Çocuk Yeşil Halı Saha Ayakkabısı"] = "cerkek8.jpg",
        };
        foreach (var (ad, gorsel) in ekGorselEslesmeleri)
        {
            var product = await db.Products.FirstOrDefaultAsync(p => p.Ad == ad);
            if (product != null) product.EkGorseller = "/images/" + gorsel;
        }
        await db.SaveChangesAsync();
    }
}

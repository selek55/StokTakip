using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StokTakip.Data;
using StokTakip.Models;

namespace StokTakip.Controllers
{
    public class StocksController : Controller
    {
        private readonly StokTakipContext _context;

        public StocksController(StokTakipContext context)
        {
            _context = context;
        }

        /*
         * metotlar 4 çeşittir
         * geri dönüşü olan ve olmayan
         * parametre alan ve almayan
         */

        // GET: Stocks
        // liste sayfamız
        public async Task<IActionResult> Index()
        {
            // stoklar db den çekiyor
            var stockList = await _context.Stock.ToListAsync();

            // Satış Gelirini Hesaplayan Döngümüz
            foreach (var item in stockList)
            {
                // SellingPrice null bir alandır
                // null olunca matetiksel işlemler yapamayız o yüzden null yerine 0 değerini atıyoruz
                item.SalesRevenue = ((item.SellingPrice ?? 0) - (item.BuyingPrice ?? 0)) * (item.Quantity ?? 0);
            }

            // Toplam Satış Gelirini Hesaplayan Döngümüz
            int? totalSalesRevenue = 0;
            for (int i = 0; i < stockList.Count; i++)
            {
                totalSalesRevenue += stockList[i].SalesRevenue;
            }

            // ön yüze view ile iletiyoruz
            ViewBag.TotalSalesRevenue = totalSalesRevenue;

            return View(stockList);
        }

        // GET: Stocks/Create
        // hiç bir paremetre ve stok göndermiyoruz 
        // create anında ön yüzden bize gelecek veriler
        public IActionResult Create()
        {
            return View();
        }

        // stok imajlarını kaydetmek için yazılan metot
        // ön yüzden alınan IFormFile nesnesini veri tabanı nesnesi olan Image nesnesine çeviriyor -> yardımcı metot
        Image? ToImage(IFormFile? image)
        {
            if (image == null)
                return null;

            // ön yüzden gelen imajı ramdan alıyoruz
            using var ms = new MemoryStream();
            image.CopyTo(ms);

            return new Image
            {
                ContentType = image.ContentType,
                Data = ms.ToArray(),
                FileName = image.FileName,
                Length = image.Length,
            };
        }

        // POST: Stocks/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        // Create anında ön yüzden gelen verileri karşılıyoruz
        // Stock parametresi ile karşılıyoruz Stock nesnemiz bizim veri tabanı nesnemiz
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Stock stock)
        {
            // ön yüzden gelen stok nesnemizde hata varmı kontrolü yapılır
            if (!ModelState.IsValid)
                // hata varsa stock nesnesi ön yüze geri gönderilir
                return View(stock);

            // hatayı bulup geri ne kadar önce döndürürsek 
            // kod okunabilirliği ve yönetimi o kadar çok olur

            // yeni bir Guid üretiyoruz
            stock.Id = Guid.NewGuid();

            //  ön yüzden alınan IFormFile nesnesini veri tabanı nesnesi olan Image nesnesine çeviriyor
            stock.ImageData = ToImage(stock.Image);

            // stoğu veri tabanına kaydediyoruz
            _context.Add(stock);

            // veri tabanına değişkenleri yazdıyoruz
            await _context.SaveChangesAsync();

            // herşey hatasız ise -> liste sayfamız gelir
            return RedirectToAction(nameof(Index));
        }

        // GET: Stocks/Details/5
        // Stoğun id sini alıp stoğu geri dönüyoruz
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null || _context.Stock == null)
            {
                return NotFound();
            }

            // stoğun db den çekiyoruz
            // identification yani id tekil numarasına göre
            var stock = await _context.Stock.FirstOrDefaultAsync(m => m.Id == id);
            if (stock == null)
            {
                // stok yoksa -> bulunamadı hatası dönüyoruz
                return NotFound();
            }

            return View(stock);
        }

        // GET: Stocks/Edit/5
        // Stoğun id sini alıp stoğu geri dönüyoruz -> detay sayfası gibi
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null || _context.Stock == null)
            {
                return NotFound();
            }

            var stock = await _context.Stock.FindAsync(id);
            if (stock == null)
            {
                return NotFound();
            }

            return View(stock);
        }

        // POST: Stocks/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        // Edit anında ön yüzden gelen verileri karşılıyoruz
        // Stock parametresi ile karşılıyoruz Stock nesnemiz bizim veri tabanı nesnemiz
        // Create sayfalama sayfamız gibi
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, Stock stock)
        {
            if (id != stock.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
                return View(stock);

            // stok olup olmadığı kontrolü yapılır
            if (!StockExists(stock.Id))
            {
                return NotFound();
            }

            try
            {
                stock.ImageData = ToImage(stock.Image);

                // stoğu veri tabanında güncelliyoruz
                _context.Update(stock);

                // veri tabanına değişkenleri yazdıyoruz
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StockExists(stock.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            // herşey hatasız ise -> liste sayfamız gelir
            return RedirectToAction(nameof(Index));
        }

        // GET: Stocks/Delete/5
        // Stoğun id sini alıp stoğu geri dönüyoruz -> detay sayfası gibi
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null || _context.Stock == null)
            {
                return NotFound();
            }

            var stock = await _context.Stock.FirstOrDefaultAsync(m => m.Id == id);
            if (stock == null)
            {
                return NotFound();
            }

            return View(stock);
        }

        // POST: Stocks/Delete/5
        // Stoğun id sini alıp bu sayfada stoğu siliyoruz
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            if (_context.Stock == null)
            {
                return Problem("Entity set 'StokTakipContext.Stock'  is null.");
            }

            var stock = await _context.Stock.FindAsync(id);
            if (stock != null)
            {
                // stok silme işlemi
                _context.Stock.Remove(stock);

                // veri tabanına değişkenleri yazdıyoruz
                await _context.SaveChangesAsync();
            }
             
            return RedirectToAction(nameof(Index));
        }

        // stok olup olmadığı kontrolü yapılır -> yardımcı metot
        private bool StockExists(Guid id)
        {
            return _context.Stock.Any(e => e.Id == id);
        }

        // herhangi bir hata olunca bu sayfaya yönlendirilir
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

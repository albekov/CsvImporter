using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using CsvImport.Database;
using CsvImport.Model;
using CsvImport.Web.ViewModels;

namespace CsvImport.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly Importer _importer = new Importer(MvcApplication.DbManager);

        public async Task<ActionResult> Index(int page = 1, int pageSize = 25, string sort = null)
        {
            var query = new Query(page, pageSize, sort.ParseSortString());
            var result = await MvcApplication.DbManager.Load(query);

            var vm = new PeopleListViewModel(result);

            return View(vm);
        }

        [HttpPost]
        public async Task<ActionResult> Import()
        {
            if (Request.Files.Count > 0)
            {
                var file = Request.Files[0];

                if ((file != null) && (file.ContentLength > 0))
                {
                    var fileName = Path.GetTempPath() + Guid.NewGuid() + ".csv";
                    file.SaveAs(fileName);

                    await _importer.ImportAsync(fileName);
                }
            }

            return RedirectToAction("Index");
        }

        public async Task<ActionResult> Export()
        {
            var csv = await _importer.ExportAsync();
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(csv));
            return File(stream, "text/csv", "export.csv");
        }

        public async Task<ActionResult> Edit(int id, int page = 1, string sort = "")
        {
            ViewBag.Page = page;
            ViewBag.SortString = sort;
            var record = await MvcApplication.DbManager.Load(id);
            return View(record);
        }

        [HttpPost]
        public async Task<ActionResult> Edit(People people, int page = 1, string sort = "")
        {
            await MvcApplication.DbManager.Update(people);
            return RedirectToAction("Index", new {page, sort});
        }

        public async Task<ActionResult> Delete(int id, int page = 1, string sort = "")
        {
            await MvcApplication.DbManager.Delete(id);
            return RedirectToAction("Index", new {page, sort});
        }
    }
}
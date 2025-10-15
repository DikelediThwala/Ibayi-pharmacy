using IBayiLibrary.Models.Domain;
using IBayiLibrary.Repository;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Mvc;

namespace ONT_PROJECT.Controllers
{
    public class PharmacistReportController : Controller
    {
        private readonly IPrescriptionLineRepository _prescriptionLineRepository;
        public PharmacistReportController(IPrescriptionLineRepository prescriptionLineRepository)
        {         
            _prescriptionLineRepository = prescriptionLineRepository;
        }

        public async Task<IActionResult> PharmacistGenerateReport(string groupBy = "FullName")
        {
            var data = await _prescriptionLineRepository.GenerateReport();
            ViewBag.GroupBy = groupBy;
            return View(data);
        }

        public async Task<IActionResult> DownloadPharmacistReport(string groupBy = "FullName")
        {
            var data = await _prescriptionLineRepository.GenerateReport();

            // Group data like your view
            IEnumerable<IGrouping<object, PrescriptionViewModel>> groupedData;
            if (groupBy == "MedicineName")
                groupedData = data.GroupBy(x => (object)x.MedicineName);
            else if (groupBy == "Schedule")
                groupedData = data.GroupBy(x => (object)x.Schedule);
            else
                groupedData = data.GroupBy(x => (object)x.FullName);

            using (var ms = new MemoryStream())
            {
                var document = new Document(PageSize.A4, 40, 40, 40, 40);
                PdfWriter.GetInstance(document, ms);
                document.Open();

                // Title
                var titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 16);
                var normalFont = FontFactory.GetFont(FontFactory.HELVETICA, 11);
                document.Add(new Paragraph("Pharmacist Prescription Report", titleFont));
                document.Add(new Paragraph("Period: 01/01/2025 - 31/12/2025", normalFont));
                document.Add(new Paragraph($"Grouped By: {groupBy}", normalFont));
                document.Add(new Paragraph("\n"));

                foreach (var group in groupedData)
                {
                    document.Add(new Paragraph($"{groupBy}: {group.Key}", FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12)));
                    document.Add(new Paragraph("\n"));

                    // Create table
                    PdfPTable table = new PdfPTable(4) { WidthPercentage = 100 };
                    table.AddCell("Date");
                    table.AddCell("Medication");
                    table.AddCell("Qty");
                    table.AddCell("Instructions");

                    foreach (var item in group)
                    {
                        table.AddCell(item.Date.ToString());
                        table.AddCell(item.MedicineName);
                        table.AddCell(item.Quantity.ToString());
                        table.AddCell(item.Instructions);
                    }

                    document.Add(table);
                    document.Add(new Paragraph("\n"));
                }

                document.Close();

                return File(ms.ToArray(), "application/pdf", $"PharmacistReport_{DateTime.Now:yyyyMMdd}.pdf");
            }
        }

    }
}

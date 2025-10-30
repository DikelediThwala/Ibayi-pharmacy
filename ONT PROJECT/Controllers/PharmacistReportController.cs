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
        private readonly IUserRepository _userRepository;
        public PharmacistReportController(IPrescriptionLineRepository prescriptionLineRepository, IUserRepository userRepository)
        {
            _prescriptionLineRepository = prescriptionLineRepository;
            _userRepository = userRepository;
        }

        public async Task<IActionResult> PharmacistGenerateReport(
             string groupBy = "FullName",
             DateTime? startDate = null,
             DateTime? endDate = null)
        {

            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId.HasValue)
            {
                var user = await _userRepository.GetByIdAsync(userId.Value);

                // if user found, build full name
                if (user != null)
                {
                    ViewBag.FullName = $"{user.FirstName} {user.LastName}";
                }
                else
                {
                    ViewBag.FullName = "Guest";
                }
            }
            else
            {
                ViewBag.FullName = "Guest";
            }

            if (startDate.HasValue && endDate.HasValue && startDate > endDate)
            {
                ModelState.AddModelError("", "Start date cannot be later than end date.");

                // Pass filters back to the view
                ViewBag.GroupBy = groupBy;
                ViewBag.StartDate = startDate;
                ViewBag.EndDate = endDate;

                // Return the view with empty or previously loaded data
                var emptyData = Enumerable.Empty<PrescriptionViewModel>();
                return View(emptyData);
            }


            // Get all report data
            var data = await _prescriptionLineRepository.GenerateReport();

            // Apply date filtering if both start and end dates are provided
            if (startDate.HasValue && endDate.HasValue)
            {
                data = data.Where(x => x.Date >= startDate.Value && x.Date <= endDate.Value);
            }
            else if (startDate.HasValue) // only start date
            {
                data = data.Where(x => x.Date >= startDate.Value);
            }
            else if (endDate.HasValue) // only end date
            {
                data = data.Where(x => x.Date <= endDate.Value);
            }

            // Pass filters to the view
            ViewBag.GroupBy = groupBy;
            ViewBag.StartDate = startDate;
            ViewBag.EndDate = endDate;

            return View(data);
        }


        public async Task<IActionResult> DownloadPharmacistReport(
    string groupBy = "FullName",
    DateTime? startDate = null,
    DateTime? endDate = null)
        {

            string fullName = "Guest"; // default

            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId.HasValue)
            {
                var user = await _userRepository.GetByIdAsync(userId.Value);
                if (user != null)
                    fullName = $"{user.FirstName} {user.LastName}";
            }

            var data = await _prescriptionLineRepository.GenerateReport();

            // Apply date filtering
            if (startDate.HasValue && endDate.HasValue)
            {
                data = data.Where(x => x.Date >= startDate.Value && x.Date <= endDate.Value);
            }
            else if (startDate.HasValue)
            {
                data = data.Where(x => x.Date >= startDate.Value);
            }
            else if (endDate.HasValue)
            {
                data = data.Where(x => x.Date <= endDate.Value);
            }

            // Group data like in the view
            IEnumerable<IGrouping<object, PrescriptionViewModel>> groupedData;
            if (groupBy == "MedicineName")
                groupedData = data.GroupBy(x => (object)x.MedicineName);
            else if (groupBy == "Schedule")
                groupedData = data.GroupBy(x => (object)x.Schedule);
            else
                groupedData = data.GroupBy(x => (object)x.FullName);

            // Calculate Grand Total
            var grandTotalQty = data.Sum(x => x.Quantity);

            using (var ms = new MemoryStream())
            {
                var document = new Document(PageSize.A4, 40, 40, 40, 40);
                PdfWriter.GetInstance(document, ms);
                document.Open();

                // Fonts
                var titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 16);
                var headerFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12);
                var normalFont = FontFactory.GetFont(FontFactory.HELVETICA, 11);
                var boldFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 11);

                // Title
                document.Add(new Paragraph("Pharmacist Prescription Report", titleFont));
                document.Add(new Paragraph($"Generated by: {fullName}", normalFont));
                document.Add(new Paragraph(
                    $"Period: {(startDate?.ToString("dd/MM/yyyy") ?? "Start")} - {(endDate?.ToString("dd/MM/yyyy") ?? "End")}",
                    normalFont
                ));
                document.Add(new Paragraph($"Grouped By: {groupBy}", normalFont));
                document.Add(new Paragraph("\n"));

                // --- Loop through grouped data ---
                foreach (var group in groupedData)
                {
                    document.Add(new Paragraph($"{groupBy}: {group.Key}", headerFont));
                    document.Add(new Paragraph("\n"));

                    // Create table
                    PdfPTable table = new PdfPTable(4) { WidthPercentage = 100 };
                    table.SetWidths(new float[] { 2f, 3f, 1f, 4f }); // column width ratio

                    // Header row
                    var headerBg = new BaseColor(230, 230, 230);
                    PdfPCell cellHeader(string text) => new PdfPCell(new Phrase(text, headerFont))
                    {
                        BackgroundColor = headerBg,
                        HorizontalAlignment = Element.ALIGN_CENTER
                    };

                    table.AddCell(cellHeader("Date"));
                    table.AddCell(cellHeader(groupBy == "MedicineName" ? "FullName" : "Medication"));
                    table.AddCell(cellHeader("Qty"));
                    table.AddCell(cellHeader("Instructions"));

                    // Rows
                    int subtotalQty = 0;
                    foreach (var item in group)
                    {
                        table.AddCell(new Phrase(item.Date.GetValueOrDefault().ToString("dd/MM/yyyy"), normalFont));
                        table.AddCell(new Phrase(groupBy == "MedicineName" ? (item.FullName ?? "") : (item.MedicineName ?? ""), normalFont));
                        table.AddCell(new Phrase(item.Quantity.ToString(), normalFont));
                        table.AddCell(new Phrase(item.Instructions ?? "", normalFont));

                        subtotalQty += item.Quantity;
                    }

                    // Subtotal row
                    PdfPCell subtotalLabel = new PdfPCell(new Phrase("Subtotal", boldFont))
                    {
                        Colspan = 2,
                        HorizontalAlignment = Element.ALIGN_RIGHT,
                        BackgroundColor = new BaseColor(240, 240, 240)
                    };
                    PdfPCell subtotalValue = new PdfPCell(new Phrase(subtotalQty.ToString(), boldFont))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        BackgroundColor = new BaseColor(240, 240, 240)
                    };

                    // Empty cell for last column
                    PdfPCell empty = new PdfPCell(new Phrase("")) { BackgroundColor = new BaseColor(240, 240, 240) };

                    table.AddCell(subtotalLabel);
                    table.AddCell(subtotalValue);
                    table.AddCell(empty);

                    document.Add(table);
                    document.Add(new Paragraph("\n"));
                }

                // --- Grand Total ---
                PdfPTable grandTable = new PdfPTable(2) { WidthPercentage = 40, HorizontalAlignment = Element.ALIGN_RIGHT };
                grandTable.SetWidths(new float[] { 2f, 1f });

                var grandLabel = new PdfPCell(new Phrase("Grand Total Quantity:", headerFont))
                {
                    HorizontalAlignment = Element.ALIGN_RIGHT,
                    BackgroundColor = new BaseColor(220, 220, 220)
                };
                var grandValue = new PdfPCell(new Phrase(grandTotalQty.ToString(), boldFont))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    BackgroundColor = new BaseColor(220, 220, 220)
                };

                grandTable.AddCell(grandLabel);
                grandTable.AddCell(grandValue);
                document.Add(grandTable);

                // Close and return PDF
                document.Close();

                return File(ms.ToArray(), "application/pdf", $"PharmacistReport_{DateTime.Now:yyyyMMdd}.pdf");
            }
        }
    }
}

using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ONT_PROJECT.Models;

namespace ONT_PROJECT.Controllers
{
    public class ManagerReportController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ManagerReportController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Manager Report Page
        public IActionResult Index()
        {
            var medicines = _context.Medicines.Include(m => m.Form)
                                              .Include(m => m.Supplier)
                                              .ToList();
            return View(medicines);
        }

        [HttpPost]
        public IActionResult GenerateReport(string GroupBy, List<int> SelectedMedications, DateTime ReportDate, string StockLevel)
        {
            var query = _context.Medicines.Include(m => m.Form)
                                          .Include(m => m.Supplier)
                                          .AsQueryable();

            if (SelectedMedications != null && SelectedMedications.Any() && !SelectedMedications.Contains(-1))
                query = query.Where(m => SelectedMedications.Contains(m.MedicineId));

            if (StockLevel == "low")
                query = query.Where(m => m.Quantity <= m.ReorderLevel);
            else if (StockLevel == "out")
                query = query.Where(m => m.Quantity == 0);

            var medicines = query.ToList();

            // If no medicines found, generate a PDF with a message
            if (!medicines.Any())
            {
                var emptyPdfBytes = Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Size(PageSizes.A4);
                        page.Margin(2, Unit.Centimetre);

                        // Header with logo and pharmacy name
                        page.Header().Row(row =>
                        {
                            row.ConstantItem(100).Image("wwwroot/images/logo_2-removebg-preview.png");
                            row.RelativeItem().Column(col =>
                            {
                                col.Item().Text("Ibhayi Pharmacy")
                                    .FontSize(24).Bold().FontColor(Colors.Blue.Darken2);
                                col.Item().Text($"Pharmacy Manager Report - {ReportDate:dd/MM/yyyy}")
                                    .FontSize(16).SemiBold();
                            });
                        });

                        // Message for empty report
                        page.Content().AlignCenter().Column(column =>
                        {
                            column.Item().Text("No medicines found for the selected criteria.")
                                  .FontSize(16).Bold().FontColor(Colors.Red.Medium); 
                        });


                        // Footer with page numbers
                        page.Footer().AlignCenter().Row(row =>
                        {
                            row.RelativeItem().Text(txt =>
                            {
                                txt.Span("Page ");
                                txt.CurrentPageNumber();
                                txt.Span(" / ");
                                txt.TotalPages();
                            });
                        });
                    });
                }).GeneratePdf();

                return File(emptyPdfBytes, "application/pdf", $"PharmacyManagerReport_{DateTime.Now:yyyyMMdd}.pdf");
            }

            // Group by selection
            IEnumerable<IGrouping<string, Medicine>> grouped = GroupBy switch
            {
                "DosageForm" => medicines.GroupBy(m => m.Form?.FormName ?? "Unknown"),
                "Schedule" => medicines.GroupBy(m => m.Schedule.ToString()),
                "Supplier" => medicines.GroupBy(m => m.Supplier?.Name ?? "Unknown"),
                _ => new List<IGrouping<string, Medicine>>() { medicines.GroupBy(m => "All").First() }
            };

            // Generate PDF for medicines
            var pdfBytes = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);

                    // Header with logo and pharmacy name
                    page.Header().Row(row =>
                    {
                        row.ConstantItem(100).Image("wwwroot/images/logo_2-removebg-preview.png");
                        row.RelativeItem().Column(col =>
                        {
                            col.Item().Text("Ibhayi Pharmacy")
                                .FontSize(24).Bold().FontColor(Colors.Blue.Darken2);
                            col.Item().Text($"Pharmacy Manager Report - {ReportDate:dd/MM/yyyy}")
                                .FontSize(16).SemiBold();
                        });
                    });

                    page.Content().PaddingVertical(10).Column(column =>
                    {
                        // Add dynamic heading based on GroupBy
                        string heading = GroupBy switch
                        {
                            "Supplier" => "Stock by Supplier",
                            "Schedule" => "Stock by Schedule",
                            "DosageForm" => "Stock by Dosage Form",
                            _ => "Stock Report"
                        };
                        column.Item().Container()
                             .PaddingBottom(10)
                             .Text(heading)
                             .FontSize(18)
                             .Bold()
                             .FontColor(Colors.Black)
                             .Underline();

                        foreach (var group in grouped)
                        {
                            // Group title
                            column.Item().Text($"{GroupBy}: {group.Key}")
                                .FontSize(14).Bold().FontColor(Colors.Black).Underline();

                            // Table
                            column.Item().Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.RelativeColumn(4);
                                    columns.RelativeColumn(1);
                                    columns.RelativeColumn(2);
                                    columns.RelativeColumn(2);
                                });

                                // Table Header
                                table.Header(header =>
                                {
                                    header.Cell().Background(Colors.Grey.Lighten2).BorderBottom(1).BorderColor(Colors.Black).Text("Medication").Bold();
                                    header.Cell().Background(Colors.Grey.Lighten2).BorderBottom(1).BorderColor(Colors.Black).Text("Schedule").Bold();
                                    header.Cell().Background(Colors.Grey.Lighten2).BorderBottom(1).BorderColor(Colors.Black).Text("Reorder Level").Bold();
                                    header.Cell().Background(Colors.Grey.Lighten2).BorderBottom(1).BorderColor(Colors.Black).Text("Quantity").Bold();
                                });

                                // Table Rows with alternating row colors
                                bool alternate = false;
                                foreach (var med in group)
                                {
                                    var bgColor = alternate ? Colors.Grey.Lighten5 : Colors.White;

                                    table.Cell().Background(bgColor).BorderBottom(0.5f).BorderColor(Colors.Grey.Medium).Text(med.MedicineName);
                                    table.Cell().Background(bgColor).BorderBottom(0.5f).BorderColor(Colors.Grey.Medium).Text(med.Schedule.ToString());
                                    table.Cell().Background(bgColor).BorderBottom(0.5f).BorderColor(Colors.Grey.Medium).Text(med.ReorderLevel.ToString());
                                    table.Cell().Background(bgColor).BorderBottom(0.5f).BorderColor(Colors.Grey.Medium).Text(med.Quantity.ToString());

                                    alternate = !alternate;
                                }

                                // Footer (sub-total)
                                table.Footer(footer =>
                                {
                                    footer.Cell().ColumnSpan(3).Background(Colors.Grey.Lighten2).BorderTop(1).BorderColor(Colors.Black).Text("Sub-total:").Bold();
                                    footer.Cell().Background(Colors.Grey.Lighten2).BorderTop(1).BorderColor(Colors.Black).Text(group.Sum(m => m.Quantity).ToString()).Bold();
                                });
                            });

                            // Space between groups
                            column.Item().PaddingBottom(10);
                        }
                    });

                    // Footer with page numbers
                    page.Footer().AlignCenter().Row(row =>
                    {
                        row.RelativeItem().Text(txt =>
                        {
                            txt.Span("Page ");
                            txt.CurrentPageNumber();
                            txt.Span(" / ");
                            txt.TotalPages();
                        });
                    });
                });
            }).GeneratePdf();

            return File(pdfBytes, "application/pdf", $"PharmacyManagerReport_{DateTime.Now:yyyyMMdd}.pdf");
        }
    }
}

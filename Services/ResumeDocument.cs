using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using RizeUp.DTOs;

namespace RizeUp.Documents
{

    public class ResumeGenerator
    {
        public byte[] GeneratePdf(ResumeDto resume)
        {
            QuestPDF.Settings.License = LicenseType.Community;

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(30);
                    page.DefaultTextStyle(x => x.FontSize(11).FontFamily("Calibri"));

                    page.Header()
                        .Column(col =>
                        {
                            col.Item().AlignCenter().Text($"{resume.FirstName} {resume.LastName}")
                                .Bold().FontSize(20);

                            col.Item().AlignCenter().Text(resume.Title ?? string.Empty)
                                .Italic().FontSize(14);

                            col.Item().PaddingVertical(5).LineHorizontal(1);
                        });

                    page.Content()
                        .PaddingVertical(15)
                        .Column(content =>
                        {
                            // Contact Information
                            content.Item().Row(row =>
                            {
                                row.RelativeItem().Component(new ContactSection(resume));
                                row.RelativeItem().Component(new LinkSection(resume));
                            });

                            // Summary
                            if (!string.IsNullOrEmpty(resume.Summary))
                            {
                                content.Item().PaddingTop(10).Text("Summary").Bold();
                                content.Item().Text(resume.Summary);
                            }

                            // Sections (Dynamically added)
                            AddSection(content, "Experience", resume.Experiences);
                            AddSection(content, "Education", resume.Educations);
                            AddSection(content, "Skills", resume.Skills);
                            AddSection(content, "Languages", resume.Languages);
                            AddSection(content, "Certificates", resume.Certificates);
                            AddSection(content, "Projects", resume.Projects);
                        });
                });
            });

            return document.GeneratePdf();
        }

        private void AddSection<T>(ColumnDescriptor column, string title, List<T>? items) where T : class
        {
            if (items == null || !items.Any()) return;

            column.Item().PaddingTop(10).Text(title).Bold();

            foreach (var item in items)
            {
                column.Item().PaddingTop(5).Component(new ListItemComponent<T>(item));
            }
        }
    }

    // Contact Information Component
    public class ContactSection : IComponent
    {
        private readonly ResumeDto _resume;

        public ContactSection(ResumeDto resume) => _resume = resume;

        public void Compose(IContainer container)
        {
            container.Column(column =>
            {
                if (!string.IsNullOrEmpty(_resume.Email))
                    column.Item().Text($"Email: {_resume.Email}");

                if (!string.IsNullOrEmpty(_resume.PhoneNumber))
                    column.Item().Text($"Phone: {_resume.PhoneNumber}");

                if (!string.IsNullOrEmpty(_resume.Address))
                    column.Item().Text($"Address: {_resume.Address}");
            });
        }
    }

    // Links Component
    public class LinkSection : IComponent
    {
        private readonly ResumeDto _resume;

        public LinkSection(ResumeDto resume) => _resume = resume;

        public void Compose(IContainer container)
        {
            container.AlignRight().Column(column =>
            {
                if (!string.IsNullOrEmpty(_resume.LinkedinLink))
                    column.Item().Text($"LinkedIn: {_resume.LinkedinLink}");

                if (!string.IsNullOrEmpty(_resume.GitHubLink))
                    column.Item().Text($"GitHub: {_resume.GitHubLink}");
            });
        }
    }

    // Dynamic List Item Renderer
    public class ListItemComponent<T> : IComponent where T : class
    {
        private readonly T _item;

        public ListItemComponent(T item) => _item = item;

        public void Compose(IContainer container)
        {
            switch (_item)
            {
                case ExperienceItem exp:
                    container.Column(col =>
                    {
                        col.Item().Text($"{exp.Title} | {exp.Company}").SemiBold();
                        col.Item().Text($"{FormatDate(exp.StartDate)} - {FormatDate(exp.EndDate, exp.IsCurrent)}");
                        if (!string.IsNullOrEmpty(exp.Duties))
                            col.Item().PaddingTop(3).Text(exp.Duties);
                    });
                    break;

                case EducationItem edu:
                    container.Column(col =>
                    {
                        col.Item().Text($"{edu.CollegeName}").SemiBold();
                        col.Item().Text($"{edu.DegreeType} in {edu.Major}");
                        col.Item().Text($"{FormatDate(edu.StartDate)} - {FormatDate(edu.EndDate)}");
                        if (edu.GPA.HasValue)
                            col.Item().Text($"GPA: {edu.GPA.Value}");
                    });
                    break;

                case SkillItem1 skill:
                    container.Text($"• {skill.SkillName} ({skill.SkillType})");
                    break;

                case LanguageItem lang:
                    container.Text($"• {lang.LanguageName} ({lang.Level})");
                    break;

                case CertificateItem cert:
                    container.Column(col =>
                    {
                        col.Item().Text($"{cert.Title}").SemiBold();
                        col.Item().Text($"{cert.ProviderName} - {cert.Field}");
                        col.Item().Text($"{FormatDate(cert.StartDate)} - {FormatDate(cert.EndDate)}");
                        if (cert.GPA.HasValue)
                            col.Item().Text($"Score: {cert.GPA.Value}");
                    });
                    break;

                case ProjectItem proj:
                    container.Column(col =>
                    {
                        col.Item().Text($"{proj.ProjectName}").SemiBold();
                        col.Item().Text($"{FormatDate(proj.StartDate)} - {FormatDate(proj.EndDate)}");
                        if (!string.IsNullOrEmpty(proj.ProjectLink))
                            col.Item().Text($"Link: {proj.ProjectLink}");
                        if (!string.IsNullOrEmpty(proj.ProjectDescription))
                            col.Item().PaddingTop(3).Text(proj.ProjectDescription);
                    });
                    break;

                default:
                    container.Text(_item.ToString()!);
                    break;
            }
        }

        private string FormatDate(string? date, bool? isCurrent = null)
        {
            if (string.IsNullOrWhiteSpace(date)) return "Present";
            if (isCurrent == true) return $"{date} - Present";
            return date!;
        }
    }
}
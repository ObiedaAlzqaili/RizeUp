using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using RizeUp.DTOs;
using RizeUp.Interfaces;
namespace RizeUp.Services.PdfGeneration;
public class ResumePdfGenerator : IResumePdfGenerator
{
    public byte[] GenerateResumePdf(ResumeDto resume)
    {
        QuestPDF.Settings.License = LicenseType.Community;
        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(30);
                page.DefaultTextStyle(x => x.FontSize(11).FontFamily("Calibri"));

                // Header with name and contact info
                page.Header().Component(new HeaderComponent(resume));
                // Main content
                page.Content().Component(new ContentComponent(resume));
            });
        });

        return document.GeneratePdf();
    }

    // Header Component
    private class HeaderComponent : IComponent
    {
        private readonly ResumeDto _resume;

        public HeaderComponent(ResumeDto resume) => _resume = resume;

        public void Compose(IContainer container)
        {
            container.Column(col =>
            {
                // Name
                col.Item().AlignCenter().Text($"{_resume.FirstName} {_resume.LastName}")
                    .Bold().FontSize(20);

                // Contact information in a single line
                var contactInfo = new List<string>();
                if (!string.IsNullOrEmpty(_resume.Email))
                    contactInfo.Add(_resume.Email);
                if (!string.IsNullOrEmpty(_resume.PhoneNumber))
                    contactInfo.Add(_resume.PhoneNumber);
                if (!string.IsNullOrEmpty(_resume.LinkedinLink))
                    contactInfo.Add($"LinkedIn: {_resume.LinkedinLink}");

                col.Item().AlignCenter().Text(string.Join(" | ", contactInfo))
                    .FontSize(10);

                col.Item().PaddingVertical(5).LineHorizontal(1);
            });
        }
    }

    // Main Content Component
    private class ContentComponent : IComponent
    {
        private readonly ResumeDto _resume;

        public ContentComponent(ResumeDto resume) => _resume = resume;

        public void Compose(IContainer container)
        {
            container.PaddingVertical(10).Column(col =>
            {
                // Summary Section
                AddSection(col, "Summary", _resume.Summary, false);

                // Education Section
                AddSection(col, "Education", _resume.Educations);

                // Experience & Projects Section
                col.Item().Text("Experience & Projects").Bold().FontSize(12);
                col.Item().PaddingBottom(5).LineHorizontal(0.5f);

                AddExperiences(col, _resume.Experiences);
                AddProjects(col, _resume.Projects);

                // Certificates Section
                AddSection(col, "Certificates", _resume.Certificates);

                // Skills Section
                AddSkillsSection(col);

                // Languages Section
                AddLanguagesSection(col);
            });
        }

        private void AddSection(ColumnDescriptor column, string title, string? content, bool addDivider = true)
        {
            if (string.IsNullOrEmpty(content)) return;

            column.Item().Text(title).Bold().FontSize(12);
            if (addDivider)
            {
                column.Item().PaddingBottom(5).LineHorizontal(0.5f);
            }
            column.Item().PaddingBottom(10).Text(content);
        }

        private void AddSection<T>(ColumnDescriptor column, string title, List<T>? items) where T : class
        {
            if (items == null || !items.Any()) return;

            column.Item().PaddingTop(10).Text(title).Bold().FontSize(12);
            column.Item().PaddingBottom(5).LineHorizontal(0.5f);

            foreach (var item in items)
            {
                column.Item().PaddingBottom(5).Component(new ListItemComponent<T>(item));
            }
        }

        private void AddExperiences(ColumnDescriptor column, List<ExperienceItem>? experiences)
        {
            if (experiences == null || !experiences.Any()) return;

            foreach (var exp in experiences)
            {
                column.Item().PaddingBottom(5).Component(new ExperienceComponent(exp));
            }
        }

        private void AddProjects(ColumnDescriptor column, List<ProjectItem>? projects)
        {
            if (projects == null || !projects.Any()) return;

            foreach (var proj in projects)
            {
                column.Item().PaddingBottom(5).Component(new ProjectComponent(proj));
            }
        }

        private void AddSkillsSection(ColumnDescriptor column)
        {
            if (_resume.Skills == null || !_resume.Skills.Any()) return;

            column.Item().PaddingTop(10).Text("Skills").Bold().FontSize(12);
            column.Item().PaddingBottom(5).LineHorizontal(0.5f);

            column.Item().Grid(grid =>
            {
                grid.Columns(3);
                foreach (var skill in _resume.Skills)
                {
                    grid.Item().Text($"• {skill.SkillName}");
                }
            });
        }

        private void AddLanguagesSection(ColumnDescriptor column)
        {
            if (_resume.Languages == null || !_resume.Languages.Any()) return;

            column.Item().PaddingTop(10).Text("Languages").Bold().FontSize(12);
            column.Item().PaddingBottom(5).LineHorizontal(0.5f);

            foreach (var lang in _resume.Languages)
            {
                column.Item().Text($"• {lang.LanguageName} ({lang.Level})");
            }
        }
    }

    // Experience Component
    private class ExperienceComponent : IComponent
    {
        private readonly ExperienceItem _experience;

        public ExperienceComponent(ExperienceItem experience) => _experience = experience;

        public void Compose(IContainer container)
        {
            container.Column(col =>
            {
                // Title and Date
                col.Item().Row(row =>
                {
                    row.RelativeItem().Text(_experience.Title).SemiBold();
                    row.ConstantItem(150).AlignRight().Text($"{FormatDate(_experience.StartDate)} - {FormatDate(_experience.EndDate, _experience.IsCurrent)}");
                });

                // Company
                col.Item().Text(_experience.Company).FontColor(Colors.Grey.Darken1);

                // Duties
                if (!string.IsNullOrEmpty(_experience.Duties))
                {
                    col.Item().PaddingTop(3).Text(_experience.Duties);
                }
            });
        }

        private string FormatDate(string? date, bool? isCurrent = null)
        {
            if (string.IsNullOrWhiteSpace(date)) return "Present";
            if (isCurrent == true) return $"{date} - Present";
            return date!;
        }
    }

    // Project Component
    private class ProjectComponent : IComponent
    {
        private readonly ProjectItem _project;

        public ProjectComponent(ProjectItem project) => _project = project;

        public void Compose(IContainer container)
        {
            container.Column(col =>
            {
                // Project Name and Date
                col.Item().Row(row =>
                {
                    row.RelativeItem().Text(_project.ProjectName).SemiBold();
                    row.ConstantItem(150).AlignRight().Text(FormatDate(_project.EndDate));
                });

                // Description
                if (!string.IsNullOrEmpty(_project.ProjectDescription))
                {
                    col.Item().PaddingTop(3).Text(_project.ProjectDescription);
                }

                // Link
                if (!string.IsNullOrEmpty(_project.ProjectLink))
                {
                    col.Item().Text($"Link: {_project.ProjectLink}").FontColor(Colors.Blue.Darken1);
                }
            });
        }

        private string FormatDate(string? date)
        {
            return string.IsNullOrWhiteSpace(date) ? "" : date;
        }
    }

    // Generic List Item Component (for Education and Certificates)
    private class ListItemComponent<T> : IComponent where T : class
    {
        private readonly T _item;

        public ListItemComponent(T item) => _item = item;

        public void Compose(IContainer container)
        {
            switch (_item)
            {
                case EducationItem edu:
                    RenderEducation(container, edu);
                    break;
                case CertificateItem cert:
                    RenderCertificate(container, cert);
                    break;
                default:
                    container.Text(_item.ToString()!);
                    break;
            }
        }

        private void RenderEducation(IContainer container, EducationItem edu)
        {
            container.Column(col =>
            {
                // University and Date
                col.Item().Row(row =>
                {
                    row.RelativeItem().Text(edu.CollegeName).SemiBold();
                    row.ConstantItem(150).AlignRight().Text($"{FormatDate(edu.StartDate)} - {FormatDate(edu.EndDate)}");
                });

                // Degree
                col.Item().Text($"{edu.Major} in {edu.Major}");

                // GPA
                if (edu.GPA.HasValue)
                {
                    col.Item().Text($"GPA: {edu.GPA.Value.ToString("0.00")}");
                }
            });
        }

        private void RenderCertificate(IContainer container, CertificateItem cert)
        {
            container.Column(col =>
            {
                // Certificate Name and Date
                col.Item().Row(row =>
                {
                    row.RelativeItem().Text(cert.Field).SemiBold();
                    row.ConstantItem(150).AlignRight().Text($"{FormatDate(cert.StartDate)} - {FormatDate(cert.EndDate)}");
                });

                // Provider
                if (!string.IsNullOrEmpty(cert.ProviderName))
                {
                    col.Item().Text(cert.ProviderName).FontColor(Colors.Grey.Darken1);
                }

                // Score if available
                if (cert.GPA.HasValue)
                {
                    col.Item().Text($"Score: {cert.GPA.Value}");
                }
            });
        }

        private string FormatDate(string? date)
        {
            return string.IsNullOrWhiteSpace(date) ? "" : date;
        }
    }
}
// In EditResume.js - Reindex collection items
function reindexCollectionItems(containerSelector, prefix) {
    $(containerSelector).children('.dynamic-list-item').each(function (index) {
        $(this).find('[name]').each(function () {
            const newName = $(this).attr('name')
                .replace(new RegExp(`${prefix}\\[\\d+\\]`), `${prefix}[${index}]`);
            $(this).attr('name', newName);
        });
    });
}

// Call this before form submission
$('#resumeForm').submit(function () {
    reindexCollectionItems('#educationsContainer', 'Educations');
    reindexCollectionItems('#experiencesContainer', 'Experiences');
    // Repeat for other collections...
    return true; // Ensure form submits
});


// Navigation between sections
document.querySelectorAll('[data-section]').forEach(link => {
    link.addEventListener('click', function (e) {
        e.preventDefault();
        const sectionId = this.getAttribute('data-section');

        // Update active nav link
        document.querySelectorAll('.nav-pills .nav-link').forEach(navLink => {
            navLink.classList.remove('active');
        });
        this.classList.add('active');

        // Show corresponding section
        document.querySelectorAll('.form-section').forEach(section => {
            section.classList.remove('active');
        });
        document.getElementById(`${sectionId}-section`).classList.add('active');

        // Scroll to top of section
        window.scrollTo({ top: 0, behavior: 'smooth' });
    });
});

// Update indexes for dynamic form elements
function updateIndexes(containerSelector, prefix) {
    const items = document.querySelectorAll(`${containerSelector} .dynamic-list-item`);
    items.forEach((item, idx) => {
        item.querySelectorAll('[name]').forEach(input => {
            const name = input.getAttribute('name');
            if (name) {
                const newName = name.replace(new RegExp(`${prefix}\\[\\d+\\]`), `${prefix}[${idx}]`);
                input.setAttribute('name', newName);
                input.setAttribute('id', newName.replace(/\./g, '_'));
            }
        });
    });
}

// Remove item from dynamic list
function removeItem(el) {
    const item = el.closest('.dynamic-list-item');
    const container = item.parentElement;
    item.remove();

    if (container.id === 'educationsContainer') updateIndexes('#educationsContainer', 'Educations');
    if (container.id === 'experiencesContainer') updateIndexes('#experiencesContainer', 'Experiences');
    if (container.id === 'skillsContainer') updateIndexes('#skillsContainer', 'Skills');
    if (container.id === 'languagesContainer') updateIndexes('#languagesContainer', 'Languages');
    if (container.id === 'certificatesContainer') updateIndexes('#certificatesContainer', 'Certificates');
    if (container.id === 'projectsContainer') updateIndexes('#projectsContainer', 'Projects');
}

// Add new education
document.getElementById('addEducation').addEventListener('click', function () {
    const container = document.getElementById('educationsContainer');
    const idx = container.querySelectorAll('.dynamic-list-item').length;
    const html = `
        <div class="dynamic-list-item">
            <i class="fas fa-times remove-item" onclick="removeItem(this)"></i>
            <div class="row g-3">
                <div class="col-md-6">
                    <label for="Educations_${idx}__CollegeName" class="form-label">College Name*</label>
                    <input name="Educations[${idx}].CollegeName" id="Educations_${idx}__CollegeName" class="form-control" required />
                </div>
                <div class="col-md-6">
                    <label for="Educations_${idx}__DegreeType" class="form-label">Degree Type*</label>
                    <input name="Educations[${idx}].DegreeType" id="Educations_${idx}__DegreeType" class="form-control" required />
                </div>
                <div class="col-md-6">
                    <label for="Educations_${idx}__StartDate" class="form-label">Start Date*</label>
                    <input name="Educations[${idx}].StartDate" id="Educations_${idx}__StartDate" type="date" class="form-control" required />
                </div>
                <div class="col-md-6">
                    <label for="Educations_${idx}__EndDate" class="form-label">End Date</label>
                    <input name="Educations[${idx}].EndDate" id="Educations_${idx}__EndDate" type="date" class="form-control" />
                </div>
                <div class="col-md-6">
                    <label for="Educations_${idx}__Major" class="form-label">Major</label>
                    <input name="Educations[${idx}].Major" id="Educations_${idx}__Major" class="form-control" />
                </div>
                <div class="col-md-6">
                    <label for="Educations_${idx}__GPA" class="form-label">GPA</label>
                    <input name="Educations[${idx}].GPA" id="Educations_${idx}__GPA" class="form-control" />
                </div>
            </div>
        </div>
    `;
    container.insertAdjacentHTML('beforeend', html);
    updateIndexes('#educationsContainer', 'Educations');
});

// Add new experience
document.getElementById('addExperience').addEventListener('click', function () {
    const container = document.getElementById('experiencesContainer');
    const idx = container.querySelectorAll('.dynamic-list-item').length;
    const html = `
        <div class="dynamic-list-item">
            <i class="fas fa-times remove-item" onclick="removeItem(this)"></i>
            <div class="row g-3">
                <div class="col-md-6">
                    <label for="Experiences_${idx}__Title" class="form-label">Job Title*</label>
                    <input name="Experiences[${idx}].Title" id="Experiences_${idx}__Title" class="form-control" required />
                </div>
                <div class="col-md-6">
                    <label for="Experiences_${idx}__Company" class="form-label">Company Name*</label>
                    <input name="Experiences[${idx}].Company" id="Experiences_${idx}__Company" class="form-control" required />
                </div>
                <div class="col-md-6">
                    <label for="Experiences_${idx}__StartDate" class="form-label">Start Date*</label>
                    <input name="Experiences[${idx}].StartDate" id="Experiences_${idx}__StartDate" type="date" class="form-control" required />
                </div>
                <div class="col-md-6">
                    <label for="Experiences_${idx}__EndDate" class="form-label">End Date</label>
                    <input name="Experiences[${idx}].EndDate" id="Experiences_${idx}__EndDate" type="date" class="form-control" />
                </div>
                <div class="col-md-4">
                    <label for="Experiences_${idx}__IsCurrent" class="form-label">Currently Working Here</label>
                    <div class="form-check">
                        <input name="Experiences[${idx}].IsCurrent" id="Experiences_${idx}__IsCurrent" class="form-check-input" type="checkbox" />
                        <label class="form-check-label">Yes</label>
                    </div>
                </div>
                <div class="col-12">
                    <label for="Experiences_${idx}__Duties" class="form-label">Job Duties*</label>
                    <textarea name="Experiences[${idx}].Duties" id="Experiences_${idx}__Duties" class="form-control" rows="3" required></textarea>
                </div>
            </div>
        </div>
    `;
    container.insertAdjacentHTML('beforeend', html);
    updateIndexes('#experiencesContainer', 'Experiences');
});

// Add new skill
document.getElementById('addSkill').addEventListener('click', function () {
    const container = document.getElementById('skillsContainer');
    const idx = container.querySelectorAll('.dynamic-list-item').length;
    const html = `
        <div class="dynamic-list-item">
            <i class="fas fa-times remove-item" onclick="removeItem(this)"></i>
            <div class="row g-3">
                <div class="col-md-6">
                    <label for="Skills_${idx}__SkillName" class="form-label">Skill Name*</label>
                    <input name="Skills[${idx}].SkillName" id="Skills_${idx}__SkillName" class="form-control" required />
                </div>
                <div class="col-md-6">
                    <label for="Skills_${idx}__SkillType" class="form-label">Skill Type</label>
                    <select name="Skills[${idx}].SkillType" id="Skills_${idx}__SkillType" class="form-select">
                        <option value="">Select type</option>
                        <option>Programming Language</option>
                        <option>Framework</option>
                        <option>Tool</option>
                        <option>Design</option>
                    </select>
                </div>
            </div>
        </div>
    `;
    container.insertAdjacentHTML('beforeend', html);
    updateIndexes('#skillsContainer', 'Skills');
});

// Add new language
document.getElementById('addLanguage').addEventListener('click', function () {
    const container = document.getElementById('languagesContainer');
    const idx = container.querySelectorAll('.dynamic-list-item').length;
    const html = `
        <div class="dynamic-list-item">
            <i class="fas fa-times remove-item" onclick="removeItem(this)"></i>
            <div class="row g-3">
                <div class="col-md-6">
                    <label for="Languages_${idx}__LanguageName" class="form-label">Language Name*</label>
                    <input name="Languages[${idx}].LanguageName" id="Languages_${idx}__LanguageName" class="form-control" required />
                </div>
                <div class="col-md-6">
                    <label for="Languages_${idx}__Level" class="form-label">Proficiency Level*</label>
                    <select name="Languages[${idx}].Level" id="Languages_${idx}__Level" class="form-select" required>
                        <option value="">Select level</option>
                        <option>Native</option>
                        <option>Fluent</option>
                        <option>Conversational</option>
                        <option>Intermediate</option>
                        <option>Beginner</option>
                    </select>
                </div>
            </div>
        </div>
    `;
    container.insertAdjacentHTML('beforeend', html);
    updateIndexes('#languagesContainer', 'Languages');
});

// Add new certificate
document.getElementById('addCertificate').addEventListener('click', function () {
    const container = document.getElementById('certificatesContainer');
    const idx = container.querySelectorAll('.dynamic-list-item').length;
    const html = `
        <div class="dynamic-list-item">
            <i class="fas fa-times remove-item" onclick="removeItem(this)"></i>
            <div class="row g-3">
                <div class="col-md-6">
                    <label for="Certificates_${idx}__Title" class="form-label">Certificate Title*</label>
                    <input name="Certificates[${idx}].Title" id="Certificates_${idx}__Title" class="form-control" required />
                </div>
                <div class="col-md-6">
                    <label for="Certificates_${idx}__ProviderName" class="form-label">Issuing Organization*</label>
                    <input name="Certificates[${idx}].ProviderName" id="Certificates_${idx}__ProviderName" class="form-control" required />
                </div>
                <div class="col-md-6">
                    <label for="Certificates_${idx}__StartDate" class="form-label">Issue Date*</label>
                    <input name="Certificates[${idx}].StartDate" id="Certificates_${idx}__StartDate" type="date" class="form-control" required />
                </div>
                <div class="col-md-6">
                    <label for="Certificates_${idx}__EndDate" class="form-label">Expiration Date</label>
                    <input name="Certificates[${idx}].EndDate" id="Certificates_${idx}__EndDate" type="date" class="form-control" />
                </div>
                <div class="col-md-6">
                    <label for="Certificates_${idx}__Field" class="form-label">Field of Study</label>
                    <input name="Certificates[${idx}].Field" id="Certificates_${idx}__Field" class="form-control" />
                </div>
                <div class="col-md-6">
                    <label for="Certificates_${idx}__GPA" class="form-label">GPA</label>
                    <input name="Certificates[${idx}].GPA" id="Certificates_${idx}__GPA" class="form-control" />
                </div>
            </div>
        </div>
    `;
    container.insertAdjacentHTML('beforeend', html);
    updateIndexes('#certificatesContainer', 'Certificates');
});

// Add new project
document.getElementById('addProject').addEventListener('click', function () {
    const container = document.getElementById('projectsContainer');
    const idx = container.querySelectorAll('.dynamic-list-item').length;
    const html = `
        <div class="dynamic-list-item">
            <i class="fas fa-times remove-item" onclick="removeItem(this)"></i>
            <div class="row g-3">
                <div class="col-md-6">
                    <label for="Projects_${idx}__ProjectName" class="form-label">Project Name*</label>
                    <input name="Projects[${idx}].ProjectName" id="Projects_${idx}__ProjectName" class="form-control" required />
                </div>
                <div class="col-12">
                    <label for="Projects_${idx}__ProjectDescription" class="form-label">Project Description*</label>
                    <textarea name="Projects[${idx}].ProjectDescription" id="Projects_${idx}__ProjectDescription" class="form-control" rows="3" required></textarea>
                </div>
                <div class="col-md-6">
                    <label for="Projects_${idx}__StartDate" class="form-label">Start Date*</label>
                    <input name="Projects[${idx}].StartDate" id="Projects_${idx}__StartDate" type="date" class="form-control" required />
                </div>
                <div class="col-md-6">
                    <label for="Projects_${idx}__EndDate" class="form-label">End Date</label>
                    <input name="Projects[${idx}].EndDate" id="Projects_${idx}__EndDate" type="date" class="form-control" />
                </div>
                <div class="col-md-6">
                    <label for="Projects_${idx}__ProjectLink" class="form-label">Project Link</label>
                    <input name="Projects[${idx}].ProjectLink" id="Projects_${idx}__ProjectLink" class="form-control" />
                </div>
            </div>
        </div>
    `;
    container.insertAdjacentHTML('beforeend', html);
    updateIndexes('#projectsContainer', 'Projects');
});

// Save button click
//document.querySelector('.save-btn-fixed').addEventListener('click', function () {
//    // Here you would collect all the form data and send it to your backend
//    this.innerHTML = '<i class="fas fa-spinner fa-spin me-2"></i> Saving...';

//    // Simulate API call
//    setTimeout(() => {
//        this.innerHTML = '<i class="fas fa-check me-2"></i> Saved!';
//        setTimeout(() => {
//            this.innerHTML = '<i class="fas fa-save me-2"></i> Save Changes';
//        }, 2000);
//    }, 1500);
//});
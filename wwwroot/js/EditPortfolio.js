// Navigation between sections
// Pseudocode:
// 1. Listen for "Add" button clicks for services, projects, and skills.
// 2. When adding, clone a hidden template or generate new HTML for the item.
// 3. Update the name attributes of all dynamic inputs to match the expected model binding (e.g., Services[0].ServiceName).
// 4. On remove, delete the item and re-index the remaining items' name attributes.
// 5. On form submit, all dynamic fields are correctly indexed and sent to the controller.

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

function removeItem(el) {
    const item = el.closest('.dynamic-list-item');
    const container = item.parentElement;
    item.remove();
    if (container.id === 'servicesContainer') updateIndexes('#servicesContainer', 'Services');
    if (container.id === 'projectsContainer') updateIndexes('#projectsContainer', 'Projects');
    if (container.id === 'skillsContainer') updateIndexes('#skillsContainer', 'Skills');
}

document.addEventListener('DOMContentLoaded', function () {
    // Add Service
    document.getElementById('addService').addEventListener('click', function () {
        const container = document.getElementById('servicesContainer');
        const idx = container.querySelectorAll('.dynamic-list-item').length;
        const html = `
            <div class="dynamic-list-item">
                <i class="fas fa-times remove-item" onclick="removeItem(this)"></i>
                <div class="row g-3">
                    <div class="col-md-6">
                        <label for="Services_${idx}__ServiceName" class="form-label">Service Name*</label>
                        <input name="Services[${idx}].ServiceName" id="Services_${idx}__ServiceName" class="form-control" />
                    </div>
                    <div class="col-md-6">
                        <label for="Services_${idx}__ServiceDescription" class="form-label">Service Description</label>
                        <input name="Services[${idx}].ServiceDescription" id="Services_${idx}__ServiceDescription" class="form-control" />
                    </div>
                </div>
            </div>
        `;
        container.insertAdjacentHTML('beforeend', html);
        updateIndexes('#servicesContainer', 'Services');
    });

    // Add Project
    document.getElementById('addProject').addEventListener('click', function () {
        const container = document.getElementById('projectsContainer');
        const idx = container.querySelectorAll('.dynamic-list-item').length;
        const html = `
            <div class="dynamic-list-item">
                <i class="fas fa-times remove-item" onclick="removeItem(this)"></i>
                <div class="row g-3">
                    <div class="col-md-6">
                        <label for="Projects_${idx}__ProjectName" class="form-label">Project Name*</label>
                        <input name="Projects[${idx}].ProjectName" id="Projects_${idx}__ProjectName" class="form-control" />
                    </div>
                    <div class="col-md-6">
                        <label for="Projects_${idx}__ProjectLink" class="form-label">Project Link</label>
                        <input name="Projects[${idx}].ProjectLink" id="Projects_${idx}__ProjectLink" class="form-control" />
                    </div>
                    <div class="col-12">
                        <label for="Projects_${idx}__ProjectDescription" class="form-label">Project Description*</label>
                        <textarea name="Projects[${idx}].ProjectDescription" id="Projects_${idx}__ProjectDescription" class="form-control" rows="3"></textarea>
                    </div>
                    <div class="col-md-4">
                        <label for="Projects_${idx}__StartDate" class="form-label">Start Date*</label>
                        <input name="Projects[${idx}].StartDate" id="Projects_${idx}__StartDate" type="date" class="form-control" />
                    </div>
                    <div class="col-md-4">
                        <label for="Projects_${idx}__EndDate" class="form-label">End Date</label>
                        <input name="Projects[${idx}].EndDate" id="Projects_${idx}__EndDate" type="date" class="form-control" />
                    </div>
                    <div class="col-12">
                        <label for="Projects_${idx}__ProjectImage" class="form-label">Project Image</label>
                        <input name="Projects[${idx}].ProjectImage" id="Projects_${idx}__ProjectImage" type="file" class="form-control" accept="image/*" />
                    </div>
                    <input type="hidden" name="Projects[${idx}].ImageBase64" />
                    <input type="hidden" name="Projects[${idx}].ImageFileName" />
                    <input type="hidden" name="Projects[${idx}].ImageContentType" />
                </div>
            </div>
        `;
        container.insertAdjacentHTML('beforeend', html);
        updateIndexes('#projectsContainer', 'Projects');
    });

    // Add Skill
    document.getElementById('addSkill').addEventListener('click', function () {
        const container = document.getElementById('skillsContainer');
        const idx = container.querySelectorAll('.dynamic-list-item').length;
        const html = `
            <div class="dynamic-list-item">
                <i class="fas fa-times remove-item" onclick="removeItem(this)"></i>
                <div class="row g-3">
                    <div class="col-md-6">
                        <label for="Skills_${idx}__SkillName" class="form-label">Skill Name*</label>
                        <input name="Skills[${idx}].SkillName" id="Skills_${idx}__SkillName" class="form-control" />
                    </div>
                    <div class="col-md-6">
                        <label for="Skills_${idx}__SkillType" class="form-label">Skill Type*</label>
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
});
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

// Add new service
document.getElementById('addService').addEventListener('click', function () {
    const container = document.getElementById('servicesContainer');
    const newItem = document.createElement('div');
    newItem.className = 'dynamic-list-item';
    newItem.innerHTML = `
                <i class="fas fa-times remove-item" onclick="removeItem(this)"></i>
                <div class="row">
                    <div class="col-md-6 mb-3">
                        <label class="form-label">Service Name*</label>
                        <input type="text" class="form-control" required>
                    </div>
                    <div class="col-md-6 mb-3">
                        <label class="form-label">Service Description</label>
                        <input type="text" class="form-control">
                    </div>
                </div>
            `;
    container.appendChild(newItem);

    // Scroll to the new item
    newItem.scrollIntoView({ behavior: 'smooth', block: 'nearest' });
});

// Add new project
document.getElementById('addProject').addEventListener('click', function () {
    const container = document.getElementById('projectsContainer');
    const newItem = document.createElement('div');
    newItem.className = 'dynamic-list-item';
    newItem.innerHTML = `
                <i class="fas fa-times remove-item" onclick="removeItem(this)"></i>
                <div class="row">
                    <div class="col-md-6 mb-3">
                        <label class="form-label">Project Name*</label>
                        <input type="text" class="form-control" required>
                    </div>
                    <div class="col-md-6 mb-3">
                        <label class="form-label">Project Link</label>
                        <input type="url" class="form-control">
                    </div>
                    <div class="col-12 mb-3">
                        <label class="form-label">Project Description*</label>
                        <textarea class="form-control" rows="3" required></textarea>
                    </div>
                    <div class="col-md-4 mb-3">
                        <label class="form-label">Start Date*</label>
                        <input type="date" class="form-control" required>
                    </div>
                    <div class="col-md-4 mb-3">
                        <label class="form-label">End Date</label>
                        <input type="date" class="form-control">
                    </div>
                    <div class="col-md-4 mb-3 d-flex align-items-end">
                        <div class="form-check form-switch">
                            <input class="form-check-input" type="checkbox" id="ongoingNew">
                            <label class="form-check-label" for="ongoingNew">Ongoing Project</label>
                        </div>
                    </div>
                    <div class="col-12 mb-3">
                        <label class="form-label">Project Image</label>
                        <div class="d-flex align-items-start">
                            <img src="https://via.placeholder.com/800x400" alt="Project Preview" class="project-image-preview me-4" style="max-width: 200px;">
                            <div class="flex-grow-1">
                                <input type="file" class="form-control" accept="image/*">
                                <div class="form-text">JPG or PNG, max 2MB</div>
                            </div>
                        </div>
                    </div>
                </div>
            `;
    container.appendChild(newItem);

    // Scroll to the new item
    newItem.scrollIntoView({ behavior: 'smooth', block: 'nearest' });
});

// Add new skill
document.getElementById('addSkill').addEventListener('click', function () {
    const container = document.getElementById('skillsContainer');
    const newItem = document.createElement('div');
    newItem.className = 'dynamic-list-item';
    newItem.innerHTML = `
                <i class="fas fa-times remove-item" onclick="removeItem(this)"></i>
                <div class="row">
                    <div class="col-md-6 mb-3">
                        <label class="form-label">Skill Name*</label>
                        <input type="text" class="form-control" required>
                    </div>
                    <div class="col-md-6 mb-3">
                        <label class="form-label">Skill Type*</label>
                        <select class="form-select" required>
                            <option value="">Select type</option>
                            <option>Programming Language</option>
                            <option>Framework</option>
                            <option>Tool</option>
                            <option>Design</option>
                        </select>
                    </div>
                </div>
            `;
    container.appendChild(newItem);

    // Scroll to the new item
    newItem.scrollIntoView({ behavior: 'smooth', block: 'nearest' });
});

// Remove item
function removeItem(element) {
    element.parentElement.style.transform = 'scale(0.9)';
    element.parentElement.style.opacity = '0';
    setTimeout(() => {
        element.parentElement.remove();
    }, 200);
}

// Image preview for profile image
document.getElementById('profileImage').addEventListener('change', function (e) {
    const file = e.target.files[0];
    if (file) {
        const reader = new FileReader();
        reader.onload = function (event) {
            document.getElementById('profileImagePreview').src = event.target.result;
        };
        reader.readAsDataURL(file);
    }
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
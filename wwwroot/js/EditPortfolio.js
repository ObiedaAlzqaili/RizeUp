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

    // Navigation between sections
    document.querySelectorAll('[data-section]').forEach(link => {
        link.addEventListener('click', function (e) {
            e.preventDefault();
            const sectionId = this.getAttribute('data-section');
            document.querySelectorAll('.nav-pills .nav-link').forEach(navLink => navLink.classList.remove('active'));
            this.classList.add('active');
            document.querySelectorAll('.form-section').forEach(section => section.classList.remove('active'));
            document.getElementById(`${sectionId}-section`).classList.add('active');
            window.scrollTo({ top: 0, behavior: 'smooth' });
        });
    });
});

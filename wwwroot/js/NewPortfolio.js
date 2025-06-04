let projectCount = 0;

const sections = document.querySelectorAll('.form-section');
const steps = document.querySelectorAll('.step');
let currentStep = 0;

function updateStep() {
    sections.forEach((sec, idx) => sec.classList.toggle('active', idx === currentStep));
    steps.forEach((circle, idx) => {
        circle.classList.remove('active', 'completed');
        if (idx === currentStep) circle.classList.add('active');
        else if (idx < currentStep) circle.classList.add('completed');
    });
    document.querySelector('.prev-step').style.display = currentStep === 0 ? 'none' : 'inline-block';
    document.querySelector('.next-step').style.display = currentStep === 2 ? 'none' : 'inline-block';
}

document.querySelector('.next-step').addEventListener('click', () => {
    if (validateSection(sections[currentStep])) {
        currentStep++;
        if (currentStep === 2) updatePreview();
        updateStep();
    }
});

document.querySelector('.prev-step').addEventListener('click', () => {
    currentStep--;
    updateStep();
});

function validateSection(section) {
    const requiredFields = section.querySelectorAll('[required]');
    let valid = true;
    requiredFields.forEach(field => {
        if (!field.value.trim()) {
            field.classList.add('is-invalid');
            valid = false;
        } else {
            field.classList.remove('is-invalid');
        }
    });
    // For page 2, ensure at least one project entry exists and its description is filled
    if (section.dataset.step === '2') {
        const projectEntries = section.querySelectorAll('.project-entry');
        if (projectEntries.length === 0) {
            alert('Please add at least one project.');
            valid = false;
        } else {
            projectEntries.forEach(entry => {
                const desc = entry.querySelector('textarea');
                if (!desc.value.trim()) {
                    desc.classList.add('is-invalid');
                    valid = false;
                } else {
                    desc.classList.remove('is-invalid');
                }
            });
        }
    }
    return valid;
}

function previewImage(input, imgElement) {
    const file = input.files[0];
    if (file) {
        const url = URL.createObjectURL(file);
        imgElement.src = url;
        imgElement.style.display = 'block';
    } else {
        imgElement.style.display = 'none';
    }
}

function addProjectEntry() {
    projectCount++;
    const container = document.getElementById('projectsContainer');

    const entryDiv = document.createElement('div');
    entryDiv.className = 'project-entry';
    entryDiv.dataset.projectId = projectCount;

    entryDiv.innerHTML = `
        <button type="button" class="remove-project" title="Remove Project">&times;</button>
        <div class="mb-3">
          <label class="form-label">Project ${projectCount} Description*</label>
          <textarea class="form-control" id="projectDesc${projectCount}" rows="3" required
            placeholder="E.g., A React-based e-commerce site with payment integration."></textarea>
        </div>
        <div class="mb-3">
          <label class="form-label">Project ${projectCount} Image</label>
          <input type="file" class="form-control project-image-input" id="projectImg${projectCount}" accept="image/*">
          <img id="projectPreview${projectCount}" class="img-preview" style="display:none;" alt="Project ${projectCount} Preview">
        </div>
      `;
    container.appendChild(entryDiv);

    // Attach event listeners
    const removeBtn = entryDiv.querySelector('.remove-project');
    removeBtn.addEventListener('click', () => {
        entryDiv.remove();
    });

    const imgInput = entryDiv.querySelector('.project-image-input');
    const imgPreview = entryDiv.querySelector(`#projectPreview${projectCount}`);
    imgInput.addEventListener('change', (e) => previewImage(e.target, imgPreview));
}

document.getElementById('addProjectBtn').addEventListener('click', addProjectEntry);

document.getElementById('profileImage').addEventListener('change', (e) => {
    const imgEl = document.getElementById('profilePreview');
    previewImage(e.target, imgEl);
});

function updatePreview() {
    const data = {
        firstName: document.getElementById('firstName').value,
        lastName: document.getElementById('lastName').value,
        phone: document.getElementById('phone').value,
        email: document.getElementById('email').value,
        linkedin: document.getElementById('linkedin').value,
        github: document.getElementById('github').value,
        summary: document.getElementById('summary').value,
        services: document.getElementById('services').value,
        skills: document.getElementById('skills').value
    };

    // Profile Image
    let profileImgHTML = '';
    const profileFile = document.getElementById('profileImage').files[0];
    if (profileFile) {
        const url = URL.createObjectURL(profileFile);
        profileImgHTML = `<p><strong>Profile Image:</strong><br><img src="${url}" class="img-preview"></p>`;
    }

    // Projects
    const projectEntries = document.querySelectorAll('.project-entry');
    let projectsHTML = '';
    projectEntries.forEach((entry, idx) => {
        const pid = entry.dataset.projectId;
        const desc = document.getElementById(`projectDesc${pid}`).value;
        const projFile = document.getElementById(`projectImg${pid}`).files[0];
        let projImgHTML = '';
        if (projFile) {
            const url = URL.createObjectURL(projFile);
            projImgHTML = `<p><strong>Project ${idx + 1} Image:</strong><br><img src="${url}" class="img-preview"></p>`;
        }
        projectsHTML += `
          <hr>
          <h5>Project ${idx + 1}</h5>
          <p><strong>Description:</strong> ${desc}</p>
          ${projImgHTML}
        `;
    });

    document.getElementById('preview').innerHTML = `
        <h5>Personal & Contact Info</h5>
        ${profileImgHTML}
        <p><strong>Name:</strong> ${data.firstName} ${data.lastName}</p>
        <p><strong>Email:</strong> ${data.email}</p>
        <p><strong>Phone:</strong> ${data.phone}</p>
        <p><strong>LinkedIn:</strong> ${data.linkedin}</p>
        <p><strong>GitHub:</strong> ${data.github}</p>
        <p><strong>Summary:</strong> ${data.summary}</p>
        <hr>
        <h5>Services</h5>
        <p>${data.services}</p>
        ${projectsHTML}
        <hr>
        <h5>Skills</h5>
        <p>${data.skills}</p>
      `;
}

document.getElementById('portfolioForm').addEventListener('submit', async function (e) {
    e.preventDefault();
    if (!validateSection(sections[2])) {
        // Should not reach here since step 3 has no required fields
    }
    // Gather data into FormData
    const formData = new FormData();
    formData.append('firstName', document.getElementById('firstName').value);
    formData.append('lastName', document.getElementById('lastName').value);
    formData.append('phone', document.getElementById('phone').value);
    formData.append('email', document.getElementById('email').value);
    formData.append('linkedin', document.getElementById('linkedin').value);
    formData.append('github', document.getElementById('github').value);
    formData.append('summary', document.getElementById('summary').value);
    const profileFile = document.getElementById('profileImage').files[0];
    if (profileFile) formData.append('profileImage', profileFile);

    formData.append('services', document.getElementById('services').value);

    // Append each project
    document.querySelectorAll('.project-entry').forEach((entry, idx) => {
        const pid = entry.dataset.projectId;
        formData.append(`projects[${idx}][description]`, document.getElementById(`projectDesc${pid}`).value);
        const pf = document.getElementById(`projectImg${pid}`).files[0];
        if (pf) formData.append(`projects[${idx}][image]`, pf);
    });

    formData.append('skills', document.getElementById('skills').value);

    console.log('Sending to API:', formData);
    // Example fetch (uncomment and replace URL):
    // await fetch('/api/ai-portfolio-enhance', {
    //   method: 'POST',
    //   body: formData
    // });
    alert('Portfolio data sent to AI for enhancement.');
});

// Initialize with one project entry
addProjectEntry();
updateStep();
// Same JavaScript as before (from your original file)
document.addEventListener('DOMContentLoaded', function () {
    // Navigation between steps
    const formSections = document.querySelectorAll('.form-section');
    const steps = document.querySelectorAll('.step');
    const stepLines = document.querySelectorAll('.step-line');

    // Next button click handler
    document.querySelectorAll('.next-step').forEach(button => {
        button.addEventListener('click', function () {
            const currentSection = document.querySelector('.form-section.active');
            const nextStepNum = this.getAttribute('data-next');

            // Validate current section before proceeding
            if (validateSection(currentSection)) {
                currentSection.classList.remove('active');
                document.querySelector(`.form-section[data-step="${nextStepNum}"]`).classList.add('active');

                // Update step indicator
                updateStepIndicator(nextStepNum);

                // Update navigation buttons
                updateNavigationButtons(nextStepNum);

                // If going to review step, update preview
                if (nextStepNum === '5') {
                    updatePreview();
                }
            }
        });
    });

    // Previous button click handler
    document.querySelectorAll('.prev-step').forEach(button => {
        button.addEventListener('click', function () {
            const currentSection = document.querySelector('.form-section.active');
            const prevStepNum = this.getAttribute('data-prev');

            currentSection.classList.remove('active');
            document.querySelector(`.form-section[data-step="${prevStepNum}"]`).classList.add('active');

            // Update step indicator
            updateStepIndicator(prevStepNum);

            // Update navigation buttons
            updateNavigationButtons(prevStepNum);
        });
    });

    // Add experience entry
    document.getElementById('addExperience').addEventListener('click', function () {
        addEntry('experienceEntries', 'experience');
    });

    // Add education entry
    document.getElementById('addEducation').addEventListener('click', function () {
        addEntry('educationEntries', 'education');
    });

    // Add skill entry
    document.getElementById('addSkill').addEventListener('click', function () {
        addEntry('skillEntries', 'skill');
    });

    // Remove entry buttons (delegated event)
    document.addEventListener('click', function (e) {
        if (e.target.classList.contains('remove-entry')) {
            e.target.closest('.experience-entry, .education-entry, .skill-entry').remove();
        }
    });

    // Form submission
    document.getElementById('resumeForm').addEventListener('submit', function (e) {
        e.preventDefault();

        // Collect all form data
        const formData = new FormData(this);
        const resumeData = {};

        // Convert FormData to object (simplified for demo)
        for (let [key, value] of formData.entries()) {
            resumeData[key] = value;
        }

        // Here you would send the data to your backend/API
        console.log('Submitting resume data:', resumeData);

        // Show success message (in a real app, you would redirect or show the enhanced resume)
        alert('Your resume has been sent to our AI for enhancement. You will receive the improved version shortly.');
    });

    // Function to update navigation buttons
    function updateNavigationButtons(currentStep) {
        const prevButton = document.querySelector('.prev-step');
        const nextButton = document.querySelector('.next-step');

        // Show/hide back button
        if (currentStep === '1') {
            prevButton.style.display = 'none';
        } else {
            prevButton.style.display = 'block';
            prevButton.setAttribute('data-prev', parseInt(currentStep) - 1);
        }

        // Update next button text and attributes
        if (currentStep === '4') {
            nextButton.innerHTML = 'Next: Review <i class="bi bi-arrow-right"></i>';
            nextButton.setAttribute('data-next', '5');
        } else if (currentStep === '5') {
            nextButton.style.display = 'none';
            document.querySelector('.prev-step').style.display = 'block';
        } else {
            const nextStepText = ['Experience', 'Education', 'Skills', 'Review'][parseInt(currentStep) - 1];
            nextButton.innerHTML = `Next: ${nextStepText} <i class="bi bi-arrow-right"></i>`;
            nextButton.setAttribute('data-next', parseInt(currentStep) + 1);
        }
    }

    // Function to add a new entry
    function addEntry(containerId, type) {
        const container = document.getElementById(containerId);
        const template = container.querySelector(`.${type}-entry`);
        const newEntry = template.cloneNode(true);

        // Clear input values in the new entry
        const inputs = newEntry.querySelectorAll('input, select, textarea');
        inputs.forEach(input => {
            if (input.type !== 'checkbox') {
                input.value = '';
            } else {
                input.checked = false;
            }
        });

        container.appendChild(newEntry);
    }

    // Function to validate a section
    function validateSection(section) {
        const inputs = section.querySelectorAll('input[required], select[required], textarea[required]');
        let isValid = true;

        inputs.forEach(input => {
            if (!input.value.trim()) {
                input.classList.add('is-invalid');
                isValid = false;
            } else {
                input.classList.remove('is-invalid');
            }
        });

        if (!isValid) {
            const firstInvalid = section.querySelector('.is-invalid');
            firstInvalid.scrollIntoView({ behavior: 'smooth', block: 'center' });
            alert('Please fill in all required fields before proceeding.');
        }

        return isValid;
    }

    // Function to update step indicator
    function updateStepIndicator(activeStep) {
        steps.forEach(step => {
            const stepNum = step.getAttribute('data-step');
            step.classList.remove('active', 'completed');

            if (stepNum === activeStep) {
                step.classList.add('active');
            } else if (stepNum < activeStep) {
                step.classList.add('completed');
            }
        });

        stepLines.forEach((line, index) => {
            line.classList.remove('completed');
            if ((index + 1) < activeStep) {
                line.classList.add('completed');
            }
        });
    }

    // Function to update preview
    function updatePreview() {
        // Personal Information
        const personalPreview = `
                    <p><strong>Name:</strong> ${document.getElementById('firstName').value} ${document.getElementById('lastName').value}</p>
                    <p><strong>Title:</strong> ${document.getElementById('title').value}</p>
                    <p><strong>Email:</strong> ${document.getElementById('email').value}</p>
                    <p><strong>Phone:</strong> ${document.getElementById('phone').value}</p>
                    <p><strong>Summary:</strong> ${document.getElementById('summary').value}</p>
                `;
        document.getElementById('previewPersonal').innerHTML = personalPreview;

        // Experience (simplified for demo)
        const experienceEntries = document.querySelectorAll('.experience-entry');
        let experienceHTML = '';
        experienceEntries.forEach(entry => {
            const company = entry.querySelector('[name="companyName"]').value;
            const title = entry.querySelector('[name="jobTitle"]').value;
            if (company || title) {
                experienceHTML += `
                            <div class="mb-3">
                                <h6>${title} at ${company}</h6>
                                <p>${entry.querySelector('[name="jobDescription"]').value}</p>
                            </div>
                        `;
            }
        });
        document.getElementById('previewExperience').innerHTML = experienceHTML || '<p>No experience added</p>';

        // Education (simplified for demo)
        const educationEntries = document.querySelectorAll('.education-entry');
        let educationHTML = '';
        educationEntries.forEach(entry => {
            const institution = entry.querySelector('[name="institutionName"]').value;
            const degree = entry.querySelector('[name="degreeType"]').value;
            if (institution || degree) {
                educationHTML += `
                            <div class="mb-3">
                                <h6>${degree} at ${institution}</h6>
                                <p>${entry.querySelector('[name="description"]').value}</p>
                            </div>
                        `;
            }
        });
        document.getElementById('previewEducation').innerHTML = educationHTML || '<p>No education added</p>';

        // Skills (simplified for demo)
        const skillEntries = document.querySelectorAll('.skill-entry');
        let skillsHTML = '<ul class="list-unstyled">';
        skillEntries.forEach(entry => {
            const skill = entry.querySelector('[name="skillName"]').value;
            const level = entry.querySelector('[name="skillLevel"]').value;
            if (skill) {
                skillsHTML += `<li class="mb-1">${skill} (${level})</li>`;
            }
        });
        skillsHTML += '</ul>';
        document.getElementById('previewSkills').innerHTML = skillsHTML || '<p>No skills added</p>';
    }
});
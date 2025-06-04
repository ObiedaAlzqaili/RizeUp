const sections = document.querySelectorAll('.form-section');
const steps = document.querySelectorAll('.step');
let currentStep = 0;

// Show/hide sections and update step circles
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

// Advance to next step if validation passes
document.querySelector('.next-step').addEventListener('click', () => {
    if (validateSection(sections[currentStep])) {
        currentStep++;
        if (currentStep === 2) updatePreview();
        updateStep();
    }
});

// Go back to previous step
document.querySelector('.prev-step').addEventListener('click', () => {
    currentStep--;
    updateStep();
});

// Ensure required fields in current section are non-empty
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
    return valid;
}

// Fill in the Review step with collected values
function updatePreview() {
    const data = {
        firstName: document.getElementById('firstName').value,
        lastName: document.getElementById('lastName').value,
        phone: document.getElementById('phone').value,
        email: document.getElementById('email').value,
        linkedin: document.getElementById('linkedin').value,
        github: document.getElementById('github').value,
        summary: document.getElementById('summary').value,
        education: document.getElementById('education').value,
        experience: document.getElementById('experience').value,
        skills: document.getElementById('skills').value
    };

    document.getElementById('preview').innerHTML = `
        <p><strong>Name:</strong> ${data.firstName} ${data.lastName}</p>
        <p><strong>Email:</strong> ${data.email}</p>
        <p><strong>Phone:</strong> ${data.phone}</p>
        <p><strong>LinkedIn:</strong> ${data.linkedin}</p>
        <p><strong>GitHub:</strong> ${data.github}</p>
        <p><strong>Summary:</strong> ${data.summary}</p>
        <p><strong>Education:</strong><br><pre>${data.education}</pre></p>
        <p><strong>Experience:</strong><br><pre>${data.experience}</pre></p>
        <p><strong>Skills:</strong><br><pre>${data.skills}</pre></p>
      `;
}

// On form submit, gather JSON and (optionally) send to your API
document.getElementById('resumeForm').addEventListener('submit', async function (e) {
    e.preventDefault();
    if (!document.getElementById('consentCheckbox').checked) {
        alert('You must agree before submitting.');
        return;
    }
    const formData = {
        firstName: document.getElementById('firstName').value,
        lastName: document.getElementById('lastName').value,
        phone: document.getElementById('phone').value,
        email: document.getElementById('email').value,
        linkedin: document.getElementById('linkedin').value,
        github: document.getElementById('github').value,
        summary: document.getElementById('summary').value,
        education: document.getElementById('education').value,
        experience: document.getElementById('experience').value,
        skills: document.getElementById('skills').value
    };
    console.log('Sending to API:', formData);
    // Example fetch (uncomment and replace URL):
    // await fetch('/api/ai-enhance', {
    //   method: 'POST',
    //   headers: { 'Content-Type': 'application/json' },
    //   body: JSON.stringify(formData)
    // });
    alert('Data sent to AI for processing.');
});

// Initialize step visibility on page load
updateStep();
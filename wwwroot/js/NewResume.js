//// Cache all form sections and steps
//const sections = document.querySelectorAll('.form-section');
//const steps = document.querySelectorAll('.step');
//let currentStep = 0;

//document.getElementById("submitBtn").addEventListener("click", function () {
//    const form = document.getElementById("resumeForm");
//    const formData = new FormData(form);
//    formData.append("command", "submit");

//    fetch("/Resume/ProcessStep", {
//        method: "POST",
//        body: formData
//    })
//        .then(response => response.json())
//        .then(data => {
//            if (data.redirectUrl) {
//                window.open(data.redirectUrl, "_blank");
//            }
//        });
//});

//// Show/hide sections and update step circles
//function updateStep() {
//    sections.forEach((sec, idx) => sec.classList.toggle('active', idx === currentStep));
//    steps.forEach((circle, idx) => {
//        circle.classList.remove('active', 'completed');
//        if (idx === currentStep) circle.classList.add('active');
//        else if (idx < currentStep) circle.classList.add('completed');
//    });
//    document.querySelector('.prev-step').style.display = currentStep === 0 ? 'none' : 'inline-block';
//    document.querySelector('.next-step').style.display = currentStep === 2 ? 'none' : 'inline-block';
//}

//// Advance to next step if validation passes
////document.querySelector('.next-step').addEventListener('click', () => {
////    if (validateSection(sections[currentStep])) {
////        currentStep++;
////        if (currentStep === 2) updatePreview();
////        updateStep();
////    }
////});

//// Go back to previous step
////document.querySelector('.prev-step').addEventListener('click', () => {
////    currentStep--;
////    updateStep();
////});

//// Ensure required fields in current section are non-empty
////function validateSection(section) {
////    const requiredFields = section.querySelectorAll('[required]');
////    let valid = true;
////    requiredFields.forEach(field => {
////        if (!field.value.trim()) {
////            field.classList.add('is-invalid');
////            valid = false;
////        } else {
////            field.classList.remove('is-invalid');
////        }
////    });
////    return valid;
////}

//// Fill in the Review step with collected values
////function updatePreview() {
////    const data = {
////        FirstName: document.getElementById('FirstName').value,
////        LastName: document.getElementById('LastName').value,
////        Phone: document.getElementById('Phone').value,
////        Email: document.getElementById('Email').value,
////        LinkedinLink: document.getElementById(' LinkedinLink').value,
////        GitHubLink: document.getElementById('GitHubLink').value,
////        Summary: document.getElementById('Summary').value,
////        Education: document.getElementById('Education').value,
////        Experience: document.getElementById('Experience').value,
////        Skills: document.getElementById('Skills').value
////    };

////    document.getElementById('preview').innerHTML = `
////        <p><strong>Name:</strong> ${data.FirstName} ${data.LastName}</p>
////        <p><strong>Email:</strong> ${data.Email}</p>
////        <p><strong>Phone:</strong> ${data.Phone}</p>
////        <p><strong>LinkedIn:</strong> ${data.LinkedinLink}</p>
////        <p><strong>GitHub:</strong> ${data.GitHubLink}</p>
////        <p><strong>Summary:</strong> ${data.Summary}</p>
////        <p><strong>Education:</strong><br><pre>${data.Education}</pre></p>
////        <p><strong>Experience:</strong><br><pre>${data.Experience}</pre></p>
////        <p><strong>Skills:</strong><br><pre>${data.Skills}</pre></p>
////      `;
////}

//// On form submit, gather JSON and (optionally) send to your API
////document.getElementById('resumeForm').addEventListener('submit', function (e) {
////    const consent = document.getElementById('consentCheckbox');
////    if (!consent.checked) {
////        e.preventDefault();
////        alert('You must agree before submitting.');
////        return;
////    }
////    // If we reach here, we allow the form to post normally.
////    // Do not call e.preventDefault(); letting ASP.NET handle the POST.
////});

//// Initialize step visibility on page load
//updateStep();
document.addEventListener('DOMContentLoaded', function () {
    const form = document.getElementById('resumeForm');
    const submitBtn = form.querySelector('button[value="submit"]');

    form.addEventListener('submit', async function (e) {
        e.preventDefault();

        const formData = new FormData(form);
        const command = e.submitter.value;
        formData.append('command', command);

        // Show loading state
        if (command === 'submit') {
            submitBtn.disabled = true;
            submitBtn.innerHTML = '<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span> Processing...';
        }

        try {
            const response = await fetch('/Resume/ProcessStep', {
                method: 'POST',
                body: formData
            });

            if (response.redirected) {
                window.location.href = response.url;
                return;
            }

            const result = await response.text();
            document.documentElement.innerHTML = result;

            // Reinitialize any scripts after content update
            const scripts = document.querySelectorAll('script');
            scripts.forEach(script => {
                if (script.src) {
                    const newScript = document.createElement('script');
                    newScript.src = script.src;
                    document.body.appendChild(newScript);
                }
            });

        } catch (error) {
            console.error('Error:', error);
            alert('An error occurred while submitting the form. Please try again.');
        } finally {
            if (command === 'submit') {
                submitBtn.disabled = false;
                submitBtn.innerHTML = 'Submit to AI';
            }
        }
    });
});
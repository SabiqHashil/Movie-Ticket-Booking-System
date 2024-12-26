// States and cities data
const statesCities = {
    'Kerala': ['Thiruvananthapuram', 'Kochi', 'Kozhikode'],
    'Goa': ['Panaji', 'Margao', 'Vasco da Gama'],
    'Karnataka': ['Bengaluru', 'Mysuru', 'Hubli'],
    'TamilNadu': ['Chennai', 'Coimbatore', 'Madurai']
};

// Populate States dropdown
const stateDropdown = document.getElementById('State');
for (let state in statesCities) {
    const option = document.createElement('option');
    option.value = state;
    option.textContent = state;
    stateDropdown.appendChild(option);
}

// Populate Cities dropdown based on selected state
const cityDropdown = document.getElementById('City');
stateDropdown.addEventListener('change', function () {
    const selectedState = stateDropdown.value;
    cityDropdown.innerHTML = '<option value="">Select a city</option>'; 
    if (selectedState) {
        const cities = statesCities[selectedState];
        cities.forEach(city => {
            const option = document.createElement('option');
            option.value = city;
            option.textContent = city;
            cityDropdown.appendChild(option);
        });
    }
});

document.getElementById('submitButton').addEventListener('click', function (event) {
    const firstName = document.getElementById('FirstName').value.trim();
    const lastName = document.getElementById('LastName').value.trim();
    const phoneNumber = document.getElementById('PhoneNumber').value.trim();
    const password = document.getElementById('Password').value.trim();
    const confirmPassword = document.getElementById('ConfirmPassword').value.trim();
    const dateOfBirth = document.getElementById('DateOfBirth').value;

    let valid = true;
    let errorMessages = [];

    // Clear previous error messages
    const errorElements = document.querySelectorAll('.validation-error');
    errorElements.forEach(el => el.remove());

    // First Name validation
    if (firstName === "") {
        valid = false;
        errorMessages.push({ field: 'FirstName', message: "First name is required" });
    }

    // Last Name validation
    if (lastName === "") {
        valid = false;
        errorMessages.push({ field: 'LastName', message: "Last name is required" });
    }

    // Date of Birth validation
    const today = new Date();
    const dob = new Date(dateOfBirth);
    const age = today.getFullYear() - dob.getFullYear();
    const monthDifference = today.getMonth() - dob.getMonth();
    const dayDifference = today.getDate() - dob.getDate();

    if (!dateOfBirth) {
        valid = false;
        errorMessages.push({ field: 'DateOfBirth', message: "Date of birth is required" });
    } else if (dob > today) {
        valid = false;
        errorMessages.push({ field: 'DateOfBirth', message: "Future dates are not allowed" });
    } else if (age < 18 || (age === 18 && (monthDifference < 0 || (monthDifference === 0 && dayDifference < 0)))) {
        valid = false;
        errorMessages.push({ field: 'DateOfBirth', message: "You must be at least 18 years old" });
    }

    // Phone Number validation
    const phoneRegex = /^[0-9]{10}$/;
    if (phoneNumber === "" || !phoneRegex.test(phoneNumber)) {
        valid = false;
        errorMessages.push({ field: 'PhoneNumber', message: "Enter a valid 10-digit phone number" });
    }

    // Password validation
    if (password.length < 6) {
        valid = false;
        errorMessages.push({ field: 'Password', message: "Password must be at least 6 characters long" });
    }

    // Confirm Password validation
    if (password !== confirmPassword) {
        valid = false;
        errorMessages.push({ field: 'ConfirmPassword', message: "Passwords do not match" });
    }

    if (!valid) {
        event.preventDefault();

        // Display error messages
        errorMessages.forEach(err => {
            const field = document.getElementById(err.field);
            const errorMessage = document.createElement('small');
            errorMessage.className = 'text-danger validation-error';
            errorMessage.textContent = err.message;
            field.parentElement.appendChild(errorMessage);
        });
    }
});

// Disable future dates in the date picker
const dateOfBirthField = document.getElementById('DateOfBirth');
const today = new Date().toISOString().split('T')[0];
dateOfBirthField.setAttribute('max', today);